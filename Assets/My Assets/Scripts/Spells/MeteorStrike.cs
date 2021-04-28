using UnityEngine;

public class MeteorStrike : MonoBehaviour
{
    //1 strike happens at the start, if duration is 2, total strikes is 3

    [Header("Base Values per Strike")]
    [SerializeField] private float m_damage = 6.0f;
    [SerializeField] private float m_duration = 1.0f;

    [Header("Increase Per Level per Strike")]
    [SerializeField] private float m_damageIncrease = 4.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    [Header("Misc")]
    [SerializeField] private float m_damageInterval = 1.0f;

    private Vector3 m_damageColliderPosition;
    private Vector3 m_size;
    private float m_nextDamageTime = 0.0f;

    void Start()
    {
        m_size = GetComponent<BoxCollider>().size / 2;
        m_damageColliderPosition = transform.position + new Vector3(0, 1, 0);

        //Sync damage with the meteor drop
        m_nextDamageTime = Time.time + 0.60f;
    }

    void Update()
    {
        if (Time.time >= m_nextDamageTime)
        {
            Damage();
            m_nextDamageTime = Time.time + m_damageInterval;
        }
    }

    public void SetValueIncrease(int fireLevel, int airLevel, float spellAmp)
    {
        m_damage += m_damageIncrease * fireLevel;
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
                enemy.gameObject.GetComponent<EnemyController>().TakeDamage(m_damage, Elements.Fire);
            }
        }
    }
}