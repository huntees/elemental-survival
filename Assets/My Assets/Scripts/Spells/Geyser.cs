using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] private float m_damage = 5f;

    [Header("Damage Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 3f;

    private ParticleSystem m_particleSystem;

    private EnemyController m_collidedEnemy;

    // Start is called before the first frame update
    void Start()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        Invoke("ActivateCollider", 1.2f);
        Destroy(gameObject, 3f);
    }

    private void ActivateCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    public void SetValueIncrease(int waterLevel, int earthLevel)
    {
        m_damage += m_damageIncrease * waterLevel;
        m_damage += m_damageIncrease * earthLevel;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.TakeDamage(m_damage, Elements.Water);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.PushUp(m_particleSystem.main.duration - m_particleSystem.time);
        }
    }
}

