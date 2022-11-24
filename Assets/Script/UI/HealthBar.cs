using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Entity _entity;

    private void Start()
    {
        // Temp
        LinkHealthBar(FindObjectOfType<TEST_PlayerMovement>());
    }

    public void LinkHealthBar(Entity entity)
    {
        if(_entity != null) _entity.OnDamageEvent -= UpdateBar;

        _entity = entity;

        // Init
        if (transform.childCount == 0) return;
        if (transform.childCount > 1)
        {
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < _entity.HealthPoints - 1; i++)
        {
            Instantiate(transform.GetChild(0), transform);
        }

        _entity.OnDamageEvent += UpdateBar;
    }

    public void UpdateBar()
    {
        for (int i = 0; i < _entity.MaxHeathPoint; i++)
        {
            if (i < _entity.HealthPoints) 
                transform.GetChild(i).gameObject.SetActive(true);
            else 
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }

}
