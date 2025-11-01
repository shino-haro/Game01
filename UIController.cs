using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class UIController : MonoBehaviour
{
    [SerializeField] SaveLoadManager saveLoadManager = null;
    [SerializeField] private CanvasGroup saveFileCanvasGroup;
    [SerializeField] private CanvasGroup gameViewCanvasGroup;

    [SerializeField] TextMeshProUGUI slot1_text = null;
    [SerializeField] TextMeshProUGUI slot2_text = null;
    [SerializeField] TextMeshProUGUI slot3_text = null;
    [SerializeField] TextMeshProUGUI slot4_text = null;

    public static int save_loadNum;
    public static string kakikomi;
    public static string yomikomi;

    
    
    //  --------------------------------
    // セーブファイル画面を引きずり出す
    //  --------------------------------
    public void OnSaveFileViewButtonClicked()
    {
        save_loadNum = 1;
        kakikomi="書き込んだ！";
        UnityEngine.Debug.Log("Saveボタンが押されたみたいだね。save_loadNumは"+save_loadNum);

        saveFileCanvasGroup.alpha = 1f;              // 表示
        saveFileCanvasGroup.blocksRaycasts = true;   // クリックを受け取るように
        gameViewCanvasGroup.blocksRaycasts = false;
    }
    //  --------------------------------
    // Loadボタンを押したとき
    //  --------------------------------
    public void OnLoadButtonClicked()
    {
        yomikomi = "読み込んだ！";
        UnityEngine.Debug.Log("Loadボタンが押されたみたいだね。");
        save_loadNum = 2;
        saveFileCanvasGroup.alpha = 1f;              // 表示
        saveFileCanvasGroup.blocksRaycasts = true;   // クリックを受け取るように
        gameViewCanvasGroup.blocksRaycasts = false;
    }
    //  --------------------------------
    // Skipボタンを押したとき
    //  --------------------------------
    public void OnSkipButtonClicked()
    {
        UnityEngine.Debug.Log("Skipボタンが押されたみたいだね。");
    }
    //  --------------------------------
    // Autoボタンを押したとき
    //  --------------------------------
    public void OnAutoButtonClicked()
    {
        UnityEngine.Debug.Log("Autoボタンが押されたみたいだね。");
    }

    //  --------------------------------
    // Backボタンを押したとき
    //  --------------------------------
    public void OnBackButtonClicked()
    {
        UnityEngine.Debug.Log("Backボタンが押されたみたいだね。");
        saveFileCanvasGroup.alpha = 0f;              // 表示
        saveFileCanvasGroup.blocksRaycasts = false;   // クリックを受け取るように
        gameViewCanvasGroup.blocksRaycasts = true;
    }

    //  --------------------------------
    // セーブスロットボタンず
    //  --------------------------------
    public void OnSlot1ButtonClicked()
    {
        if(save_loadNum==1)
        {
            // セーブのとき
            saveLoadManager.Save(0);
            slot1_text.text=kakikomi;
        }
        else
        {
            UnityEngine.Debug.Log("貴様……セーブじゃないな……？");
            slot1_text.text=yomikomi;
        }
        
    }
    public void OnSlot2ButtonClicked()
    {
        if(save_loadNum==1)
        {
            // セーブのとき
            saveLoadManager.Save(1);
            slot2_text.text=kakikomi;
        }
        else
        {
            UnityEngine.Debug.Log("貴様……セーブじゃないな……？");
            slot2_text.text=yomikomi;
        }
        
    }
    public void OnSlot3ButtonClicked()
    {
        if(save_loadNum==1)
        {
            // セーブのとき
            saveLoadManager.Save(2);
            slot3_text.text=kakikomi;
        }
        else
        {
            UnityEngine.Debug.Log("貴様……セーブじゃないな……？");
            slot3_text.text=yomikomi;
        }
        
    }
    public void OnSlot4ButtonClicked()
    {
        if(save_loadNum==1)
        {
            // セーブのとき
            saveLoadManager.Save(3);
            slot4_text.text=kakikomi;
        }
        else
        {
            UnityEngine.Debug.Log("貴様……セーブじゃないな……？");
            slot4_text.text=yomikomi;
        }
        
    }
}
