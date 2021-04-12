using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class EnemyController : MonoBehaviour
{
    private EnemyStats m_enemyStats;

    [Header("Components")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private NavMeshAgent m_agent;
    private Transform m_player;

    [SerializeField] private Transform m_attackPoint;
    private float m_nextAttackTime = 0.0f;
    private float m_attackAnimationDelay;

    [Header("Elemental Damage")]
    [SerializeField] private float m_vulnerableMultiplier = 2.0f;
    [SerializeField] private float m_resistantMultiplier = 0.5f;

    [Header("HP Bar")]
    [SerializeField] private Canvas m_healthBar;
    [SerializeField] private Slider m_healthSlider;

    [Header("Status")]
    private bool m_isDead = false;

    private bool m_isStunned = false;
    [SerializeField] private GameObject m_stunnedParticle;

    private bool m_isPushedUp = false;
    private float m_yPositionBeforePushUp;
    private bool m_triggerFallDown = false;

    //Event
    private Action<float, Elements> takeElementalDamage;
    public event Action enemyDies;

    //Remove on release
    [SerializeField] bool m_TempManuallyPlaced = false;

    #region Enable & Disable
    private void OnEnable()
    {
        m_enemyStats.ResetStats();

        //Enabling a range of stuff that are disabled when enemy dies
        m_isDead = false;
        m_isStunned = false;
        m_stunnedParticle.SetActive(false);
        m_triggerFallDown = false;

        m_agent.isStopped = false;
        m_agent.updatePosition = true;

        GetComponent<BoxCollider>().enabled = true;

        //Enable following
        InvokeRepeating("FollowPlayer", 0f, 0.5f);

        m_healthBar.enabled = true;
        UpdateHealthBar(m_enemyStats.m_currentHealth, m_enemyStats.m_maxHealth);
    }

    //This needs to be done because OnEnable() gets called on Instantiation instead of when its enabled :/
    private void OnDisable()
    {
        CancelInvoke();
    }

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        m_enemyStats = GetComponent<EnemyStats>();
        m_player = GameObject.Find("Player").transform;

        //remove on release
        if (m_TempManuallyPlaced)
        {
            InvokeRepeating("FollowPlayer", 0f, 0.5f);
        }
    }

    void FixedUpdate()
    {
        m_animator.SetFloat("Move Speed", m_agent.velocity.magnitude, 0f, Time.deltaTime);

        //For putting enemies back down smoothly
        if(m_triggerFallDown)
        {
            if (transform.position.y > m_yPositionBeforePushUp)
            {
                transform.position -= Vector3.up * 0.2f;
            } 
            else 
            {
                m_isPushedUp = false;

                //This makes sure enemy dont leave stunned state or dead state after push up is finished
                if (!m_isStunned && !m_isDead)
                {
                    m_agent.isStopped = false;
                }
                m_agent.updatePosition = true;
                m_triggerFallDown = false;
            }
        }
    }

    void FollowPlayer()
    {
        m_agent.SetDestination(m_player.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isDead || m_isStunned)
        {
            return;
        }

        //Attack
        if ((m_player.position - transform.position).magnitude <= 2f)
        {
            if (Time.time >= m_nextAttackTime)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        m_animator.SetTrigger("Attack");

        //To sync animation with attack
        m_attackAnimationDelay = m_animator.GetCurrentAnimatorStateInfo(1).length * 0.45f;
        Invoke("AttackOnAnim", m_attackAnimationDelay);
        m_nextAttackTime = Time.time + m_enemyStats.m_attackFrequency + m_attackAnimationDelay;
    }

    private void AttackOnAnim()
    {

        Collider[] enemiesHit = Physics.OverlapSphere(m_attackPoint.position, m_enemyStats.m_attackRange);
        //Collider[] enemiesHit = Physics.OverlapBox(m_attackPoint.position, )

        foreach (Collider enemy in enemiesHit)
        {
            if (enemy.gameObject.CompareTag("Player"))
            {
                enemy.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            }
        }
    }

    #region Take Damage

    public void TakeDamage(float damage, Elements element)
    {

        m_animator.SetTrigger("Take Damage");
        takeElementalDamage?.Invoke(damage, element);

        CheckDeath();
        UpdateHealthBar(m_enemyStats.m_currentHealth, m_enemyStats.m_maxHealth);
    }

    private void IfFireElement(float damage, Elements element)
    {
        switch (element)
        {
            case Elements.Water:
                m_enemyStats.m_currentHealth -= damage * m_vulnerableMultiplier;
                break;

            case Elements.Earth:
                m_enemyStats.m_currentHealth -= damage * m_resistantMultiplier;
                break;

            default:
                m_enemyStats.m_currentHealth -= damage;
                break;
        }
    }

    private void IfWaterElement(float damage, Elements element)
    {
        switch (element)
        {
            case Elements.Earth:
                m_enemyStats.m_currentHealth -= damage * m_vulnerableMultiplier;
                break;

            case Elements.Fire:
                m_enemyStats.m_currentHealth -= damage * m_resistantMultiplier;
                break;

            default:
                m_enemyStats.m_currentHealth -= damage;
                break;
        }
    }

    private void IfEarthElement(float damage, Elements element)
    {
        switch (element)
        {
            case Elements.Fire:
                m_enemyStats.m_currentHealth -= damage * m_vulnerableMultiplier;
                break;

            case Elements.Water:
                m_enemyStats.m_currentHealth -= damage * m_resistantMultiplier;
                break;

            default:
                m_enemyStats.m_currentHealth -= damage;
                break;
        }
    }

    private void CheckDeath()
    {
        if (m_enemyStats.m_currentHealth <= 0)
        {
            m_animator.SetTrigger("Die");
            m_isDead = true;
            GetComponent<BoxCollider>().enabled = false;
            m_stunnedParticle.SetActive(false);

            StopAllCoroutines();
            CancelInvoke();
            m_agent.isStopped = true;


            //inform SpawnManager this enemy died
            enemyDies?.Invoke();

            //Disables object after some time, m_animator.GetCurrentAnimatorStateInfo(0).length + 1f = 2.34f
            Invoke("DisableObject", 2.34f);
        }
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Status Effects
    public void Stun(float seconds)
    {
        if(m_isDead)
        {
            return ;
        }

        if(m_isStunned)
        {
            StopCoroutine(StunCooldown(seconds));
        }

        StartCoroutine(StunCooldown(seconds));
    }

    IEnumerator StunCooldown(float seconds)
    {
        m_isStunned = true;
        m_stunnedParticle.SetActive(true);
        m_agent.isStopped = true;
        yield return new WaitForSeconds(seconds);
        m_isStunned = false;
        m_stunnedParticle.SetActive(false);
        m_agent.isStopped = false;
    }

    public void PushBack(float force)
    {
        transform.position -= (m_player.position - transform.position).normalized * force;
    }

    public void PushUp(float duration)
    {
        //checking isdead not needed here because if enemy died upon entry, the collider is no longer staying inside the geysey collider and therefore is not triggered

        if (!m_isPushedUp)
        {
            m_yPositionBeforePushUp = transform.position.y;
            StartCoroutine(PushUpCooldown(duration));
        }

        transform.position += Vector3.up * 0.1f;
    }

    IEnumerator PushUpCooldown(float seconds)
    {
        m_isPushedUp = true;
        m_agent.isStopped = true;
        m_agent.updatePosition = false;
        yield return new WaitForSeconds(seconds);
        m_triggerFallDown = true;
    }

    #endregion


    //void OnDrawGizmosSelected()
    //{
    //    if (m_attackPoint == null)
    //        return;

    //    Gizmos.DrawWireSphere(m_attackPoint.position, 0.7f);
    //}

    public void ChangeElement(Elements element)
    {

        //switch takeElementalDamage based of what type the enemy is
        switch (element)
        {
            case Elements.Fire:
                takeElementalDamage = IfFireElement;
                break;

            case Elements.Water:
                takeElementalDamage = IfWaterElement;
                break;

            case Elements.Earth:
                takeElementalDamage = IfEarthElement;
                break;

            default:
                break;
        }

        m_enemyStats.ChangeElement(element);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if(m_isDead)
        {
            m_healthBar.enabled = false;
        }

        m_healthSlider.maxValue = maxHealth;
        m_healthSlider.value = currentHealth;
    }
}
