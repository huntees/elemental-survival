using UnityEngine;

public class InnerFire : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    [Header("Base Values")]
    [SerializeField] private float m_damage = 5.0f;
    [SerializeField] private float m_duration = 3.0f;

    [Header("Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 6.0f;
    [SerializeField] private float m_durationIncrease = 1.0f;

    private Transform m_playerTransform;
    private Vector3 m_axis = Vector3.up;
    private float m_speed = 100.0f;

    //needed because it is the same object
    private float m_totalDamage;
    private float m_totalDuration;

    [Header("Audio")]
    [SerializeField] private AudioClip m_triggerSound;
    private AudioSource m_audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        m_playerTransform = transform.parent;
    }

    void Update()
    {
        transform.RotateAround(m_playerTransform.position, m_axis, m_speed * Time.deltaTime);
    }

    public void SetValueIncrease(int fireLevel, int natureLevel, float spellAmp)
    {
        m_totalDamage = m_damage;
        m_totalDuration = m_duration;

        m_totalDamage += m_damageIncrease * fireLevel;
        m_totalDamage += m_totalDamage * spellAmp;

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

    public float GetDamageAmount()
    {
        return m_totalDamage;
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
