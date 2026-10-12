using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    private GameData _gameData;
    private MetaData _metaData;

    [SerializeField]
    public GameObject loadPanel;

    [Header("UI")]
    [SerializeField] private GameObject ToolTipPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource ButtonClickSound;
    [SerializeField] private AudioSource CloseButtonClickSound;

    public void ButtonClicked()
    {
        ButtonClickSound.Play();
    }

    public void CloseButtonClicked()
    {
        CloseButtonClickSound.Play();
    }

    public void StartTutorialClicked()
    {
        //// 튜토리얼 시작 로직을 여기에 작성합니다.
        //Debug.Log("튜토리얼 시작!");

        //// Loading 화면 띄우기

        //// 1. RawTutorialGameData를 적재함(그곳에 저장만 안하면 덮어씌워지진 않으니까 문제 無)
        //string tutorialFilePath = Path.Combine(Application.streamingAssetsPath, "tutorial.json");

        //string jsonText = File.ReadAllText(tutorialFilePath);
        //_gameData = JsonUtility.FromJson<GameData>(jsonText);

        //// 2. Transfer에 Data 전달
        //bool taskSuccess = GameDataTransfer.SetGameData(_gameData);

        //if (!taskSuccess)
        //{
        //    Debug.Log("GameDataTransfer does not work rightly while transfer GameData.");
        //}
        //else {

        //    // 3. MetaData 생성
        //    MetaData _metaData = new MetaData();
        //    _metaData.displayName = "tutorial_" + DateTime.Now.ToString();
        //    _metaData.saveVersion = Application.version;

        //    // 4. MetaData 전달
        //    taskSuccess = GameDataTransfer.SetMetaData(_metaData);
        //    if (!taskSuccess)
        //    {
        //        Debug.Log("GameDataTransfer does not work rightly while transfer MetaData.");
        //    }
        //    // MetaData는 아마도 없어도 작동이 안되진 않으니까 그냥 실행.
        //    // Todo: 대신 정보 받아올 때 MetaData 있는지 확인하고 없으면 생성하는 메커니즘 추가해야함

        //    SceneManager.LoadScene("InGameScene");
        //}

        SceneManager.LoadScene("PrototypeGameplayScene");
    }

    public void NewGameButtonClicked()
    {
        //// 새로운 게임 시작 로직을 여기에 작성합니다.
        //Debug.Log("새로운 게임 시작!");

        //// Loading 화면 띄우기

        //// 1. Panel 띄워서 정보 이것저것 받은 후에
        //// 2. 그걸 절차적 생성 class로 넘겨 GameData를 만들고
        //GameDataTransfer.SetGameData(_gameData);   // 3. GameData를 전달
        ////SceneManager.LoadScene("InGameScene");  // 4. LoadScene으로 열기
        ///
        ToolTipPanel.SetActive(true);
    }

    public void LoadGameButtonClicked()
    {
        //Debug.Log("게임 불러오기!");

        //// 1. LoadPanel을 열기

        //loadPanel.SetActive(true);





        //// 여기부터는 LoadPanelScript에서 동작할 건데 일단 여기에 임시로 작성
        // 
        //info.SelectPanel();

        // 3. 선택한 SaveFile의 GameData를 읽어오고
        //_gameData = SaveManager.LoadGameData(info.SelectedSlotNum);
        //_metaData = SaveManager.LoadMetaData(info.SelectedSlotNum);

        // 4. Transfer로 전달
        //GameDataTransfer.SetGameData(_gameData);
        //GameDataTransfer.SetMetaData(_metaData);

        // 5. LoadScene으로 열기
        //SceneManager.LoadScene("InGameScene");   


        ToolTipPanel.SetActive(true);
    }

    public void ExitButtonClicked()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
