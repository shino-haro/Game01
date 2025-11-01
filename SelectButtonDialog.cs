using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;  // 待機処理用

public class SelectButtonDialog : MonoBehaviour
{
    // 背景のトランジション
    [SerializeField] UITransition bgTransition = null;
    // ボタンの親
    [SerializeField] Transform buttonParent = null;
    // ボタンプレハブ
    [SerializeField] SelectButton buttonPrefab = null;
    // レスポンス
    int response = -1;
    // ボタンリスト
    List<SelectButton> buttons = new List<SelectButton>();

    void Start()
    {
    }

    //  --------------------------------
    // ボタンの生成
    // --------------------------------
    public async UniTask<int> CreateButtons(bool bgOpen, string[] selectParams)
    {
        // ボタンに表示する文字列がないときは-1を返して処理を終える
        if (selectParams == null || selectParams.Length == 0) return -1;

        try
        {
            // UniTaskのリストを準備
            var tasks = new List<UniTask>();
            int index = 0;  // ボタンの数を数える用

            // 背景の設定　trueで表示処理をtasksに保管
            if (bgOpen == true)
            {
                bgTransition.gameObject.SetActive(true);
                tasks.Add(bgTransition.TransitionInWait());
            }
            else
            {
                bgTransition.gameObject.SetActive(false);
            }

            // ボタンの生成
            foreach (var param in selectParams)
            {
                // Instantiate( 生成するゲームオブジェクトのプレハブ, 生成するゲームオブジェクトの親 ) : プレハブからゲームオブジェクトの生成
                var button = Instantiate(buttonPrefab, buttonParent);
                buttons.Add(button);
                tasks.Add(button.OnCreated(param, index, OnAnyButtonClicked));
                index++;
            }

            // レイアウトグループの確実な反映のためにキャンバスを更新
            // Canvas.ForceUpdateCanvases();　キャンバスを強制的に更新
            // VerticalLayoutGroup(ボタン配置を自動で調整してくれるやつ)を確実に更新するためにキャンバスを更新する
            Canvas.ForceUpdateCanvases();

            // 実行予定の処理が全部終了するまで待ち
            await UniTask.WhenAll(tasks);

            // ここで何かしらのボタンが押されるまで待機
            // いずれかのボタンが押されると、responseの値が-1以外になる
            await UniTask.WaitUntil(() => response != -1, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            // Close()関数でresponseが初期化されてしまうので値を別の場所に避難させる
            var res = response;
            // 閉じる
            await Close();

            return res;
        }
        catch (System.OperationCanceledException)
        {
            UnityEngine.Debug.Log("CreateButtonsがキャンセルされたよ");
            throw;
        }
    }

    //  --------------------------------
    // ダイアログを閉じる
    //  --------------------------------
    public async UniTask Close()
    {
        // ボタンを閉じる。tasksトいう名前の、UniTaskリストを準備
        var tasks = new List<UniTask>();
        // ボタン全てにClose()を呼び出して、tasksに追加
        foreach (var b in buttons)
        {
            tasks.Add(b.Close());
        }
        // 背景を閉じる
        if (bgTransition.gameObject.activeSelf == true)
        {
            tasks.Add(bgTransition.TransitionOutWait());
        }

        await UniTask.WhenAll(tasks);
        bgTransition.gameObject.SetActive(false);
        response = -1;
    }

    //  --------------------------------
    // いずれかのボタンがクリックされた
    //  --------------------------------
    void OnAnyButtonClicked(int index)
    {
        // レスポンスを決定
        UnityEngine.Debug.Log(index + "をクリック");
        response = index;
    }
}
