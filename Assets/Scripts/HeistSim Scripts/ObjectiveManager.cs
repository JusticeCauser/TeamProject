//using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;
    public enum ObjectiveType { restrictedLoadout, specificItem, timeLimit, heatBelow, undetected }
    

    [Header("Objectives Settings")]
    [SerializeField] int objectivesPerGame = 2;

   
    public List<ObjectiveStats> objectivesActive = new List<ObjectiveStats>();

    bool detected;
    bool restrictedLoadoutFail;

    float levelStartTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelStartTime = Time.time;
        randomizeObjectives();
    }

    void randomizeObjectives() //assigning objectives to player 
    {
        List<ObjectiveStats> rObjectivePool = new List<ObjectiveStats>();
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.specificItem,
            objectiveDescripton = "Steal the *insert item here*",
        });
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.heatBelow,
            objectiveDescripton = "Keep HEAT below 60%",
            heatLimit = 60f
        });
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.timeLimit,
            objectiveDescripton = "Completet heist within 5 minutes",
            timeLimit = 300f
        });
        rObjectivePool.Add(new ObjectiveStats
        {
            type = ObjectiveType.restrictedLoadout,
            objectiveDescripton = "Complete the heist using only 2 gadgets"
        });

        for (int i = 0; i < objectivesPerGame; i++)
        {
            int rand = Random.Range(0, rObjectivePool.Count);
            objectivesActive.Add(rObjectivePool[rand]);
            rObjectivePool.RemoveAt(rand);
        }
    }
    public void playerDetected()
    {
        detected = true;
    }
 
    public void specifimItemStolen(string item)
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

                case ObjectiveType.restrictedLoadout: //complete only using certain items
                    objective.objectiveComplete = !restrictedLoadoutFail;
                    break;
            }

        }
        checkObjectivesCompleted();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
