using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;    // TextMeshProを使うため
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

//  --------------------------------
// 会話ウィンドウ
//  --------------------------------
public class TalkWindow : MonoBehaviour
{
    // 名前のテキスト
    [SerializeField] TextMeshProUGUI nameText = null;
    // 会話内容のテキスト
    [SerializeField] TextMeshProUGUI talkText = null;
    // 次ページへ表示画像
    [SerializeField] Image nextArrow = null;
    // 会話のトランジション
    [SerializeField] UITransition talkWindowTransition = null;

    // キャラデータ
    [SerializeField] CharacterData data = null;

    // 左キャラ画像・トランジション
    [SerializeField] Image leftCharacterImage = null;
    [SerializeField] UITransition leftCharacterImageTransition = null;
    // 真ん中キャラ画像・トランジション
    [SerializeField] Image centerCharacterImage = null;
    [SerializeField] UITransition centerCharacterImageTransition = null;
    // 右キャラ画像・トランジション
    [SerializeField] Image rightCharacterImage = null;
    [SerializeField] UITransition rightCharacterImageTransition = null;

    // 選択肢
    [SerializeField] SelectButtonDialog selectButtonDialog = null;

    // 背景データ
    [SerializeField] BackgroundData bgData = null;
    // 背景イメージ
    [SerializeField] Image bgImage = null;
    // 背景トランジション
    [SerializeField] UITransition bgTransition = null;



    // 会話パラメータリスト
    //[SerializeField] public List<StoryData> Talks = new List<StoryData>();

    // 現在のキャラ情報
    string currentLeft = "";
    string currentCenter = "";
    string currentRight = "";

    // 次へフラグ
    bool goToNextPage = false;
    // 次へ行けるフラグ
    bool currentPageCompleted = false;
    // スキップフラグ
    bool isSkip = false;

    
    

    void Awake()
    {
        // 前のシーンから来た時にキャラとかメッセージを非表示にしとく
        SetCharacter(null).Forget();
        talkWindowTransition.gameObject.SetActive(false);
    }

    void Start()
    {
        /* 
        await Open();
        await TalkStart(talks);
        await Close();
        */
    }

    //  --------------------------------
    // ウィンドウを開く
    //  --------------------------------
    // initName、initText : 初期表示する名前とテキスト。今は空白。
    public async UniTask Open(string initName = "", string initText = "")
    {
        SetCharacter(null).Forget();
        nameText.text = initName;
        talkText.text = initText;
        nextArrow.gameObject.SetActive(false);
        talkWindowTransition.gameObject.SetActive(true);
        await talkWindowTransition.TransitionInWait();
    }

    //  --------------------------------
    // ウィンドウを閉じる
    //  --------------------------------
    public async UniTask Close()
    {
        await talkWindowTransition.TransitionOutWait();
        talkWindowTransition.gameObject.SetActive(false);
    }

    // 色変え用追加部分******************
    bool isInTag = false;
    string tagStrings = "";
    // ********************************

    //  --------------------------------
    // 会話の開始
    //  --------------------------------
    // wordInterval : 1文字ずつ表示する間隔(秒)
    public async UniTask<List<int>> TalkStart(List<StoryData> talkList, float wordInterval = 0.05f)
    {
        try
        {
            currentLeft = "";
            currentCenter = "";
            currentRight = "";

            List<int> responseList = new List<int>();

            foreach (var talk in talkList)
            {
                // string.IsNullOrEmpty( 文字列 ) : 文字列が「Null（値がない）」か空（「””」の場合）の時「true」
                // 背景が変わらない場合空白が送られてくるため、空白じゃない時だけSetBgを実行する
                if (string.IsNullOrEmpty(talk.Place) == false)
                {
                    await SetBg(talk.Place, false);
                }
                // 選択肢の場合
                if (talk.Name == "30")
                {
                    goToNextPage = false;
                    currentPageCompleted = false;
                    isSkip = false;
                    nextArrow.gameObject.SetActive(false);
                    SetCharacter(talk).Forget();

                    // 文字列.Split( ‘分割のキー’ ) : 文字列を分割のキーで分割して配列を返す
                    // シングルクォーテーションをダブルに変えると意味合いが変わってくる（同じように処理できない）
                    string[] arr = talk.Talk.Split(',');    // 文字列をカンマで分割

                    // 引数：bgOpenにtrue、selectParamsにarr（選択肢の文字列）
                    // 選択されるまで待機。選択されたら、押されたボタンの番号を受け取りresにいれる。
                    var res = await selectButtonDialog.CreateButtons(true, arr);

                    responseList.Add(res);

                    // 選択肢が押されたらフラグをtrueにして先に進めるようにしておく
                    goToNextPage = true;
                }
                else
                {
                    nameText.text = data.GetCharacterName(talk.Name);
                    talkText.text = "";
                    goToNextPage = false;
                    currentPageCompleted = false;
                    isSkip = false;
                    nextArrow.gameObject.SetActive(false);
                    await SetCharacter(talk);

                    // ページが開いたときに少し間を開ける。（なくてもよい）
                    await UniTask.Delay((int)(0.5f * 1000f),cancellationToken : this.gameObject.GetCancellationTokenOnDestroy());

                    // stringはforeach内ではchar型で扱われる
                    foreach (char word in talk.Talk)
                    {
                        // 色変え用追加部分**********************
                        bool isCloseTag = false;
                        if (word.ToString() == "<")
                        {
                            isInTag = true;
                        }
                        else if (word.ToString() == ">")
                        {
                            isInTag = false;
                            isCloseTag = true;
                        }

                        if (isInTag == false && isCloseTag == false && string.IsNullOrEmpty(tagStrings) == false)
                        {
                            var _word = tagStrings + word;
                            talkText.text += _word;
                            tagStrings = "";
                        }
                        else if (isInTag == true || isCloseTag == true)
                        {
                            tagStrings += word;
                            continue;
                        }
                        else
                        {
                            talkText.text += word;
                        }
                        // ************************************あと2行下をコメントアウトしました。
                        // 1文字ずつくっつける
                        // talkText.text += word;
                        // 0.2秒待ってから次のループへ
                        await UniTask.Delay((int)(wordInterval * 1000f), cancellationToken : this.gameObject.GetCancellationTokenOnDestroy());

                        // テキスト表示中にタップされたとき
                        if (isSkip == true)
                        {
                            // テキストを一気に表示してループを抜ける
                            talkText.text = talk.Talk;
                            break;
                        }
                    }
                }

                currentPageCompleted = true;
                nextArrow.gameObject.SetActive(true);
                // 次へボタンが押されるまで待機するときの設定
                await UniTask.WaitUntil(() => goToNextPage == true, cancellationToken : this.gameObject.GetCancellationTokenOnDestroy());
            }

            // 選択肢がある場合の答えをリストとして返す
            return responseList;
        }
        catch(System.OperationCanceledException)
        {
            throw;
        }
    }

