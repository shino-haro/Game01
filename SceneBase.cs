using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SceneBase : MonoBehaviour
{
    // 初期ビューインデックス
    // protected 継承先のクラスにのみ公開。
    [SerializeField] protected int initialViewIndex = 0;
    // 初期ビュートランジションフラグ
    [SerializeField] protected bool isInitialTransition = true;
    // ビューのリスト
    [SerializeField] protected List<ViewBase> viewList = new List<ViewBase>();
    // 現在のビュー
    protected ViewBase currentView = null;

    protected virtual void Start()
    {
        // 初期インデックスがちゃんと設定されてるかチェック
        if (initialViewIndex >= 0)
        {
            // ビューのリストすべてに
            foreach (var view in viewList)
            {
                // シーンを設定
                // this 変数Sceneにこのオブジェクト自身を補完
                view.Scene = this;
                // 初期インデックスのビューに対しての処理
                // IndexOf(要素)　要素のインデックスを取得
                if (viewList.IndexOf(view) == initialViewIndex)
                {
                    // トランジションする場合
                    if (view.Transition != null && isInitialTransition == true)
                    {
                        // アルファを0(透明)にして非表示にする
                        view.Transition.Canvas.alpha = 0;
                        view.gameObject.SetActive(true);
                        view.OnViewOpened();
                        view.Transition.TransitionIn();
                    }
                    // トランジションしない場合
                    else
                    {
                        view.OnViewOpened();
                        view.gameObject.SetActive(true);
                    }
                    // 現在のビューを設定
                    currentView = view;
                }
                // 初期ビュー以外に対しての処理
                else
                {
                    view.gameObject.SetActive(false);
                }
            }
        }
    }

    //  --------------------------------
    // シーン移動処理
    //  --------------------------------
    public virtual async UniTask ChangeScene(string sceneName)
    {
        // 現在のビューが設定されている場合close処理を使う
        if (currentView != null)
        {
            currentView.OnViewClosed();
            if (currentView.Transition != null) await currentView.Transition.TransitionOutWait();
        }
        // SceneManager.LoadSceneAsync(シーン名) 非同期でシーン名のシーンに移動
        await SceneManager.LoadSceneAsync(sceneName);
    }

    //  --------------------------------
    // ビュー移動処理
    //  --------------------------------
    public virtual async UniTask ChangeView(int index)
    {
        // 現在のビューが設定されている場合close処理を使う
        if (currentView != null)
        {
            currentView.OnViewClosed();
            if (currentView.Transition != null) await currentView.Transition.TransitionOutWait();
        }
        // ビューリストから指定のインデックスビューを検索
        foreach (var view in viewList)
        {
            // 該当ビューに
            if (viewList.IndexOf(view) == index)
            {
                // オープン処理を行う
                view.gameObject.SetActive(true);
                view.OnViewOpened();
                if (view.Transition != null) await view.Transition.TransitionInWait();

                currentView = view;
            }
            else
            {
                view.gameObject.SetActive(false);
            }
        }
    }
}
