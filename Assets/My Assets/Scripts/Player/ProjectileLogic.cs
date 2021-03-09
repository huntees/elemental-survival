using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private GameObject m_hitPrefab;
    [SerializeField] private float m_projectileSpeed = 15f;

    private GameObject collidedObject;

    public void FireProjectile(Vector3 shootDirection)
    {
        Rigidbody projectileRB = GetComponent<Rigidbody>();
        projectileRB.AddForce(shootDirection * m_projectileSpeed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        collidedObject = collision.gameObject;

        if (!(collidedObject.CompareTag("Player") || collidedObject.CompareTag("Projectile")))
        {
            if (m_hitPrefab != null)
            {
                var hitEffect = Instantiate(m_hitPrefab, transform.position, Quaternion.identity);
                Destroy(hitEffect, 2f);
            }

            if(collidedObject.CompareTag("Enemy"))
            {
                collidedObject.GetComponent<EnemyController>().TakeDamage(10);
            }

            Destroy(gameObject);
        }
    }
}
