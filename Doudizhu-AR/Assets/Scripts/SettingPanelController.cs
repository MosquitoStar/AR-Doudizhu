using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour {
	public GameObject settingPanel;
	public GameObject loginPanel;
	public Button backButton;
	public Slider soundSlider;
	public AudioSource welcomeMusic;

	//初始化函数
	void OnEnable () {
		backButton.onClick.RemoveAllListeners ();		//移除返回按钮绑定的所有监听事件
		backButton.onClick.AddListener (delegate() {	//为返回按钮绑定新的监听事件
			settingPanel.SetActive(false);				//禁用游戏设置面板
			loginPanel.SetActive(true);					//启用游戏登录面板
		});
	}

	//调节声音大小
	public void ChangeSound(){
		welcomeMusic.volume = soundSlider.value;
	}
}
