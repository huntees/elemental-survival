using UnityEngine;

public class Cyclone : MonoBehaviour
{
    [HideInInspector] public EnemyController m_enemyController;
    [HideInInspector] public float m_cycloneDuration = 0.0f;

    void Start()
    {
        Destroy(gameObject, m_cycloneDuration);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if(other.gameObject.GetComponent<EnemyController>() == m_enemyController)
            {
                m_enemyController.PushUp(m_cycloneDuration);
            }

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyController>() == m_enemyController)
            {
                m_enemyController.PushUp(m_cycloneDuration);
            }
        }
    }
}
