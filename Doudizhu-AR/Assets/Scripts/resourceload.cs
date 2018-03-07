using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resourceload : MonoBehaviour {
    private string cubePath = "PlayingCards_BoxOpen";
    private string spherePath = "PlayingCards_Box";
	// Use this for initialization
	void Start () {
        //把资源加载到内存中
        Object cubePreb = Resources.Load(cubePath, typeof(GameObject));
        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        GameObject cube = Instantiate(cubePreb) as GameObject;

        //以下同理实现Sphere的动态实例化
        //把资源加载到内存中
        Object spherePreb = Resources.Load(spherePath, typeof(GameObject));
        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        GameObject sphere = Instantiate(spherePreb) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
