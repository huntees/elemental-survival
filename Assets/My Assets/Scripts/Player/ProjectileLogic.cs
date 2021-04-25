using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    [Header("Projectile Base Values")]
    [SerializeField] private Elements m_elementType;
    [SerializeField] private int m_damage = 5;
    [SerializeField] private float m_projectileSpeed = 15.0f;

    [Header("Damage Increase")]
    [SerializeField] private int m_damageIncrease = 3;

    [Header("Damage Variance")]
    [SerializeField] private int m_baseDamageVariance = 2;
    private int m_maxDamageThreshold;
    private int m_minDamageThreshold;

    [Header("On Hit Effect")]
    [SerializeField] private GameObject m_hitPrefab;

    private GameObject collidedObject;

    void Start()
    {
        //Destroy itself after 10 seconds
        Invoke("SelfDestruct", 10.0f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    public void FireProjectile(Vector3 shootDirection)
    {
        Rigidbody projectileRB = GetComponent<Rigidbody>();
        projectileRB.AddForce(shootDirection * m_projectileSpeed, ForceMode.Impulse);
    }

    public void SetElementLevel(int elementLevel, float additionalDamage)
    {
        m_damage += m_damageIncrease * elementLevel;
        m_damage += (int)additionalDamage;

        m_minDamageThreshold = m_damage - m_baseDamageVariance - (int)(elementLevel * 0.5f);
        m_maxDamageThreshold = m_damage + m_baseDamageVariance + (int)(elementLevel * 0.5f);
        m_damage = Random.Range(m_minDamageThreshold, m_maxDamageThreshold + 1);
        //Debug.Log("Variance = " + m_minDamageThreshold + "-" + m_maxDamageThreshold + " = " + m_damage);
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
