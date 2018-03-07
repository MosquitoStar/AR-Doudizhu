using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player {
    //个人信息
    /*
     * 虚位以待
    */

    //游戏数据
    private int gameID;                 //游戏中的编号
    private bool isLandlord;            //是否是地主
    private List<card> cards;           //玩家手牌
    private cardcombine cardsToPlay;    //玩家选中的牌
    private bool hangUp;                //是否托管

    //跟“提示”相关的变量
    private cardcombine lastHint;
    private bool newHint;

    //判断手牌中是否有这张牌
    public int hasCard(card c)
    {
        for (int i = 0; i < cards.Count; i++)
            if (c.equals(cards[i]))
                return i;
        return -1;
    }

    //判断手牌中是否包含牌组    
    private bool hasCombine(cardcombine combine)
    {
        List<card> list=combine.getList();
        int N = cards.Count;
        for (int i = 0; i < N; i++)
            if (hasCard(list[i]) == -1)
                return false;
        return true;
    }

    //判断手牌中是否有相同大小的牌组
    private bool hasCombineEquals(cardcombine combine)
    {
        if(combine.getList().Count>cards.Count){
            return false;
        }
        cardcombine.Type combineType=combine.getType();
        if(combineType == cardcombine.Type.Pass || combineType == cardcombine.Type.Unknown)
        {
            return true;
        }
        int[] count=new int[15];
        for(int i=0;i<15;i++){
            count[i]=0;
        }
        for(int i=0;i<cards.Count;i++){
            count[(int)cards[i].getPoint()]++;
        }
        card.point combinePoint=combine.representPoint(combineType);
        if (combineType==cardcombine.Type.Three_One||combineType==cardcombine.Type.Three_Two||
            combineType==cardcombine.Type.Four_Two)
        {
            for(int i=0;i<combine.size();i++)
            {
                if (combine.getList()[i].getPoint()==combinePoint)
                {
                    count[(int)combine.getList()[i].getPoint()]--;
                }
            }
        }
        else if (combineType==cardcombine.Type.Plane_Single||combineType==cardcombine.Type.Plane_Pair)
        {
            int[] ccount=new int[15];
            for(int i=0;i<15;i++){
                ccount[(int)combine.getList()[i].getPoint()]++;
            }
            for(int i=0;i<combine.size();i++)
            {
                if (ccount[(int)combine.getList()[i].getPoint()] == 3)
                {
                    count[(int)combine.getList()[i].getPoint()]--;
                }
            }
        }
        else
        {
            for(int i=0;i<combine.size();i++)
            {
                count[(int)combine.getList()[i].getPoint()]--;
            }
        }
        foreach(int i in count)
        {
            if(i<0)
            {
                return false;
            }
        }
        if (combineType==cardcombine.Type.Three_One||combineType==cardcombine.Type.Three_Two)
        {
            for(int i=0;i<15;i++){
                if(i!=(int)combinePoint){
                    if(count[i]>=combine.getList().Count-3){
                        return true;
                    }
                }
            }
            return false;
        }
        else if (combineType==cardcombine.Type.Four_Two)
        {
            int cnt=0;
            for(int i=0;i<15;i++){
                if(i!=(int)combinePoint){
                    if(count[i]>=1){
                        cnt++;
                    }
                }
            }
            if(cnt>=2){
                return true;
            }
            return false;
        }
        else if (combineType==cardcombine.Type.Plane_Single||combineType==cardcombine.Type.Plane_Pair)
        {
            int plNum=combine.size()/4;
            int tail=1;
            if(combineType==cardcombine.Type.Plane_Pair){
                plNum=combine.size()/5;
                tail=2;
            }
            int cnt=0;
            for(int i=0;i<15;i++){
                for(int j=0;j<plNum;j++){
                    if(i!=(int)combinePoint-j){
                        if(count[i]>=tail){
                            cnt++;
                        }
                    }
                }
            }
            if(cnt>=plNum){
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    private cardcombine getCombineEquals(cardcombine combine)
    {
        cardcombine.Type combineType = combine.getType();
        if (combineType == cardcombine.Type.Pass || combineType == cardcombine.Type.Unknown)
        {
            return new cardcombine();
        }
        card.point combinePoint = combine.representPoint(combineType);
        bool[] get = new bool[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            get[i] = false;
        }
        if (combineType==cardcombine.Type.Single||combineType==cardcombine.Type.Pair||
            combineType==cardcombine.Type.Straight||combineType==cardcombine.Type.PairStraight||
            combineType==cardcombine.Type.Three||combineType==cardcombine.Type.Boom||
            combineType == cardcombine.Type.Plane||combineType==cardcombine.Type.KingBoom)
        {
            cardcombine ncc = new cardcombine();
            for (int i = 0; i < combine.size(); i++)
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    if (cards[j].pointsEquals(combine.getList()[i]) && !get[j])
                    {
                        ncc.add(cards[j]);
                        get[j] = true;
                        break;
                    }
                }
            }
            return ncc;
        }
        int[] count = new int[15];
        for (int i = 0; i < 15; i++)
        {
            count[i] = 0;
        }
        for (int i = 0; i < cards.Count; i++)
        {
            count[(int)cards[i].getPoint()]++;
        }
        if (combineType == cardcombine.Type.Three_One || combineType == cardcombine.Type.Three_Two)
        {
            cardcombine ncc = new cardcombine();
            int tail = combine.size() - 3;
            for (int i = 0; i < combine.size(); i++)
            {
                if (combine.getList()[i].getPoint() != combinePoint)
                {
                    continue;
                }
                for (int j = 0; j < cards.Count; j++)
                {
                    if (cards[j].pointsEquals(combine.getList()[i]) && !get[j])
                    {
                        ncc.add(cards[j]);
                        get[j] = true;
                        break;
                    }
                }
            }
            //有单张(或对子)就选单张(或对子)
            for (int i = 0; i < 15; i++)
            {
                if (count[i] == tail)
                {
                    for (int k = 0; k < tail; k++)
                    {
                        for (int j = 0; j < cards.Count; j++)
                        {
                            if (cards[j].getPoint() == (card.point)i && !get[j])
                            {
                                ncc.add(cards[j]);
                                get[j] = true;
                                break;
                            }
                        }
                    }
                    return ncc;    
                }
            }
            //没有单张(或对子)就选最小的
            for (int i = 0; i < 15; i++)
            {
                if (count[i] > tail && combinePoint != (card.point)i)
                {
                    for (int k = 0; k < tail; k++)
                    {
                        for (int j = 0; j < cards.Count; j++)
                        {
                            if (cards[j].getPoint() == (card.point)i && !get[j])
                            {
                                ncc.add(cards[j]);
                                get[j] = true;
                                break;
                            }
                        }
                    }
                    return ncc;
                }
            }
        }
        if (combineType == cardcombine.Type.Four_Two)
        {
            cardcombine ncc = new cardcombine();
            for (int i = 0; i < combine.size(); i++)
            {
                if (combine.getList()[i].getPoint() != combinePoint)
                {
                    continue;
                }
                for (int j = 0; j < cards.Count; j++)
                {
                    if (cards[j].pointsEquals(combine.getList()[i]) && !get[j])
                    {
                        ncc.add(cards[j]);
                        get[j] = true;
                        break;
                    }
                }
            }
            int cnt = 0;
            bool find = false;
            //有两张单张就选两张单张
            for (int n = 0; n < 2; n++)
            {
                find = false;
                for (int i = 0; i < 15; i++)
                {
                    if (count[i] == 1)
                    {
                        for (int j = 0; j < cards.Count; j++)
                        {
                            if (cards[j].getPoint() == (card.point)i && !get[j])
                            {
                                ncc.add(cards[j]);
                                get[j] = true;
                                cnt++;
                                find = true;
                                break;
                            }
                        }
                    }
                    if (find)
                    {
                        break;
                    }
                }
            }
            //没有两张单张就选最小的
            for (int m = 0; m < 2 - cnt; m++)
            {
                find = false;
                for (int i = 0; i < 15; i++)
                {
                    if (count[i] > 1 && combinePoint != (card.point)i)
                    {
                        for (int j = 0; j < cards.Count; j++)
                        {
                            if (cards[j].getPoint() == (card.point)i && !get[j])
                            {
                                ncc.add(cards[j]);
                                get[j] = true;
                                cnt++;
                                find = true;
                                break;
                            }
                        }
                    }
                    if (find)
                    {
                        break;
                    }
                }
            }
            return ncc;
        }
        if (combineType == cardcombine.Type.Plane_Single || combineType == cardcombine.Type.Plane_Pair)
        {
            cardcombine ncc = new cardcombine();
            int plNum = combine.size() / 4;
            int tail = 1;
            if (combineType == cardcombine.Type.Plane_Pair)
            {
                plNum = combine.size() / 5;
                tail = 2;
            }
            List<card.point> points = new List<card.point>();
            for (int i = 0; i < plNum; i++)
            {
                points.Add(combinePoint - i);
            }
            for (int i = 0; i < combine.size(); i++)
            {
                if (points.Contains(combine.getList()[i].getPoint()))
                {
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (cards[j].pointsEquals(combine.getList()[i]) && !get[j])
                        {
                            ncc.add(cards[j]);
                            get[j] = true;
                            break;
                        }
                    }
                }
            }
            //有单张(或对子)就选单张(或对子)
            int cnt = 0;
            bool find = false;
            for (int n = 0; n < plNum; n++)
            {
                find = false;
                for (int i = 0; i < 15; i++)
                {
                    if (count[i] == tail)
                    {
                        for (int k = 0; k < tail; k++)
                        {
                            for (int j = 0; j < cards.Count; j++)
                            {
                                if (cards[j].getPoint() == (card.point)i && !get[j])
                                {
                                    ncc.add(cards[j]);
                                    get[j] = true;
                                    cnt++;
                                    find = true;
                                    break;
                                }
                            }
                        }
                            
                    }
                    if (find)
                    {
                        break;
                    }
                }
            }
            //没有单张(或对子)就选最小的
            for (int m = 0; m < plNum - cnt; m++)
            {
                find = false;
                for (int i = 0; i < 15; i++)
                {
                    if (count[i] > tail && !points.Contains((card.point)i))
                    {
                        for (int k = 0; k < tail; k++)
                        {
                            for (int j = 0; j < cards.Count; j++)
                            {
                                if (cards[j].getPoint() == (card.point)i && !get[j])
                                {
                                    ncc.add(cards[j]);
                                    get[j] = true;
                                    cnt++;
                                    find = true;
                                    break;
                                }
                            }
                        }

                    }
                    if (find)
                    {
                        break;
                    }
                }
            }
            return ncc;
        }
        return new cardcombine();
    }

    private cardcombine next(int count, card.point combinePoint)
    {
        cardcombine ncc = new cardcombine();
        if (combinePoint >= card.point.Card_2 && count > 1)
        {
            return ncc;
        }
        if (combinePoint == card.point.Card_RJ)
        {
            return ncc;
        }
        for (int i = 0; i < count; i++)
        {
            ncc.add(new card((card.suit)i, (card.point)((int)combinePoint + 1)));
        }
        return ncc;
    }

    private cardcombine next_d(int count, int same, card.point combinePoint)
    {
        cardcombine ncc = new cardcombine();
        if (same > 3 || same < 1)
        {
            return ncc;
        }
        if (same == 1 && count < 5)
        {
            return ncc;
        }
        if (combinePoint >= card.point.Card_A)
        {
            return ncc;
        }
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < same; j++)
            {
                ncc.add(new card((card.suit)j, combinePoint - i + 2));
            }
        }
        return ncc;
    }

    private cardcombine next_p(cardcombine lastCombine)
    {
        cardcombine.Type combineType = lastCombine.getType();
        card.point combinePoint = lastCombine.representPoint(combineType);
        int cardNum = lastCombine.getList().Count;
        if (combineType == cardcombine.Type.Single)
        {
            return next(1, combinePoint);
        }
        if (combineType == cardcombine.Type.Pair)
        {
            return next(2, combinePoint);
        }
        if (combineType == cardcombine.Type.Three)
        {
            return next(3, combinePoint);
        }
        if (combineType == cardcombine.Type.Boom)
        {
            return next(4, combinePoint);
        }
        if (combineType == cardcombine.Type.Straight)
        {
            return next_d(cardNum, 1, combinePoint);
        }
        if (combineType == cardcombine.Type.PairStraight)
        {
            return next_d(cardNum, 2, combinePoint);
        }
        if (combineType == cardcombine.Type.Plane)
        {
            return next_d(cardNum, 3, combinePoint);
        }
        if (combineType == cardcombine.Type.Three_One)
        {
            cardcombine three = next(3, combinePoint);
            if (three.isEmpty())
            {
                return new cardcombine();
            }
            card one = new card(card.suit.Diamond, card.point.Card_3);
            three.add(one);
            return three;
        }
        if (combineType == cardcombine.Type.Three_Two)
        {
            cardcombine three = next(3, combinePoint);
            if (three.isEmpty())
            {
                return new cardcombine();
            }
            card two1 = new card(card.suit.Diamond, card.point.Card_3);
            card two2 = new card(card.suit.Club, card.point.Card_3);
            three.add(two1);
            three.add(two2);
            return three;
        }
        if (combineType == cardcombine.Type.Plane_Single)
        {
            cardcombine plane = next_d(cardNum / 4, 3, combinePoint);
            if (plane.isEmpty())
            {
                return new cardcombine();
            }
            for (int i = 0; i < cardNum / 4; i++)
            {
                card one = new card(card.suit.Diamond, (card.point)i);
                plane.add(one);
            }
            return plane;
        }
        if (combineType == cardcombine.Type.Plane_Pair)
        {
            cardcombine plane = next_d(cardNum / 5, 3, combinePoint);
            if (plane.isEmpty())
            {
                return new cardcombine();
            }
            for (int i = 0; i < cardNum / 5; i++)
            {
                card two1 = new card(card.suit.Diamond, (card.point)i);
                card two2 = new card(card.suit.Club, (card.point)i);
                plane.add(two1);
                plane.add(two2);
            }
            return plane;
        }
        if (combineType == cardcombine.Type.Four_Two)
        {
            cardcombine four = next(4, combinePoint);
            if (four.isEmpty())
            {
                return new cardcombine();
            }
            card one1 = new card(card.suit.Diamond, card.point.Card_3);
            card one2 = new card(card.suit.Diamond, card.point.Card_4);
            four.add(one1);
            four.add(one2);
            return four;
        }
        return new cardcombine();
    }

    private cardcombine cardcombineLargerThan(cardcombine lastCombine)
    {
        //Debug.Log("call cclt(" + lastCombine.toString() + ")");
        if (lastCombine.isEmpty())
        {
            return new cardcombine();
        }
        cardcombine.Type combineType=lastCombine.getType();
        card.point combinePoint=lastCombine.representPoint(combineType);
        if (combineType == cardcombine.Type.Unknown || combineType == cardcombine.Type.KingBoom)
        {
            return new cardcombine();
        }
        if (combineType == cardcombine.Type.Boom)
        {
            if (combinePoint == card.point.Card_2)
            {
                cardcombine kingBoom=new cardcombine();
                kingBoom.add(new card(card.suit.Black,card.point.Card_BJ));
                kingBoom.add(new card(card.suit.Red,card.point.Card_RJ));
                if (hasCombineEquals(kingBoom))
                {
                    return getCombineEquals(kingBoom);
                }
                return new cardcombine();
            }
            cardcombine nextBoom=next(4,combinePoint);
            if (!hasCombineEquals(nextBoom))
            {
                return cardcombineLargerThan(nextBoom);
            }
            return getCombineEquals(nextBoom);
        }
        cardcombine nextCardcombine = next_p(lastCombine);
        //Debug.Log("nxcc = " + nextCardcombine.toString());
        if (nextCardcombine.isEmpty())
        {
            cardcombine m1Boom = new cardcombine();
            m1Boom.add(new card(card.suit.Diamond, card.point.Card_MinusOne));
            m1Boom.add(new card(card.suit.Club, card.point.Card_MinusOne));
            m1Boom.add(new card(card.suit.Heart, card.point.Card_MinusOne));
            m1Boom.add(new card(card.suit.Spades, card.point.Card_MinusOne));
            return cardcombineLargerThan(m1Boom);
        }
        if (!hasCombineEquals(nextCardcombine))
        {
            //Debug.Log("Do not have nxcc");
            return cardcombineLargerThan(nextCardcombine);
        }
        return getCombineEquals(nextCardcombine);
    }

    public player()
    {
        cards = new List<card>();
        cardsToPlay = new cardcombine();
        isLandlord = false;
        gameID = -1;
        lastHint = new cardcombine();
        newHint = true;
        hangUp = false;
    }
    //get
    public int getGameID()
    {
        return gameID;
    }
    public bool getIsLandlord()
    {
        return isLandlord;
    }
    public List<card> getCards()
    {
        return cards;
    }
    public cardcombine getCardsToPlay()
    {
        return cardsToPlay;
    }
    public bool getHangUp()
    {
        return hangUp;
    }

    //set
    public void setGameID(int ID){
        gameID=ID;
    }
    public void setIsLandlord(bool ifLandlord)
    {
        isLandlord = ifLandlord;
    }
    public void setCards(List<card> c)
    {
        cards.Clear();
        foreach (card ca in c)
        {
            cards.Add(ca);
        }
    }
    public void setNewHint(bool nh)
    {
        newHint = nh;
    }
    public void enableHangUp()
    {
        hangUp = true;
    }
    public void disableHangUp()
    {
        hangUp = false;
    }

    //添加手牌
    public void addCard(card c)
    {
        cards.Add(c);
    }

    //选中手牌
    public void selectCard()
    {
        string cardname = "";
        bool isClicked = false;
        if (Input.GetMouseButtonDown(0)/*单击选中牌*/)
        {
            //检测点中了哪张牌，选中牌动画
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (!hit.transform.name.Equals("Plane"))
                {
                    cardname = hit.transform.name;
                    isClicked = true;
                }
            }
        }
        if (!isClicked)
            return;
        int index = 0;
        bool cardSelected = false;
        //判断点中的牌是哪个玩家手中的哪一张牌
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].model.transform.name.Equals(cardname))
            {
                index = i;
                cardSelected = true;
            }
        }
        if(!cardSelected){
            return;
        }
        //执行选中牌的动画效果
        if (cardsToPlay.hasCard(cards[index]))
        {
            if (cardsToPlay.hasCard(cards[index]))
            {
                cards[index].model.transform.position += new Vector3(0, 0, -dou.myCardsDX * 0.8f);
                cardsToPlay.remove(cards[index]);
            }
        }
        else
        {
            if (!cardsToPlay.hasCard(cards[index]))
            {
                cards[index].model.transform.position += new Vector3(0, 0, dou.myCardsDX * 0.8f);
                cardsToPlay.add(cards[index]);
            }
        }
    }

    //提示
    public void hint(cardcombine lastCombine)
    {
        List<card> cardChosen = cardsToPlay.getList();
        for (int i = 0; i < cardChosen.Count; i++)
        {
            if (cardsToPlay.hasCard(cardChosen[i]))
            {
                cardChosen[i].model.transform.position += new Vector3(0, 0, -0.3f);
            }
        }
        cardsToPlay.removeAll();
        cardcombine cc;
        if (newHint)
        {
            newHint = false;
            //Debug.Log("lastCombine: " + lastCombine.toString());
            cc = cardcombineLargerThan(lastCombine);
            if (cc.isEmpty())
            {
                Debug.Log("没有比上家更大的牌");
                return;
            }
        }
        else
        {
            cc = cardcombineLargerThan(lastHint);
            if (cc.isEmpty())
            {
                cc = cardcombineLargerThan(lastCombine);
            }
        }
        for (int i = 0; i < cc.size(); i++)
        {
            Debug.Log(cc.getList()[i].toString());
            if (!cardsToPlay.hasCard(cc.getList()[i]))
            {
                cc.getList()[i].model.transform.position += new Vector3(0, 0, 0.3f);
                cardsToPlay.add(cc.getList()[i]);
            }
        }
        lastHint = cc;
    }

    //清除玩家所有手牌
    public void removeCards()
    {
        cards.Clear();
        cardsToPlay.removeAll();
    }
    
    //玩家手牌排序（按牌面大小从小到大）
    public void sortCards()
    {
        List<card> tmpC = new List<card>();
        int cardsNum = cards.Count;
        while (tmpC.Count < cardsNum)
        {
            int minvalue = cards[0].toInt();
            int minindex = 0;
            card min = cards[0];
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].toInt() < minvalue)
                {
                    minvalue = cards[i].toInt();
                    minindex = i;
                    min = cards[i];
                }
            }
            tmpC.Add(min);
            cards.Remove(min);
        }
        cards.Clear();
        for (int i = 0; i < tmpC.Count; i++)
        {
            cards.Add(tmpC[i]);
        }
    }

    //移除选中的牌（也就是把牌打出）       注:该函数已过时
    public void removeCardsToPlay()
    {
        //先将目前所有手牌对应的模型移除
        for (int i = 0; i < cards.Count; i++)
        {
            Object.Destroy(cards[i].model);
        }
        //再将打出的牌从手牌移除
        List<card> list = cardsToPlay.getList();
        int N = list.Count;
        for (int i = 0; i < N; i++)
        {
            int index = hasCard(list[i]);
            if (index != -1)
                cards.RemoveAt(index);
        }
        cardsToPlay.removeAll();
        //最后显示打出后剩余的牌
        float dist = -4.5f;
        for (int i = 0; i < cards.Count; i++, dist += 0.5f)
        {
            int indexInPrefabs = this.cards[i].toInt();
            this.cards[i].model = Object.Instantiate(dou.cardPrefabs[indexInPrefabs]) as GameObject;
            this.cards[i].model.transform.position = dou.position + new Vector3(dist, 0.0f, -2.5f);
            this.cards[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
        }
    }

    //（出牌失败后）重置牌模型位置    注:该函数已过时
    public void resetModelPosition()
    {
        float dist = -4.5f;
        float height = 10.0f;
        //删除之前的模型
        for (int i = 0; i < cards.Count; i++ )
        {
            Object.Destroy(cards[i].model);
        }
        //重新设置模型
        for (int i = 0; i < cards.Count; i++, dist += 0.5f)
        {
            int indexInPrefabs = this.cards[i].toInt();
            this.cards[i].model = Object.Instantiate(dou.cardPrefabs[indexInPrefabs]) as GameObject;
            this.cards[i].model.transform.position = dou.position + new Vector3(dist, height, this.gameID - 0.5f);
            this.cards[i].model.transform.Rotate(new Vector3(1, 0, 0), -90.0f);
        }
        cardsToPlay.removeAll();
    }

    //判断玩家是否胜利
    public bool ifWin()
    {
        return cards.Count == 0;
    }

    //出牌,参数分别表示(自己要出的牌组,牌局上当前最大的牌组是谁出的,牌局上当前最大的牌组)
    /*
    * 如果本人手牌中没有 要出的那个牌组,则出牌失败
    * if 自己==牌局上的当前霸主( 即上轮自己出牌后,轮了一圈没人接,又轮到自己了 )
    * {
    *    若自己要出的牌是 弃权过 or 非法牌型
    *      则出牌失败
    *    反之:{ 从自己手牌中移除要出的牌组,出牌成功 }
    * }
    * 反之 if 自己要出的牌组 打得过 牌局上称霸的牌组
    * {
    *    从自己手牌中移除要出的牌组
    *    出牌成功
    * }
    * 反之 出牌失败
    */
    public bool play(int thID, cardcombine thCC)
    {
        //Debug.Log("Player" + gameID + "'s cards to play: " + cardsToPlay.toString());
        //Debug.Log(cardsToPlay.getType());
        if(gameID==thID){
            cardcombine.Type type=cardsToPlay.getType();
            if(type==cardcombine.Type.Pass||type==cardcombine.Type.Unknown)
                return false;
            return true;
        }
        else if(cardsToPlay.compareTo(thCC)==cardcombine.CompareRes.Larger){
            return true;
        }
        return false;
    }
}
