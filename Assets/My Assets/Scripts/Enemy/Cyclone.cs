using UnityEngine;

public class Cyclone : MonoBehaviour
{
    [HideInInspector] public EnemyController m_enemyController;
    [HideInInspector] public float m_cycloneDuration = 0.0f;

    void Start()
    {
        //Destroy object after parameter duration
        Destroy(gameObject, m_cycloneDuration);
    }

    //Sends enemy up on enter
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

    //Sends enemy up on stay
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
