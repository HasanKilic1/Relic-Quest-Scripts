using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageWorldText : MonoBehaviour , IPooledObject
{
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] Color zero_to_hundred;
    [SerializeField] Color hundred_to_twoHundreds;
    [SerializeField] Color twohundred_to_threeHundreds;
    [SerializeField] Color threeHundred_to_else;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), 0.3f);
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {}

    public void SetColorOnDamage(int damage)
    {
        if(damage <= 100)
        {
            damageText.color = zero_to_hundred;
        }
        if (damage > 100 && damage <= 200)
        {
            damageText.color = hundred_to_twoHundreds;
        }
        if (damage > 200 && damage <= 300)
        {
            damageText.color = twohundred_to_threeHundreds;
        }
        if (damage > 300)
        {
            damageText.color = threeHundred_to_else;
        }
        damageText.text = damage.ToString();
    }

    
}
