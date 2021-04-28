using System.Collections;
using UnityEngine;

public class NaturesRejuvenation : MonoBehaviour
{
    private ParticleSystem m_particleSystem;
    private PlayerController m_playerController;

    [Header("Base Values")]
    [SerializeField] private float m_healPerSecond = 2.0f;
    [SerializeField] private float m_duration = 2.0f; 

    [Header("Increase Per Level")]
    [SerializeField] private float m_healIncrease = 3.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    [HideInInspector] public bool m_isActive = false;
    private float m_nextHealTime = 0.0f;

    //needed because it is the same object
    private float m_totalHealPerSecond;
    private float m_totalDuration;

    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if(!m_isActive)
        {
            return ;
        }

        if (Time.time >= m_nextHealTime)
        {
            m_playerController.RestoreHealth(m_totalHealPerSecond);
            m_nextHealTime = Time.time + 1.0f;
        }
    }

    public void SetValueIncrease(int waterLevel, int natureLevel, float spellAmp)
    {
        m_totalHealPerSecond = m_healPerSecond;
        m_totalDuration = m_duration;

        m_totalHealPerSecond += m_healIncrease * waterLevel;
        m_totalHealPerSecond += m_totalHealPerSecond * spellAmp;

        m_totalDuration += m_durationIncrease * natureLevel;
    }

    public void Activate()
    {
        m_particleSystem.Play();
        m_isActive = true;
        StartCoroutine(NaturesRejuvenationCountdown(m_totalDuration));
    }

    IEnumerator NaturesRejuvenationCountdown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Deactivate();
    }

    public void Deactivate()
    {
        m_particleSystem.Stop();
        m_isActive = false;
    }
}
