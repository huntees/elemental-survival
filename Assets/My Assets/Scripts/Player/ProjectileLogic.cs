using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    [Header("Projectile Base Values")]
    [SerializeField] private Elements m_elementType;
    [SerializeField] private int m_damage = 5;
    [SerializeField] private float m_projectileSpeed = 15.0f;

    [Header("Damage Increase (Addition to Element Level)")]
    [SerializeField] private int m_damageIncrease = 3;

    [Header("Damage Variance (Addition to Element Level)")]
    [SerializeField] private int m_baseDamageVariance = 2;
    private int m_maxDamageThreshold;
    private int m_minDamageThreshold;

    [Header("On Hit Effect")]
    [SerializeField] private GameObject m_hitPrefab;

    private GameObject collidedObject;

    public void FireProjectile(Vector3 shootDirection)
    {
        Rigidbody projectileRB = GetComponent<Rigidbody>();
        projectileRB.AddForce(shootDirection * m_projectileSpeed, ForceMode.Impulse);
    }

    public void SetElementLevel(int elementLevel)
    {
        m_damage += m_damageIncrease * elementLevel;

        m_minDamageThreshold = m_damage - m_baseDamageVariance - (int)(elementLevel * 0.5f);
        m_maxDamageThreshold = m_damage + m_baseDamageVariance + (int)(elementLevel * 0.5f);
        m_damage = Random.Range(m_minDamageThreshold, m_maxDamageThreshold + 1);
    }

    void OnCollisionEnter(Collision collision)
    {
        collidedObject = collision.gameObject;

        if (!(collidedObject.CompareTag("Player") || collidedObject.CompareTag("Projectile")))
        {
            if (m_hitPrefab != null)
            {
                var hitEffect = Instantiate(m_hitPrefab, transform.position, Quaternion.identity);
                Destroy(hitEffect, 2f);
            }

            if (collidedObject.CompareTag("Enemy"))
            {
                collidedObject.GetComponent<EnemyController>().TakeDamage(m_damage, m_elementType);
            }

            Destroy(gameObject);
        }
    }
}
