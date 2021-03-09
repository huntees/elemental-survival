using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    private SkinnedMeshRenderer m_meshRender;

    [SerializeField] private Material m_fireGolemMaterial;
    [SerializeField] private Material m_waterGolemMaterial;
    [SerializeField] private Material m_earthGolemMaterial;

    [SerializeField] private int m_health = 20;
    public float movementSpeed = 5f;
    public float attackFrequency = 1f;
    public float attackRange = 0.5f;
    
    public float currentHealth;

    [SerializeField] private Elements elementType; 

    // Start is called before the first frame update
    void Awake()
    {
        m_meshRender = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();

        currentHealth = m_health;
    }

    public void ChangeElement(Elements element)
    {
        elementType = element;

        switch (elementType)
        {
            case Elements.Fire:
                m_meshRender.material = m_fireGolemMaterial;
                break;

            case Elements.Water:
                m_meshRender.material = m_waterGolemMaterial;
                break;

            case Elements.Earth:
                m_meshRender.material = m_earthGolemMaterial;
                break;
        }
    }

    public void ResetStats()
    {
        currentHealth = m_health;
    }
}
