using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour
{
    private EnemyStats m_enemyStats;

    [SerializeField] private Animator m_animator;
    [SerializeField] private NavMeshAgent m_agent;
    private Transform m_player;

    [SerializeField] private Transform m_attackPoint;
    private float m_nextAttackTime = 0.0f;

    private bool isDead = false;
    private bool isStunned = false;
    [SerializeField] private GameObject m_stunnedParticle;

    public event Action enemyDies;

    //Remove on release
    [SerializeField] bool m_TempManuallyPlaced = false;

    private void OnEnable()
    {
        m_enemyStats.ResetStats();

        //Enabling a range of stuff that are disabled when enemy dies
        isDead = false;
        m_agent.isStopped = false;
        isStunned = false;
        m_stunnedParticle.SetActive(false);
        GetComponent<BoxCollider>().enabled = true;

        //Enable following
        InvokeRepeating("FollowPlayer", 0f, 0.5f);
    }

    //This needs to be done because OnEnable() gets called on Instantiation instead of when its enabled :/
    private void OnDisable()
    {
        CancelInvoke();
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_enemyStats = GetComponent<EnemyStats>();
        m_player = GameObject.Find("Player").transform;
        if (m_TempManuallyPlaced)
        {
            InvokeRepeating("FollowPlayer", 0f, 0.5f);
        }
    }

    void FixedUpdate()
    {
        m_animator.SetFloat("Move Speed", m_agent.velocity.magnitude, 0f, Time.deltaTime);
    }

    void FollowPlayer()
    {
        m_agent.SetDestination(m_player.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || isStunned)
        {
            return;
        }

        //Attack
        if ((m_player.position - transform.position).magnitude <= 2f)
        {
            if (Time.time >= m_nextAttackTime)
            {
                Attack();
                m_nextAttackTime = Time.time + m_enemyStats.attackFrequency;
            }
        }
    }

    private void Attack()
    {
        m_animator.SetTrigger("Attack");
        Invoke("AttackOnAnim", m_animator.GetCurrentAnimatorStateInfo(1).length * 0.45f);
    }

    private void AttackOnAnim()
    {

        Collider[] enemiesHit = Physics.OverlapSphere(m_attackPoint.position, m_enemyStats.attackRange);
        //Collider[] enemiesHit = Physics.OverlapBox(m_attackPoint.position, )

        foreach (Collider enemy in enemiesHit)
        {
            if (enemy.gameObject.CompareTag("Player"))
            {
                enemy.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        m_animator.SetTrigger("Take Damage");
        m_enemyStats.currentHealth -= damage;


        if (m_enemyStats.currentHealth < 1)
        {
            m_animator.SetTrigger("Die");
            isDead = true;
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

    public void ChangeElement(Elements element)
    {
        m_enemyStats.ChangeElement(element);
    }

    public void Stun(float seconds)
    {
        if(isStunned)
        {
            StopCoroutine(StunCooldown(seconds));
        }

        isStunned = true;
        m_stunnedParticle.SetActive(true);
        m_agent.isStopped = true;
        StartCoroutine(StunCooldown(seconds));
    }

    IEnumerator StunCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isStunned = false;
        m_stunnedParticle.SetActive(false);
        m_agent.isStopped = false;
    }


    //void OnDrawGizmosSelected()
    //{
    //    if (m_attackPoint == null)
    //        return;

    //    Gizmos.DrawWireSphere(m_attackPoint.position, 0.7f);
    //}
}
