using UnityEngine;

public class LivingArmor : MonoBehaviour
{
    private ParticleSystem m_particleSystem;
    private ParticleSystem.MainModule m_particleMain;
    private ParticleSystem.MainModule m_particleMain2;

    [Header("Base Values")]
    [SerializeField] private float m_damageBlock = 0.1f;
    [SerializeField] private float m_duration = 4.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_damageBlockIncrease = 0.1f; //1.5 reaches 60% block, reaches 50% block on earth level 10
    [SerializeField] private float m_durationIncrease = 1.0f;

    [Header("Misc")]
    [SerializeField] private float m_damageBlockCap = 0.6f;

    //needed because it is the same object
    private float m_totalDamageBlock;
    private float m_totalDuration;

    [Header ("Audio")]
    [SerializeField] private AudioClip m_triggerSound;
    private AudioSource m_audioSource;

    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_particleMain = m_particleSystem.main;
        m_particleMain2 = transform.GetChild(2).GetComponent<ParticleSystem>().main;

        m_audioSource = GetComponent<AudioSource>();
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
        gameObject.SetActive(true);
        m_particleMain.simulationSpeed = 0.3f;
        m_particleMain2.simulationSpeed = 0.2f;
        m_particleSystem.Play();
        m_audioSource.PlayOneShot(m_triggerSound);
        m_audioSource.Play();
    }

    public void Deactivate()
    {
        m_particleMain.simulationSpeed = 1.0f;
        m_particleMain2.simulationSpeed = 1.0f;
        m_particleSystem.Stop();
        m_audioSource.Stop();

        Invoke("DisableObject", 1.0f);
    }

    public float GetDamageBlockAmount()
    {
        return m_totalDamageBlock;
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
