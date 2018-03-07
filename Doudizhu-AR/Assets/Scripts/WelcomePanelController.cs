using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;

public class WelcomePanelController : PunBehaviour {
    public GameObject welcomePanel;		//游戏欢迎界面
	public GameObject loginPanel;		//游戏登录面板
	public GameObject settingPanel; 	//游戏设置面板
	public GameObject userMessage;		//玩家昵称信息
	public Button backButton;			//后退按钮
	public GameObject lobbyPanel;		//游戏大厅面板
	public GameObject roomPanel;		//游戏房间面板
	public Text username;				//玩家昵称文本
	public Text connectionState;		//网络连接状态

	public Image backGround;

	//初始化，根据当前客户端连接状态，显示相应的游戏面板
	void Start () {
		SetWelcomePanelActive();
	}

	void Update(){		
		//在游戏画面左下角显示当前的网络连接状态
		connectionState.text = PhotonNetwork.connectionStateDetailed.ToString ();
	}

    //启用欢迎面板
	public void SetWelcomePanelActive(){
		backGround.color = new Color(1.0f,1.0f,1.0f);
		welcomePanel.SetActive (true);				//启用欢迎面板	
		loginPanel.SetActive (false);				//禁用游戏登录面板
		settingPanel.SetActive (false);				//禁用游戏设置面板			
		userMessage.SetActive (false);				//禁用玩家昵称信息
		backButton.gameObject.SetActive (false);	//禁用后退按钮
		lobbyPanel.SetActive (false);				//禁用游戏大厅面板
	}

	//启用游戏登录面板
	public void SetLoginPanelActive(){
		// backGround.color = new Color(0.5f,0.5f,0.5f);
		StartCoroutine(whiteToGrey());
        // StartCoroutine(GetLoginPanel());
		loginPanel.SetActive(true);
		welcomePanel.SetActive (false);				//禁用欢迎面板
		userMessage.SetActive (false);				//禁用玩家昵称信息
		settingPanel.SetActive (false);				//禁用游戏设置面板
		lobbyPanel.SetActive (false);				//禁用游戏大厅面板
		if(roomPanel!=null)
			roomPanel.SetActive (false);			//禁用游戏房间面板
	}

	// private IEnumerator GetLoginPanel(){
	// 	while(loginPanel.transform.position.x - 350.0f > 1.0f){
    //         loginPanel.transform.position = new Vector3(loginPanel.transform.position.x - 30.0f,settingPanel.transform.position.y,settingPanel.transform.position.z);
    //         yield return new WaitForSeconds(0.02f);
    //     }
	// }
	private IEnumerator whiteToGrey(){
		while(backGround.color.r - 0.5f > 0.001 ){
            backGround.color = new Color(backGround.color.r-0.05f,backGround.color.g-0.05f,backGround.color.b-0.05f);
            yield return new WaitForSeconds(0.02f);
        }
	}
	//"开始游戏"按钮事件处理函数
	public void ClickStartGameButton(){							
		SetLoginPanelActive ();			//启用游戏大厅面板
	}
	//"退出游戏"按钮事件处理函数
	public void ClickExitGameButton(){
		Application.Quit ();			//退出游戏应用
	}
}