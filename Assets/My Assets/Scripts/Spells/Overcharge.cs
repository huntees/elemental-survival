using UnityEngine;

public class Overcharge : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    [Header("Base Values")]
    [SerializeField] private float m_attackSpeed = 5.0f;
    [SerializeField] private float m_duration = 2.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_attackSpeedIncrease = 3.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    [HideInInspector] public bool m_isActive = false;

    //needed because it is the same object
    private float m_totalAttackSpeed;
    private float m_totalDuration;

    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    public void SetValueIncrease(int airLevel, int natureLevel)
    {
        m_totalAttackSpeed = m_attackSpeed;
        m_totalDuration = m_duration;

        m_totalAttackSpeed += m_attackSpeedIncrease * airLevel;
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

    public float GetAttackSpeedAmount()
    {
        return m_totalAttackSpeed;
    }

    public float GetDuration()
    {
        return m_totalDuration;
    }
}
