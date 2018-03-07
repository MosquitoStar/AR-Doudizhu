using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameSettingPanelController : MonoBehaviour {
	public GameObject settingPanel;
	// public Button settingButton;
	public Slider soundSlider;
    public Toggle soundEffectToggle;
    
    private int isDisplay;

    void Start(){
        settingPanel.SetActive(false);
        isDisplay = 0;
    }
	//初始化函数
	public void SetSettingPanelActive () {
        isDisplay = (isDisplay+1)%2;
        if(isDisplay == 1){
            settingPanel.SetActive(true);
        }
        else   
            settingPanel.SetActive(false);
	}
	// //调节背景声音大小
	// public void ChangeSound(){
	// 	AudioManager.setMusicVolume(soundSlider.value);
	// }
    //开关音效
    public void SwitchSoundEffect(){
        if(soundEffectToggle.isOn)
            AudioManager.setCanPlay(true);
        else  
            AudioManager.setCanPlay(false);
    }
}
