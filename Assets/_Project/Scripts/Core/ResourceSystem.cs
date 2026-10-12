using System;
using System.Collections.Generic;
using System.Linq;

public class ResourceSystem
{
    public GameState GameState { get; private set; }
    public List<BuildingState> ActiveBuildings { get; private set; }
    public List<BuildingState> InactiveBuildings { get; private set; }

    private bool _flag;
    
    public ResourceSystem(GameState gameState, List<BuildingState> ActiveB, List<BuildingState> InactB)
    {
        GameState = gameState;
        ActiveBuildings = ActiveB;
        InactiveBuildings = InactB;
    }

    public void CalculateResourceProduction()
    {

        foreach (BuildingState building in ActiveBuildings)
        {
            if (building is FoodVaultState FoodBuilding)
            {
                int production = FoodBuilding.Work(GameState.StoredMaterials <= 0);

                if (GameState.FoodStorageCapacity < GameState.StoredFood + production)
                {
                    production = GameState.FoodStorageCapacity - GameState.StoredFood;
                }
                GameState.StoredFood += production;
            }
            else if (building is SupplyMineState MaterialBuilding)
            {
                int production = MaterialBuilding.Work(GameState.StoredMaterials <= 0);

                if (GameState.MaterialsStorageCapacity < GameState.StoredMaterials + production)
                {
                    production = GameState.MaterialsStorageCapacity - GameState.StoredMaterials;
                }
                GameState.StoredMaterials += production;
            }
        }
    }

    public void CalculateResourceConsumption()
    {
        // 식량 소모 공식: 인간 수 * 시간당 소모량 * 위기값
        // 자재 소모 공식: (인간 수 * 인간 시간당 소모량 + 인형 수 * 인형 시간당 소모량) * 위기값
                        // + 노동자 수 * 노동 시간당 소모량

        // 기본적인 소모량
        GameState.FoodConsumedPerHour = 
            GameState.LivingAnthropoiCount * GameState.FoodConsumedPerAnthroposPerHour;
        GameState.MaterialsConsumedPerHour = 
            (GameState.LivingAnthropoiCount * GameState.MaterialsConsumedPerAnthroposPerHour) 
            + (GameState.ActiveGynaikoeideisCount * GameState.MaterialsConsumedPerGynaikoeidesPerHour);
        GameState.RequiredWorkingMaterials = (GameState.WorkingAnthropoiCount + GameState.WorkingGynaikoeideisCount)
            * GameState.MaterialsConsumedPerLabourer;


        // 위기값 적용 소모량
        GameState.FoodConsumedPerHourWithMultiplier = 
            GameState.FoodConsumedPerHour * GameState.ConsumeMultiplier;
        GameState.MaterialsConsumedPerHourWithMultiplier =
            GameState.MaterialsConsumedPerHour * GameState.ConsumeMultiplier
            + GameState.RequiredWorkingMaterials;

        // 실제 소비량 적용
        GameState.StoredFood -= GameState.FoodConsumedPerHourWithMultiplier;
        GameState.StoredMaterials -= GameState.MaterialsConsumedPerHourWithMultiplier;
    }


    public void CalculateDiesPersons()
    {
        IEnumerator<BuildingState> enumerator = InactiveBuildings.GetEnumerator();
        bool hasCurrent = enumerator.MoveNext();

        // 0. 건설 노동자부터 전부 빼기
        while (GameState.StoredFood < GameState.FoodThreshold)
        {

            if (!hasCurrent)
            {
                break;
            }

            // 더 비울거 없으면 다음 건물로 이동
            if (!enumerator.Current.FireAnthroposBuilder())
            {
                hasCurrent = enumerator.MoveNext();
            }
        }

        enumerator = ActiveBuildings.GetEnumerator();
        hasCurrent = enumerator.MoveNext();

        // 1. 앞부터 순회하며 인간 노동자 전부 빼기
        // 2. 수가 부족하면 그냥 종료하기
        // 어차피 인간이 다 죽으면 겜오버임
        while (GameState.StoredFood < GameState.FoodThreshold)
        {
            if(!hasCurrent)
            {
                break;
            }

            if(enumerator.Current is DepotState)
            {
                hasCurrent = enumerator.MoveNext();
            }
            else
            {
                if(enumerator.Current is FoodVaultState foodBuilding)
                {
                    if(!foodBuilding.FireAnthroposWorker())
                    {
                        hasCurrent = enumerator.MoveNext();
                    }
                }
                else if(enumerator.Current is SupplyMineState materialsBuilding)
                {
                    if (!materialsBuilding.FireAnthroposWorker())
                    {
                        hasCurrent = enumerator.MoveNext();
                    }
                }
                else
                {
                    hasCurrent = enumerator.MoveNext();
                }
            }
        }
    }

    public void CalculateDismantle()
    {
        IEnumerator<BuildingState> enumerator = InactiveBuildings.GetEnumerator();
        bool hasCurrent = enumerator.MoveNext();

        // 0. 건설 노동자부터 전부 빼기
        while (GameState.StoredMaterials < GameState.MaterialsThreshold)
        {

            if (!hasCurrent)
            {
                break;
            }

            if (enumerator.Current.FireGynaikoeidesBuilder())
            {
                GameState.WorkingGynaikoeideisCount--;
                GameState.ActiveGynaikoeideisCount--;
                GameState.StoredMaterials += GameState.GynaikoeidesToMaterials;
            }
            else 
            {
                 hasCurrent = enumerator.MoveNext();
            }
        }

        enumerator = ActiveBuildings.GetEnumerator();
        hasCurrent = enumerator.MoveNext();

        // 1. 앞부터 순회하며 인형 노동자 전부 빼기
        // 2. 수가 부족하면 그냥 종료하기.
        while (GameState.StoredMaterials < GameState.MaterialsThreshold)
        {

            if (!hasCurrent)
            {
                break;
            }

            if (enumerator.Current is DepotState)
            {
                hasCurrent = enumerator.MoveNext();
            }
            else
            {
                if (enumerator.Current is FoodVaultState foodBuilding)
                {
                    if (foodBuilding.FireGynaikoeidesWorker())
                    {
                        GameState.WorkingGynaikoeideisCount--;
                        GameState.ActiveGynaikoeideisCount--;
                        GameState.StoredMaterials += GameState.GynaikoeidesToMaterials;
                    }
                    else
                    {
                        hasCurrent = enumerator.MoveNext();
                    }
                }
                else if (enumerator.Current is SupplyMineState materialsBuilding)
                {
                    if (materialsBuilding.FireGynaikoeidesWorker())
                    {
                        GameState.WorkingGynaikoeideisCount--;
                        GameState.ActiveGynaikoeideisCount--;
                        GameState.StoredMaterials += GameState.GynaikoeidesToMaterials;
                    }
                    else
                    {
                        hasCurrent = enumerator.MoveNext();
                    }
                }
                else
                {
                    hasCurrent = enumerator.MoveNext();
                }
            }
        }

        // 다 돌았는데도 부족하면 그냥 0으로 만들기.
        if(GameState.StoredMaterials < 0)
        {
            GameState.StoredMaterials = 0;
        }
    }
}
