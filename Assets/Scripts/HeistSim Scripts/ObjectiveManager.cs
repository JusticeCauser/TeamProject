//using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;
    public enum ObjectiveType { restrictedLoadout, specificItem, timeLimit, heatBelow, undetected, amount }

    [Header("Objectives Settings")]
    [SerializeField] int objectivesPerGame = 2;

    [Header("Objectives")]
    public List<ObjectiveStats> objectivesActive = new List<ObjectiveStats>();
    public string objectivesText;
    public string objectivesCompleteText;

    int totalBonus;

    bool detected;
    bool restrictedLoadoutFail;

    float levelStartTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (objectivesActive.Count == 0)
                randomizeObjectives();
            
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelStartTime = Time.time;
    }
    public string objectives()
    {
        objectivesText = "OBJECTIVES:\n";

        foreach (var objective in objectivesActive)
        {
            objectivesText += "* " + objective.objectiveDescripton + "\n";
        }
        
        return objectivesText;
    }
    void randomizeObjectives() //assigning objectives to player 
    {
        
        List<ObjectiveStats> rObjectivePool = new List<ObjectiveStats>();
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.specificItem,
            objectiveDescripton = "Steal the *insert item here*",
            moneyBonus = 500
        });
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.heatBelow,
            objectiveDescripton = "Keep HEAT below 60%",
            heatLimit = 60f,
            moneyBonus = 75
        });
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.timeLimit,
            objectiveDescripton = "Completet heist within 5 minutes",
            timeLimit = 300f,
            moneyBonus = 300
        });
        //rObjectivePool.Add(new ObjectiveStats
        //{
        //    type = ObjectiveType.restrictedLoadout,
        //    objectiveDescripton = "Complete the heist using only 2 gadgets",
        //    moneyBonus = 75
        //});
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.amount,
            objectiveDescripton = "Steal at least $1500 of valuables",
            moneyBonus = 400
        });
        for (int i = 0; i < objectivesPerGame; i++)
        {
            int rand = Random.Range(0, rObjectivePool.Count);
            objectivesActive.Add(rObjectivePool[rand]);
            rObjectivePool.RemoveAt(rand);
        }
        
    }
    public int GetTotalMoneyBonus()
    {
        totalBonus = 0;

        foreach(var objective in objectivesActive)
        {
            if (objective.objectiveComplete)
                totalBonus += objective.moneyBonus;
        }
        return totalBonus;
    }
    public void playerDetected()
    {
        detected = true;
    }
 
    public void specificItemStolen(string item)
    {
        foreach(var objective in objectivesActive)
        {
            if(objective.type == ObjectiveType.specificItem && objective.itemName == item)
                objective.objectiveComplete = true;
        }
    }

    //gadget objective check needs to be added still
    public void checkObjectivesCompleted()
    {
       foreach(var objective in objectivesActive)
        {
            if (!objective.objectiveComplete)
                return;
        }
        GameManager.instance.missionComplete();
    }
    public void objectivesCompleted()
    {
        float totalTime = Time.time - levelStartTime;

        foreach(var objective in objectivesActive)
        {
            switch (objective.type)
            { 
                case ObjectiveType.timeLimit: //player escaped before timer runs out 
                    objective.objectiveComplete = totalTime <= objective.timeLimit; 
                    break;

                case ObjectiveType.heatBelow: //heat needs to stay below 60%
                    objective.objectiveComplete = HeatManager.Instance.maxHeatReached <= objective.heatLimit;
                    break;

                case ObjectiveType.undetected: //if player is not detected at all
                    objective.objectiveComplete = !detected;
                    break;

                //case ObjectiveType.restrictedLoadout: //complete only using certain items
                //    objective.objectiveComplete = !restrictedLoadoutFail;
                //    break;
                case ObjectiveType.amount:
                    objective.objectiveComplete = GameManager.instance.playerScript.totalValue >= 1500;
                    break;
            }

        }
        checkObjectivesCompleted();
    }
    public void showObjectivesCompleted()
    {
        objectivesCompleteText = "OBJECTIVES:\n";

       foreach( var objective in objectivesActive)
        {
            if (objective.objectiveComplete)
                objectivesCompleteText += "Completed - " + objective.objectiveDescripton + "+ $" + objective.moneyBonus + "\n";
            else
                objectivesCompleteText += "Failed - " + objective.objectiveDescripton + "($" + objective.moneyBonus + ")\n";
        }
        objectivesCompleteText += "\nTotal Bonus Received: $" + GetTotalMoneyBonus();
    }
    public void resetObjectives() //if clicking retry on fail reset and randomize objectives again
    {
        objectivesActive.Clear();
        detected = false;
        restrictedLoadoutFail = false;
        levelStartTime = Time.time;
        randomizeObjectives();
    }
  
}
