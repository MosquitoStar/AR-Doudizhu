using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

	public AudioSource manClaim;
	public AudioSource womanClaim;
	public AudioSource manNoClaim;
	public AudioSource womanNoClaim;
	public AudioSource[] manPass;
	public AudioSource[] womanPass;
	public AudioSource[] manDani;
	public AudioSource[] womanDani;
	public AudioSource[] manSingle;
	public AudioSource[] womanSingle;
	public AudioSource[] manPair;
	public AudioSource[] womanPair;
	public AudioSource[] manThree;
	public AudioSource[] womanThree;
	public AudioSource manThreeOne;
	public AudioSource womanThreeOne;
	public AudioSource manThreeTwo;
	public AudioSource womanThreeTwo;
	public AudioSource manFourTwo;
	public AudioSource womanFourTwo;
	public AudioSource manPlane;
	public AudioSource womanPlane;
	public AudioSource manStraight;
	public AudioSource womanStraight;
	public AudioSource manPairStraight;
	public AudioSource womanPairStraight;
	public AudioSource manBomb;
	public AudioSource womanBomb;
	public AudioSource manKingBomb;
	public AudioSource womanKingBomb;

    public AudioSource bomb;
    public AudioSource plane;
    public AudioSource pairStraight;

	public AudioSource normalSong;
	public AudioSource excitingSong;
	public AudioSource winSong;
	public AudioSource loseSong;

	public Slider soundSlider;

	private static bool canPlay;
	public static bool getCanPlay(){
		return canPlay;
	}

	public static void setCanPlay(bool flag){
		canPlay = flag;
	}

	private int gender;

	// Use this for initialization
	void Start () {
		gender=0;
		canPlay = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setVoiceGender(int g){
		gender=g;
	}

	public void claimLandLordAudio (bool claim) {
		if(!canPlay)
			return;
		if(claim){
			if(gender==0){
                manClaim.Play();
            }
			else
				womanClaim.Play();
		}else{
			if(gender==0)
				manNoClaim.Play();
			else
				womanNoClaim.Play();
		}
	}

	public void passAudio(){
		if(!canPlay)
			return;
		System.Random ran=new System.Random();
		int i=ran.Next(5);
		if(gender==0){
			manPass[i].Play();
		}else{
			womanPass[i].Play();
		}
	}

    private bool hasPrev;
    public void setHasPrev(bool b)
    {
        hasPrev = b;
    }

	public void playCardAudio (cardcombine lastCombine) {
		if(!canPlay)
			return;
		cardcombine.Type combineType=lastCombine.getType();
		card.point combinePoint=lastCombine.representPoint(combineType);
		int cp=(int)combinePoint;
		if(combineType==cardcombine.Type.Pass||combineType==cardcombine.Type.Unknown){
			return;
		}
		if(combineType==cardcombine.Type.Single){
			if(gender==0){
				manSingle[cp].Play();
			}else{
				womanSingle[cp].Play();
			}
		}else if(combineType==cardcombine.Type.Pair){
			if(gender==0){
				manPair[cp].Play();
			}else{
				womanPair[cp].Play();
			}
		}else if(combineType==cardcombine.Type.Three){
			if(gender==0){
				manThree[cp].Play();
			}else{
				womanThree[cp].Play();
			}
        }
        else if (combineType == cardcombine.Type.Boom)
        {
            if (gender == 0)
            {
                manBomb.Play();
            }
            else
            {
                womanBomb.Play();
            }
            bomb.Play();
        }
        else if (combineType == cardcombine.Type.KingBoom)
        {
			if(gender==0){
				manKingBomb.Play();
			}else{
				womanKingBomb.Play();
			}
            bomb.Play();
		}else{
			if(hasPrev){
				System.Random ran=new System.Random();
				int i=ran.Next(4);
				if(gender==0){
					manDani[i].Play();
				}else{
					womanDani[i].Play();
				}
                if (combineType == cardcombine.Type.PairStraight)
                {
                    pairStraight.Play();
                }
                else if(combineType==cardcombine.Type.Plane||
					 combineType==cardcombine.Type.Plane_Single||
                     combineType == cardcombine.Type.Plane_Pair)
                {
                    plane.Play();
                }
			}else if(combineType==cardcombine.Type.Straight){
				if(gender==0){
					manStraight.Play();
				}else{
					womanStraight.Play();
				}
			}else if(combineType==cardcombine.Type.PairStraight){
				if(gender==0){
					manPairStraight.Play();
				}else{
					womanPairStraight.Play();
				}
                pairStraight.Play();
			}else if(combineType==cardcombine.Type.Three_One){
				if(gender==0){
					manThreeOne.Play();
				}else{
					womanThreeOne.Play();
				}
			}else if(combineType==cardcombine.Type.Three_Two){
				if(gender==0){
					manThreeTwo.Play();
				}else{
					womanThreeTwo.Play();
				}
			}else if(combineType==cardcombine.Type.Four_Two){
				if(gender==0){
					manFourTwo.Play();
				}else{
					womanFourTwo.Play();
				}
			}else if(combineType==cardcombine.Type.Plane||
					 combineType==cardcombine.Type.Plane_Single||
					 combineType==cardcombine.Type.Plane_Pair){
				if(gender==0){
					manPlane.Play();
				}else{
					womanPlane.Play();
				}
                plane.Play();
			}
		}
	}

	public void playExcitingSong(){
		normalSong.Stop();
		excitingSong.Play();
	}

	public void playWinSong(){
		normalSong.Stop();
		excitingSong.Stop();
		winSong.Play();
	}

	public void playLoseSong(){
		normalSong.Stop();
		excitingSong.Stop();
		loseSong.Play();
	}
	public void setMusicVolume(){
		normalSong.volume = soundSlider.value;
		excitingSong.volume = soundSlider.value;
		winSong.volume = soundSlider.value;
		loseSong.volume = soundSlider.value;
	}
}
