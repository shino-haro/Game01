using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SplashView : ViewBase
{
    void Start()
    {        
    }

    // -------------------------------------------------------
    // ビューオープン時コール.
    // -------------------------------------------------------
    public override async void OnViewOpened()
    {
        base.OnViewOpened();

        await ChangeViewWaitForSeconds();
    }

    // -------------------------------------------------------
    // ビュークローズ時コール.
    // -------------------------------------------------------
    public override void OnViewClosed()
    {
        base.OnViewClosed();
    }

    //  --------------------------------
    // 2秒待ってタイトルビューに移動
    //  --------------------------------
    async UniTask ChangeViewWaitForSeconds(float waitTime=2f)
    {
        try
        {
            // Delay() の第1引数はマイクロ秒で指定するため1000をかけてる。
            // Delay(秒数(ms), 時間の経過速度を無視するかどうか, delayの判定を行うタイミング, キャンセルトークン)
            // GetCancellationTokenOnDestroy()　このゲームオブジェクトが破棄されたらキャンセルするトークンを取得する
            await UniTask.Delay((int)(waitTime * 1000f), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            // 非同期処理.Forget() 非同期処理を待たずに次の処理に進む
            Scene.ChangeView(1).Forget();
        }
        catch (System.OperationCanceledException e)
        {
            Debug.Log("キャンセルされました:" + e);
        }
    }
}