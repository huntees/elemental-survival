using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    private SkinnedMeshRenderer m_meshRender;

    [SerializeField] private Material m_fireGolemMaterial;
    [SerializeField] private Material m_waterGolemMaterial;
    [SerializeField] private Material m_earthGolemMaterial;

    public float m_maxHealth = 20;
    public float m_movementSpeed = 5f;
    public float m_attackFrequency = 1f;
    public float m_attackRange = 0.5f;

    [HideInInspector] public float m_currentHealth;

    public Elements m_elementType; 

    // Start is called before the first frame update
    void Awake()
    {
        m_meshRender = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();

        m_currentHealth = m_maxHealth;
    }

    public void ChangeElement(Elements element)
    {
        m_elementType = element;

        switch (m_elementType)
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
        m_currentHealth = m_maxHealth;
    }
}
