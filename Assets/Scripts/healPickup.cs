using UnityEngine;
using System.Collections;

public class healPickup : MonoBehaviour
{
    [SerializeField] int healAmount;
    [SerializeField] float healSpeed; //for implementing hot

    private void OnTriggerEnter(Collider other)
    {
        IHeal _heal = other.GetComponent<IHeal>();

        if(_heal != null)
        {
             _heal.heal(healAmount);
            StartCoroutine(healDuration());
          //healDuration();
            Destroy(gameObject);
        }
    }
    IEnumerator healDuration() //hot mechanic
    { //small healthpack heal 1 sec interval
        //medium healthpack health 2 sec interval
        //large healthpack heal 3 sec interval 
        float duration = 0f;
        duration = healAmount / healSpeed;
        yield return new WaitForSeconds(duration);
    }
}
