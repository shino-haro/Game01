using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // DOTweenを有効化
using System.Threading; // スレッド関連の機能を使用するための物
using UnityEngine.Events;   // UnityActionを使うために必要
using Cysharp.Threading.Tasks;
using System.Numerics;  // UniTaskを使うために必要

// UITransitionをアタッチすると自動的にCanvasGroupもアタッチされるようになる
[RequireComponent(typeof(CanvasGroup))]
public class UITransition : MonoBehaviour
{
    // レクトトランスフォーム取得用
    public RectTransform Rect
    {
        // プロパティを取得できるようにする。
        // setを使用した場合、指定した変数に値を代入できるようになる
        get
        {
            // 変数rectがnullなら、このコンポーネントが付与されているゲームオブジェクトから「RectTransform」を取得して「rect」に保管
            // 簡単に言うと、変数rectをnullでなくするための処理。Nullチェックってやつ。
            if(rect==null)rect=GetComponent<RectTransform>();
            return rect;
        }
    }
    // レクトトランスフォーム保管用
    RectTransform rect = null;

    // 設定値
    // 数値パラメータを定義しておくクラス。
    // Vector2も変数の型の内のひとつ。だと思う。
    [System.Serializable]
    public class TransitionParam
    {
        // 実行フラグ
        public bool IsActive = true;
        // インの値　(フェードインの初期透明度, フェードインの最終透明度)
        public UnityEngine.Vector2 In=new UnityEngine.Vector2(0,1f);
        // アウトの値　(フェードアウトの初期透明度,フェードアウトの最終透明度)
        public UnityEngine.Vector2 Out= new UnityEngine.Vector2(1f,0);
    }

    // フェード設定値
    // UnityのInspectorに表示して値を変更できるようにしてるとこ。
    [SerializeField] TransitionParam fade = new TransitionParam();
    // スケール設定値
    // IsActiveをfalseにすることで、初期状態では実行されないように設定。使うときはUnityのInspectorでフラグをtrueに変える。
    [SerializeField] TransitionParam scale = new TransitionParam(){ IsActive = false, In = UnityEngine.Vector2.zero, Out = UnityEngine.Vector2.zero };
    // 遷移時間
    [SerializeField] float duration = 1f;

    // インのシーケンス
    Sequence inSequence = null;
    // アウトのシーケンス
    Sequence outSequence = null;

    // キャンバスグループ取得用
    public CanvasGroup Canvas
    {
        get
        {
            if (this == null) return null;
            if (canvas == null) canvas = GetComponent<CanvasGroup>();
            return canvas;
        }
    }

    // キャンバスグループ保管用
    CanvasGroup canvas =null;

    // CancellationTokenSource キャンセルトークン。TaskやUniTaskの処理を途中でキャンセルするために使う。
    // インのキャンセルトークン
    CancellationTokenSource inCancellation = null;
    // アウトのキャンセルトークン
    CancellationTokenSource outCancellation = null;

    // async　awaitを使うときに必要
    void Start()
    {
        
    }

    //  --------------------------------
    // トランジションイン
    //  --------------------------------
    // UnityAction　関数などの処理のまとまりを引数にして実行するための型。
    public void TransitionIn(UnityAction onCompleted = null)
    {
        // 既にシーケンスが存在していたら破棄。前に動いているものを停止させてから次を始めるため。
        if (inSequence != null)
        {
            inSequence.Kill();
            inSequence = null;
        }
        inSequence = DOTween.Sequence();

        // &&がand、!=はnot equalって意味。
        if (fade.IsActive == true && Canvas != null)
        {
            Canvas.alpha = fade.In.x;

            // sequence.Join(処理)　カッコ内の処理を、前の処理と同時に行う
            //　sequence.Append(処理)　カッコ内の処理を、前の処理が終わった後に行う
            inSequence.Join
            (
                // <DOTween>
                // CanvasGroup.DOFade( 最終値, 遷移時間 ) : キャンバスグループの「Alpha(透明度)」を遷移時間で変化させる
                Canvas.DOFade(fade.In.y, duration)
                .SetLink(gameObject) // ゲームオブジェクトが破棄されたらアニメーションも停止するように設定
            );
        }

        if(scale.IsActive==true)
        {
            var current = Rect.transform.localScale;
            Rect.transform.localScale = new UnityEngine.Vector3(scale.In.x, scale.In.y, current.z);

            // 〇〇.DOScale( 目標値, 遷移時間 ) : スケールを目標値に遷移時間で変化
            inSequence.Join(
                Rect.DOScale(current, duration)
                .SetLink(gameObject)
            );
        }
        // sequence.OnComplete( (引数) => { 処理 } ) : シークエンス（またはDOTween関数）の完了時に処理を実行する
        // UnityAction.Invoke() : UnityActionを実行する
        inSequence
        .SetLink(gameObject)
        .OnComplete(() => onCompleted?.Invoke());

    }

