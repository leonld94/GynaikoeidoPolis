using System;
using Unity.VisualScripting;
using UnityEngine;
using static SimulationController;

public class ScreenUIManager : MonoBehaviour
{
    public SimulationClock Clock { get; set; }
    public GameState GameState { get; set; }


    [Header("UIPanel")]
    [SerializeField] private TopUIBarScript TopUIBar;
    [SerializeField] private LeftDownUIPanelScript LeftDownUIPanel;
    [SerializeField] private RightDownUIPanelScript RightDownUIPanel;
    [SerializeField] private EndingPanel EndingPanel;
    [SerializeField] private BuildingInfoUIPanelScript BuildingInfoUIPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource ButtonClickSound;
    [SerializeField] private AudioSource CloseButtonClickSound;
    [SerializeField] private AudioSource TimeChangedSound;
    [SerializeField] private AudioSource ConstructionCompleteSound;

    ////TopUIBar			                    Hour(식량, 자재, 인형)    Button(인형)
    ////LeftDownUIPanel	    Day(추위강도)		Hour(사망/분해)           Button(인형)
    ////RightDownUIPanel	Day(D-Day)			Hour(시간)
    ////EndingPanel								                                        별도호출
    ////LorePanel														                별도호출

    // 임시 사용용 변수: GC 생기지 마라고 깡으로 할당
    private bool _flag;
    private int _num;

    public void Initialize(SimulationClock clock, GameState gameState, 
        bool isPaused, TimeSpeed gameSpeed)
    {
        Clock = clock;
        GameState = gameState;
        TopUIBar.Initialize(gameState);
        LeftDownUIPanel.Initialize(gameState);
        RightDownUIPanel.Initialize(gameState, clock, isPaused, gameSpeed);
        BuildingInfoUIPanel.Initialize();
    }

    // 1. 날짜
    // 필요정보: 날짜; SimulationClock
    public void DayUIRefresh()
    {
        RightDownUIPanel.RescueDayUIRefresh(GameState.RescueArrivalDay - Clock.PureDay);
        LeftDownUIPanel.HazardLevelUIRefresh();
    }


    // 1. 자원 실제 생산/소비
    // 2. 인구 사망/분해
    // 필요정보: 자원값; GameState, 인구값; GameState
    public void HourUIRefresh()
    {
        TopUIBar.HourUIRefresh();
        LeftDownUIPanel.PopUIRefresh();
        RightDownUIPanel.TimeUIRefresh(Clock.PureDay, Clock.Hour);
        BuildingInfoUIPanel.ConstructionPrgressUIRefresh();
    }

    public void ActiveGynaikoeideisPlusButtonClicked()
    {
        // 저장된 인형이 있다면
        if (0 < GameState.InactiveGynaikoeideisCount)
        {
            //Debug.Log("Plus Done");

            // 비활성화 감소
            // 활성화 증가
            GameState.InactiveGynaikoeideisCount--;
            GameState.ActiveGynaikoeideisCount++;

            LeftDownUIPanel.PopUIRefresh();
            TopUIBar.GynaikoeideisUIRefresh();
        }
    }

    public void ActiveGynaikoeideisMinusButtonClicked()
    {
        // 수용량에 여유가 있다면
        if (GameState.InactiveGynaikoeideisCount < GameState.InactiveGynaikoeideisStorageCapacity)
        {
            // 일하지 않는 인형이 있다면
            if (GameState.WorkingGynaikoeideisCount < GameState.ActiveGynaikoeideisCount)
            {
                //Debug.Log("Minus Done");

                // 비활성화 증가
                // 활성화 감소
                GameState.InactiveGynaikoeideisCount++;
                GameState.ActiveGynaikoeideisCount--;

                LeftDownUIPanel.PopUIRefresh();
                TopUIBar.GynaikoeideisUIRefresh();
            }
        }
    }

    public void OnBuildingConstructionCompleted(BuildingState building)
    {
        BuildingInfoUIPanel.RefreshIfSelected(building);
        LeftDownUIPanel.PopUIRefresh();
        ConstructionCompleteSound.Play();
    }

    // 주기: 버튼이 눌릴 때 => 즉 호출 경우가 버튼 눌릴 때인거잖아?
    // 1. 노동자 변화
    // 2. 자재 예측 요구량/생산량
    // 필요정보: 노동자값; GameState, 자원값; GameState
    public void ResourceUIRefresh()
    {
        LeftDownUIPanel.PopUIRefresh();
        TopUIBar.HourUIRefresh();
    }

    public void ShowVictoryUI()
    {
        // 1. GameState 정보를 가지고 패널 채우기
        int anthropoiNum = GameState.LivingAnthropoiCount;
        int gynaikoeideisNum = GameState.ActiveGynaikoeideisCount + GameState.InactiveGynaikoeideisCount;

        // 2. 패널 띄우기
        EndingPanel.ShowVictoryPanel(anthropoiNum, gynaikoeideisNum);
    }

    public void ShowFailureUI()
    {
        // 1. GameState 정보를 가지고 패널 채우기
        int livedDay = Clock.Day + Clock.Year * 15;
        int DDay = GameState.RescueArrivalDay - livedDay;

        // 2. 패널 띄우기
        EndingPanel.ShowFailurePanel(livedDay, DDay);
    }

    public void TimeSwitchChanged(bool isPaused, TimeSpeed gameSpeed)
    {
        //Debug.Log("ScreenUIManager.TimeSwitchCahged() Called");
        //TimeChangedSound.Play();
        RightDownUIPanel.TimeSwitchChanged(isPaused, gameSpeed);
    }

