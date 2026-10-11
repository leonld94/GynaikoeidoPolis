//using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 전체 시뮬레이션 자체를 표상하는 Class. 즉 Simulation 그 자체.
/// </summary>
public class Simulation
{
    private ResourceSystem _resourceSystem;

    public GameState GameState { get; private set; }
    public SimulationClock Clock { get; private set; }

    public List<BuildingState> BuildingStates { get; private set; }

    public List<BuildingState> ActiveBuildings { get; set; }
    public List<BuildingState> InactiveBuildings { get; set; }

    public event Action FailureOccurred;
    public event Action VictoryAchieved;
    public event Action<BuildingState> BuildingConstructionCompleted;

    private BuildingState _buildingIter;

    // 몸뚱아리 있는 것들. 필멸자, 불멸자, 가축 등
    //public List<Soma> Somata;
    // 건물들. 주거지, 상업지, 공장 등
    //public List<Oikodomema> Oikodomemata;
    // 맵 정보
    public Map Map { get; private set; }

    public Simulation(GameData gameData)
    {
        // Todo: 생성자 제대로 만들기. 지금은 임시로 채워만 놓음.
        // List<Soma> Somata = new List<Soma>();
        // List<Oikodomema> Oikodomemata = new List<Oikodomema>();

        if (gameData == null)
        {
            //Debug.Log("Prototype 감지 from Simulation Constructor");
            Clock = new SimulationClock();
            Clock.Reset();
            GameState = new GameState();
        }
        else
        {
            Map = new Map(gameData.mapData);
            Clock = new SimulationClock(gameData.simulationClockData);
        }
    }

    
    /// <summary>
    /// 일단은 Prototype에서만 쓰일 생성자.
    /// </summary>
    /// <param name="gameData"></param>
    /// <param name="initialBuildings">SimulationController를 통해 받아온 건물 상태 목록</param>
    public Simulation(GameData gameData, List<BuildingState> initialBuildings)
    {
        //Debug.Log("Prototype 감지 from Simulation Constructor");
        Clock = new SimulationClock();
        Clock.Reset();
        GameState = new GameState();
        BuildingStates = initialBuildings;

        // 켜진 것과 꺼진 BuildingState만 모으기

        ActiveBuildings = BuildingStates.FindAll(building => building.IsConstructed);
        InactiveBuildings = BuildingStates.FindAll(building => !building.IsConstructed);

        _resourceSystem = new ResourceSystem(GameState, ActiveBuildings, InactiveBuildings);

        int foodStgNum = 0;
        int gnkStgNum = 0;
        int mtrStgNum = 0;

        foreach(var building in ActiveBuildings)
        {
            if(building is DepotState depot)
            {
                switch (depot.StoreType)
                {
                    case StoreType.Food:
                        foodStgNum++;
                        break;
                    case StoreType.Gynaikoeideis:
                        gnkStgNum++;
                        break;
                    case StoreType.Materials:
                        mtrStgNum++;
                        break;
                    default:
                        break;
                }
            }
        }

        GameState.EditStorageValue(foodStgNum, gnkStgNum, mtrStgNum);


        // 맨 처음엔 창고 꽉 채워두기
        GameState.StoredFood = GameState.FoodStorageCapacity;
        GameState.StoredMaterials = GameState.MaterialsStorageCapacity;

        //foreach(BuildingState building in BuildingStates)
        //{
        //    Debug.Log(building.ToString());
        //}
    }

    private void CheckVictoryCondition()
    {
        if (Clock.PureDay >= GameState.RescueArrivalDay)
        {
            if(GameState.GameFlow == 0)
            {
                return;
            }

            // 승리 처리
            GameState.GameFlow = 0; // 게임 종료
            // 승리 UI 띄우기
            //UIManager.ShowVictoryUI();
            VictoryAchieved?.Invoke();

            // UI는 다른 곳에서 띄우니까 여기서는 GameState에 있는 승리 Flag를 세우는 형태여야할듯?
        }
    }

