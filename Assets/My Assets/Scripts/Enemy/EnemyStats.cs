using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    private SkinnedMeshRenderer m_meshRender;

    [Header ("Golem Materials")]
    [SerializeField] private Material m_fireGolemMaterial;
    [SerializeField] private Material m_waterGolemMaterial;
    [SerializeField] private Material m_earthGolemMaterial;
    [SerializeField] private Material m_natureGolemMaterial;
    [SerializeField] private Material m_airGolemMaterial;

    [Header("Default Values")]
    [SerializeField] private float m_defaultMaxHealth = 40.0f;
    [SerializeField] private float m_defaultMovementSpeed = 2.0f;
    [SerializeField] private float m_defaultAttackDamage = 12.0f;

    [Header("Current Values")]
    public float m_maxHealth;
    [HideInInspector] public float m_currentHealth = 40.0f;
    public float m_movementSpeed = 2.0f;
    public float m_attackDamage = 12.0f;

    public float m_attackFrequency = 1f;
    public float m_attackRange = 0.5f;

    public Elements m_elementType; 

    // Start is called before the first frame update
    void Awake()
    {
        m_meshRender = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();

        m_maxHealth = m_defaultMaxHealth;
        m_currentHealth = m_maxHealth;

        m_movementSpeed = m_defaultMovementSpeed;

        m_attackDamage = m_defaultAttackDamage;

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

            case Elements.Nature:
                m_meshRender.material = m_natureGolemMaterial;
                break;

            case Elements.Air:
                m_meshRender.material = m_airGolemMaterial;
                break;
        }
    }

    public void ResetHealth()
    {
        m_currentHealth = m_maxHealth;
    }

    public void IncrementStats(float health, float movementSpeed, float attackDamage)
    {
        m_maxHealth = m_defaultMaxHealth + health;
        m_movementSpeed = m_defaultMovementSpeed + movementSpeed;
        m_attackDamage = m_defaultAttackDamage + attackDamage;
    }
}
