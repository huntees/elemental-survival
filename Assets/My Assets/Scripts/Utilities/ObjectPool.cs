using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject m_prefabObject;
    [SerializeField] private int m_poolDepth;

    private SpawnManager m_spawnManager;
    private GameObject m_pooledObject;

    private EnemyController m_instantiatedEnemyController;

    private readonly List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        m_spawnManager = GetComponent<SpawnManager>();

        for (int i = 0; i < m_poolDepth; i++)
        {
            CreateNewObject();
        }
    }

    public GameObject GetAvailableObject()
    {
        //return available object
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].activeInHierarchy == false)
            {
                return pool[i];
            }
        }

        //if pool is exhausted, make new ones
        CreateNewObject();

        return m_pooledObject;
    }

    private void CreateNewObject()
    {
        m_pooledObject = Instantiate(m_prefabObject);

        //subscribe to enemycontroller so spawnmanager gets to know when an enemy dies
        m_instantiatedEnemyController = m_pooledObject.GetComponent<EnemyController>();
        if(m_instantiatedEnemyController != null)
        {
            m_instantiatedEnemyController.enemyDies += m_spawnManager.DecreaseEnemyCount;
        }

        m_pooledObject.SetActive(false);
        pool.Add(m_pooledObject);
    }
}

