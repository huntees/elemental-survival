using UnityEngine;

public class EarthShatter : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] private float m_damage = 20f;
    [SerializeField] private float m_stunDuration = 0.5f;

    [Header("Damage Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 8f;
    [SerializeField] private float m_stunDurationIncrease = 1.0f;

    [Header("Misc")]
    [SerializeField] private Transform m_parentTransform;
    private float m_scaleExtension = 0f;
    private float m_yVelocity = 0f;

    private EnemyController m_collidedEnemy;

    void Start()
    {
        //Destroy itself after 2 seconds
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        m_scaleExtension = Mathf.SmoothDamp(m_scaleExtension, 17f, ref m_yVelocity, 0.5f);

        m_parentTransform.localScale = new Vector3(m_parentTransform.localScale.x, m_parentTransform.localScale.y, m_scaleExtension);

    }

    public void SetValueIncrease(int fireLevel, int earthLevel, float spellAmp)
    {
        m_damage += m_damageIncrease * fireLevel;
        m_damage += m_damage * spellAmp;

        m_stunDuration += m_stunDurationIncrease * earthLevel;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.TakeDamage(m_damage, Elements.Fire);
            m_collidedEnemy.Stun(m_stunDuration);
        }
    }
}
