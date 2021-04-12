using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamBlast : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] private float m_damage = 0.1f;
    [SerializeField] private float m_pushBackForce = 0.02f; //0.2 should be absolute max

    [Header("Damage Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 0.05f;
    [SerializeField] private float m_pushBackForceIncrease = 0.01f;

    //needed because everytime space is pressed, damage is increased
    private float m_totalDamage;
    private float m_totalPushBackForce;

    public void SetElementLevel(int fireLevel, int waterLevel)
    {
        m_totalDamage = m_damage;
        m_totalPushBackForce = m_pushBackForce;

        m_totalDamage += m_damageIncrease * fireLevel;
        m_totalPushBackForce += m_pushBackForceIncrease * waterLevel;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().PushBack(m_totalPushBackForce);
            other.GetComponent<EnemyController>().TakeDamage(m_totalDamage, Elements.Fire);

        }

    }
}
