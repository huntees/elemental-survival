using UnityEngine;

public class ItemParticleCollision : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float m_damage = 10.0f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(m_damage, Elements.Neutral);
        }
    }
}
