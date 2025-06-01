using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questName;
    public string questDescription;
    public QuestCondition condition;
    public QuestStatus status;

    public string requiredFishName;
    public string requiredFishSize;
    public int requiredFishCount;

    public Quest(string name, string description, QuestCondition condition)
    {
        questName = name;
        questDescription = description;
        this.condition = condition;
        status = QuestStatus.Active;
    }
    public bool CheckIfQuestCompleted(Inventory inventory)
    {
        Item requiredItem = new Item(requiredFishName, "риба", requiredFishSize, requiredFishCount);
        return inventory.HasItem(requiredItem);
    }
}

public enum QuestStatus
{
    Active,
    Completed,
    Finished
}

public abstract class QuestCondition
{
    public abstract bool IsCompleted();
}

public class CatchAnyFishCondition : QuestCondition
{
    public int requiredCount;
    private int currentCount;

    public CatchAnyFishCondition(int count)
    {
        requiredCount = count;
        currentCount = 0;
    }

    public void IncrementCount()
    {
        currentCount++;
    }

    public override bool IsCompleted()
    {
        return currentCount >= requiredCount;
    }
}

public class CatchSpecificFishCondition : QuestCondition
{
    public string fishType;
    public int requiredCount;
    private int currentCount;

    public CatchSpecificFishCondition(string type, int count)
    {
        fishType = type;
        requiredCount = count;
        currentCount = 0;
    }

    public void IncrementCount(string caughtFishType)
    {
        if (caughtFishType == fishType) currentCount++;
    }

    public override bool IsCompleted()
    {
        return currentCount >= requiredCount;
    }
}

public class CatchMediumFishCondition : QuestCondition
{
    public int requiredCount;
    private int currentCount;

    public CatchMediumFishCondition(int count)
    {
        requiredCount = count;
        currentCount = 0;
    }

    public void IncrementCount(string fishSize)
    {
        if (fishSize == "середній") currentCount++;
    }

    public override bool IsCompleted()
    {
        return currentCount >= requiredCount;
    }
}