//
// Source code recreated from a .class file by IntelliJ IDEA
// (powered by Fernflower decompiler)
//

package com.steve.unityDemo;

import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.content.IntentSender.SendIntentException;
import android.net.Uri;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;
import android.util.Log;
import android.widget.Toast;

import com.android.vending.billing.IInAppBillingService;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
import java.io.File;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.Timer;
import java.util.UUID;
import org.json.JSONException;
import org.json.JSONObject;

public class MainActivity extends UnityPlayerActivity {
    IInAppBillingService mService;
    Context mContext;
    ServiceConnection mServiceConn;
    private static final String TAG = "ManiActivity";
    private static MainActivity activity = null;

    Timer timer = new Timer();
//    private String ad_sku = "summation_removeads";
    private static boolean flag =true;

    public MainActivity() {
    }

    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        this.mContext = this.getApplicationContext();
        this.mServiceConn = new ServiceConnection() {
            public void onServiceDisconnected(ComponentName name) {
                MainActivity.this.mService = null;
            }

            public void onServiceConnected(ComponentName name, IBinder service) {
                MainActivity.this.mService = IInAppBillingService.Stub.asInterface(service);

                try {
                    int e = MainActivity.this.mService.isBillingSupported(3, MainActivity.this.mContext.getPackageName(), "inapp");
                } catch (RemoteException var4) {
                    var4.printStackTrace();
                }

            }
        };
        activity =this;
        Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingService.BIND");
        serviceIntent.setPackage("com.android.vending");
        this.bindService(serviceIntent, this.mServiceConn, 1);


    }

    public void onDestroy() {
        super.onDestroy();
        if(this.mService != null) {
            this.unbindService(this.mServiceConn);
        }

    }

    public void GetPurchases(String purchase) throws RemoteException {
        Bundle ownedItems = this.mService.getPurchases(3, this.mContext.getPackageName(), "inapp", (String)null);
        int response = ownedItems.getInt("RESPONSE_CODE");
        if(response == 0) {
            ArrayList ownedSkus = ownedItems.getStringArrayList("INAPP_PURCHASE_ITEM_LIST");
            ArrayList purchaseDataList = ownedItems.getStringArrayList("INAPP_PURCHASE_DATA_LIST");
            ArrayList signatureList = ownedItems.getStringArrayList("INAPP_DATA_SIGNATURE_LIST");
            String continuationToken = ownedItems.getString("INAPP_CONTINUATION_TOKEN");

            for(int i = 0; i < purchaseDataList.size(); ++i) {
                String purchaseData = (String)purchaseDataList.get(i);
                String signature = (String)signatureList.get(i);
                String sku = (String)ownedSkus.get(i);
                if (sku.equals(purchase)){
                    flag = true;
                }


                Log.v("MyLib", "sku:" + sku);
            }

            if (flag){
                UnityPlayer.UnitySendMessage("GameObject", "PayCallBack", purchase);
            }else{
                Toast.makeText(MainActivity.this,"Restore Transaction Failed!",Toast.LENGTH_SHORT);

            }



        }

    }

    public void GetProducts() throws RemoteException, JSONException {
        String data = "";
        ArrayList skuPartList = new ArrayList();
        skuPartList.add("summation_removeads");
        skuPartList.add("summation_hints10");
        Bundle querySkus = new Bundle();
        querySkus.putStringArrayList("ITEM_ID_LIST", skuPartList);
        Bundle skuDetails = this.mService.getSkuDetails(3, this.mContext.getPackageName(), "inapp", querySkus);
        int response = skuDetails.getInt("RESPONSE_CODE");
        Log.v("MyLib", "response:" + String.valueOf(response));
        if(response == 0) {
            ArrayList responseList = skuDetails.getStringArrayList("DETAILS_LIST");

            String sku;
            String price;
            for(Iterator var7 = responseList.iterator(); var7.hasNext(); data = data + sku + "," + price + ",") {
                String thisResponse = (String)var7.next();
                JSONObject object = new JSONObject(thisResponse);
                sku = object.getString("productId");
                price = object.getString("price");
            }

            UnityPlayer.UnitySendMessage("GameObject", "Products", data);
        }

    }

    public void BuyProduct(String pID) throws RemoteException, SendIntentException {
        Bundle buyIntentBundle = this.mService.getBuyIntent(3, this.mContext.getPackageName(), pID, "inapp", UUID.randomUUID().toString());
        PendingIntent pendingIntent = (PendingIntent)buyIntentBundle.getParcelable("BUY_INTENT");
        Log.v("steve", "Android  购买");
//        UnityPlayer.UnitySendMessage("GameObject", "PayCallBack", "");
        this.startIntentSenderForResult(pendingIntent.getIntentSender(), 1001, new Intent(), Integer.valueOf(0).intValue(), Integer.valueOf(0).intValue(), Integer.valueOf(0).intValue());
    }

    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if(requestCode == 1001) {
            int responseCode = data.getIntExtra("RESPONSE_CODE", 0);
            String purchaseData = data.getStringExtra("INAPP_PURCHASE_DATA");
            String dataSignature = data.getStringExtra("INAPP_DATA_SIGNATURE");
            if(resultCode == -1) {
                try {
                    JSONObject e = new JSONObject(purchaseData);
                    String sku = e.getString("productId");
//                    UnityPlayer.UnitySendMessage("ShopListener", "Transaction", sku);
                    //发送消息给Unity，第一个参数一Object的名字，第二个参数是该Object上挂载的脚本的某个方法，第三个为方法的参数

                    UnityPlayer.UnitySendMessage("GameObject", "PayCallBack", "");

//                    if (sku.equals(ad_sku) ){
//                        Log.d(TAG,"Not consume");
//                    }else{
//                        Log.d(TAG,"consume");

                        this.mService.consumePurchase(3, this.mContext.getPackageName(), e.getString("purchaseToken"));
//                    }

                } catch (JSONException var9) {
                    var9.printStackTrace();
                } catch (RemoteException var10) {
                    var10.printStackTrace();
                }
            }
        } else {
//            UnityPlayer.UnitySendMessage("ShopListener", "HideLoading", "");
        }
//        UnityPlayer.UnitySendMessage("ShopListener", "HideLoading", "");


    }


    public void ShareApp(String msg, String title, String imgPath) {
        Intent sharingIntent = new Intent("android.intent.action.SEND");
        sharingIntent.setType("text/plain");
        File f = new File(imgPath);
        if(f != null && f.exists() && f.isFile()) {
            sharingIntent.setType("image/jpg");
            Uri u = Uri.fromFile(f);
            sharingIntent.putExtra("android.intent.extra.STREAM", u);
        }

        sharingIntent.putExtra("android.intent.extra.SUBJECT", "Share Game");
        sharingIntent.putExtra("android.intent.extra.TEXT", msg);
        this.startActivity(Intent.createChooser(sharingIntent, title));
    }


    public void shareWithText(String text){
        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.putExtra(Intent.EXTRA_TEXT, text);
        shareIntent.setType("text/plain");
        startActivity(Intent.createChooser(shareIntent, "分享到"));
    }

}
