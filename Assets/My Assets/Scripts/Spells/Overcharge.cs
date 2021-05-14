using UnityEngine;

public class Overcharge : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    [Header("Base Values")]
    [SerializeField] private float m_attackSpeed = 3.0f;
    [SerializeField] private float m_duration = 3.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_attackSpeedIncrease = 5.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    //needed because it is the same object
    private float m_totalAttackSpeed;
    private float m_totalDuration;

    [Header("Audio")]
    [SerializeField] private AudioClip m_triggerSound;
    private AudioSource m_audioSource;

    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();
    }

    public void SetValueIncrease(int airLevel, int natureLevel, float spellAmp)
    {
        m_totalAttackSpeed = m_attackSpeed;
        m_totalDuration = m_duration;

        m_totalAttackSpeed += m_attackSpeedIncrease * airLevel;
        m_totalAttackSpeed += m_totalAttackSpeed * spellAmp;

        m_totalDuration += m_durationIncrease * natureLevel;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        m_particleSystem.Play();
        m_audioSource.PlayOneShot(m_triggerSound);
        m_audioSource.Play();
    }

    public void Deactivate()
    {
        m_particleSystem.Stop();
        m_audioSource.Stop();

        Invoke("DisableObject", 1.0f);
    }

    public float GetAttackSpeedAmount()
    {
        return m_totalAttackSpeed;
    }

    public float GetDuration()
    {
        return m_totalDuration;
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