    /// <summary>
    /// 빌더를 건물에서 풀어줌
    /// </summary>
    public void BuilderGnkMinusButtonClicked()
    {
        // 해고를 시도해보고
        _flag = BuildingInfoUIPanel.FireGynaikoeidesBuilder();

        // 성공한다면
        if (_flag)
        {
            // 갱신
            GameState.WorkingGynaikoeideisCount--;
            ResourceUIRefresh();
        }
    }

    /// <summary>
    /// 빌더를 건물에서 풀어줌
    /// </summary>
    public void BuilderAnthrpMinusButtonClicked()
    {
        // 해고를 시도해보고
        _flag = BuildingInfoUIPanel.FireAnthroposBuilder();

        // 성공한다면
        if (_flag)
        {
            // 갱신
            GameState.WorkingAnthropoiCount--;
            ResourceUIRefresh();
        }
    }

    /// <summary>
    ///  빌더를 건물에 할당함
    /// </summary>
    public void BuilderGnkPlusButtonClicked()
    {
        // 충분한 수의 백수가 있다면
        if (GameState.WorkingGynaikoeideisCount < GameState.ActiveGynaikoeideisCount)
        {
            // 시도를 해보고
            _flag = BuildingInfoUIPanel.AssignGynaikoeidesBuilder();
            // 성공하면
            if (_flag)
            {
                GameState.WorkingGynaikoeideisCount++;
                ResourceUIRefresh();
            }
        }
    }

    public void BuilderAnthrpPlusButtonClicked()
    {
        // 충분한 수의 백수가 있다면
        if (GameState.WorkingAnthropoiCount < GameState.LivingAnthropoiCount)
        {
            // 시도를 해보고
            _flag = BuildingInfoUIPanel.AssignAnthroposBuilder();
            // 성공하면
            if (_flag)
            {
                GameState.WorkingAnthropoiCount++;
                ResourceUIRefresh();
            }
        }
    }

    public void BuildAssignMaterialsButtonClicked()
    {
        // 여유가 있다면
        if(100 <= GameState.StoredMaterials)
        {
            _num = 100;
            GameState.StoredMaterials -= _num;
            // 일정수만큼 넣어보고
            _num = BuildingInfoUIPanel.AssignMaterials(_num);
            // 반환값 다시 넣기
            GameState.StoredMaterials += _num;
            ResourceUIRefresh();
        }
    }

    public void SelectFoodButtonClicked()
    {
        StoreType storeType = BuildingInfoUIPanel.SelectFood();

        switch (storeType)
        {
            case StoreType.Gynaikoeideis:
                GameState.EditStorageValue(1, -1, 0);
                break;
            case StoreType.Materials:
                GameState.EditStorageValue(1, 0, -1);
                break;
            default:
                break;
        }

        TopUIBar.HourUIRefresh();
    }

    public void SelectGynaikoeideisButtonClicked()
    {
        StoreType storeType = BuildingInfoUIPanel.SelectGynaikoeideis();

        switch (storeType)
        {
            case StoreType.Food:
                GameState.EditStorageValue(-1, 1, 0);
                break;
            case StoreType.Materials:
                GameState.EditStorageValue(0, 1, -1);
                break;
            default:
                break;
        }

        TopUIBar.HourUIRefresh();
    }

    public void SelectMaterialsButtonClicked()
    {
        StoreType storeType = BuildingInfoUIPanel.SelectMaterials();

        switch (storeType)
        {
            case StoreType.Food:
                GameState.EditStorageValue(-1, 0, 1);
                break;
            case StoreType.Gynaikoeideis:
                GameState.EditStorageValue(0, -1, 1);
                break;
            default:
                break;
        }

        TopUIBar.HourUIRefresh();
    }

    public void WorkerGnkMinusButtonClicked()
    {
        // 해고를 시도해보고
        _flag = BuildingInfoUIPanel.FireGynaikoeidesWorker();

        // 성공한다면
        if (_flag)
        {
            // 갱신
            GameState.WorkingGynaikoeideisCount--;
            ResourceUIRefresh();
        }
    }

    public void WorkerAnthrpMinusButtonClicked()
    {
        // 해고를 시도해보고
        _flag = BuildingInfoUIPanel.FireAnthroposWorker();

        // 성공한다면
        if (_flag)
        {
            // 갱신
            GameState.WorkingAnthropoiCount--;
            ResourceUIRefresh();
        }
    }

    public void WorkerGnkPlusButtonClicked()
    {
        // 충분한 수의 백수가 있다면
        if (GameState.WorkingGynaikoeideisCount < GameState.ActiveGynaikoeideisCount)
        {
            // 시도를 해보고
            _flag = BuildingInfoUIPanel.AssignGynaikoeidesWorker();
            // 성공하면
            if (_flag)
            {
                GameState.WorkingGynaikoeideisCount++;
                ResourceUIRefresh();
            }
        }
    }
    public void WorkerAnthrpPlusButtonClicked()
    {
        // 충분한 수의 백수가 있다면
        if (GameState.WorkingAnthropoiCount < GameState.LivingAnthropoiCount)
        {
            // 시도를 해보고
            _flag = BuildingInfoUIPanel.AssignAnthroposWorker();
            // 성공하면
            if (_flag)
            {
                GameState.WorkingAnthropoiCount++;
                ResourceUIRefresh();
            }
        }
    }

    public void PlayButtonClickSound()
    {
        ButtonClickSound.Play();
    }

    public void PlayCloseButtonClickSound()
    {
        CloseButtonClickSound.Play();
    }
}
