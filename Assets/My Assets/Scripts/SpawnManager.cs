using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private PlayerInventory m_playerInventory;

    [Header("Enemy Gold Bounty")]
    [SerializeField] private int m_maxBounty = 40;
    [SerializeField] private int m_minBounty = 34;
    private int m_bountyAmount;

    [Header("Enemy Spawn Properties")]
    [SerializeField] Transform[] m_spawnLocations;
    private ObjectPool m_enemyObjectPool;

    [SerializeField] float m_maxSpawnTime = 8f;
    [SerializeField] float m_minSpawnTime = 4f;

    private int m_randomSpawnIndex;
    private int m_randomElement;
    private float m_randomSpawnTime = 4f;

    private GameObject m_instantiatedEnemy;
    public int m_enemiesLeft = 0;
    public bool m_isSpawning = false;

    private int m_elementCount;

    // Start is called before the first frame update
    void Start()
    {
        m_enemyObjectPool = GetComponent<ObjectPool>();

        m_playerInventory = PlayerInventory.instance;
        m_elementCount = Elements.GetNames(typeof(Elements)).Length - 1;
    }

    public void InitiateEnemySpawn(int numOfEnemies)
    {
        StartCoroutine(SpawnEnemies(numOfEnemies));
    }

    IEnumerator SpawnEnemies(int numOfEnemies)
    {
        m_isSpawning = true;
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
            m_enemiesLeft++;
        }

        m_isSpawning = false;
    }

    //Triggers when an enemy dies
    public void DecreaseEnemyCount()
    {
        m_enemiesLeft--;

        m_bountyAmount = Random.Range(m_minBounty, m_maxBounty + 1);
        m_playerInventory.GivePlayerGold(m_bountyAmount);
    }
}
