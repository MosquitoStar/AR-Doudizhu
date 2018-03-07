using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    public ParticleSystem straight;
    public ParticleSystem pairstraight;
    public GameObject airplane;
    public ParticleSystem plane;
    public ParticleSystem bomb;
    public ParticleSystem kingbomb;
    public ParticleSystem[] winfirework;
    public ParticleSystem losesnow;

    public Vector3 position;
    private int myID;
    private int playerID;

	// Use this for initialization
	void Start () {
        position = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setMyID(int id)
    {
        myID = id;
    }

    public void setEffectPositionByID(int id)
    {
        playerID = id;
        
    }

    public void activateEffect(cardcombine lastCombine)
    {
        cardcombine.Type combineType = lastCombine.getType();
        int cardNum=lastCombine.size();
        if (combineType == cardcombine.Type.Straight)
        {
            if (playerID == myID)
            {
                if (lastCombine.size() % 2 != 0)
                {
                    position = new Vector3(-cardNum / 2 * dou.myCardsDX, dou.myCardsY, dou.myCardsZ + dou.myCardsDX * 2.5f);
                }
                else
                {
                    position = new Vector3(-(cardNum / 2 - 1) * dou.myCardsDX + dou.myCardsDX, dou.myCardsY, dou.myCardsZ + dou.myCardsDX * 2.5f);
                }
            }
            else if (playerID == (myID + 1) % 3)
            {
                position = new Vector3(dou.otherPlayerCardsX - dou.myCardsDX * 1.5f, dou.otherPlayerCardsY, dou.otherPlayerCardsZ / 2.0f);
            }
            else if (playerID == (myID + 2) % 3)
            {
                position = new Vector3(-dou.otherPlayerCardsX + dou.myCardsDX * 1.5f, dou.otherPlayerCardsY, dou.otherPlayerCardsZ / 2.0f);
            }
            straight.transform.position = position;
            straight.Play();
        }
        else if (combineType == cardcombine.Type.PairStraight)
        {
            if (playerID == myID)
            {
                if (lastCombine.size() % 2 != 0)
                {
                    position = new Vector3(-cardNum / 2 * dou.myCardsDX, dou.myCardsY, dou.myCardsZ + dou.myCardsDX * 2.5f);
                }
                else
                {
                    position = new Vector3(-(cardNum / 2 - 1) * dou.myCardsDX + dou.myCardsDX, dou.myCardsY, dou.myCardsZ + dou.myCardsDX * 2.5f);
                }
            }
            else if (playerID == (myID + 1) % 3)
            {
                position = new Vector3(dou.otherPlayerCardsX - dou.myCardsDX * 1.5f, dou.otherPlayerCardsY, dou.otherPlayerCardsZ / 2.0f);
            }
            else if (playerID == (myID + 2) % 3)
            {
                position = new Vector3(-dou.otherPlayerCardsX + dou.myCardsDX * 1.5f, dou.otherPlayerCardsY, dou.otherPlayerCardsZ / 2.0f);
            }
            pairstraight.transform.position = position;
            pairstraight.Play();
        }
        else if (combineType == cardcombine.Type.Plane||
                combineType==cardcombine.Type.Plane_Single||
                combineType==cardcombine.Type.Plane_Pair)
        {
            StartCoroutine(playPlaneEffect());
        }
        else if (combineType == cardcombine.Type.Boom)
        {
            bomb.transform.position = new Vector3(0, 0, 0);
            bomb.Play();
        }
        else if (combineType == cardcombine.Type.KingBoom)
        {
            kingbomb.transform.position = new Vector3(0, 0, 0);
            kingbomb.Play();
        }
    }

    private IEnumerator playPlaneEffect()
    {
        airplane.transform.position = new Vector3(-8.0f, 1.0f, 3.0f);
        plane.Play();
        for (int i = 0; i < 18; i++)
        {
            airplane.transform.position += new Vector3(1.5f, -0.01f, -0.4f);
            yield return new WaitForSeconds(0.04f);
        }
        plane.Stop();
    }

    public void playWinScene()
    {
        StartCoroutine(firework());
    }

    private IEnumerator firework()
    {
        for (int i = 0; i < 10; i++)
        {
            winfirework[0].Play();
            yield return new WaitForSeconds(1.0f);
            winfirework[1].Play();
            yield return new WaitForSeconds(1.5f);
            winfirework[2].Play();
            yield return new WaitForSeconds(2.5f);
        }
    }

    public void playLoseScene()
    {
        losesnow.Play();
    }
}
