
/// <summary>
/// 건물의 핵심 그 자체이자 건물의 상태를 담당하는 Class
/// </summary>
public class BuildingState
{

    public string SiteId { get; }
    public bool IsConstructed { get; protected set; }
    public float ConstructionProgress { get; protected set; } // 0.0f ~ 1.0f
    public float ConstructionGrowth { get; protected set; }
    public int RequiredMaterials { get; protected set; }
    public int AssignedMaterials { get; protected set; }
    public int AnthropoiBuilderCount { get; set; } // 현재 건설 노동자 수
    public int GynaikoeideisBuilderCount { get; set; } // 현재 건설 노동자 수
    public int BuilderLimit { get; }
    public string BuildingName { get; protected set; }


    public BuildingState(string siteId, bool isConstructed)
    {
        SiteId = siteId;
        IsConstructed = isConstructed;
        ConstructionProgress = isConstructed ? 1.0f : 0.0f;
        ConstructionGrowth = 1.0f / (10 * 24 * 25);      // 10명이서 하루 일하면 되는 값
        RequiredMaterials = 0;      // 자식 Class에서 재할당
        AssignedMaterials = 0;
        AnthropoiBuilderCount = 0;
        GynaikoeideisBuilderCount = 0;
        BuilderLimit = 15;
    }

    /// <summary>
    /// 건설 진행도를 업데이트하는 Method. BuilderCount에 따라 건설 진행도가 증가함.
    /// </summary>
    public void Construct()
    {

        //// 자원이 전부 할당되기 전까지는 증가금지.
        //if(AssignedMaterials < RequiredMaterials)
        //{
        //    return;
        //}
        ConstructionProgress += ConstructionGrowth 
            * (AnthropoiBuilderCount + GynaikoeideisBuilderCount);


        // 자원이 할당된 비율만큼까지만 건설도 증가
        // 100%일 때는 따로 두는게 안전할듯. float 나눗셈이라서
        // 어쩌면 else if 문 내부의 값이 절대로 1이 안 될수도 있으니까
        if (AssignedMaterials == RequiredMaterials)
        {

        }
        else if ((float)AssignedMaterials / (float)RequiredMaterials < ConstructionProgress)
        {
            // 준비가 안됐으면 강제로 낮추기.
            ConstructionProgress = (float)AssignedMaterials / (float)RequiredMaterials;
        }
    }

    /// <summary>
    /// 자재
    /// </summary>
    /// <param name="materialsNum">할당 시도한 자재량</param>
    /// <returns>반환된 자재량</returns>
    public int AssignMaterials(int materialsNum)
    {
        AssignedMaterials += materialsNum;

        int returnValue = 0;
        if (RequiredMaterials < AssignedMaterials)
        {
            returnValue = AssignedMaterials - RequiredMaterials;
            AssignedMaterials -= returnValue;
        }
        return returnValue;
    }

    public bool AssignAnthroposBuilder()
    {
        if(AnthropoiBuilderCount + GynaikoeideisBuilderCount < BuilderLimit)
        {
            AnthropoiBuilderCount++;
            return true;
        }

        return false;
    }

    public bool FireAnthroposBuilder()
    {
        if(0 < AnthropoiBuilderCount)
        {
            AnthropoiBuilderCount--;
            return true;
        }

        return false;
    }

    public bool AssignGynaikoeidesBuilder()
    {
        if (AnthropoiBuilderCount + GynaikoeideisBuilderCount < BuilderLimit)
        {
            GynaikoeideisBuilderCount++;
            return true;
        }

        return false;
    }

    public bool FireGynaikoeidesBuilder()
    {
        if (0 < GynaikoeideisBuilderCount)
        {
            GynaikoeideisBuilderCount--;
            return true;
        }

        return false;
    }

    public bool TryConstructionComplete()
    {
        if(ConstructionProgress >= 1.0f)
        {
            BuildingName = BuildingName.Split(" ")[0];
            IsConstructed = true;
            return true;
        }
        return false;
    }

    public int ReturnAnthropoiBuilderNum()
    {
        int returnValue = AnthropoiBuilderCount;
        AnthropoiBuilderCount = 0;
        return returnValue;
    }

    public int ReturnGynaikoeideisBuilderNum()
    {
        int returnValue = GynaikoeideisBuilderCount;
        GynaikoeideisBuilderCount = 0;
        return returnValue;
    }
}