using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamBlast : MonoBehaviour
{
    private ParticleSystem m_steamBlastParticles;

    [Header("Base Values")]
    [SerializeField] private float m_damage = 0.1f;
    [SerializeField] private float m_pushBackForce = 0.02f; //0.2 should be absolute max

    [Header("Damage Increase Per Level")]
    [SerializeField] private float m_damageIncrease = 0.05f;
    [SerializeField] private float m_pushBackForceIncrease = 0.01f;

    //needed because everytime space is pressed, damage is increased
    private float m_totalDamage;
    private float m_totalPushBackForce;

    private void Awake()
    {
        m_steamBlastParticles = GetComponent<ParticleSystem>();
    }

    public void SetValueIncrease(int waterLevel, int fireLevel)
    {
        m_totalDamage = m_damage;
        m_totalPushBackForce = m_pushBackForce;

        m_totalDamage += m_damageIncrease * waterLevel;
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
        m_steamBlastParticles.Play();
    }

    public void StopSteamBlast()
    {
        m_steamBlastParticles.Stop();
    }
}
