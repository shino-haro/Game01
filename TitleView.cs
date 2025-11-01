using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleView : ViewBase
{
    void Start()
    {        
    }

    // -------------------------------------------------------
    // ビューオープン時コール.
    // -------------------------------------------------------
    public override void OnViewOpened()
    {
        base.OnViewOpened();
    }

    // -------------------------------------------------------
    // ビュークローズ時コール.
    // -------------------------------------------------------
    public override void OnViewClosed()
    {
        base.OnViewClosed();
    }

    //  --------------------------------
    // 画面タップコールバック
    //  --------------------------------
    public async void OnScreenButtonClicked()
    {
        await Scene.ChangeScene("01_Home");
    }
}