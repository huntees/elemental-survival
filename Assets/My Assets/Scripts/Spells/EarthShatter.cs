using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthShatter : MonoBehaviour
{
    [SerializeField] private Transform m_parentTransform;
    private float m_scaleExtension = 0f;
    private float m_yVelocity = 0f;

    private EnemyController m_collidedEnemy;

    void Start()
    {
        Destroy(gameObject, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        m_scaleExtension = Mathf.SmoothDamp(m_scaleExtension, 17f, ref m_yVelocity, 0.5f);

        m_parentTransform.localScale = new Vector3(m_parentTransform.localScale.x, m_parentTransform.localScale.y, m_scaleExtension);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            m_collidedEnemy = other.GetComponent<EnemyController>();
            m_collidedEnemy.Stun(1);
            m_collidedEnemy.TakeDamage(10);
        }
    }
}
