using UnityEngine;

public class InnerFire : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    [Header("Base Values")]
    [SerializeField] private float m_damage = 5.0f;
    [SerializeField] private float m_duration = 2.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 2.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    private Transform m_playerTransform;
    private Vector3 m_axis = Vector3.up;
    private float m_speed = 100.0f;

    //needed because it is the same object
    private float m_totalDamage;
    private float m_totalDuration;

    [HideInInspector] public bool m_isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = transform.parent;
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (m_isActive)
        {
            transform.RotateAround(m_playerTransform.position, m_axis, m_speed * Time.deltaTime);
        }
    }

    public void SetValueIncrease(int fireLevel, int natureLevel)
    {
        m_totalDamage = m_damage;
        m_totalDuration = m_duration;

        m_totalDamage += m_damageIncrease * fireLevel;
        m_totalDuration += m_durationIncrease * natureLevel;
    }

    public void Activate()
    {
        m_particleSystem.Play();
        m_isActive = true;
    }

    public void Deactivate()
    {
        m_particleSystem.Stop();
        m_isActive = false;
    }

    public float GetDamageAmount()
    {
        return m_totalDamage;
    }

    public float GetDuration()
    {
        return m_totalDuration;
    }
}
