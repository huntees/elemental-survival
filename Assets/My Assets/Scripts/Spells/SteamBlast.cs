using UnityEngine;

public class SteamBlast : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    [Header("Base Values")]
    [SerializeField] private float m_damage = 0.2f;
    [SerializeField] private float m_pushBackForce = 0.02f; //0.2 should be absolute max

    [Header("Damage Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 0.1f;
    [SerializeField] private float m_pushBackForceIncrease = 0.02f;

    //needed because everytime space is pressed, damage is increased
    private float m_totalDamage;
    private float m_totalPushBackForce;

    [Header("Audio")]
    [SerializeField] private AudioClip m_triggerSound;
    private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    public void SetValueIncrease(int waterLevel, int fireLevel, float spellAmp)
    {
        m_totalDamage = m_damage;
        m_totalPushBackForce = m_pushBackForce;

        m_totalDamage += m_damageIncrease * waterLevel;
        m_totalDamage += m_totalDamage * spellAmp;
        m_totalPushBackForce += m_pushBackForceIncrease * fireLevel;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().PushBack(m_totalPushBackForce);
            other.GetComponent<EnemyController>().TakeDamage(m_totalDamage, Elements.Water);
        }
    }

    public void PlaySteamBlast()
    {
        m_particleSystem.Play();
        m_audioSource.PlayOneShot(m_triggerSound);
        m_audioSource.Play();
    }

    public void StopSteamBlast()
    {
        m_particleSystem.Stop();
        m_audioSource.Stop();
    }
}
