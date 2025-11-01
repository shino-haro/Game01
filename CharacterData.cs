using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CreateAssetMenu : アセットメニュー（Projectウインドウ右クリックなど）で出るメニューにアセットを作成する項目を追加。
// filrName -> アセットを作成した時の初期ファイル名、menuName -> アセットメニューに追加されるメニューの名前。「/」で区切ると階層構造になる
[CreateAssetMenu(fileName="CharacterData", menuName="ScriptableObjects/CharakterData")]
public class CharacterData : ScriptableObject
{
    //  --------------------------------
    // キャラクター定義
    //  --------------------------------
    // 　enum型：定数をグループにする型
    public enum Type
    {
        None=0,
        Io=1,
        Ake=2,
        Toki=3
    }

    //  --------------------------------
    // 差分表情定義
    //  --------------------------------
    public enum EmotionType
    {
        None,
        Normal,
        Emi,
        Naki
    }

    //  --------------------------------
    // 差分画像と表情の関連付けパラメータ
    //  --------------------------------
    [System.Serializable]
    public class ImageParam
    {
        // 表情タイプ
        public EmotionType Emotion = EmotionType.None;
        // 差分画像
        public Sprite Sprite = null;
    }

    //  --------------------------------
    // キャラのパラメータ
    //  --------------------------------
    [System.Serializable]
    public class Parameter
    {
        // 名前
        public string DisplayName = "";
        // キャラタイプ
        public Type Character = Type.None;
        // 差分画像パラメータのリスト
        public List<ImageParam> ImageParams = new List<ImageParam>();

        //  --------------------------------
        // 表情タイプから差分画像を取得する
        //  --------------------------------
        public Sprite GetEmotionSprite(EmotionType emotion)
        {
            foreach(var img in ImageParams)
            {
                if(img.Emotion == emotion)return img.Sprite;
            }
            return null;
        }
    }

    // メモ
    [SerializeField] string Memo="";
    // パラメータリスト
    public List<Parameter> Parameters = new List<Parameter>();

    //  --------------------------------
    // キャラ番号から表示名を取得
    //  --------------------------------
    public string GetCharacterName(string characterNumber)
    {
        // １～３の場合
        if(characterNumber == "1"||characterNumber=="2"||characterNumber=="3")
        {
            var param = GetParameterFromNumber(characterNumber);
            return param.DisplayName;
        }

        // その他の場合は何もなしで返す
        return "";
    }

    //  --------------------------------
    // キャラ番号からパラメータを取得
    //  --------------------------------
    // 返り値はParameter。
    Parameter GetParameterFromNumber(string characterNumber)
    {
        // パラメータを総検索する
        foreach(var param in Parameters)
        {
            // type型をint型に
            var typeInt=(int)param.Character;
            var typeStr=typeInt.ToString();

            if(typeStr==characterNumber)return param;
        }
        return null;
    }

    //  --------------------------------
    // 文字列のデータからキャラ画像を取得する
    //  --------------------------------
    public Sprite GetCharacterSprite(string dataString)   
    {
        // 先頭の一文字を抜き出す
        // 文字列.SubString( 開始インデックス, 文字数（省略すると最後まで)） : 文字列の指定部分を抜き出す
        var num = dataString.Substring(0,1);
        // それ以外
        var emo = dataString.Substring(1);

        if(num!="0" && num!="1" && num!="2" && num!="3")
        {
            Debug.Log("入力データがまちがってるねぇ"+dataString);
            return null;
        }

        var param = GetParameterFromNumber(num);
        var emotion = GetEmotionType(emo);
        var sprite = param.GetEmotionSprite(emotion);

        return sprite;
    }

    //  --------------------------------
    // 表情部分の文字列からEmotionTypeを取得する
    //  --------------------------------
    EmotionType GetEmotionType(string emotionString)
    {
        // switch( 条件 )…case ( 値 ): その時の処理.
        switch (emotionString)
        {
            case "Normal":return EmotionType.Normal;
            case "Emi":return EmotionType.Emi;
            case "Naki":return EmotionType.Naki;
            default:return EmotionType.None;
        }
    }
}
