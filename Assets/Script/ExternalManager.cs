using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SocialPlatforms;
public class ExternalManager : MonoBehaviour {

	bool debug = false;

	public Dictionary<string, string> productsDict;

	[DllImport("__Internal")]
	private static extern void hapticFeedback ();

	[DllImport("__Internal")]
	private static extern void post (string arg);

	[DllImport("__Internal")]
	private static extern void shareWeibo(string arg, string filepath);

	[DllImport("__Internal")]
	private static extern void shareFacebook(string arg, string filepath);

	[DllImport("__Internal")]
	private static extern void shareTwitter(string arg, string filepath);

	[DllImport("__Internal")]
	private static extern void restoreTransaction();

	[DllImport("__Internal")]
	private static extern void loginGC();

	[DllImport("__Internal")]
	private static extern void showGCLeaderboard(string arg);

	[DllImport("__Internal")]
	private static extern void showGCAchievements();

	[DllImport("__Internal")]
	private static extern void uploadLeaderboard(string arg, string leaderboardID);

	[DllImport("__Internal")]
	private static extern void shareApp(string arg, string address);

	[DllImport("__Internal")]
	private static extern void commentApp(string arg);

	[DllImport("__Internal")]
	private static extern void getLanguage();

	[DllImport("__Internal")]
	private static extern void buyProduct(string arg);

	[DllImport("__Internal")]
	private static extern void getProducts();


	public void HapticFeedback()
	{
		if (this.debug == false) {
			#if UNITY_IOS
			hapticFeedback ();
			#endif
		}
	}

	public void Post(string msg)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			post (msg);
			#endif
		}
	}

	public void ShareApp(string message, string filePath)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			shareApp (message, "");
			#endif

			#if UNITY_ANDROID
			//https://play.google.com/store/apps/details?id=com.lemonjamstudio.summation
			AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
			ajo.Call("ShareApp", message, "Share Summation", filePath);
			#endif
		}
	}
		

	public void ShowAchievements()
	{
		if (this.debug == false) {
			#if UNITY_IOS
			showGCAchievements();
			#endif

			#if UNITY_ANDROID
			Social.ShowAchievementsUI ();
			#endif
		}
	}

	public void Leaderboard(int score, string leaderboardID)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			uploadLeaderboard (score.ToString (), leaderboardID);
			#endif

			#if UNITY_ANDROID
			string leaderboardID_android = "";
			if(leaderboardID == "summation_timing"){
				leaderboardID_android = "CgkI4ueo8aASEAIQAg";
			}
			if(leaderboardID == "summation_ranklist"){
				leaderboardID_android = "CgkI4ueo8aASEAIQAA";
			}
			Social.ReportScore(score, leaderboardID, (bool success) => {
			// handle success or failure
			});
			#endif
		}
	}
		
	public void CommentApp()
	{
		if (this.debug == false) {
			#if UNITY_IOS
			commentApp (GameData.instance ().APPID);
			#endif

			#if UNITY_ANDROID
			AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
			ajo.Call("RateApp", "com.lemonjamstudio.summation");
			#endif
		}
	}

	public void BuyProduct(string arg)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			buyProduct (arg);
			#endif

			#if UNITY_ANDROID
			Debug.Log("steve BuyProduct");
			AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
			ajo.Call("BuyProduct", arg);
			#endif
		}
	}

	public void GetProducts()
	{
		if (this.debug == false) {
			#if UNITY_IOS
			getProducts ();
			#endif

			#if UNITY_ANDROID
			AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
			ajo.Call("GetProducts");
			#endif
		} else {
//			GameObject.Find ("ShopListener").GetComponent<ShopListener> ().Products ("SM_REMOVEADS,￥12.00,SM_HINTS,￥6.00,");
//			GameObject.Find ("ShopListener").GetComponent<ShopListener> ().Products ("");
		}
	}

	public void PayCallBack()
	{
		Debug.Log ("steve 购买完成回调！");
	}

	public void Products(string str)
	{
		print ("steve Products:" + str);
		productsDict = new Dictionary<string, string>();
		if (str != "") {
			str = str.Substring (0, str.Length - 1);
			string[] data = str.Split (',');

			for (int i = 0; i < data.Length; i += 2) {
				productsDict.Add (data [0 + i], data [1 + i]);
				//				print (data[0 + i] + "/" + data[1 + i]);
			}
		}

	}



	public void RestoreTransaction()
	{
		if (this.debug == false) {
			#if UNITY_IOS
			restoreTransaction ();
			#endif

			#if UNITY_ANDROID
			AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
			ajo.Call("GetProducts");
			#endif
		}
	}
		
	public void ShareWeibo(string msg, string filepath)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			shareWeibo (msg, filepath);
			#endif
		}
	}
	public void ShareTwitter(string msg, string filepath)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			shareTwitter (msg, filepath);
			#endif
		}
	}

	public void ShareFacebook(string msg, string filepath)
	{
		if (this.debug == false) {
			#if UNITY_IOS
			shareFacebook (msg, filepath);
			#endif
		}
	}
		
	public void AndroidQuitGame()
	{
		#if UNITY_ANDROID
		AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
		ajo.Call("QuitGame");
		#endif
	}
}
