using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using System.ComponentModel;

public class GameView : ViewBase
{
    // トークウィンドウ
    [SerializeField] TalkWindow talkWindow = null;
    // スプレッドシートリーダー
    [SerializeField] SpreadSheetReader spreadSheetReader = null;


    [SerializeField] SaveLoadManager saveLoadManager = null;

    [SerializeField] Halloween halloween = null;



    void Start()
    {/*
        var data = Load();
        data.TestString = System.DateTime.Now.ToString();

        Save(data);*/
    }

    

    // -------------------------------------------------------
    // ビューオープン時コール.
    // -------------------------------------------------------
    public override async void OnViewOpened()
    {
        base.OnViewOpened();
        // セーブデータ読み込み
        var savedata = saveLoadManager.Load();
        try
        {
            // 会話パラメータのTalksを取得してdataに格納、awaitにて背景を設定
            // var data = talkWindow.Talks;

            string _sheetId = "1Xk0gHLsZtm0dRrwRiUKkBWeAnuHFr4d1cqs0q9_Yguw";
            string _sheetName2_0 = "TalkData002_0";
            string _sheetName2_1 = "TalkData002_1";
            string _sheetName2_2 = "TalkData002_2";

            if (savedata.StoryNumber == 0)
            {
                string _sheetName = "TalkData001";
                var data = await spreadSheetReader.LoadSpreadSheet(_sheetId, _sheetName);


                // 引数：data[0].Place -> 会話の最初に設定されてる背景データ、トランジションの有無（trueの時はトランジションしない）
                await talkWindow.SetBg(data[0].Place, true);

                await talkWindow.Open();
                var response = await talkWindow.TalkStart(data);
                await talkWindow.Close();

                // 結果をセーブ
                //savedata.StoryNumber = 1;
                //savedata.TestString = response[0].ToString();
                //saveLoadManager.Save(savedata);

                // ******************************************************
                switch (response[0])
                {
                    case 0:
                        {
                            var data2_0 = await spreadSheetReader.LoadSpreadSheet(_sheetId, _sheetName2_0);
                            await talkWindow.SetBg(data2_0[0].Place, true);
                            await talkWindow.Open();
                            var response2 = await talkWindow.TalkStart(data2_0);
                        }
                        break;
                    case 1:
                        {
                            var data2_1 = await spreadSheetReader.LoadSpreadSheet(_sheetId, _sheetName2_1);
                            await talkWindow.SetBg(data2_1[0].Place, true);
                            await talkWindow.Open();
                            var response2 = await talkWindow.TalkStart(data2_1);
                        }
                        break;
                    case 2:
                        {
                            var data2_2 = await spreadSheetReader.LoadSpreadSheet(_sheetId, _sheetName2_2);
                            await talkWindow.SetBg(data2_2[0].Place, true);
                            await talkWindow.Open();
                            var response2 = await talkWindow.TalkStart(data2_2);
                        }
                        break;
                }
                await talkWindow.Close();
                // ******************************************************
            }
            else if(savedata.StoryNumber==1)
            {
                switch( savedata.TestString )
                {
                    case "0":
                        {
                            var data2_0 = await spreadSheetReader.LoadSpreadSheet( _sheetId , _sheetName2_0 );
                            await talkWindow.SetBg( data2_0[ 0 ].Place , true );
                            await talkWindow.Open();
                            var response2 = await talkWindow.TalkStart( data2_0 );
                        }
                        break;
                    case "1":
                        {
                            var data2_1 = await spreadSheetReader.LoadSpreadSheet( _sheetId , _sheetName2_1 );
                            await talkWindow.SetBg( data2_1[ 0 ].Place , true );
                            await talkWindow.Open();
                            var response2 = await talkWindow.TalkStart( data2_1 );
                        }
                        break;
                    case "2":
                        {
                            var data2_2 = await spreadSheetReader.LoadSpreadSheet( _sheetId , _sheetName2_2 );
                            await talkWindow.SetBg( data2_2[ 0 ].Place , true );
                            await talkWindow.Open();
                            var response2 = await talkWindow.TalkStart( data2_2 );
                        }
                        break;
                }
                await talkWindow.Close();
            }
            else
            {
                Debug.Log( "エラー番号" + savedata.StoryNumber );
            }
        
        }
        catch(System.OperationCanceledException e)
        {
            UnityEngine.Debug.Log("テスト会話がキャンセルされました"+e);
        }
    }

    // -------------------------------------------------------
    // ビュークローズ時コール.
    // -------------------------------------------------------
    public override void OnViewClosed()
    {
        base.OnViewClosed();
    }

    // -------------------------------------------------------
    // ホームに戻る
    // -------------------------------------------------------
    public void OnBackToHomeButtonClicked()
    {
        Scene.ChangeScene("01_Home").Forget();
    }

    
}