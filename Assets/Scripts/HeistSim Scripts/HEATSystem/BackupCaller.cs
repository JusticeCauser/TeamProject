using UnityEngine;

public class BackupCaller : MonoBehaviour
{
    public float callTime = 10f;
    private float timer;
    private bool calling;

    // Update is called once per frame
    void Update()
    {
        if (!calling)
            return;

        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            HeatManager.Instance.AddHeat(30f);
            calling = false;
        }
    }

    public void StartCallingBackup()
    {
        calling = true;
        timer = callTime;
    }

    public void InterruptCall()
    {
        calling = false;
        timer = callTime;
    }
}
