using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sendcards : MonoBehaviour {
	public static Object[] cardPrefabs;     //54张牌的prefab模型
    public static Vector3 position;         //牌桌（invisible）的中心坐标

    //游戏状态
    public enum gameStatus
    {
        gameEnds,           //游戏结束或者未开始
        dealing,            //发牌中
        afterDeal,          //发牌结束
        claimLandlord,      //叫地主
        setLandlord,        //设置地主和地主牌
        gameStarts,         //游戏开始
        someoneWin,         //地主/农民胜利动画
    }
    public gameStatus state;

    //进入下一状态
    private void nextState()
    {
        state++;
        if ((int)state > 6)
        {
            state = (gameStatus)0;
        }
    }
    private bool dealComplete;      //发牌结束标记

	public int landlord;             //地主玩家的ID
    public int whoseTurn;            //该谁出牌了(玩家游戏编号,0 or 1 or 2)
    public int lastWho;              //牌局上最大的牌组是谁出的
    cardcombine lastCombine;         //牌局上最大的牌组

    //Cards
    card[] deck = new card[54];                 //牌库，54张牌
    card[] landlordCard = new card[3];          //地主牌，3张牌
    player[] players;                           //玩家，3个

    //Camera
    public Camera cam;

	//temporary varibles
	float height=10.0f;

	// Use this for initialization
	void Start () {
        //确定牌桌位置
        position = transform.position;

        //初始化游戏状态
        state = gameStatus.gameEnds;
		landlord = -1;
        lastWho = landlord;
        whoseTurn = landlord;
        lastCombine = new cardcombine();

		//初始化牌库
        resetDeck();

        //初始化玩家
        players=new player[3];
        for (int i = 0; i < 3; i++)
        {
            players[i] = new player();
            players[i].setGameID(i);
        }

        //导入54张牌的prefab模型
        string path = "prefab/";
        cardPrefabs = new Object[54];
        for (int i = 0; i < 54; i++)
        {
            cardPrefabs[i] = Resources.Load(path + toCardName(i), typeof(GameObject));
        }
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case gameStatus.gameEnds:
                {
                    landlord = -1;
                    whoseTurn = landlord;
                    lastWho = landlord;
                    lastCombine.removeAll();
                    if (Input.GetKeyDown(KeyCode.Space)/*发牌*/)
                    {
                        nextState();
                        dealComplete = false;
                        StartCoroutine(Deal());
                    }
                    return;
                }
            case gameStatus.dealing:
                {
                    //发牌动画阶段
                    if (dealComplete)
                    {
                        nextState();
                    }
                    return;
                }
            case gameStatus.afterDeal:
                {
                    /*在这里需要随机确定一个玩家，从他开始叫地主*/
                    nextState();
                    return;
                }
            case gameStatus.claimLandlord:
                {
                    //开始叫地主
                    nextState();
                    return;
                }
            case gameStatus.setLandlord:
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1)/*玩家1当地主*/)
                    {
                        StartCoroutine(addLandlordCardsForPlayer(0));
                        nextState();
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2)/*玩家2当地主*/)
                    {
                        StartCoroutine(addLandlordCardsForPlayer(1));
                        nextState();
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3)/*玩家3当地主*/)
                    {
                        StartCoroutine(addLandlordCardsForPlayer(2));
                        nextState();
                    }
                    return;
                }
            case gameStatus.gameStarts:
                {
                    
                    if (players[0].getHangUp())
                    {
                        if (Input.GetKeyDown(KeyCode.U))
                        {
                            players[0].disableHangUp();
                        }
                        someoneHangUp(0);
                    }
                    else
                    {
                        players[0].selectCard();
                        if (Input.GetKeyDown(KeyCode.J))//玩家0按下出牌键
                        {
                            if (someoneAction(0) == 1)
                            {
                                nextState();
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.U))
                        {
                            players[0].enableHangUp();
                        }
                    }
                    if (players[1].getHangUp())
                    {
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            players[1].disableHangUp();
                        }
                        someoneHangUp(1);
                    }
                    else
                    {
                        players[1].selectCard();
                        if (Input.GetKeyDown(KeyCode.K))//玩家1按下出牌键
                        {
                            if (someoneAction(1) == 1)
                            {
                                nextState();
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            players[1].enableHangUp();
                        }
                    }
                    if (players[2].getHangUp())
                    {
                        if (Input.GetKeyDown(KeyCode.O))
                        {
                            players[2].disableHangUp();
                        }
                        someoneHangUp(2);
                    }
                    else
                    {
                        players[2].selectCard();
                        if (Input.GetKeyDown(KeyCode.L))//玩家2按下出牌键
                        {
                            if (someoneAction(2) == 1)
                            {
                                nextState();
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.O))
                        {
                            players[2].enableHangUp();
                        }
                    }

                    /*for (int i = 0; i < 3; i++)
                    {
                        players[i].selectCard();
                    }
                    if (Input.GetKeyDown(KeyCode.K))//玩家1按下出牌键
                    {
                        if (someoneAction(1) == 1)
                        {
                            nextState();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.L))//玩家2按下出牌键
                    {
                        if (someoneAction(2) == 1)
                        {
                            nextState();
                        }
                    }*/
                    if (Input.GetKeyDown(KeyCode.H))
                    {
                        players[whoseTurn].hint(lastCombine);
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        nextState();
                    }
                    return;
                }
            case gameStatus.someoneWin:
                {
                    /*这里加上赢了的动画*/
                    clearCards();
                    resetDeck();
                    nextState();
                    return;
                }
        }
	}

	private IEnumerator Deal()   /* deal vt. 发牌 */
	{
        //先洗牌
		Shuffle();

        //发地主牌
		for (int i = 0; i < 3; i++)
		{
			landlordCard[i] = deck[i];
		}
        //显示地主牌
        float dist = -0.5f;     //发牌初始x坐标
        for (int i = 0; i < 3; i++, dist += 0.5f)
        {
            int indexInPrefabs = landlordCard[i].toInt();
            landlordCard[i].model = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            landlordCard[i].model.transform.position = transform.position + new Vector3(dist, height, -1.5f);
            landlordCard[i].model.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
        }

        //给玩家发牌
		for (int i = 0; i < 17; i++)
		{
            players[0].addCard(deck[i + 3]);
            players[1].addCard(deck[i + 20]);
            players[2].addCard(deck[i + 37]);
		}
        //给牌排序
        for (int i = 0; i < 3; i++)
        {
            players[i].sortCards();
        }
        //显示发牌动画
        dist = -4.5f;       //发牌初始x坐标
        for (int i = 0; i < 17; i++, dist += 0.5f)
        {
            for (int j = 0; j < 3; j++)
            {
                int indexInPrefabs = players[j].getCards()[i].toInt();
                GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                players[j].getCards()[i].setModel(cardModel);
                players[j].getCards()[i].model.transform.position = transform.position + new Vector3(dist, height, players[j].getGameID() - 0.5f);
                players[j].getCards()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            }
            yield return new WaitForSeconds(0.15f);
        }
        //设置发牌完成标记
        dealComplete = true;
	}

	private void Shuffle(int times=100)   /* shuffle vt. 洗牌 */
	{
		System.Random ran = new System.Random();
		int index1, index2;
        card tmp;
		for (int i = 0; i < times; i++)
		{
			index1 = ran.Next(0,54);
			index2 = ran.Next(0,54);
			tmp = deck[index1];
			deck[index1] = deck[index2];
			deck[index2] = tmp;
		}
	}

    //将地主牌加入地主手牌中
	private IEnumerator addLandlordCardsForPlayer(int landlordID)
	{
        int[] insertIndex = new int[3];     //3张地主牌插入后，在手牌中的序号/下标

		//地主牌加入手牌
        for (int i = 0; i < 3; i++)
		{
            insertIndex[i] = 0;
            card tmp = new card(landlordCard[i].getSuit(), landlordCard[i].getPoint());
            players[landlordID].addCard(tmp);
		}
        //先将之前的牌模型清空
        for (int i = 0; i < 17; i++)
        {
            Destroy(players[landlordID].getCards()[i].model);
        }

        //手牌排序
        players[landlordID].sortCards();

        //分别找到三张牌插入的位置
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < players[landlordID].getCards().Count; j++)
            {
                if (landlordCard[i].equals(players[landlordID].getCards()[j]))
                {
                    insertIndex[i] = j;
                    break;
                }
            }
        }
        
        //显示插入地主牌和翻开地主牌的动画
		float dist = -4.5f;
        //先找到起始位置（原来的手牌留出空位，地主牌从上往下插）
		for (int i = 0; i < 20; i++, dist += 0.5f)
		{
            int indexInPrefabs = players[landlordID].getCards()[i].toInt();
            GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
            players[landlordID].getCards()[i].setModel(cardModel);
            players[landlordID].getCards()[i].model.transform.position = transform.position + new Vector3(dist, height, landlordID - 0.5f);
            if (i == insertIndex[0] || i == insertIndex[1] || i == insertIndex[2])
            {
                players[landlordID].getCards()[i].model.transform.Translate(new Vector3(0.0f, 0.0f, 0.5f));
            }
            players[landlordID].getCards()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
	    }
        //再显示动画效果
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                players[landlordID].getCards()[insertIndex[j]].model.transform.position += new Vector3(0, 0, -0.02f);
                landlordCard[j].model.transform.Rotate(new Vector3(1, 0, 0), -7.2f);
            }
            yield return new WaitForSeconds(0.04f);
        }

        //更新状态值
        landlord = landlordID;
        lastWho = landlord;
        whoseTurn = landlord;
	}

    //清除屏幕内的所有牌
    private void clearCards()
    {
        //清除玩家剩余手牌
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < players[i].getCards().Count; j++)
            {
                Destroy(players[i].getCards()[j].model);
            }
            players[i].removeCards();
        }
        //清除地主牌
        for (int i = 0; i < 3; i++)
        {
            Destroy(landlordCard[i].model);
        }
        //清除lastCombine
        for (int i = 0; i < lastCombine.getList().Count; i++)
        {
            Destroy(lastCombine.getList()[i].model);
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

    //将牌的名字转成整数
    private int intValueof(string name)
    {
        int p = 0;
        int s = 0;
        string point = "";
        string suit = "";
        point = name.Substring(0, 1);
        switch (point)
        {
            case "3":
                p = 0;
                suit = name.Substring(1, 1);
                break;
            case "4":
                p = 1;
                suit = name.Substring(1, 1);
                break;
            case "5":
                p = 2;
                suit = name.Substring(1, 1);
                break;
            case "6":
                p = 3;
                suit = name.Substring(1, 1);
                break;
            case "7":
                p = 4;
                suit = name.Substring(1, 1);
                break;
            case "8":
                p = 5;
                suit = name.Substring(1, 1);
                break;
            case "9":
                p = 6;
                suit = name.Substring(1, 1);
                break;
            case "1":
                p = 7;
                suit = name.Substring(2, 1);
                break;
            case "J":
                if (name.Substring(1, 1).Equals("o"))
                {
                    p = 13;
                    suit = name.Substring(5, 1);
                }
                else
                {
                    p = 8;
                    suit = name.Substring(1, 1);
                }
                break;
            case "Q":
                p = 9;
                suit = name.Substring(1, 1);
                break;
            case "K":
                p = 10;
                suit = name.Substring(1, 1);
                break;
            case "A":
                p = 11;
                suit = name.Substring(1, 1);
                break;
            case "2":
                p = 12;
                suit = name.Substring(1, 1);
                break;
        }
        switch (suit)
        {
            case "S":
                s = 3;
                break;
            case "H":
                s = 2;
                break;
            case "C":
                s = 1;
                break;
            case "D":
                s = 0;
                break;
            case "R":
                s = 1;
                break;
            case "B":
                s = 0;
                break;
        }
        return p * 4 + s;
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

    //某人出牌
    //  (返回值 -1:失败 0:成功但尚未胜利 1:成功且胜利了)
    //  参数分别为(第一个参数是出牌玩家的编号 0 or 1 or 2,第二个参数是出的牌)
    /*
     * if 轮不到编号为gameID的玩家出牌
     *    则返回-1
     * if 出牌者不是当前牌局上的霸主 且 出牌着弃权
     *    更新轮谁出牌
     *    返回0
     * 利用cards参数构造出一个牌组combine
     * 调用索引为gameID的玩家对象的出牌函数,记录返回值res
     * if res==true:
     * {
     *    更新该轮到谁出牌了
     *    更新牌局上最大的牌组是谁出的
     *    更新牌局上最大的牌组
     *    若该玩家已经没有手牌了
     *        返回1
     *    反之,返回0
     * }
     * 返回 -1
    */
    private int someoneAction(int gameID)
    {
        players[gameID].setNewHint(true);

        //没轮到该玩家出牌
        if (gameID != whoseTurn)
        {
            players[gameID].resetModelPosition();
            Debug.Log("现在是玩家" + whoseTurn + "出牌");
            return -1;
        }

        //该玩家过（不出任何牌）
        if(players[gameID].getCardsToPlay().isEmpty() && gameID!=lastWho)
        {
            whoseTurn = (gameID + 1) % 3;
            players[gameID].resetModelPosition();
            return 0;
        }

        //该玩家出牌成功（出牌合法且大过上家）
        if (players[gameID].play(lastWho, lastCombine))
        {
            whoseTurn = (gameID + 1) % 3;   //更新该轮到谁出牌了
            lastWho = gameID;               //更新牌局上最大的牌组是谁出的
            for (int i = 0; i < lastCombine.getList().Count; i++)
            {
                Destroy(lastCombine.getList()[i].model);        //删除之前的牌的模型
            }
            lastCombine.setCombine(players[gameID].getCardsToPlay());    //更新牌局上最大的牌组
            float dist = -1.5f;
            for (int i = 0; i < lastCombine.getList().Count; i++, dist += 0.5f)
            {
                int indexInPrefabs = lastCombine.getList()[i].toInt();
                GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                lastCombine.getList()[i].setModel(cardModel);
                lastCombine.getList()[i].model.transform.position = transform.position + new Vector3(dist, height, -2.5f);
                lastCombine.getList()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            }
            Debug.Log("cards played: "+lastCombine.toString());
            players[gameID].removeCardsToPlay();
            if(players[gameID].ifWin())     //该玩家把牌打完了
                return 1;
            return 0;
        }

        //该玩家出牌失败（出牌不合法或者没有上家大）
        players[gameID].resetModelPosition();
        return -1;
    }

    private int someoneHangUp(int gameID)
    {
        players[gameID].setNewHint(true);

        //没轮到该玩家出牌
        if (gameID != whoseTurn)
        {
            players[gameID].resetModelPosition();
            return -1;
        }

        //如果是上局的霸主,则出最小的单张
        if (gameID == lastWho)
        {
            cardcombine ncc = new cardcombine();
            ncc.add(new card(card.suit.Diamond, card.point.Card_MinusOne));
            players[gameID].hint(ncc);
            if (players[gameID].play(lastWho, lastCombine))
            {
                whoseTurn = (gameID + 1) % 3;   //更新该轮到谁出牌了
                lastWho = gameID;               //更新牌局上最大的牌组是谁出的
                for (int i = 0; i < lastCombine.getList().Count; i++)
                {
                    Destroy(lastCombine.getList()[i].model);        //删除之前的牌的模型
                }
                lastCombine.setCombine(players[gameID].getCardsToPlay());    //更新牌局上最大的牌组
                float dist = -1.5f;
                for (int i = 0; i < lastCombine.getList().Count; i++, dist += 0.5f)
                {
                    int indexInPrefabs = lastCombine.getList()[i].toInt();
                    GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                    lastCombine.getList()[i].setModel(cardModel);
                    lastCombine.getList()[i].model.transform.position = transform.position + new Vector3(dist, height, -2.5f);
                    lastCombine.getList()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
                }
                Debug.Log("cards played: " + lastCombine.toString());
                players[gameID].removeCardsToPlay();
                if (players[gameID].ifWin())     //该玩家把牌打完了
                    return 1;
                return 0;
            }
            //托管逻辑出错了
            return -1;
        }

        //如果没有比上家大的牌组就过
        players[gameID].hint(lastCombine);
        if (players[gameID].getCardsToPlay().isEmpty())
        {
            whoseTurn = (gameID + 1) % 3;
            players[gameID].resetModelPosition();
            return 0;
        }

        //如果有比上家大的牌组就打出
        if (players[gameID].play(lastWho, lastCombine))
        {
            whoseTurn = (gameID + 1) % 3;   //更新该轮到谁出牌了
            lastWho = gameID;               //更新牌局上最大的牌组是谁出的
            for (int i = 0; i < lastCombine.getList().Count; i++)
            {
                Destroy(lastCombine.getList()[i].model);        //删除之前的牌的模型
            }
            lastCombine.setCombine(players[gameID].getCardsToPlay());    //更新牌局上最大的牌组
            float dist = -1.5f;
            for (int i = 0; i < lastCombine.getList().Count; i++, dist += 0.5f)
            {
                int indexInPrefabs = lastCombine.getList()[i].toInt();
                GameObject cardModel = Instantiate(cardPrefabs[indexInPrefabs]) as GameObject;
                lastCombine.getList()[i].setModel(cardModel);
                lastCombine.getList()[i].model.transform.position = transform.position + new Vector3(dist, height, -2.5f);
                lastCombine.getList()[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
            }
            Debug.Log("cards played: " + lastCombine.toString());
            players[gameID].removeCardsToPlay();
            if (players[gameID].ifWin())     //该玩家把牌打完了
                return 1;
            return 0;
        }

        //托管逻辑出错了
        players[gameID].resetModelPosition();
        return -1;
    }
}