    //  --------------------------------
    // 次へボタンが押されたとき
    //  --------------------------------
    public void OnNextButtonClicked()
    {
        // 画面がタップされたとき、テキスト表示が完了していたら次へフラグをtrueにする
        if (currentPageCompleted == true) goToNextPage = true;
        else isSkip = true;
    }

    //  --------------------------------
    // キャラ画像の設定
    //  --------------------------------
    async UniTask SetCharacter(StoryData storyData)
    {
        // Nullならすべて消す
        if (storyData == null)
        {
            leftCharacterImage.gameObject.SetActive(false);
            centerCharacterImage.gameObject.SetActive(false);
            rightCharacterImage.gameObject.SetActive(false);
            return;
        }

        try
        {
            // タスク内容をしまっておくリスト
            var tasks = new List<UniTask>();
            bool hideLeft = false;
            bool hideCenter = false;
            bool hideRight = false;

            // 左キャラ設定
            // string.IsNullOrEmpty( 文字列 ) : 文字列が「Null」か何もない（「””」のこと。値はあるが中身がない状態）場合「true」
            if (string.IsNullOrEmpty(storyData.Left) == true)
            {
                tasks.Add(leftCharacterImageTransition.TransitionOutWait());
                hideLeft = true;
            }
            // キャラが変わるとき
            else if (currentLeft != storyData.Left)
            {
                var img = data.GetCharacterSprite(storyData.Left);
                leftCharacterImage.sprite = img;
                leftCharacterImage.gameObject.SetActive(true);
                tasks.Add(leftCharacterImageTransition.TransitionInWait());

                currentLeft = storyData.Left;
            }

            // 真ん中キャラ設定
            if (string.IsNullOrEmpty(storyData.Center) == true)
            {
                tasks.Add(centerCharacterImageTransition.TransitionOutWait());
                hideCenter = true;
            }
            else if (currentCenter != storyData.Center)
            {
                var img = data.GetCharacterSprite(storyData.Center);
                centerCharacterImage.sprite = img;
                centerCharacterImage.gameObject.SetActive(true);
                tasks.Add(centerCharacterImageTransition.TransitionInWait());

                currentCenter = storyData.Center;
            }

            // 右キャラ設定
            if (string.IsNullOrEmpty(storyData.Right) == true)
            {
                tasks.Add(rightCharacterImageTransition.TransitionOutWait());
                hideRight = true;
            }
            else if (currentRight != storyData.Right)
            {
                var img = data.GetCharacterSprite(storyData.Right);
                rightCharacterImage.sprite = img;
                rightCharacterImage.gameObject.SetActive(true);
                tasks.Add(rightCharacterImageTransition.TransitionInWait());

                currentRight = storyData.Right;
            }
            // 待機
            // UniTask.WhenAll -> 複数の「UniTask」を同時に待機する
            await UniTask.WhenAll(tasks);

            // 消したいキャラを消す
            if (hideLeft == true) leftCharacterImage.gameObject.SetActive(false);
            if (hideCenter == true) centerCharacterImage.gameObject.SetActive(false);
            if (hideRight == true) rightCharacterImage.gameObject.SetActive(false);
        }
        catch(System.OperationCanceledException)
        {
            throw;
        }
    }

    //  --------------------------------
    // 背景の設定
    //  --------------------------------
    // isImmediate -> 遷移処理（つまりトランジション）を入れるか入れないかのbool変数
    public async UniTask SetBg(string place, bool isImmediate)
    {
        // テキストから対応する画像を見つけてきてspにいれる
        var sp = bgData.GetSprite(place);
        bgTransition.gameObject.SetActive(true);

        // 今の背景の名前を調べて、spに入ってる背景の名前と比較する
        var currentBg = bgImage.sprite.name;
        if (currentBg == sp.name)
        {
            return;
        }

        if (isImmediate == false)
        {
            try
            {
                await bgTransition.TransitionOutWait();
                bgImage.sprite = sp;
                await bgTransition.TransitionInWait();
            }
            catch(System.OperationCanceledException)
            {
                throw;
            }
        }
        else
        {
            bgImage.sprite = sp;
        }
    }
}
