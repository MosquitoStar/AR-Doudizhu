using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class dou : PunBehaviour
{
    public static Object[] cardPrefabs;     //54张牌的prefab模型
    public static Vector3 position;         //牌桌（invisible）的中心坐标
    public static dou gm;		            //dou静态实例

    public GameObject twoClaimButton;//叫地主键+不叫键
    public GameObject playingButton;//出牌键+不出键+提示键
    public GameObject hangUp;//托管键
    public GameObject notHangUp;//取消托管键
    public GameObject winPanel;//结束面板
    public GameObject losePanel;

    public GameObject p0NotClaim;//p0不叫
    public GameObject p1NotClaim;//p1不叫
    public GameObject p2NotClaim;//p2不叫
    public GameObject p0NotPlay;//p0不出
    public GameObject p1NotPlay;//p1不出
    public GameObject p2NotPlay;//p2不出

    public Animator player0;//左手边玩家人物
    public Animator player1;//我的玩家人物
    public Animator player2;//右手边玩家人物

    public enum GameState
    {		//游戏状态枚举
        PreStart,				//游戏开始前
        dealing,                //发牌中
        claimLandlord,			//叫地主
        Playing,				//游戏进行中
        GameWin,				//游戏胜利
        GameLose				//游戏失败
    };
    public GameState state = GameState.PreStart;	//初始化游戏状态
    public int landlord;            //地主玩家的ID
    public int whoseTurn;           //该谁出牌了(玩家游戏编号,0 or 1 or 2)
    public int lastWho;             //牌局上最大的牌组是谁出的
    public int winner;              //胜利者
    cardcombine lastCombine;        //牌局上最大的牌组
    public bool dealComplete;       //发牌完成标记
    private int firstClaim;         //第一个叫地主的玩家
    private int whoClaim;           //轮到谁叫地主
    private int claimTimes;         //叫地主的次数(暂时用不着)
    private int notClaimTimes;      //不叫地主的次数
    bool pressClaim;                //本机玩家是否叫/不叫过地主
    bool beginPlay;
    bool[] isAI = new bool[3];
    AIPlayer[] AI = new AIPlayer[3];
    public GameState getGameState()
    {
        return state;
    }
    public int getLandlordID()
    {
        return landlord;
    }
    public int getWhoseTurn()
    {
        return whoseTurn;
    }
    public int getLastWho()
    {
        return lastWho;
    }

    player me;

    card[] deck = new card[54];                 //牌库，54张牌
    card[] landlordCard = new card[3];          //地主牌，3张牌
    Object cardBack;
    List<GameObject> leftPlayerCards;           //左手边玩家的牌模型
    List<GameObject> rightPlayerCards;          //右手边玩家的牌模型

    int loadedPlayerNum = 0;		//已加载场景的玩家个数
    ExitGames.Client.Photon.Hashtable playerCustomProperties;

    public static float myCardsX = -1.125f;
    public static float myCardsDX = 0.125f;
    public static float myCardsY = 0.0f;
    public static float myCardsZ = -0.5f;
    public static float otherPlayerCardsX = 1.3f;
    public static float otherPlayerCardsY = 0.0f;
    public static float otherPlayerCardsZ = 0.5f;
    public static float otherPlayerCardsDZ = -0.025f;
    public static float landlordCardsX = -0.125f;
    public static float landlordCardsDX = 0.125f;
    public static float landlordCardsY = 0.0f;
    public static float landlordCardsZ = 0.75f;

    //初始化
    void Start()
    {
        gm = GetComponent<dou>();		//初始化dou静态实例gm
        photonView.RPC("ConfirmLoad", PhotonTargets.All);				//使用RPC,告知所有玩家有一名玩家已成功加载场景
        
        SetButtonInactive();
        
        p0NotClaim.SetActive(false);
        p1NotClaim.SetActive(false);
        p2NotClaim.SetActive(false);
        p0NotPlay.SetActive(false);
        p1NotPlay.SetActive(false);
        p2NotPlay.SetActive(false);

        //确定牌桌位置
        position = transform.position;

        //初始化游戏状态
        landlord = -1;
        lastWho = landlord;
        whoseTurn = landlord;
        winner = -1;
        lastCombine = new cardcombine();
        dealComplete = false;
        beginPlay=false;
        pressClaim = false;

        //初始化牌库
        resetDeck();

        //初始化玩家编号
        playerCustomProperties = PhotonNetwork.player.customProperties;	//获取玩家自定义属性
        me = new player();
        if (playerCustomProperties["Team"].ToString().Equals("Team1"))
        {
            me.setGameID(0);
        }
        else if (playerCustomProperties["Team"].ToString().Equals("Team2"))
        {
            me.setGameID(1);
        }
        else if (playerCustomProperties["Team"].ToString().Equals("Team3"))
        {
            me.setGameID(2);
        }
        int gender = 0;
        SendMessage("setVoiceGender", gender);
        SendMessage("setMyID", me.getGameID());

        //初始化AI
        for (int i = 0; i < 3; i++)
        {
            isAI[i] = false;
        }
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < 3; i++ ) {
                if (playerCustomProperties["Position" + (i + 1).ToString()].ToString().Equals("AI"))
                {
                    AI[i]=new AIPlayer();
                    isAI[i] = true;
                    AI[i].setPlayerNo(i);
                }
            }
        }

        //导入54张牌的prefab模型
        string path = "prefab/";
        cardPrefabs = new Object[54];
        for (int i = 0; i < 54; i++)
        {
            cardPrefabs[i] = Resources.Load(path + toCardName(i), typeof(GameObject));
        }

        //导入卡背的prefab模型
        cardBack = Resources.Load(path + "cardBack", typeof(GameObject));

        //
        leftPlayerCards = new List<GameObject>();
        rightPlayerCards = new List<GameObject>();

        //随机选择一名玩家优先叫地主
        System.Random r = new System.Random();
        int times = r.Next(300);
        whoClaim = 0;
        for(int i=0;i<times;i++){
            whoClaim=(whoClaim+1)%3;
        }
        firstClaim = whoClaim;
        claimTimes = 0;
        notClaimTimes = 0;
    }


    void Update()
    {
        switch (state)
        {
            case GameState.PreStart:
                //Debug.Log("游戏连接阶段");
                if (PhotonNetwork.isMasterClient)
                {
                    CheckPlayerConnected();
                }
                break;
            case GameState.dealing:
                //Debug.Log("发牌阶段");
                if (dealComplete)
                {
                    gm.state = GameState.claimLandlord;
                }
                break;
            case GameState.claimLandlord:
                //Debug.Log("叫地主阶段");
                if (PhotonNetwork.isMasterClient)
                {
                    if (notClaimTimes >= 3)
                    {
                        notClaimTimes = 0;
                        photonView.RPC("restartGame", PhotonTargets.All);
                    }
                    photonView.RPC("PlayerClaim", PhotonTargets.All, whoClaim);
                    //ClaimLandlord ();
                }
                break;
            case GameState.Playing:
                //Debug.Log("打牌阶段");
                //Debug.Log("现在轮到玩家" + whoseTurn + "出牌");
                if (PhotonNetwork.isMasterClient)
                {					//MasterClient检查游戏状态
                    if (winner != -1)
                    {
                        photonView.RPC("EndGame", PhotonTargets.All, winner);
                    }
                }
                //AI出牌(如果有的话)
                if (PhotonNetwork.isMasterClient)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (isAI[i])
                        {
                            if (i == whoseTurn)
                            {
                                AIAction(i);
                                break;
                            }
                        }
                    }
                }
                if (!me.getHangUp())
                {
                    //Debug.Log("你(玩家" + me.getGameID() + ")现在可以单击选中牌了");
                    me.selectCard();
                    if (Input.GetKeyDown(KeyCode.T) || hangUpButton.isdown)
                    {
                        //Debug.Log("进入托管模式");
                        me.enableHangUp();
                        SetNotHangUpButtonActive();
                        notHangUpButton.isdown = false;
                    }
                    //没轮到该玩家出牌
                    if (me.getGameID() != whoseTurn)
                    {
                        //Debug.Log("还没轮到你出牌,现在是玩家" + whoseTurn + "出牌");
                    }
                    else if (!me.getHangUp())
                    {
                        // if(!hasPlayed)
                        playingButton.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.Space) || dealButton.isdown)
                        {
                            //Debug.Log("你(玩家" + me.getGameID() + ")按下了出牌键");
                            PlayCard();
                            dealButton.isdown = false;
                        }
                        if (Input.GetKeyDown(KeyCode.P) || notDealButton.isdown)
                        {
                            //Debug.Log("你(玩家" + me.getGameID() + ")按下了不要键");
                            Pass();
                            notDealButton.isdown = false;
                        }
                        if (Input.GetKeyDown(KeyCode.H) || hintButton.isdown)
                        {
                            //Debug.Log("你(玩家" + me.getGameID() + ")按下了提示键");
                            //Debug.Log("上一个牌组是" + lastCombine.toString());
                            me.hint(lastCombine);
                            hintButton.isdown = false;
                        }
                    }
                }
                if (me.getHangUp())
                {
                    playingButton.SetActive(false);
                    if (Input.GetKeyDown(KeyCode.T) || notHangUpButton.isdown)
                    {
                        //Debug.Log("退出托管模式");
                        me.disableHangUp();
                        SetHangUpButtonActive();
                        hangUpButton.isdown = false;
                    }
                    HangUp();
                }
                break;
            case GameState.GameWin:		//游戏胜利状态，倒计时结束，退出游戏房间
                //Debug.Log("胜利");
                displayWinScene();
                SetButtonInactive();
                winPanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    LeaveRoom();
                }
                break;
            case GameState.GameLose:	//游戏结束状态，倒计时结束，退出游戏房间
                //Debug.Log("失败");
                displayLoseScene();
                SetButtonInactive();
                losePanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    LeaveRoom();
                }
                break;
        }
    }

    //重置牌库（按牌面从小到大顺序排好）
    public void resetDeck()
    {
        for (int i = 0; i < 54; i++)
        {
            deck[i] = card.form(i);
        }
    }

    //将一个整数（0~53）转成牌的名字
    private string toCardName(int value)
    {
        if (value > 53 || value < 0)
        {
            return "";
        }
        int point = value / 4;
        int suit = value % 4;
        string name = "";
        if (point == 13)
        {
            name += "Joker";
            if (suit == 0)
            {
                name += "Black";
                return name;
            }
            else
            {
                name += "Red";
                return name;
            }
        }
        switch (point)
        {
            case 0:
                name += "3";
                break;
            case 1:
                name += "4";
                break;
            case 2:
                name += "5";
                break;
            case 3:
                name += "6";
                break;
            case 4:
                name += "7";
                break;
            case 5:
                name += "8";
                break;
            case 6:
                name += "9";
                break;
            case 7:
                name += "10";
                break;
            case 8:
                name += "J";
                break;
            case 9:
                name += "Q";
                break;
            case 10:
                name += "K";
                break;
            case 11:
                name += "A";
                break;
            case 12:
                name += "2";
                break;
        }
        switch (suit)
        {
            case 0:
                name += "Diamond";
                break;
            case 1:
                name += "Club";
                break;
            case 2:
                name += "Heart";
                break;
            case 3:
                name += "Spades";
                break;
        }
        return name;
    }

    //RPC函数，增加成功加载场景的玩家个数
    [PunRPC]
    void ConfirmLoad()
    {
        loadedPlayerNum++;
    }

    //主机专用函数:检查所有玩家是否已经加载场景
    void CheckPlayerConnected()
    {
        if (loadedPlayerNum == PhotonNetwork.playerList.Length)
        {	//所有玩家加载场景
            Shuffle();
            int[] ii = new int[54];
            for (int i = 0; i < 54; i++)
            {
                ii[i] = deck[i].toInt();
            }
            photonView.RPC("UpdateDeck", PhotonTargets.All, ii);
            photonView.RPC("Deal", PhotonTargets.All);
        }
    }

    //洗牌并发地主牌
    private void Shuffle(int times = 300)
    {
        System.Random ran = new System.Random();
        int index1, index2;
        card tmp;
        for (int i = 0; i < times; i++)
        {
            index1 = ran.Next(0, 54);
            index2 = ran.Next(0, 54);
            tmp = deck[index1];
            deck[index1] = deck[index2];
            deck[index2] = tmp;
        }
    }

    //将洗好的牌库同步到所有客户端
    [PunRPC]
    void UpdateDeck(int[] Mdeck)
    {
        for (int i = 0; i < 54; i++)
        {
            int s, p;
            if (Mdeck[i] == 53)
            {
                s = (int)card.suit.Red;
                p = (int)card.point.Card_RJ;
            }
            else if (Mdeck[i] == 52)
            {
                s = (int)card.suit.Black;
                p = (int)card.point.Card_BJ;
            }
            else
            {
                s = Mdeck[i] % 4;
                p = Mdeck[i] / 4;
            }
            deck[i] = new card((card.suit)s, (card.point)p);
        }
    }

    //RPC函数,发牌
    [PunRPC]
    void Deal()
    {
        FetchCards();			                //拿走属于自己的牌
        StartCoroutine(DealAnimation());        //发牌动画效果
        gm.state = GameState.dealing;	        //游戏状态切换到发牌状态
    }

    //拿走属于自己的牌
    private void FetchCards()
    {
        //取走前三张牌作为地主牌
        for (int i = 0; i < 3; i++)
        {
            landlordCard[i] = deck[i];
        }
        //如果玩家座位号为0,则取走第3~19张牌
        if (me.getGameID() == 0)
        {
            for (int i = 3; i < 20; i++)
            {
                me.addCard(deck[i]);
            }
        }
        //如果玩家座位号为1,则取走第20~36张牌
        else if (me.getGameID() == 1)
        {
            for (int i = 20; i < 37; i++)
            {
                me.addCard(deck[i]);
            }
        }
        //如果玩家座位号为2,则取走第37~53张牌
        else if (me.getGameID() == 2)
        {
            for (int i = 37; i < 54; i++)
            {
                me.addCard(deck[i]);
            }
        }
        me.sortCards();
        //主机给AI发牌(如果有的话)
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < 3; i++)
            {
                if (isAI[i])
                {
                    List<card> AICards = new List<card>();
                    for (int j = 0; j < 17; j++)
                    {
                        AICards.Add(deck[i * 17 + j + 3]);
                    }
                    AI[i].pickUpHand(AICards);
                }
            }
        }
    }

    //发牌动画
    private IEnumerator DealAnimation()
    {
        //显示地主牌
        float dist = landlordCardsX;     //发牌初始x坐标
        for (int i = 0; i < 3; i++, dist += landlordCardsDX)
        {
            int indexInPrefabs = landlordCard[i].toInt();
            landlordCard[i].model = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            landlordCard[i].model.transform.position = transform.position + new Vector3(dist, landlordCardsY, landlordCardsZ);
            landlordCard[i].model.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
        }
        //发牌动画
        dist = myCardsX;
        float ndist = otherPlayerCardsZ;       //发牌初始x坐标
        for (int i = 0; i < 17; i++, dist += myCardsDX, ndist += otherPlayerCardsDZ)
        {
            //自己的牌
            int indexInPrefabs = me.getCards()[i].toInt();
            GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            me.getCards()[i].setModel(cardModel);
            me.getCards()[i].model.transform.position = transform.position + new Vector3(dist, myCardsY, myCardsZ);
            me.getCards()[i].model.transform.Rotate(new Vector3(1, 0, 0), -80.0f);
            //上家的牌
            GameObject cardBackModelLeft = Instantiate(cardBack) as GameObject;
            cardBackModelLeft.transform.position = transform.position + new Vector3(-otherPlayerCardsX, otherPlayerCardsY, ndist);
            cardBackModelLeft.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            leftPlayerCards.Add(cardBackModelLeft);
            //下家的牌
            GameObject cardBackModelRight = Instantiate(cardBack) as GameObject;
            cardBackModelRight.transform.position = transform.position + new Vector3(otherPlayerCardsX, otherPlayerCardsY, ndist);
            cardBackModelRight.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            rightPlayerCards.Add(cardBackModelRight);
            yield return new WaitForSeconds(0.1f);
        }
        dealComplete = true;
    }

    //RPC函数,重新开始
    [PunRPC]
    void restartGame()
    {
        clearScreen();
        pressClaim=false;
        dealComplete=false;
        System.Random r = new System.Random();
        int times = r.Next(300);
        whoClaim = 0;
        for(int i=0;i<times;i++){
            whoClaim=(whoClaim+1)%3;
        }
        firstClaim = whoClaim;
        gm.state = GameState.PreStart;
    }

    //清除屏幕中所有模型
    private void clearScreen()
    {
        //清除自己的牌
        for (int j = 0; j < me.getCards().Count; j++)
        {
            Destroy(me.getCards()[j].model);
        }
        me.removeCards();
        //清除地主牌
        for (int i = 0; i < 3; i++)
        {
            Destroy(landlordCard[i].model);
        }
        //清除另外两个玩家的牌
        for (int i = 0; i < leftPlayerCards.Count; i++)
        {
            Destroy(leftPlayerCards[i]);
        }
        leftPlayerCards.Clear();
        for (int i = 0; i < rightPlayerCards.Count; i++)
        {
            Destroy(rightPlayerCards[i]);
        }
        rightPlayerCards.Clear();
        //清除lastCombine
        for (int i = 0; i < lastCombine.size(); i++)
        {
            Destroy(lastCombine.getList()[i].model);
        }
        p0NotClaim.SetActive(false);
        p1NotClaim.SetActive(false);
        p2NotClaim.SetActive(false);
    }

    //RPC函数,抢地主
    [PunRPC]
    void PlayerClaim(int playerID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < 3; i++)
            {
                if (isAI[i])
                {
                    if (playerID == i)
                    {
                        if (AI[i].call() > 0)
                        {
                            photonView.RPC("SetLandlord", PhotonTargets.All, playerID);
                        }
                        else
                        {
                            photonView.RPC("noClaimLandlord", PhotonTargets.All, playerID);
                            playerID = (playerID + 1) % 3;
                            photonView.RPC("updateWhoClaim", PhotonTargets.MasterClient, playerID);
                        }
                        return;
                    }
                }
            }
        }
        //Debug.Log("现在轮到玩家" + playerID + "叫地主");
        if (playerID != me.getGameID())
        {
            return;
        }
        if (!pressClaim)
            twoClaimButton.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Y) || claimButton.isdown)
        {
            pressClaim = true;
            photonView.RPC("SetLandlord", PhotonTargets.All, playerID);
            claimButton.isdown = false;
            twoClaimButton.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.N) || notClaimButton.isdown)
        {
            pressClaim = true;
            photonView.RPC("noClaimLandlord", PhotonTargets.All, playerID);
            playerID = (playerID + 1) % 3;
            photonView.RPC("updateWhoClaim", PhotonTargets.MasterClient, playerID);
            notClaimButton.isdown = false;
            twoClaimButton.SetActive(false);
        }
    }

    //RPC函数,更新主机上当前叫地主的玩家的编号
    [PunRPC]
    void updateWhoClaim(int playerID)
    {
        if (!PhotonNetwork.isMasterClient)		//如果函数不是由MasterClient调用，结束函数执行
            return;
        whoClaim = playerID;
        notClaimTimes++;
    }

    [PunRPC]
    void noClaimLandlord(int id)
    {
        if (id == me.getGameID())
        {
            p1NotClaim.SetActive(true);
        }
        else if (id == (me.getGameID() + 1) % 3)
        {
            p2NotClaim.SetActive(true);
        }
        else if (id == (me.getGameID() + 2) % 3)
        {
            p0NotClaim.SetActive(true);
        }
        SendMessage("claimLandLordAudio",false);
        StartCoroutine(notClaimAnimation(id));
    }

    private IEnumerator notClaimAnimation(int id){
        if (id == me.getGameID())
        {
            player1.SetBool("notClaim",true);
        }
        else if (id == (me.getGameID() + 1) % 3)
        {
            player2.SetBool("notClaim",true);
        }
        else if (id == (me.getGameID() + 2) % 3)
        {
            player0.SetBool("notClaim",true);
        }
        yield return new WaitForSeconds(0.8f);
        if (id == me.getGameID())
        {
            player1.SetBool("notClaim",false);
        }
        else if (id == (me.getGameID() + 1) % 3)
        {
            player2.SetBool("notClaim",false);
        }
        else if (id == (me.getGameID() + 2) % 3)
        {
            player0.SetBool("notClaim",false);
        }
    }

    //RPC函数,设置地主
    [PunRPC]
    void SetLandlord(int id)
    {
        Debug.Log("地主是玩家" + id);
        landlord = id;
        lastWho = landlord;
        whoseTurn = landlord;
        SendMessage("claimLandLordAudio", true);
        StartCoroutine(RevealLandlordCards(id));
        if (id == me.getGameID())
        {
            AddLandlordCardsToHand();
        }
        else
        {
            AddLandlordCardsForPlayer(id);
        }
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < 3; i++)
            {
                if (isAI[i])
                {
                    AI[i].setDizhuNo(i);
                    if (id == i)
                    {
                        List<card> clist = new List<card>();
                        for (int j = 0; j < 3; j++)
                        {
                            clist.Add(landlordCard[j]);
                        }
                        AI[i].addLandlordCards(clist);
                    }
                }
            }
        }
        p0NotClaim.SetActive(false);
        p1NotClaim.SetActive(false);
        p2NotClaim.SetActive(false);
        SetHangUpButtonActive();
        gm.state = GameState.Playing;	//游戏状态切换到游戏进行状态
    }

    //翻开地主牌动画
    private IEnumerator RevealLandlordCards(int id)
    {
        if(id==me.getGameID()){
            player1.SetBool("claim",true);
        }else if(id==(me.getGameID()+1)%3){
            player2.SetBool("claim",true);
        }else if(id==(me.getGameID()+2)%3){
            player0.SetBool("claim",true);
        }
        for (int i = 0; i < 18; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                landlordCard[j].model.transform.Rotate(new Vector3(1, 0, 0), -10.0f);
            }
            yield return new WaitForSeconds(0.03f);
        }
        if(id==me.getGameID()){
            player1.SetBool("claim",false);
        }else if(id==(me.getGameID()+1)%3){
            player2.SetBool("claim",false);
        }else if(id==(me.getGameID()+2)%3){
            player0.SetBool("claim",false);
        }
    }

    //给自己插入地主牌
    private void AddLandlordCardsToHand()
    {
        int[] insertIndex = new int[3];     //3张地主牌插入后，在手牌中的序号/下标
        //地主牌加入手牌
        for (int i = 0; i < 3; i++)
        {
            insertIndex[i] = 0;
            card tmp = new card(landlordCard[i].getSuit(), landlordCard[i].getPoint());
            me.addCard(tmp);
        }
        //先将之前的牌模型清空
        for (int i = 0; i < 17; i++)
        {
            Destroy(me.getCards()[i].model);
        }
        //手牌排序
        me.sortCards();
        //分别找到三张牌插入的位置
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < me.getCards().Count; j++)
            {
                if (landlordCard[i].equals(me.getCards()[j]))
                {
                    insertIndex[i] = j;
                    break;
                }
            }
        }
        //显示插入地主牌的动画
        float dist = myCardsX;
        //先找到起始位置（原来的手牌留出空位，地主牌从上往下插）
        for (int i = 0; i < 20; i++, dist += myCardsDX)
        {
            int indexInPrefabs = me.getCards()[i].toInt();
            GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            me.getCards()[i].setModel(cardModel);
            me.getCards()[i].model.transform.position = transform.position + new Vector3(dist, myCardsY, myCardsZ);
            if (i == insertIndex[0] || i == insertIndex[1] || i == insertIndex[2])
            {
                me.getCards()[i].model.transform.Translate(new Vector3(0.0f, 0.0f, myCardsDX));
            }
            me.getCards()[i].model.transform.Rotate(new Vector3(1, 0, 0), -80.0f);
        }
        //再显示动画效果
        StartCoroutine(addCardAnimation(insertIndex));
    }

    //给其他玩家添加地主牌(无动画)
    private void AddLandlordCardsForPlayer(int gameID)
    {
        if (gameID == me.getGameID())
        {
            //Debug.Log("你搞错地主了");
            return;
        }
        if (gameID == (me.getGameID() + 2) % 3)     //地主是上家(左手边)
        {
            int N = leftPlayerCards.Count;
            for (int i = 0; i < N; i++)
            {
                Destroy(leftPlayerCards[i]);
            }
            leftPlayerCards.Clear();
            float ndist = otherPlayerCardsZ;
            for (int i = 0; i < N + 3; i++, ndist += otherPlayerCardsDZ)
            {
                GameObject cardBackModelLeft = Instantiate(cardBack) as GameObject;
                cardBackModelLeft.transform.position = transform.position + new Vector3(-otherPlayerCardsX, otherPlayerCardsY, ndist);
                cardBackModelLeft.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
                leftPlayerCards.Add(cardBackModelLeft);
            }
        }
        if (gameID == (me.getGameID() + 1) % 3)     //地主是下家(右手边)
        {
            int N = rightPlayerCards.Count;
            for (int i = 0; i < N; i++)
            {
                Destroy(rightPlayerCards[i]);
            }
            rightPlayerCards.Clear();
            float ndist = otherPlayerCardsZ;
            for (int i = 0; i < N + 3; i++, ndist += otherPlayerCardsDZ)
            {
                GameObject cardBackModelRight = Instantiate(cardBack) as GameObject;
                cardBackModelRight.transform.position = transform.position + new Vector3(otherPlayerCardsX, otherPlayerCardsY, ndist);
                cardBackModelRight.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
                rightPlayerCards.Add(cardBackModelRight);
            }
        }
    }

    //给自己添加地主牌动画
    private IEnumerator addCardAnimation(int[] insertIndex)
    {
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                me.getCards()[insertIndex[j]].model.transform.position += new Vector3(0, 0, -myCardsDX / 25.0f);
            }
            yield return new WaitForSeconds(0.04f);
        }
    }

    //托管出牌
    private void HangUp()
    {
        me.setNewHint(true);
        //没轮到该玩家出牌
        if (me.getGameID() != whoseTurn)
        {
            //Debug.Log("没轮到玩家" + me.getGameID() + "的托管脚本出牌");
            return;
        }
        //如果是上局的霸主,则出最小的单张
        if (me.getGameID() == lastWho)
        {
            //Debug.Log("托管脚本竟然是上一轮的霸主,那就出单张吧");
            cardcombine ncc = new cardcombine();
            ncc.add(new card(card.suit.Diamond, card.point.Card_MinusOne));
            me.hint(ncc);
            if (me.play(lastWho, lastCombine))
            {
                int N = me.getCardsToPlay().size();
                int[] combinePlayed = new int[N];
                for (int i = 0; i < N; i++)
                {
                    combinePlayed[i] = me.getCardsToPlay().getList()[i].toInt();
                }
                photonView.RPC("PlayerPlay", PhotonTargets.All, me.getGameID(), combinePlayed, N);
                if (me.ifWin())     //该玩家把牌打完了
                    photonView.RPC("PlayerWin", PhotonTargets.MasterClient, me.getGameID());
                return;
            }
            //托管逻辑出错了
            //Debug.Log("托管逻辑出错了");
            return;
        }
        me.hint(lastCombine);
        //如果没有比上家大的牌组就过
        if (me.getCardsToPlay().isEmpty())
        {
            //Debug.Log("托管脚本:没有比上家的牌可打,gg");
            photonView.RPC("PlayerPass", PhotonTargets.All, me.getGameID());
            return;
        }
        //如果有比上家大的牌组就打出
        if (me.play(lastWho, lastCombine))
        {
            //Debug.Log("托管脚本:我有比上家大的牌,打他妈的");
            int N = me.getCardsToPlay().size();
            int[] combinePlayed = new int[N];
            for (int i = 0; i < N; i++)
            {
                combinePlayed[i] = me.getCardsToPlay().getList()[i].toInt();
            }
            photonView.RPC("PlayerPlay", PhotonTargets.All, me.getGameID(), combinePlayed, N);
            if (me.ifWin())     //该玩家把牌打完了
            {
                //Debug.Log("托管脚本都赢了,你的牌也太好了吧");
                photonView.RPC("PlayerWin", PhotonTargets.MasterClient, me.getGameID());
            }
            return;
        }
        //托管逻辑出错了
        //Debug.Log("托管逻辑出错了");
        return;
    }

    //电脑出牌
    private void AIAction(int id)
    {
        cardcombine AICombine = AI[id].selectCardsToPlay();
        if (AICombine.isEmpty())
        {
            if (id != lastWho)
            {
                photonView.RPC("PlayerPass", PhotonTargets.All, id);
            }
            else
            {
                Debug.Log("轮到电脑" + id + "出牌，但它为什么不出呢？");
            }
        }
        else
        {
            if (id == lastWho || AICombine.compareTo(lastCombine) == cardcombine.CompareRes.Larger)
            {
                int N = AICombine.size();
                int[] combinePlayed = new int[N];
                for (int i = 0; i < N; i++)
                {
                    combinePlayed[i] = AICombine.getList()[i].toInt();
                }
                photonView.RPC("PlayerPlay", PhotonTargets.All, id, combinePlayed, N);
                if (AI[id].ifWin())
                {
                    photonView.RPC("PlayerWin", PhotonTargets.MasterClient, id);
                }
            }
            else
            {
                Debug.Log("电脑" + id + "出的牌不合法！");
                Debug.Log("电脑想出的牌为:" + AICombine.toString());
            }
        }
    }

    //玩家出牌
    private void PlayCard()
    {
        me.setNewHint(true);
        //没轮到该玩家出牌
        if (me.getGameID() != whoseTurn)
        {
            //Debug.Log("现在是玩家" + whoseTurn + "出牌,为什么你能按下出牌键?");
            return;
        }
        //该玩家没有选中任何牌
        if (me.getCardsToPlay().isEmpty())
        {
            //Debug.Log("请选择要打出的牌");
            return;
        }
        //该玩家出牌成功（出牌合法且大过上家）
        if (me.play(lastWho, lastCombine))
        {
            playingButton.SetActive(false);
            int N = me.getCardsToPlay().size();
            int[] combinePlayed = new int[N];
            for (int i = 0; i < N; i++)
            {
                combinePlayed[i] = me.getCardsToPlay().getList()[i].toInt();
            }
            //Debug.Log("(本机)出牌成功,你即将要打出的牌为:" + me.getCardsToPlay().toString());
            photonView.RPC("PlayerPlay", PhotonTargets.All, me.getGameID(), combinePlayed, N);
            if (me.ifWin())     //我把牌打完了
            {
                //Debug.Log("(本机)你的牌都出完了,理论上讲,你胜利了");
                photonView.RPC("PlayerWin", PhotonTargets.MasterClient, me.getGameID());
            }
            return;
        }
        //该玩家出牌失败（出牌不合法或者没有上家大）
        resetModelPosition();
        //Debug.Log("你出的牌不符合规则");
        return;
    }

    //玩家过
    private void Pass()
    {
        me.setNewHint(true);
        //没轮到该玩家出牌
        if (me.getGameID() != whoseTurn)
        {
            //Debug.Log("现在是玩家" + whoseTurn + ",为什么你能按下不要键?");
            return;
        }
        //该玩家过（不出任何牌）(该玩家必须不是上一轮的霸主)
        if (me.getGameID() != lastWho)
        {
            playingButton.SetActive(false);
            photonView.RPC("PlayerPass", PhotonTargets.All, me.getGameID());
            return;
        }
        //Debug.Log("你是上一轮的霸主,你必须至少打出一张牌");
        return;
    }

    //重置牌的位置
    public void resetModelPosition()
    {
        float dist = myCardsX;
        int N = me.getCards().Count;
        //删除之前的模型
        for (int i = 0; i < N; i++)
        {
            Destroy(me.getCards()[i].model);
        }
        //重新设置模型
        for (int i = 0; i < N; i++, dist += myCardsDX)
        {
            int indexInPrefabs = me.getCards()[i].toInt();
            me.getCards()[i].model = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            me.getCards()[i].model.transform.position = transform.position + new Vector3(dist, myCardsY, myCardsZ);
            me.getCards()[i].model.transform.Rotate(new Vector3(1, 0, 0), -80.0f);
        }
        me.getCardsToPlay().removeAll();
    }

    //RPC函数,玩家过
    [PunRPC]
    void PlayerPass(int gameID)
    {
        //photonView.RPC("logPlayerPassInfo", PhotonTargets.All, me.getGameID(), gameID);
        if (gameID == me.getGameID())
        {
            p1NotPlay.SetActive(true);
        }
        else if (gameID == (me.getGameID() + 1) % 3)
        {
            p2NotPlay.SetActive(true);
        }
        else if (gameID == (me.getGameID() + 2) % 3)
        {
            p0NotPlay.SetActive(true);
        }
        StartCoroutine(passAnimation(gameID));
        SendMessage("passAudio");
        whoseTurn = (gameID + 1) % 3;
    }

    private IEnumerator passAnimation(int gameID){
        if (gameID == me.getGameID())
        {
            player1.SetBool("notPlayCard",true);
        }
        else if (gameID == (me.getGameID() + 1) % 3)
        {
            player2.SetBool("notPlayCard",true);
        }
        else if (gameID == (me.getGameID() + 2) % 3)
        {
            player0.SetBool("notPlayCard",true);
        }
        yield return new WaitForSeconds(0.8f);
        if (gameID == me.getGameID())
        {
            player1.SetBool("notPlayCard",false);
        }
        else if (gameID == (me.getGameID() + 1) % 3)
        {
            player2.SetBool("notPlayCard",false);
        }
        else if (gameID == (me.getGameID() + 2) % 3)
        {
            player0.SetBool("notPlayCard",false);
        }
    }

    [PunRPC]
    void logPlayerPassInfo(int myID, int passID)
    {
        Debug.Log("客户端" + myID + "已收到信息:玩家" + passID + "过了");
    }

    //RPC函数,玩家出牌
    [PunRPC]
    void PlayerPlay(int gameID, int[] combine, int cardNum)
    {
        cardcombine PlayerCombine = new cardcombine();
        for (int i = 0; i < cardNum; i++)
        {
            int s, p;
            if (combine[i] == 53)
            {
                s = (int)card.suit.Red;
                p = (int)card.point.Card_RJ;
            }
            else if (combine[i] == 52)
            {
                s = (int)card.suit.Black;
                p = (int)card.point.Card_BJ;
            }
            else
            {
                s = combine[i] % 4;
                p = combine[i] / 4;
            }
            PlayerCombine.add(new card((card.suit)s, (card.point)p));
        }
        for (int i = 0; i < lastCombine.getList().Count; i++)
        {
            Destroy(lastCombine.getList()[i].model);        //删除之前打出的牌的模型
        }
        lastCombine.setCombine(PlayerCombine);    //更新牌局上最大的牌组
        SendMessage("setHasPrev", gameID != lastWho);
        SendMessage("setEffectPositionByID", gameID);
        SendMessage("playCardAudio", lastCombine);
        SendMessage("activateEffect", lastCombine);
        if (gameID == lastWho)
        {
            p0NotPlay.SetActive(false);
            p1NotPlay.SetActive(false);
            p2NotPlay.SetActive(false);
        }
        else if (gameID == me.getGameID())
        {
            p1NotPlay.SetActive(false);
        }
        else if (gameID == (me.getGameID() + 1) % 3)
        {
            p2NotPlay.SetActive(false);
        }
        else if (gameID == (me.getGameID() + 2) % 3)
        {
            p0NotPlay.SetActive(false);
        }
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < 3; i++)
            {
                if (isAI[i])
                {
                    AI[i].updateGameInfo(gameID, PlayerCombine);
                }
            }
        }
        whoseTurn = (gameID + 1) % 3;   //更新该轮到谁出牌了
        lastWho = gameID;               //更新牌局上最大的牌组是谁出的
        //photonView.RPC("logLastCombineInfo", PhotonTargets.All, me.getGameID(), gameID, combine, cardNum);
        StartCoroutine(PlayCardAnimation(gameID));
    }

    [PunRPC]
    void logLastCombineInfo(int myID,int playID, int[] combine, int N)
    {
        cardcombine last=new cardcombine();
        for (int i = 0; i < N; i++)
        {
            int s, p;
            if (combine[i] == 53)
            {
                s = (int)card.suit.Red;
                p = (int)card.point.Card_RJ;
            }
            else if (combine[i] == 52)
            {
                s = (int)card.suit.Black;
                p = (int)card.point.Card_BJ;
            }
            else
            {
                s = combine[i] % 4;
                p = combine[i] / 4;
            }
            last.add(new card((card.suit)s, (card.point)p));
        }
        Debug.Log("客户端" + myID + "已收到信息:玩家" + playID + "打出了" + last.toString());
    }

    //打牌的动画
    private IEnumerator PlayCardAnimation(int gameID)
    {
        if (gameID == me.getGameID())
        {
            removeCardsToPlay();
            float dist = 0.0f;
            float[] distf = new float[lastCombine.size()];
            float[] distp = new float[lastCombine.size()];
            if (lastCombine.size() % 2 != 0)
            {
                dist = -(lastCombine.size() / 2) * myCardsDX / 5.0f;
            }
            else
            {
                dist = -(lastCombine.size() / 2 - 1) * myCardsDX / 5.0f + myCardsDX / 10.0f;
            }
            for (int i = 0; i < lastCombine.size(); i++, dist += myCardsDX / 5.0f)
            {
                int indexInPrefabs = lastCombine.getList()[i].toInt();
                GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                lastCombine.getList()[i].setModel(cardModel);
                lastCombine.getList()[i].model.transform.position = transform.position + new Vector3(dist, myCardsY, myCardsZ + myCardsDX * 1.5f);
                lastCombine.getList()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
                distf[i] = dist;
                distp[i] = 5 * distf[i];
            }
            player1.SetBool("playCard", true);
            for (int t = 0; t < 15; t++)
            {
                for (int i = 0; i < lastCombine.size(); i++)
                {
                    lastCombine.getList()[i].model.transform.position += new Vector3((distp[i] - distf[i]) / 15.0f, 0.0f, myCardsDX / 12.0f);
                }
                yield return new WaitForSeconds(0.02f);
            }
            player1.SetBool("playCard", false);
        }
        else if (gameID == (me.getGameID() + 2) % 3)
        {
            int combineCount = lastCombine.size();
            int cardCount = leftPlayerCards.Count;
            for (int i = 0; i < cardCount; i++)
            {
                Destroy(leftPlayerCards[i]);
            }
            leftPlayerCards.Clear();
            float ndist = otherPlayerCardsZ;
            for (int i = 0; i < cardCount - combineCount; i++, ndist += otherPlayerCardsDZ)
            {
                GameObject cardBackModelLeft = Instantiate(cardBack) as GameObject;
                cardBackModelLeft.transform.position = transform.position + new Vector3(-otherPlayerCardsX, otherPlayerCardsY, ndist);
                cardBackModelLeft.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
                leftPlayerCards.Add(cardBackModelLeft);
            }
            float dist = -otherPlayerCardsX + myCardsDX * 1.5f;
            for (int i = 0; i < lastCombine.size(); i++, dist += myCardsDX / 5.0f)
            {
                int indexInPrefabs = lastCombine.getList()[i].toInt();
                GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                lastCombine.getList()[i].setModel(cardModel);
                lastCombine.getList()[i].model.transform.position = transform.position + new Vector3(dist, otherPlayerCardsY, otherPlayerCardsZ / 2.0f);
                lastCombine.getList()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            }
            player0.SetBool("playCard",true);
            for (int t = 0; t < 15; t++)
            {
                for (int i = 0; i < lastCombine.size(); i++)
                {
                    lastCombine.getList()[i].model.transform.position += new Vector3(i * myCardsDX * 4.0f / 5.0f / 15.0f, 0.0f, 0.0f);
                }
                yield return new WaitForSeconds(0.02f);
            }
            player0.SetBool("playCard",false);
        }
        else if (gameID == (me.getGameID() + 1) % 3)
        {
            int combineCount = lastCombine.size();
            int cardCount = rightPlayerCards.Count;
            for (int i = 0; i < cardCount; i++)
            {
                Destroy(rightPlayerCards[i]);
            }
            rightPlayerCards.Clear();
            float ndist = otherPlayerCardsZ;
            for (int i = 0; i < cardCount - combineCount; i++, ndist += otherPlayerCardsDZ )
            {
                GameObject cardBackModelRight = Instantiate(cardBack) as GameObject;
                cardBackModelRight.transform.position = transform.position + new Vector3(otherPlayerCardsX, otherPlayerCardsY, ndist);
                cardBackModelRight.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
                rightPlayerCards.Add(cardBackModelRight);
            }
            float dist = otherPlayerCardsX - myCardsDX * 1.5f;
            for (int i = lastCombine.size() - 1; i >= 0; i--, dist -= myCardsDX / 5.0f)
            {
                int indexInPrefabs = lastCombine.getList()[i].toInt();
                GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                lastCombine.getList()[i].setModel(cardModel);
                lastCombine.getList()[i].model.transform.position = transform.position + new Vector3(dist, otherPlayerCardsY, otherPlayerCardsZ / 2.0f);
                lastCombine.getList()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            }
            player2.SetBool("playCard",true);
            for (int t = 0; t < 15; t++)
            {
                for (int i = lastCombine.size() - 1; i >= 0; i--)
                {
                    int j = lastCombine.size() - 1 - i;
                    lastCombine.getList()[i].model.transform.position += new Vector3(-j * myCardsDX * 4.0f / 5.0f / 15.0f, 0.0f, 0.0f);
                }
                yield return new WaitForSeconds(0.02f);
            }
            player2.SetBool("playCard",false);
        }
    }

    //移除选中的牌（也就是把牌打出）
    public void removeCardsToPlay()
    {
        //先将目前所有手牌对应的模型移除
        for (int i = 0; i < me.getCards().Count; i++)
        {
            Destroy(me.getCards()[i].model);
        }
        //再将打出的牌从手牌移除
        List<card> list = me.getCardsToPlay().getList();
        int N = list.Count;
        for (int i = 0; i < N; i++)
        {
            int index = me.hasCard(list[i]);
            if (index != -1)
                me.getCards().RemoveAt(index);
        }
        me.getCardsToPlay().removeAll();
        //最后显示打出后剩余的牌
        float dist = myCardsX;
        for (int i = 0; i < me.getCards().Count; i++, dist += myCardsDX)
        {
            int indexInPrefabs = me.getCards()[i].toInt();
            me.getCards()[i].model = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            me.getCards()[i].model.transform.position = transform.position + new Vector3(dist, myCardsY, myCardsZ);
            me.getCards()[i].model.transform.Rotate(new Vector3(1, 0, 0), -80.0f);
        }
    }

    //RPC函数,某个玩家胜利
    [PunRPC]
    void PlayerWin(int gameID)
    {
        if (!PhotonNetwork.isMasterClient)		//如果函数不是由MasterClient调用，结束函数执行
            return;
        winner = gameID;
    }

    //RPC函数,结束游戏
    [PunRPC]
    void EndGame(int winner)
    {
        bool win = false;
        if (me.getGameID() == winner)
        {
            win = true;
        }
        else if (me.getGameID() != landlord && landlord != winner)
        {
            win = true;
        }
        if (win)
        {
            gm.state = GameState.GameWin;		//游戏状态切换为游戏胜利状态
        }
        else
        {
            gm.state = GameState.GameLose;		//游戏状态切换为游戏失败状态
        }
    }

    //胜利画面
    private void displayWinScene()
    {
        SendMessage("playWinScene");
        if(!beginPlay){
            beginPlay=true;
            SendMessage("playWinSong");
        }
        player1.SetBool("win",true);
        if(me.getGameID()==landlord){
            player0.SetBool("lose",true);
            player2.SetBool("lose",true);
        }else{
            if(landlord==(me.getGameID()+1)%3){
                player0.SetBool("win",true);
                player2.SetBool("lose",true);
            }else{
                player0.SetBool("lose",true);
                player2.SetBool("win",true);
            }
        }
    }

    //失败画面
    private void displayLoseScene()
    {
        SendMessage("playLoseScene");
        if(!beginPlay){
            beginPlay=true;
            SendMessage("playLoseSong");
        }
        player1.SetBool("lose",true);
        if(me.getGameID()==landlord){
            player0.SetBool("win",true);
            player2.SetBool("win",true);
        }else{
            if(landlord==(me.getGameID()+1)%3){
                player0.SetBool("lose",true);
                player2.SetBool("win",true);
            }else{
                player0.SetBool("win",true);
                player2.SetBool("lose",true);
            }
        }
    }

    //离开房间函数
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();				//玩家离开游戏房间
        PhotonNetwork.LoadLevel("GameLobby");	//加载场景GameLobby
    }

    //禁用所有按钮
    public void SetButtonInactive()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        twoClaimButton.SetActive(false);
        playingButton.SetActive(false);
        notHangUp.SetActive(false);
        hangUp.SetActive(false);
    }

    // //启用出牌面板
    // public void SetPlayingButtonActive()
    // {	
    //     playingButton.SetActive (true);					
    //     if(hangUpButton.isdown == true && notHangUpButton.isdown == false)	{
    //         SetHangUpButtonActive();
    //     }else if(hangUpButton.isdown == false && notHangUpButton == true){
    //         SetNotHangUpButtonActive();         
    //     }
    // }
    //启用托管面板
    public void SetHangUpButtonActive()
    {
        hangUp.SetActive(true);
        notHangUp.SetActive(false);
    }
    //启用停止托管面板
    public void SetNotHangUpButtonActive()
    {
        notHangUp.SetActive(true);
        hangUp.SetActive(false);
    }

}