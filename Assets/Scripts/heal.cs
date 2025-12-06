using System.Collections;
using UnityEngine;

public class heal : MonoBehaviour
{
    enum HealType {HOT, still} //HOT capsule that disappears after healed
    [SerializeField] int healAmount; //total heal amount
    [SerializeField] int healTime; //how long it heals
    [SerializeField] int healSpeed; //rate it heals at

    [SerializeField] HealType _heal;

    bool healing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other) //capsule heals
    {
        if (other.isTrigger)
            return;

        IHeal health = other.GetComponent<IHeal>(); //does it have IHeal

        if(health != null && _heal == HealType.HOT) //if not null and is Heal over time
        {
            health.heal(healAmount); //heal for that amount
            Destroy(gameObject);
        }
        

     
    }
    private void OnTriggerStay(Collider other) //heal area
    {
        if (other.isTrigger)
            return;
        IHeal health = other.GetComponent<IHeal>();

        if(health != null && _heal == HealType.still && !healing) //if health isnt null and type is still
        {
            StartCoroutine(healOther(health)); 
        }
    }

    IEnumerator healOther(IHeal h)
    {
        healing = true;
        h.heal(healAmount);
        yield return new WaitForSeconds(healTime); //how fast is heal time
        healing = false;
    }
}
    
