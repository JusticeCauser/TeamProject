using UnityEngine;

public class LootManager : MonoBehaviour
{

    public static LootManager instance;

    public int totalCash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addCash(int amount)
    {
        totalCash += amount;
        Save();
    }

    public void removeCash(int amount)
    { 
        totalCash = Mathf.Max(0, totalCash - amount);
        Save();
    }

    public void updateCash(int amount)
    {
        
    }

    void Save()
    {
        PlayerPrefs.SetInt("TotalCash", totalCash);
        PlayerPrefs.Save();
    }

    void Load()
    {
        totalCash = PlayerPrefs.GetInt("TotalCash", 0);
    }
}
