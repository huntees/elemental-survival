using UnityEngine;

public class ItemParticleCollision : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float m_damage = 10.0f;

    private AudioSource m_audioSource;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        if(m_audioSource != null)
        {
            m_audioSource.Play();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(m_damage, Elements.Neutral);
        }
    }
}
