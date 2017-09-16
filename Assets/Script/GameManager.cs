using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//初始化操作

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPay (string sku){
		Debug.Log("购买商品");

		GetComponent<ExternalManager> ().BuyProduct ("com.steve.unity.coins");

	}

	public void ShowLeaderBoard (){
		Debug.Log ("调用排行榜");
//		GetComponent<ExternalManager> ().BuyProduct ();

	}
}
