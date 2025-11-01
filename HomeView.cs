using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Xml.Linq;

public class HomeView : ViewBase
{
    // ボタントランジション
    [SerializeField] UITransition buttonTransition = null;
    [SerializeField] SaveLoadManager saveLoadManager = null;
    void Start()
    {
    }

    // -------------------------------------------------------
    // ビューオープン時コール.
    // -------------------------------------------------------
    public override async void OnViewOpened()
    {
        base.OnViewOpened();
        await Open();
    }

    //  --------------------------------
    // ウィンドウを開く
    //  --------------------------------
    async UniTask Open()
    {
        // alphaは透明度
        buttonTransition.Canvas.alpha = 0;
        // SetActiveは表示／非表示
        buttonTransition.gameObject.SetActive(true);

        await buttonTransition.TransitionInWait();
    }

    //  --------------------------------
    // ウィンドウを閉じる
    //  --------------------------------
    async UniTask Close()
    {
        await buttonTransition.TransitionOutWait();
        buttonTransition.gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // ビュークローズ時コール.
    // -------------------------------------------------------
    public override void OnViewClosed()
    {
        base.OnViewClosed();
    }
    //  --------------------------------
    // ゲームシーンに移動
    //  --------------------------------
    public async void OnGameButtonClicked()
    {
        await Scene.ChangeScene("02_Game");
    }

    //  --------------------------------
    // セーブデータをリセット
    //  --------------------------------
    public void OnSavedataResetButtonClicked()
    {
        var data = saveLoadManager.Load();
        data.StoryNumber = 0;
        data.TestString = "";
        //saveLoadManager.Save(data);
    }
}