    private void CheckFailureCondition()
    {
        //Debug.Log("CheckFailureCondition Called");
        //Debug.Log($"Remain Humans are {GameState.LivingAnthropoiCount}");

        if (GameState.LivingAnthropoiCount < GameState.AnthropoiDeathLimit)
        {
            if (GameState.GameFlow == 0)
            {
                return;
            }

            // 패배 처리
            GameState.GameFlow = 0; // 게임 종료
            // Todo: 패배 UI 띄우기
            //UIManager.ShowFailureUI();
            FailureOccurred?.Invoke();
        }
    }

    // 위기값을 지정된 날짜에 맞게 변화시키는 Method. 
    private void CheckHazardLevel()
    {
        switch (GameState.ConsumeMultiplier)
        {
            case 1:
                if((int)GameState.HazardLevelDay.Level1 < Clock.PureDay)
                {
                    GameState.ConsumeMultiplier = 2;
                }
                break;
            case 2:
                if ((int)GameState.HazardLevelDay.Level2 < Clock.PureDay)
                {
                    GameState.ConsumeMultiplier = 3;
                }
                break;
            case 3:
                if ((int)GameState.HazardLevelDay.Level3 < Clock.PureDay)
                {
                    GameState.ConsumeMultiplier = 4;
                }
                break;
            case 4:
                if ((int)GameState.HazardLevelDay.Level4 < Clock.PureDay)
                {
                    GameState.ConsumeMultiplier = 5;
                }
                break;
            case 5:
                if ((int)GameState.HazardLevelDay.Level5 < Clock.PureDay)
                {
                    GameState.ConsumeMultiplier = 1;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 시뮬레이션 틱 진행. Clock은 시간값을 진행시킬 뿐이고 실제론 뭐든 여기서 연산.
    /// </summary>
    public TimeChange Tick()
    {

        //Debug.Log("Tick Flow");

        TimeChange timeChange = Clock.AdvanceTick();

        // 1. 건설
        // 건물순회
        for (int i = InactiveBuildings.Count - 1; i >= 0; i--)
        {
            _buildingIter = InactiveBuildings[i];

            // 건설값 증가
            _buildingIter.Construct();

            if (!_buildingIter.TryConstructionComplete())
            {
                continue;
            }

            // 완성이 되었다면

            // UI가 켜져있다면 Refresh
            // 근데 여기서 접근할 수 있는 방도가 없는데.

            // 건설 노동자 반환
            GameState.WorkingAnthropoiCount -= _buildingIter.ReturnAnthropoiBuilderNum();
            GameState.WorkingGynaikoeideisCount -= _buildingIter.ReturnGynaikoeideisBuilderNum();

            // 리스트 옮기기
            ActiveBuildings.Add(_buildingIter);
            InactiveBuildings.RemoveAt(i);

            BuildingConstructionCompleted?.Invoke(_buildingIter);


            // 만약 완성된 건물이 depot라면 미리 값 넣어두기: 이 친구는 기본값이 Food라서 이래도 됨
            if(_buildingIter is DepotState depot)
            {
                GameState.EditStorageValue(1, 0, 0);
            }
        }

        // 2 승리 계산
        if(timeChange.DayChanged)
        {
            CheckVictoryCondition();
            CheckHazardLevel();
        }
        // 3. 자원 계산(생산 => 소비 계산)
        // 4. 사망/분해 계산
        if (timeChange.HourChanged)
        {
            _resourceSystem.CalculateResourceProduction();
            _resourceSystem.CalculateResourceConsumption();
            bool isSomeoneDying = GameState.CalculateDies();
            bool isWorkerDismantled = GameState.CalculateDismantle();
            if (isSomeoneDying)
            {
                // GameStatus.CalculateDies()에서 목표인구수는 줄였는데 노동자수가 그대로라면
                if(GameState.LivingAnthropoiCount < GameState.WorkingAnthropoiCount)
                {
                    // 노동자수도 줄이자
                    _resourceSystem.CalculateDiesPersons();
                }

                CheckFailureCondition();
            }

            if (isWorkerDismantled)
            {
                _resourceSystem.CalculateDismantle();
            }
        }


        return timeChange;
    }
}
