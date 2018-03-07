using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;

public class LoginPanelController : PunBehaviour {
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
	void OnEnable () {
		backButton.gameObject.SetActive (true);
		//如果未连接Photon服务器
		if (!PhotonNetwork.connected) {
			SetLoginPanelActive ();								//启用游戏登录面板
			username.text = PlayerPrefs.GetString ("Username");	//在本地保存玩家昵称
			backButton.onClick.RemoveAllListeners ();		//移除返回按钮绑定的所有监听事件
			backButton.onClick.AddListener (delegate() {	//为返回按钮绑定新的监听事件
				SetWelcomePanelActive();
			});
		} 
		//如果已连接Photon服务器
		else
			SetLobbyPanelActive ();	//启用游戏大厅面板
		connectionState.text = "";	//初始化网络连接状态文本信息
	}

	//启用游戏登录面板
	public void SetLoginPanelActive(){
		loginPanel.SetActive (true);				//启用游戏登录面板
		welcomePanel.SetActive (false);				//禁用欢迎面板
		userMessage.SetActive (false);				//禁用玩家昵称信息
		settingPanel.SetActive (false);				//禁用游戏设置面板
		lobbyPanel.SetActive (false);				//禁用游戏大厅面板
		backButton.gameObject.SetActive (true);	
		if(roomPanel!=null)
			roomPanel.SetActive (false);				//禁用游戏房间面板
	}
	//启用游戏大厅面板
	public void SetLobbyPanelActive(){				
		loginPanel.SetActive (false);				//禁用游戏登录面板
		welcomePanel.SetActive (false);				//禁用欢迎面板
		settingPanel.SetActive (false);				//禁用游戏设置面板
		userMessage.SetActive (true);				//启用玩家昵称信息
		lobbyPanel.SetActive (true);				//启用游戏大厅面板
		backButton.gameObject.SetActive (true);	
	}
	//启用游戏设置面板
	public void SetSettingPanelActive(){
		welcomePanel.SetActive (false);				//禁用欢迎面板	
		settingPanel.SetActive (true);				//启用游戏设置面板			
		loginPanel.SetActive (false);				//禁用游戏登录面板
		userMessage.SetActive (false);				//禁用玩家昵称信息
		lobbyPanel.SetActive (false);				//禁用游戏大厅面板
		backButton.gameObject.SetActive (true);	
	}
	public void SetWelcomePanelActive(){
		backGround.color = new Color(1.0f,1.0f,1.0f);
		welcomePanel.SetActive (true);				//启用欢迎面板	
		settingPanel.SetActive (false);				//禁用游戏设置面板			
		loginPanel.SetActive (false);				//禁用游戏登录面板
		userMessage.SetActive (false);				//禁用玩家昵称信息
		backButton.gameObject.SetActive (false);	//启用后退按钮
		lobbyPanel.SetActive (false);				//禁用游戏大厅面板
	}
	//"登录"按钮事件处理函数
	public void ClickLogInButton(){							
		SetLobbyPanelActive ();			//启用游戏大厅面板
		//客户端连接Photon服务器，游戏版本标识符为“1.0”
		if (!PhotonNetwork.connected)						
			PhotonNetwork.ConnectUsingSettings ("1.0");		
		//如果玩家未输入昵称，这里自动为其分配一个昵称
		if (username.text == "")							
			username.text = "游客" + Random.Range (1, 9999);
		PhotonNetwork.player.name = username.text;			//设置玩家昵称
		PlayerPrefs.SetString ("Username", username.text);	//将玩家昵称保存在本地
	}
	//"退出"按钮事件处理函数
	public void ClickSettingButton(){
		// Application.Quit ();			//退出游戏应用
		SetSettingPanelActive();
	}

	/**覆写IPunCallback回调函数，当玩家进入游戏大厅时调用
	 * 显示玩家昵称
	 */
	public override void OnJoinedLobby(){
		userMessage.GetComponentInChildren<Text> ().text
		= "欢迎，" + PhotonNetwork.player.name;
	}

	/**覆写IPunCallback回调函数，当客户端断开与Photon服务器的连接时调用
	 * 游戏画面返回游戏登录面板
	 */
	public override void OnConnectionFail(DisconnectCause cause){
		SetLoginPanelActive ();
	}
}
