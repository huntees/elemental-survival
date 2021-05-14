using UnityEngine;
using UnityEngine.AI;

public class FoxController : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_agent;
    private Transform m_player;
    private AudioSource m_audioSource;

    private float m_nextUpdateDestinationTime = 0.0f;
    private float m_nextJumpTime = 5.0f;
    private float m_jumpTime = 0.0f;
    private bool m_isFound = false;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_audioSource = GetComponent<AudioSource>();

        m_agent.isStopped = true;
    }

    void Start()
    {
        m_player = GameObject.Find("FoxFollow").transform;
    }

    void FixedUpdate()
    {
        HandleFollowPlayer();
        m_animator.SetFloat("Move Speed", m_agent.velocity.magnitude, 0f, Time.deltaTime);

        //barks at player if afk for too long
        if(m_agent.velocity.magnitude <= 0.05f)
        {
            if (m_jumpTime >= m_nextJumpTime)
            {
                m_animator.SetTrigger("Jump");
                m_audioSource.PlayDelayed(0.5f);

                m_nextJumpTime = Random.Range(5, 20);
                m_jumpTime = 0.0f;
            }

            m_jumpTime += Time.deltaTime;
        }
    }

    void HandleFollowPlayer()
    {
        if (Time.time >= m_nextUpdateDestinationTime)
        {
            m_agent.SetDestination(m_player.position);
            m_nextUpdateDestinationTime = Time.time + 1.0f;

            //if approached, start following the player
            if (!m_isFound && (m_player.position - transform.position).magnitude <= 5.0f)
            {
                m_isFound = true;

                m_animator.SetTrigger("Jump");
                m_audioSource.PlayDelayed(0.5f);

                m_agent.isStopped = false;
            }
        }
    }
}
