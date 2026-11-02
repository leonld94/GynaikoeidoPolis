using System;

public class DepotState : BuildingState
{
    private StoreType _storeType;
    public StoreType StoreType { get => _storeType; private set => _storeType = value; }

    public DepotState(string siteId, bool isConstructed,
        StoreType storeType = StoreType.Food) 
        : base(siteId, isConstructed)
    {
        if (!IsConstructed)
        {
            BuildingName = "창고 부지";
        }
        else
        {
            BuildingName = "창고";
        }
        RequiredMaterials = 1000;
        StoreType = storeType; // 기본이 Food 
    }

    /// <summary>
    /// 저장요소를 변경하는 Method
    /// </summary>
    /// <param name="newStoreType">변경할 StorageType</param>
    /// <returns>이전에 저장되어있던 StorageType을 반환</returns>
    public StoreType ChangeStoreType(StoreType newStoreType)
    {
        if (StoreType != newStoreType)
        {
            StoreType prev;
            prev = StoreType;
            StoreType = newStoreType;
            return prev;
        }
        return StoreType;
    }
}

public enum StoreType
{
    Food,
    Materials,
    Gynaikoeideis
}