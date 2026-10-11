using UnityEngine;

public class SupplyMineState : BuildingState, IWorkable
{
    public const int WorkerLimit = 15;

    // 현재 노동자 수
    public int AnthroposWorkerCount { get; protected set; }
    public int GynaikoeidesWorkerCount { get; protected set; }

    public const int ResourceProduction = 15;
    public bool lastIsHalf;

    public SupplyMineState(string siteId, bool isConstructed) 
        : base(siteId, isConstructed)
    {
        AnthroposWorkerCount = 0;
        GynaikoeidesWorkerCount = 0;
        RequiredMaterials = 600;
    }

    public int Work(bool isHalf)
    {
        lastIsHalf = isHalf;
        return (AnthroposWorkerCount + GynaikoeidesWorkerCount) * SupplyMineState.ResourceProduction
            / (isHalf ? 2 : 1);
    }

    public bool AssignAnthroposWorker()
    {
        if (AnthroposWorkerCount + GynaikoeidesWorkerCount < WorkerLimit)
        {
            AnthroposWorkerCount++;
            return true;
        }
        return false;
    }

    public bool FireAnthroposWorker()
    {
        if (AnthroposWorkerCount > 0)
        {
            AnthroposWorkerCount--;
            return true;
        }
        return false;
    }

    public bool AssignGynaikoeidesWorker()
    {
        if (AnthroposWorkerCount + GynaikoeidesWorkerCount < WorkerLimit)
        {
            GynaikoeidesWorkerCount++;
            return true;
        }
        return false;
    }

    public bool FireGynaikoeidesWorker()
    {
        if (GynaikoeidesWorkerCount > 0)
        {
            GynaikoeidesWorkerCount--;
            return true;
        }
        return false;
    }
}
