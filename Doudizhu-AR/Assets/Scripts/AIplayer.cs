using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer
{

    List<card> myCards; //手牌List
    int[] myCardMarker; //数组统计手牌

    int lastPlayerNo;//场上最近一次打出牌的玩家编号
    cardcombine lastCombine;//场上最近一次打出的牌或牌组


    //所有玩家编号分别为0、1、2
    int dizhuNo;//地主编号
    int myPlayerNo;//AI玩家编号

    //构造函数
    public AIPlayer()
    {
        myCards = new List<card>();
        lastCombine = new cardcombine();
        myCardMarker = new int[15];
    }

    //设置玩家编号 (游戏开始时调用)
    public void setPlayerNo(int myNo)
    {
        myPlayerNo = myNo;
    }

    //获取手牌 (发牌时调用)
    public void pickUpHand(List<card> hand)
    {
        myCards.Clear();

        for (int i = 0; i < 15; i++){
            myCardMarker[i] = 0;
        }

        for (int i = 0; i < hand.Count; i++)
        {
            card newcard = new card(hand[i].getSuit(), hand[i].getPoint());
            myCards.Add(newcard);
            
            myCardMarker[(int)hand[i].getPoint()]++;
        }

        // foreach(card c in myCards){
        //     Debug.Log(myPlayerNo + "After hand:" + c.toString());
        // }
    }

    //获取地主编号 (叫完地主后调用)
    public void setDizhuNo(int dizhu)
    {
        dizhuNo = dizhu;
        lastPlayerNo = dizhu;
    }

    public void addLandlordCards(List<card> clist)
    {
        foreach (card c in clist)
        {
            card newcard = new card(c.getSuit(), c.getPoint());
            myCards.Add(newcard);
            
            
            myCardMarker[(int)c.getPoint()]++;
        }

        // foreach(card c in myCards){
        //     Debug.Log(myPlayerNo + "After lld:" + c.toString());
        // }
    }

    //更新游戏信息(每当一个玩家出牌后调用，玩家过则不调用)
    public void updateGameInfo(int playerNo, cardcombine combine)
    {
        lastPlayerNo = playerNo;
        lastCombine.setCombine(combine);
        //Debug.Log("ai receive info: player" + playerNo + " plays " + combine.toString());
    }


    //计算牌组分数
    public int getScore(int[] cards)
    {
        int score = 0;
        int total = 0;
        int cnt = 0;

        for (int i = 0; i < 15; i++)
        {
            total += cards[i];
        }

        if (cards[13] > 0)
            score += 100;
        if (cards[14] > 0)
            score += 150;

        for (int i = 0; i < 13; ++i)
        {
            if (cards[i] == 4)
            {
                score += 300;
                cards[i] = 0;
            }
            if (cards[i] == 3)
            {
                score += 100;
            }
        }


        for (int i = 0; i < 13; ++i)
        {
            if (cards[i] > 0)
            {
                cnt++;
            }
            else
            {
                if (cnt >= 50)
                    score += cnt * 5;
                cnt = 0;
            }
        }

        //score /= total;

        return score;
    }


    public cardcombine checkSeq()
    {
        cardcombine result = new cardcombine();
        int begin = 0, length = 0;

        //三连
        for (int i = 0; i < 12; ++i)
        {
            if (myCardMarker[i] == 3)
            {
                if (length == 0)
                    begin = i;
                length++;
            }
            else
            {
                length = 0;
            }
        }

        if (length >= 3)
        {
            for (int i = begin; i < begin + length; i++)
            {
                int flag = 0;
                foreach (card c in myCards)
                {
                    if ((int)c.getPoint() == i)
                    {
                        flag++;
                        result.add(c);
                    }
                    if (flag >= 3)
                        break;
                }
            }
        }

        begin = length = 0;

        //二连
        for (int i = 0; i < 12; ++i)
        {
            if (myCardMarker[i] >= 2)
            {
                if (length == 0)
                    begin = i;
                length++;
            }
            else
            {
                length = 0;
            }
        }

        if (length >= 3)
        {
            for (int i = begin; i < begin + length; i++)
            {
                int flag = 0;
                foreach (card c in myCards)
                {
                    if ((int)c.getPoint() == i)
                    {
                        flag++;
                        result.add(c);
                    }
                    if (flag >= 2)
                        break;
                }
            }
        }

        begin = length = 0;

        //单顺子
        for (int i = 0; i < 12; ++i)
        {
            if (myCardMarker[i] >= 1)
            {
                if (length == 0)
                    begin = i;
                length++;
            }
            else
            {
                length = 0;
            }
        }

        if (length >= 5)
        {
            for (int i = begin; i < begin + length; i++)
            {
                foreach (card c in myCards)
                {
                    if ((int)c.getPoint() == i)
                    {

                        result.add(c);
                        break;
                    }
                }
            }
        }

        return result;

    }

    //出牌压死，参数表示为true表示尽量出大的
    public cardcombine checkAble(bool big)
    {
        lastCombine.combineSort();
        cardcombine result = new cardcombine();

        //单排
        if (lastCombine.is_Single())
        {
            int point = (int)lastCombine.getList()[0].getPoint();
            if (big)
            {
                for (int i = 14; i > point; --i)
                {
                    if (myCardMarker[i] > 0)
                    {
                        int cnt = 0;
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i && cnt < 1)
                            {
                                cnt++;
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                for (int i = point + 1; i < 15; ++i)
                {
                    if (myCardMarker[i] > 0)
                    {
                        int cnt = 0;
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i && cnt < 1)
                            {
                                cnt++;
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
        }

        //对子
        else if (lastCombine.is_Pair())
        {
            int point = (int)lastCombine.getList()[0].getPoint();
            if (big)
            {
                for (int i = 14; i > point; --i)
                {
                    if (myCardMarker[i] == 2)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                    if (result.isEmpty() && myCardMarker[i] > 2)
                    {
                        int cnt = 0;
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i && cnt < 2)
                            {
                                cnt++;
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }

            else
            {
                for (int i = point + 1; i < 15; ++i)
                {
                    if (myCardMarker[i] == 2)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                    if (result.isEmpty() && myCardMarker[i] > 2)
                    {
                        int cnt = 0;
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i && cnt < 2)
                            {
                                cnt++;
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
        }

        //三
        else if (lastCombine.is_Three())
        {
            int point = (int)lastCombine.getList()[0].getPoint();
            if (big)
            {
                for (int i = 14; i > point; --i)
                {
                    if (myCardMarker[i] == 3)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }

            else
            {
                for (int i = point + 1; i < 15; ++i)
                {
                    if (myCardMarker[i] == 3)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
        }

        //三带一
        else if (lastCombine.is_Three_One())
        {

            int point = (int)lastCombine.getList()[1].getPoint();

            if (big)
            {
                for (int i = 14; i > point; --i)
                {
                    if (myCardMarker[i] == 3)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                for (int i = point + 1; i < 15; ++i)
                {
                    if (myCardMarker[i] == 3)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }

            bool flag1 = result.isEmpty();
            int flag2 = 0;
            foreach (card c in myCards)
            {
                if (myCardMarker[(int)c.getPoint()] == 1 || myCardMarker[(int)c.getPoint()] == 2)
                {
                    result.add(c);
                    flag2 = 1;
                    break;
                }
            }
            if (flag1 || flag2 == 0)
            {
                result = new cardcombine();
            }

        }


        //三带二
        else if (lastCombine.is_Three_Two())
        {
            int point = (int)lastCombine.getList()[2].getPoint();
            if (big)
            {
                for (int i = 14; i > point; --i)
                {
                    if (myCardMarker[i] == 3)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                for (int i = point + 1; i < 15; ++i)
                {
                    if (myCardMarker[i] == 3)
                    {
                        foreach (card c in myCards)
                        {
                            if ((int)c.getPoint() == i)
                            {
                                result.add(c);
                            }
                        }
                        break;
                    }
                }
            }
            bool flag1 = result.isEmpty();
            int flag = 0;
            foreach (card c in myCards)
            {
                if (myCardMarker[(int)c.getPoint()] == 2)
                {
                    result.add(c);
                    flag = 1;
                    break;
                }
            }
            if (flag1 || flag == 0)
            {
                result = new cardcombine();
            }
        }


        //无牌更大，出炸弹
        else
        {
            for (int i = 0; i < 13; ++i)
            {
                if (myCardMarker[i] == 4)
                {
                    foreach (card c in myCards)
                    {
                        if ((int)c.getPoint() == i)
                        {
                            result.add(c);
                        }
                    }
                    break;
                }
            }

        }
        return result;
    }


    public cardcombine freePlay()
    {
        cardcombine result = new cardcombine();
        cardcombine temp = new cardcombine();
        foreach (card c in myCards)
        {
            temp.add(c);
        }
        if (temp.is_Boom() || temp.is_Four_Two() ||
            temp.is_KingBoom() || temp.is_Pair() ||
            temp.is_PairStraight() || temp.is_Plane() ||
            temp.is_Single() || temp.is_Straight() ||
            temp.is_Three() || temp.is_Three_One() || temp.is_Three_Two())
        {
            result.setCombine(temp);
        }

        else if (!checkSeq().isEmpty())
        {
            result.setCombine(checkSeq());
            Debug.Log("here! 1");
        }

        else
        {
            Debug.Log("here! 2");
            for (int i = 0; i < 15; i++)
            {
                if (myCardMarker[i] > 0)
                {
                    foreach (card c in myCards)
                    {
                        if ((int)c.getPoint() == i){
                            result.add(c);
                            Debug.Log("add what? " + c.toString());
                        }
                            
                    }
                    Debug.Log(i + " fuck " + result.toString());
                    break;
                }
            }
        }
        return result;
    }

    //是否叫地主 (0:不叫,1:1分,2:2分,3:3分) (轮到AI叫地主的时候调用)
    public int call()
    {
        int isCall = 0;
        int score = getScore(myCardMarker);
        if (score > 250 && score <= 350)
        {
            isCall = 1;
        }
        else if (score > 350 && score <= 500)
        {
            isCall = 2;
        }
        else if (score > 500)
        {
            isCall = 3;
        }

        return isCall;
    }

    //选择要打出的牌 (cardcombine为空则视为过) (轮到AI出牌的时候调用)
    public cardcombine selectCardsToPlay()
    {
        cardcombine result = new cardcombine();
        result.setCombine(selectCard());

        foreach (card c in result.getList())
        {

            myCardMarker[(int)c.getPoint()]--;

        }

        List<card> tmp = new List<card>();
        foreach (card c in myCards)
        {
            int flag = 0;
            foreach (card d in result.getList())
            {
                if (c.equals(d))
                {
                    flag = 1;
                }
            }
            if (flag == 0)
            {
                tmp.Insert(0, c);
            }
        }
        myCards.Clear();
 
        foreach (card c in tmp)
        {
            myCards.Insert(0, c);
        }

        // foreach(card c in myCards){
        //     Debug.Log(myPlayerNo + "After chupai:" + c.toString());
        // }

        System.DateTime orinowTime = System.DateTime.Now;
        while (orinowTime.AddSeconds(2).CompareTo(System.DateTime.Now) >= 0);

        return result;
    }

    public cardcombine selectCard()
    {
        Debug.Log("last combine:" + lastCombine.toString());
        cardcombine cards = new cardcombine();
        if (lastPlayerNo == myPlayerNo)
        {
            cards.setCombine(freePlay());
        }
        else
        {
            if (myPlayerNo == dizhuNo)
            {
                cards.setCombine(checkAble(false));
            }
            else
            {
                if (lastPlayerNo == dizhuNo)
                {
                    cards.setCombine(checkAble(true));
                }

            }
        }
        return cards;
    }

    public bool ifWin()
    {
        return myCards.Count == 0;
    }

}
