using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnePF;

public class GameManager : MonoBehaviour {
	
	private bool _processingPayment = false;
	const string SKU = "sku";
	#pragma warning disable 0414
	string _label = "";
	#pragma warning restore 0414
	bool _isInitialized = false;
	Inventory _inventory = null;

	private string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAkP0eDV/I54FrnZgFibDPhhAlTn+XYVcRa5IrtTEkHX7mC6Gq4UzI8WVvTJ4GnhcavZn87wYGJYr7/dMgI90S3Smowrort0XCqIVzg890f2Z+x+6Hg1DWGl8Pf5Nz0S/g2Yg8axv+6GQbv8XqJ4Ris/vZYNPJIG/Epm7mSDtfZxt1lV4DqNMngH3QwC9WzHxQU2wZjLLJ3YtOQXeg2IK2Mv5UkAWrxzuHI5+wQTT+3idfu9GRo8LWO7WNQApXY8NfWQXKReKC5K1ETA/P/F9Vtak8Mr48gahdXkMDYi/FBT5734luhRzWsTXaDwy/S7EbJLEalDXyNC+gGL1WzaR0tQIDAQAB";
	void OnEnable() {
		Debug.Log ("xixi 设置回调");
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}

	void OnDisable (){
		// Remove all event handlers
		OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

	void Start()
	{
		Debug.Log ("xixi 初始化");
		// Map skus for different stores       
		OpenIAB.mapSku(SKU, OpenIAB_Android.STORE_GOOGLE, "com.steve.test.hhh");
		var options = new Options();

		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;

		options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE, OpenIAB_Android.STORE_YANDEX };

		options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, publicKey} };

		options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;

		options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;

//		options.checkInventory = false;

		options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;

		OpenIAB.init(options);



	}

//	void Start () {
//		//初始化操作
//		// Map SKUs for Google Play
//
//		// Set some library options
//		var options = new Options();
//
//		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
//
//		options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE, OpenIAB_Android.STORE_YANDEX };
//
//		options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, publicKey} };
//
//		options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;
//
//		OpenIAB.init(options);
//
//
//	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void queryInventory (){
		Debug.Log ("xixi 查询");
		OpenIAB.queryInventory();
	}


	public void OnPay (string sku){
		Debug.Log("xixi 购买商品");
		if (null == _inventory) {
			Debug.Log ("xixi _inventory is null");
		} else {
			if (_inventory.HasPurchase ("com.steve.test.hhh")) {
				Debug.Log ("xixi 测试显示已购买");

			} else {
				Debug.Log("xixi 测试显示未购买");

			}

		}

		OpenIAB.purchaseProduct("com.steve.test.hhh");
		if (_inventory != null && _inventory.HasPurchase ("com.steve.test.hhh")) {
			Debug.Log ("xixi 消耗");
			OpenIAB.consumeProduct (_inventory.GetPurchase ("com.steve.test.hhh"));

		} else {
			Debug.Log ("xixi 不消耗");

		}


//		GetComponent<ExternalManager> ().BuyProduct ("com.steve.test.sss");

	}

	public void ShowLeaderBoard (){
		Debug.Log ("调用排行榜");
//		GetComponent<ExternalManager> ().BuyProduct ();

	}
		
	void billingSupportedEvent()
	{
		Debug.Log("xixi 支持事件");

		_isInitialized = true;
		Debug.Log("billingSupportedEvent");
	}
	void billingNotSupportedEvent(string error)
	{
		Debug.Log("xixi 不支持事件");

		Debug.Log("billingNotSupportedEvent: " + error);
	}
	void queryInventorySucceededEvent(Inventory inventory)
	{
		Debug.Log("xixi 查询成功" + inventory);

		Debug.Log("queryInventorySucceededEvent: " + inventory);
		if (inventory != null)
		{
			_label = inventory.ToString();
			_inventory = inventory;
		}
	}
	void queryInventoryFailedEvent(string error)
	{
		Debug.Log("xixi 查询失败");

		Debug.Log("queryInventoryFailedEvent: " + error);
		_label = error;
	}
	void purchaseSucceededEvent(Purchase purchase)
	{
		Debug.Log("xixi 购买成功");

		Debug.Log("purchaseSucceededEvent: " + purchase);
		_label = "PURCHASED:" + purchase.ToString();
	}
	void purchaseFailedEvent(int errorCode, string errorMessage)
	{
		Debug.Log("xixi 购买失败");

		Debug.Log("purchaseFailedEvent: " + errorMessage);
		_label = "Purchase Failed: " + errorMessage;
	}
	void consumePurchaseSucceededEvent(Purchase purchase)
	{
		Debug.Log("xixi 消耗成功");

		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
		_label = "CONSUMED: " + purchase.ToString();
	}
	void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("xixi 消耗失败");

		Debug.Log("consumePurchaseFailedEvent: " + error);
		_label = "Consume Failed: " + error;
	}



}
