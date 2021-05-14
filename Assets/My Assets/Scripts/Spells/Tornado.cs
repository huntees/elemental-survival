using UnityEngine;

public class Tornado : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] private float m_damage = 20.0f;
    [SerializeField] private float m_distance = 0.3f;

    [Header("Damage Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 10.0f;
    [SerializeField] private float m_distanceIncrease = 0.2f;

    [Header("Misc")]
    [SerializeField] private float m_speed = 10.0f;
    [SerializeField] private float m_cycloneDuration = 2.5f;

    private EnemyController m_collidedEnemy;

    void FixedUpdate()
    {
        transform.position += transform.forward * m_speed * Time.deltaTime;
    }

    public void SetValueIncrease(int airLevel, int waterLevel, float spellAmp)
    {
        m_damage += m_damageIncrease * airLevel;
        m_damage += m_damage * spellAmp;

        m_distance += m_distanceIncrease * waterLevel;

        //Destroy itself after time travelled by m_distance
        Destroy(gameObject, m_distance);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.TakeDamage(m_damage, Elements.Air);
            m_collidedEnemy.Cyclone(m_cycloneDuration);
        }
    }
}
