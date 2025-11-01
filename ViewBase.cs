using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    // ファイルの書き込み・読み込みをするときに使う

public class ViewBase : MonoBehaviour
{
    


    // ビューのトランジション
    UITransition transition = null;
    public UITransition Transition
    {
        get
        {
            if (transition == null) transition = GetComponent<UITransition>();
            return transition;
        }
    }
    // シーンベースクラス
    public SceneBase Scene = null;

    //  --------------------------------
    // ビューオープン時コール
    //  --------------------------------
    // virtual 継承先で上書きできるようにするためのキーワード。
    public virtual void OnViewOpened()
    {

    }
    
    //  --------------------------------
    // ビュークローズ時コール
    //  --------------------------------
    public virtual void OnViewClosed()
    {

    }
}
