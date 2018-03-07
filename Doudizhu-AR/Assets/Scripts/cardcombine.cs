using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardcombine {
    private List<card> cardList;

    //判断牌型:
    public bool is_Pass()
    {
        return cardList.Count == 0;
    }
    public bool is_Single()
    {
        return cardList.Count == 1;
    }
    public bool is_Straight()
    {
        int N=cardList.Count;
        if(N<5)
            return false;
        combineSort();
        if(cardList[N-1].getPoint() > card.point.Card_A)
            return false;
        for(int i=0;i<N-1;i++){
            if(cardList[i].getPoint() - cardList[i+1].getPoint() != -1)
                return false;
        }
        return true;
    }
    public bool is_Pair()
    {
        if (cardList.Count != 2)
            return false;
        if (cardList[0].getPoint() != cardList[1].getPoint())
            return false;
        return true;
    }
    public bool is_PairStraight()
    {
        int N = cardList.Count;
        if (N < 6 || N % 2 != 0)
        {
            return false;
        }
        combineSort();
        int n = N / 2;
        for (int i = 0; i < n; i++)
        {
            if (cardList[2 * i].getPoint() != cardList[2 * i + 1].getPoint())
            {
                return false;
            }
                
        }
        for (int i = 0; i < N - 2; i+=2)
        {
            if (cardList[i].getPoint() - cardList[i + 2].getPoint() != -1)
            {
                Debug.Log(3);
                return false;
            }
                
        }
        return true;
    }
    public bool is_Three()
    {
        if (cardList.Count != 3)
            return false;
        if (cardList[0].getPoint() != cardList[1].getPoint() || cardList[0].getPoint() != cardList[2].getPoint())
            return false;
        return true;
    }
    public bool is_Three_One()
    {
        if (cardList.Count != 4)
        {
            return false;
        }
        combineSort();
        if (cardList[0].getPoint() == cardList[1].getPoint() && cardList[0].getPoint() == cardList[2].getPoint()
            && cardList[0].getPoint() != cardList[3].getPoint())
            return true;
        if (cardList[1].getPoint() == cardList[2].getPoint() && cardList[1].getPoint() == cardList[3].getPoint()
            && cardList[0].getPoint() != cardList[1].getPoint())
            return true;
        return false;
    }
    public bool is_Three_Two()
    {
        if (cardList.Count != 5)
            return false;
        combineSort();
        if (cardList[0].getPoint() == cardList[1].getPoint() && cardList[0].getPoint() == cardList[2].getPoint()
            && cardList[3].getPoint() == cardList[4].getPoint() && cardList[0].getPoint() != cardList[3].getPoint())
            return true;
        if (cardList[0].getPoint() == cardList[1].getPoint() && cardList[2].getPoint() == cardList[3].getPoint()
            && cardList[2].getPoint() == cardList[4].getPoint() && cardList[0].getPoint() != cardList[2].getPoint())
            return true;
        return false;
    }
    public bool is_Four_Two()
    {
        int N = cardList.Count;
        if (N != 6)
            return false;
        int[] count = new int[16];
        for (int i = 0; i < N; i++)
            count[(int)cardList[i].getPoint()]++;
        bool flag = false;
        for (int i = 0; i < 16; i++)
        {
            if (count[i] == 2)
                return false;
            if (count[i] == 4)
                flag = true;
        }
        return flag;
    }
    public bool is_Plane()
    {
        int N=cardList.Count;
        if (N < 6 || N % 3 != 0)
            return false;
        combineSort();
        int n=N/3;
        for(int i=0;i<n;i++){
            if (cardList[3 * i].getPoint() != cardList[3 * i + 1].getPoint() ||
                cardList[3 * i].getPoint() != cardList[3 * i + 2].getPoint())
                return false;
        }
        if(cardList[N-1].getPoint()>card.point.Card_A)
            return false;
        for (int i = 0; i < N - 3; i += 3)
            if (cardList[i].getPoint() - cardList[i + 3].getPoint() != -1)
                return false;
        return true;
    }
    public bool is_Plane_Single()
    {
        int N = cardList.Count;
        if (N < 8 || N % 4 != 0)
            return false;
        int[] count = new int[16];
        for (int i = 0; i < cardList.Count; i++)
            count[(int)cardList[i].getPoint()]++;
        cardcombine c1 = new cardcombine();
        cardcombine c2 = new cardcombine();
        for (int i = 0; i < cardList.Count; i++)
            if (count[(int)cardList[i].getPoint()] == 1)
                c1.add(cardList[i]);
            else
                c2.add(cardList[i]);
        return c2.getList().Count == 3*c1.getList().Count && c2.is_Plane();
    }
    public bool is_Plane_Pair()
    {
        int N = cardList.Count;
        if (N < 10 || N % 5 != 0)
            return false;
        int[] count = new int[16];
        for (int i = 0; i < cardList.Count; i++)
            count[(int)cardList[i].getPoint()]++;
        cardcombine c1 = new cardcombine();
        cardcombine c2 = new cardcombine();
        for (int i = 0; i < cardList.Count; i++)
            if (count[(int)cardList[i].getPoint()] == 2)
                c1.add(cardList[i]);
            else
                c2.add(cardList[i]);
        return 2*c2.getList().Count == 3 * c1.getList().Count && c2.is_Plane();
    }
    public bool is_Boom()
    {
        if (cardList.Count != 4)
            return false;
        if (cardList[0].getPoint() != cardList[1].getPoint() || cardList[0].getPoint() != cardList[2].getPoint()
            || cardList[0].getPoint() != cardList[3].getPoint())
            return false;
        return true;
    }
    public bool is_KingBoom()
    {
        if (cardList.Count != 2)
            return false;
        combineSort();
        if (cardList[0].getPoint() != card.point.Card_BJ)
            return false;
        if (cardList[1].getPoint() != card.point.Card_RJ)
            return false;
        return true;
    }      

    //从小到大排序
    public void combineSort()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            int min = i;
            for (int j = i + 1; j < cardList.Count; j++)
            {
                if ( cardList[j].toInt() < cardList[min].toInt() )
                {
                    min = j;
                }
            }
            card tmp = cardList[min];
            cardList[min] = cardList[i];
            cardList[i] = tmp;
        }
    }

    //牌型类型
    public enum Type
    {
        Unknown,                      //非法牌型
        Pass,                         //过
        Single,                       //单张
        Straight,                     //顺子
        Pair,                         //对子
        PairStraight,                 //连对
        Three,                        //三张
        Three_One,                    //三带一
        Three_Two,                    //三带二
        Four_Two,                     //四带二
        Plane,                        //无翅飞机
        Plane_Single,                 //飞机带同数量的单张
        Plane_Pair,                   //飞机带同数量的对子
        Boom,                         //炸弹
        KingBoom,                     //王炸
    };
    //牌组比较关系类型
    public enum CompareRes
    {
        Smaller,                      //比不过
        Equal,                        //一样大
        Larger,                       //比得过
        CannotCompare,                //无法比较
    };

    public cardcombine(){
        cardList=new List<card>();
    }

    //牌组中牌的数量
    public int size()
    {
        return cardList.Count;
    }

    //获取牌组（用List容器保存的）
    public List<card> getList()
    {
        return cardList;
    }

    //设置牌组（用List容器保存的）        
    public void setList(List<card> list)
    {
        cardList.Clear();
        foreach(card c in list){
            cardList.Add(c);
        }
    }

    //设置牌组（相当于用另一个cardcombine给该对象拷贝赋值）
    public void setCombine(cardcombine com)
    {
        cardList.Clear();
        foreach (card c in com.getList())
        {
            card nc = new card(c.getSuit(), c.getPoint());
            cardList.Add(nc);
        }
    }

    //向牌组添加一张牌
    public void add(card c)
    {
        cardList.Add(c);
    }

    //从牌组中移除一张牌
    public void remove(card c)
    {
        cardList.Remove(c);
    }

    //移除牌组所有牌
    public void removeAll()
    {
        cardList.Clear();
    }

    //判断某张牌是否在牌组内
    public bool hasCard(card c)
    {
        return cardList.Contains(c);
    }

    //判断牌组是否为空
    public bool isEmpty()
    {
        return cardList.Count == 0;
    }

    //返回牌型
    public Type getType()
    {
        if(is_Pass())
            return Type.Pass;
        if(is_Single())
            return Type.Single;
        if(is_Straight())
            return Type.Straight;
        if(is_Pair())
            return Type.Pair;
        if(is_PairStraight())
            return Type.PairStraight;
        if(is_Three())
            return Type.Three;
        if(is_Three_One())
            return Type.Three_One;
        if(is_Three_Two())
            return Type.Three_Two;
        if(is_Four_Two())
            return Type.Four_Two;
        if(is_Plane())
            return Type.Plane;
        if(is_Plane_Single())
            return Type.Plane_Single;
        if(is_Plane_Pair())
            return Type.Plane_Pair;
        if(is_Boom())
            return Type.Boom;
        if(is_KingBoom())
            return Type.KingBoom;
        return Type.Unknown;
    }

    //计算某牌型的优先级
    /*
    * 按照 王炸 > 普通炸弹 > 一般牌型 > 过 > 非法牌型
    * 优先级分别是 5,4,3,2,1(优先级大的,牌组的威力就大)
    */
    public static int priority(Type type)
    {
        switch(type)
        {
        case Type.KingBoom:
            return 5;
        case Type.Boom:
            return 4;
        case Type.Pass:
            return 2;
        case Type.Unknown:
            return 1;
        default:
            return 3;
        }
    }

    //计算代表点数
    /*
    * 过: 0
    * 单牌: 单牌的点数
    * 顺子: 按照顺子时候牌的顺序(注意顺子的顺序和点数的顺序可能是不同的),此时最后一张牌的点数
    * 对子: 对子中任意一张牌的点数
    * 连对: 按照连对的顺序(注意连对的顺序和点数的顺序可能是不同的),此时最后一张牌的点数
    * 三张: 三张中任意一张牌的点数
    * 三带一: 三张的那个点数
    * 三带二: 三张的那个点数
    * 四带二: 四张的那个点数
    * 无翅飞机: 按照飞机的顺序(注意飞机的顺序和点数的顺序可能是不同的),此时最后一张牌的点数
    * 飞机带单牌: 无翅飞机部分的代表点数
    * 飞机带对子: 无翅飞机部分的代表点数
    * 炸弹: 炸弹中任意一张的点数
    * 王炸: 大王的点数
    */
    public card.point representPoint(Type type)
    {
        combineSort();
        switch(type)
        {
        case Type.Pass:
            return card.point.Card_MinusOne;
        case Type.Single:
            return cardList[0].getPoint();
        case Type.Straight:{
            int N=cardList.Count;
            if(cardList[N-1].getPoint()-cardList[N-2].getPoint()==1){
                if(cardList[N-2].getPoint()-cardList[N-3].getPoint()==1)
                    return cardList[N-1].getPoint();
                return cardList[N-3].getPoint();
            }
            return cardList[N-2].getPoint();
        }
        case Type.Pair:
            return cardList[0].getPoint();
        case Type.PairStraight:{
            int N=cardList.Count;
            if(cardList[N-1].getPoint()-cardList[N-3].getPoint()==1){
                if(cardList[N-3].getPoint()-cardList[N-5].getPoint()==1)
                    return cardList[N-1].getPoint();
                return cardList[N-5].getPoint();
            }
            return cardList[N-3].getPoint();
        }
        case Type.Three:
            return cardList[0].getPoint();
        case Type.Three_One:{
            if(cardList[0]==cardList[1])
                return cardList[0].getPoint();
            return cardList[1].getPoint();
        }
        case Type.Three_Two:{
            if(cardList[2]==cardList[3])
                return cardList[2].getPoint();
            return cardList[0].getPoint();
        }
        case Type.Four_Two:{
            if(cardList[1]==cardList[2])
                return cardList[1].getPoint();
            return cardList[2].getPoint();
        }
        case Type.Plane:{
            int N=cardList.Count;
            if(N==6)
            {
                if(cardList[0].getPoint()==card.point.Card_A)
                    return card.point.Card_MinusOne;
                else if(cardList[N-1].getPoint()==card.point.Card_2)
                    return card.point.Card_3;
                return cardList[N-1].getPoint();
            }else{
                if(cardList[N-1].getPoint()==card.point.Card_2){
                    if(cardList[N-4].getPoint()==card.point.Card_A)
                        return cardList[N-7].getPoint();
                    return cardList[N-4].getPoint();
                }
                return cardList[N-1].getPoint();
            }
        }
        case Type.Plane_Single:{
            int[] count = new int[16];
            int N=cardList.Count;
            for( int i=0;i<N;i++)
                count[(int)cardList[i].getPoint()]++;
            cardcombine tmp = new cardcombine();
            for( int i=0;i<N;i++){
                if(count[(int)cardList[i].getPoint()]==3)
                    tmp.add(cardList[i]);
            }
            return tmp.representPoint(Type.Plane);
        }
        case Type.Plane_Pair:
            return this.representPoint(Type.Plane_Single);
        case Type.Boom:
            return cardList[0].getPoint();
        case Type.KingBoom:
            return card.point.Card_RJ;
        }
        return card.point.Card_MinusOne;
    }

    //牌组优先级比较
    public CompareRes compareTo(cardcombine combine)
    {
        Type L_type=getType();
        Type R_type=combine.getType();
        int L_priority=priority(L_type);
        int R_priority=priority(R_type);
        if(L_priority<R_priority)
            return CompareRes.Smaller;
        if(L_priority>R_priority)
            return CompareRes.Larger;
        if(L_type==R_type){
            card.point L_point=representPoint(L_type);
            card.point R_point=combine.representPoint(R_type);
            if(L_point<R_point)
                return CompareRes.Smaller;
            if(L_point>R_point)
                return CompareRes.Larger;
            return CompareRes.Equal;
        }
        return CompareRes.CannotCompare;
    }

    //转成字符串（一般用于输出）
    public string toString()
    {
        string name = "";
        for (int i = 0; i < cardList.Count; i++)
        {
            name += cardList[i].toString();
            if (i < cardList.Count - 1)
            {
                name += ",";
            }
        }
        return name;
    }
}