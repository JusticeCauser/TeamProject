using UnityEngine;

[System.Serializable]
public class ObjectiveStats
{
    public ObjectiveManager.ObjectiveType type;

    public string objectiveDescripton;
    public string itemName;
    public bool objectiveComplete;

    public float heatLimit;
    public float timeLimit;

    public int moneyBonus;
}
