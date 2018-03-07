using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class card {
    //花色类型
    public enum suit { Diamond, Club, Heart, Spades, Black, Red }
    //点数类型
    public enum point { Card_3, Card_4, Card_5, Card_6, Card_7, Card_8, Card_9,
               Card_10, Card_J, Card_Q, Card_K, Card_A, Card_2, Card_BJ, Card_RJ, Card_MinusOne = -1 }

    //牌基本信息
    private suit huase;
    private point dianshu;
    public GameObject model;


    public card(suit s, point p)
    {
        huase = s;
        dianshu = p;
    }
    public card(int s, int p)
    {
        huase = (suit)s;
        dianshu = (point)p;
    }

    //get
    public suit getSuit()
    {
        return huase;
    }
    public point getPoint()
    {
        return dianshu;
    }
    public GameObject getModel()
    {
        return model;
    }

    //set
    public void setSuit(suit s)
    {
        huase = s;
    }
    public void setSuit(int s)
    {
        huase = (suit)s;
    }
    public void setPoint(point p)
    {
        dianshu = p;
    }
    public void setPoint(int p)
    {
        dianshu = (point)p;
    }
    public void setModel(GameObject m)
    {
        model = m;
    }

    //花色和点数都相等（全等）
    public bool equals(card c)
    {
        return (this.dianshu == c.dianshu) && (this.huase == c.huase);
    }

    //点数相等（牌面大小相等）
    public bool pointsEquals(card c)
    {
        return this.dianshu == c.dianshu;
    }

    //比较牌面大小
    public int compareTo(card c)
    {
        return this.toInt() - c.toInt();
    }

    //转成整数（0~53）
    public int toInt()
    {
        if ( dianshu == point.Card_RJ )
        {
            return 53;
        }
        else if (dianshu == point.Card_BJ)
        {
            return 52;
        }
        else
        {
            return (int)huase + (int)dianshu * 4;
        }
    }

    //转成字符串（一般用来输出）
    public string toString()
    {
        string name = "";
        switch (dianshu)
        {
            case point.Card_10:
                name += "10";
                break;
            case point.Card_2:
                name += "2";
                break;
            case point.Card_A:
                name += "A";
                break;
            case point.Card_3:
                name += "3";
                break;
            case point.Card_4:
                name += "4";
                break;
            case point.Card_5:
                name += "5";
                break;
            case point.Card_6:
                name += "6";
                break;
            case point.Card_7:
                name += "7";
                break;
            case point.Card_8:
                name += "8";
                break;
            case point.Card_9:
                name += "9";
                break;
            case point.Card_J:
                name += "J";
                break;
            case point.Card_Q:
                name += "Q";
                break;
            case point.Card_K:
                name += "K";
                break;
            case point.Card_BJ:
                name += "JokerBlack";
                break;
            case point.Card_RJ:
                name += "JokerRed";
                break;
        }
        switch (huase)
        {
            case suit.Diamond:
                name += "Diamond";
                break;
            case suit.Club:
                name += "Club";
                break;
            case suit.Heart:
                name += "Heart";
                break;
            case suit.Spades:
                name += "Spades";
                break;
        }
        return name;
    }

    //基于0~53的整数构造一张牌
    public static card form(int value)
    {
        if (value == 52)
        {
            return new card(suit.Black,point.Card_BJ);
        }
        else if (value == 53)
        {
            return new card(suit.Red, point.Card_RJ);
        }
        else
        {
            return new card(value % 4, value / 4);
        }
    }
}
