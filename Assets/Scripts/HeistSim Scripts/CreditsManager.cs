using UnityEngine;

public class CreditsManager : MonoBehaviour
{

    [Header("---Credits---")]
    [SerializeField] GameObject creditsUI;
    [SerializeField] ScrollManager scrollManager;

    public bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(creditsUI != null) 
            creditsUI.SetActive(false);

        isActive = false;
        
    }

    public void openCredits()
    {
        if(creditsUI != null)
        {
           creditsUI.SetActive(true);
            isActive = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            
        }
    }
    public void closeCredits()
    {
        if (creditsUI != null)
            creditsUI.SetActive(false);

        isActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(scrollManager  != null)
            scrollManager.resetCredits();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
