using System.Collections;
using UnityEngine;

public class LivingArmor : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    [Header("Base Values")]
    [SerializeField] private float m_damageBlock = 0.1f;
    [SerializeField] private float m_duration = 4.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_damageBlockIncrease = 0.1f; //1.5 reaches 60% block, reaches 50% block on earth level 10
    [SerializeField] private float m_durationIncrease = 1.0f;

    [Header("Misc")]
    [SerializeField] private float m_damageBlockCap = 0.6f;

    [HideInInspector] public bool m_isActive = false;

    //needed because it is the same object
    private float m_totalDamageBlock;
    private float m_totalDuration;

    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    public void SetValueIncrease(int earthLevel, int natureLevel, float spellAmp)
    {
        m_totalDamageBlock = m_damageBlock;
        m_totalDuration = m_duration;

        m_totalDamageBlock += m_damageBlockIncrease * earthLevel;
        m_totalDamageBlock += m_totalDamageBlock * spellAmp;

        m_totalDuration += m_durationIncrease * natureLevel;

        m_totalDamageBlock = (m_totalDamageBlock / (m_totalDamageBlock + 1));

        if (m_totalDamageBlock > m_damageBlockCap)
        {
            m_totalDamageBlock = m_damageBlockCap;
        }
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

    public float GetDamageBlockAmount()
    {
        return m_totalDamageBlock;
    }

    public float GetDuration()
    {
        return m_totalDuration;
    }
}
