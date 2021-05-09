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

    [Header("Default Values [Normal Mode]")]
    [SerializeField] private float m_defaultMaxHealth = 30.0f;
    [SerializeField] private float m_defaultMovementSpeed = 2.0f;
    [SerializeField] private float m_defaultAttackDamage = 10.0f;
    [SerializeField] private float m_defaultStoppingDistance = 1.0f;

    [Header("Default Values [Hard Mode]")]
    [SerializeField] private float m_defaultHardMaxHealth = 40.0f;
    [SerializeField] private float m_defaultHardMovementSpeed = 3.0f;
    [SerializeField] private float m_defaultHardAttackDamage = 13.0f;
    [SerializeField] private float m_defaultHardStoppingDistance = 0.5f;

    [Header("Current Values")]
    public float m_maxHealth;
    [HideInInspector] public float m_currentHealth = 30.0f;
    public float m_movementSpeed = 2.0f;
    public float m_stoppingDistance = 1.0f;
    public float m_attackDamage = 12.0f;

    public float m_attackFrequency = 1f;
    public float m_attackRange = 0.9f;

    public Elements m_elementType; 

    // Start is called before the first frame update
    void Awake()
    {
        m_meshRender = transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();

        if (Difficulty.instance != null && Difficulty.instance.m_isHardMode)
        {
            m_defaultMaxHealth = m_defaultHardMaxHealth;
            m_defaultMovementSpeed = m_defaultHardMovementSpeed;
            m_defaultAttackDamage = m_defaultHardAttackDamage;
            m_defaultStoppingDistance = m_defaultHardStoppingDistance;
        }

        m_stoppingDistance = m_defaultStoppingDistance;
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