    //  --------------------------------
    // トランジションアウト
    //  --------------------------------
    public void TransitionOut(UnityAction onCompleted = null)
    {
        if (outSequence != null)
        {
            outSequence.Kill();
            outSequence = null;
        }
        outSequence = DOTween.Sequence();

        if (fade.IsActive == true && Canvas != null)
        {
            Canvas.alpha = fade.Out.x;

            outSequence.Join
            (
                Canvas.DOFade(fade.Out.y, duration)
                .SetLink(gameObject)
            );
        }

        if(scale.IsActive==true)
        {
            var current = Rect.transform.localScale;
            outSequence.Join(
                Rect.DOScale(new UnityEngine.Vector3(scale.Out.x, scale.Out.y, current.z), duration)
                .SetLink(gameObject)
                .OnComplete(()=>Rect.transform.localScale=current)
            );
        }
        outSequence
        .SetLink(gameObject)
        .OnComplete(() => onCompleted?.Invoke());
    }

    //  --------------------------------
    // トランジションイン終了待機
    //  --------------------------------
    public async UniTask TransitionInWait()
    {
        // フラグの用意
        bool isDone = false;
        // 前回の物を残さないようにするためのキャンセル処理
        if (inCancellation != null)
        {
            inCancellation.Cancel();
        }
        inCancellation = new CancellationTokenSource();

        // トランジションを実行して、完了したらisDoneをtrueにする
        TransitionIn(() => { isDone = true; });

        // try 内の処理でエラーや例外が発生するか確認し、発生した場合はcatchの処理を実行する
        try
        {
            // isDoneがtrueになるまで待機する
            // PlayerLoopTiming.Update　毎フレーム更新時にチェックするよって意味。
            // inCancellation.Token　UniTaskの待機処理をキャンセルできるようにするための物。
            await UniTask.WaitUntil(() => isDone == true, PlayerLoopTiming.Update, inCancellation.Token);
        }
        catch (System.OperationCanceledException e)
        {
            Debug.Log("キャンセルされました:" + e);
            throw;
        }
    }

    //  --------------------------------
    // トランジションアウト終了待機
    //  --------------------------------
    public async UniTask TransitionOutWait()
    {
        bool isDone = false;
        if (outCancellation != null)
        {
            outCancellation.Cancel();
        }
        outCancellation = new CancellationTokenSource();

        TransitionOut(() => { isDone = true; });

        try
        {
            await UniTask.WaitUntil(() => isDone == true, PlayerLoopTiming.Update, outCancellation.Token);
        }
        catch (System.OperationCanceledException e)
        {
            Debug.Log("キャンセルされました:" + e);
            throw;
        }
    }
    
    //  --------------------------------
    // 破棄されたときのコールバック
    //  --------------------------------
    // ゲームオブジェクトが破棄されたら止まるようにする
    void OnDestroy()
    {
        if (inCancellation != null)
        {
            inCancellation.Cancel();
        }
        if(outCancellation != null)
        {
            outCancellation.Cancel();
        }
    }
}
