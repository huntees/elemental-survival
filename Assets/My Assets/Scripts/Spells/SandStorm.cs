using UnityEngine;

public class SandStorm : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] private float m_damage = 1.0f;
    [SerializeField] private float m_duration = 2.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 1.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    [Header("Misc")]
    [SerializeField] private float m_damageInterval = 1.0f;
    [SerializeField] private float m_slowMultiplier = 0.50f;
    

    private EnemyController m_collidedEnemy;

    private Vector3 m_damageColliderPosition;
    private Vector3 m_size;
    private float m_nextDamageTime = 0.0f;

    void Start()
    {
        m_size = GetComponent<BoxCollider>().size / 2;
        m_damageColliderPosition = transform.position + new Vector3(0, 0, -2);
    }

    void Update()
    {
        if (Time.time >= m_nextDamageTime)
        {
            Damage();
            m_nextDamageTime = Time.time + m_damageInterval;
        }
    }

    public void SetValueIncrease(int earthLevel, int airLevel, float spellAmp)
    {
        m_damage += m_damageIncrease * earthLevel;
        m_damage += m_damage * spellAmp;

        m_duration += m_durationIncrease * airLevel;
        Destroy(gameObject, m_duration);
    }

    private void Damage()
    {
        Collider[] enemiesHit = Physics.OverlapBox(m_damageColliderPosition, m_size);

        foreach (Collider enemy in enemiesHit)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                enemy.gameObject.GetComponent<EnemyController>().TakeDamage(m_damage, Elements.Earth);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.SlowEnemySpeed(m_slowMultiplier);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.ResetEnemySpeed();
        }
    }

    //Reset all enemy inside the collider when sandstorm is over
    void OnDestroy()
    {
        Collider[] enemiesHit = Physics.OverlapBox(m_damageColliderPosition, m_size);

        foreach (Collider enemy in enemiesHit)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                enemy.gameObject.GetComponent<EnemyController>().ResetEnemySpeed();
            }
        }
    }
}
