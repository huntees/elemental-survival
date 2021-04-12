using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] m_spawnLocations;
    private ObjectPool m_enemyObjectPool;

    [SerializeField] float m_maxSpawnTime = 8f;
    [SerializeField] float m_minSpawnTime = 4f;

    private int m_randomSpawnIndex;
    private int m_randomElement;
    private float m_randomSpawnTime = 4f;

    private GameObject m_instantiatedEnemy;
    public int enemiesLeft = 0;
    public bool isSpawning = false;

    private int m_elementCount;

    // Start is called before the first frame update
    void Start()
    {
        m_enemyObjectPool = GetComponent<ObjectPool>();

        m_elementCount = Elements.GetNames(typeof(Elements)).Length - 1;
    }

    public void InitiateEnemySpawn(int numOfEnemies)
    {
        StartCoroutine(SpawnEnemies(numOfEnemies));
    }

    IEnumerator SpawnEnemies(int numOfEnemies)
    {
        isSpawning = true;
        int enemiesSpawned = 0;

        while (enemiesSpawned < numOfEnemies)
        {
            m_randomSpawnTime = Random.Range(m_minSpawnTime, m_maxSpawnTime);
            m_randomElement = Random.Range(0, m_elementCount);
            m_randomSpawnIndex = Random.Range(0, m_spawnLocations.Length);
            
            yield return new WaitForSeconds(m_randomSpawnTime);

            //m_instantiatedEnemy = Instantiate(m_enemyPrefab, m_spawnLocations[m_randomSpawnIndex].position, Quaternion.identity);
            m_instantiatedEnemy = m_enemyObjectPool.GetAvailableObject();
            m_instantiatedEnemy.GetComponent<EnemyController>().ChangeElement((Elements)m_randomElement);
            m_instantiatedEnemy.transform.position = m_spawnLocations[m_randomSpawnIndex].position;
            m_instantiatedEnemy.SetActive(true);

            enemiesSpawned++;
            enemiesLeft++;
        }

        isSpawning = false;
    }

    public void DecreaseEnemyCount()
    {
        enemiesLeft--;
    }
}
