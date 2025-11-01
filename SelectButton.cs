using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;   // イベント用
using Cysharp.Threading.Tasks;  // 待機処理用
using TMPro;    // テキスト操作用

//  --------------------------------
// 選択肢のボタン
//  --------------------------------
public class SelectButton : MonoBehaviour
{
    // テキスト
    [SerializeField] TextMeshProUGUI buttonText = null;
    // トランジション
    [SerializeField] UITransition transition = null;
    // クリックイベント定義
    // public class ClickEvent -> クラス： UnityEvent<int>{}; -> int型を入れて継承
    public class ClickEvent : UnityEvent<int>{};
    // クリックイベント
    public ClickEvent OnClicked = new ClickEvent();
    // ボタンインデックス
    public int buttonIndex = 0;

    void Start()
    {
    }

    // 作成時コール
    // 引数（ボタンのテキスト内容, ボタンの識別用番号, 引数にintを持つクリック時の処理onClick）
    public async UniTask OnCreated(string txt, int index, UnityAction<int> onClick)
    {
        // プレイヤーに見えないように、まず透明にしておく
        transition.Canvas.alpha = 0;
        buttonText.text = txt;  // ボタンのテキスト設定
        buttonIndex = index;    // ボタンの識別用番号を保管
        // イベント.AddListener( 関数 ) : イベントに関数を登録
        OnClicked.AddListener(onClick);  // クリック時の処理をAddListenerで登録

        // 表示完了を待機する
        await transition.TransitionInWait();
    }

    //  --------------------------------
    // 閉じる
    //  --------------------------------
    public async UniTask Close()
    {
        if (transition != null)
            // 閉じきるまで待機してから削除
            await transition.TransitionOutWait();
        if (this != null)
            Destroy(gameObject);
    }

    //  --------------------------------
    // ボタンクリックコールバック
    //  --------------------------------
    public void OnButtonClicked()
    {
        // buttonIndexを引数にいれてInvokeで実行することで、押されたボタンの識別が可能になる
        OnClicked.Invoke(buttonIndex);
    }
}
