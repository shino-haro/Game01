using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    [System.Serializable]
    public class SaveDataList
    {
        public List<SaveData> saves = new List<SaveData>();
    }
    [System.Serializable]
    public class SaveData
    {
        public int StoryNumber;
        public string TestString;
    }


    //  --------------------------------
    // フォルダとファイルの存在チェック
    //  --------------------------------
    public void Check(string folderPath, string filePath)
    {
        if (Directory.Exists(folderPath) == true)
        {

        }
        else
        {
            // ディレクトリの作成
            Directory.CreateDirectory(folderPath);
        }
        CheckFile(filePath);
    }

    //  --------------------------------
    // セーブファイルが一個もなければ新規作成
    //  --------------------------------
    void CheckFile(string filePath)
    {
        // 指定したパスのファイルがないか
        // File.Exists( パス ) : パス名のファイルが存在するか
        if (File.Exists(filePath) == true)
        {
            // あるとき
            return;
        }
        else
        {
            // ないとき
            var saveData = new SaveDataList();
            var oneSave = new SaveData();

            
            oneSave.TestString = "これはテスト文字列です";

            // 空のセーブスロットをいくつか作っておきたい場合（例：3つ）
            for (int i = 0; i < 4; i++)
            {
                oneSave.StoryNumber = i;
                saveData.saves.Add(oneSave);
            }
            
        
            var json = JsonUtility.ToJson(saveData);

            // 書き込み
            bool isAppend = false;  // 上書き(false)か追記か(多分true)
            // using：ユージングステートメント。プログラムに使ったリソースを、処理が終わったら勝手に開放してくれる
            // 引数（ファイルパス、上書きフラグ、エンコーダ）
            using (var fw = new StreamWriter(filePath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
            {
                fw.Write(json);
            }
        }
    }
    
    //  --------------------------------
    // セーブ
    //  --------------------------------
    //public void Save(SaveData data)
    public void Save(int slotIndex)
    {
        //*****************************************

        string folderPath = Application.persistentDataPath + "/savedata";
        string filePath = folderPath + "/savedata.json";
        Check(folderPath, filePath);

        // JSON読み込み
        var json = File.ReadAllText(filePath);
        var saveDataList = JsonUtility.FromJson<SaveDataList>(json);

        // 2番目(インデックス1)のスロットに上書き
        saveDataList.saves[slotIndex].StoryNumber = 99;
        saveDataList.saves[slotIndex].TestString = "上書きされました！";

        UnityEngine.Debug.Log(saveDataList.saves);

        // JSONとして再保存
        var newJson = JsonUtility.ToJson(saveDataList);
        File.WriteAllText(filePath, newJson);
        //*****************************************     
        /*   
        var json = JsonUtility.ToJson(data);

        // 書き込み
        UnityEngine.Debug.Log(Application.persistentDataPath);
        string folderPath = Application.persistentDataPath + "/savedata";
        string filePath = folderPath + "/savedata.json";
        Check(folderPath, filePath);

        bool isAppend = false;  // 上書き(false)か追記か
        using (var fs = new StreamWriter(filePath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            fs.Write(json);
        }*/
    }

    //  --------------------------------
    // ロード 返り値がSaveData
    //  --------------------------------
    public SaveData Load()
    {
        string folderPath = Application.persistentDataPath + "/savedata";
        string filePath = folderPath + "/savedata.json";
        Check(folderPath, filePath);

        // 一気に読み込む
        using(var sr=new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            string result = sr.ReadToEnd();
            UnityEngine.Debug.Log(result);
            return JsonUtility.FromJson<SaveData>(result);
        }
    }
}
