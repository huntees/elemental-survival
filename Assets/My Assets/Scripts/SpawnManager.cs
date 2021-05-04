using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private PlayerInventory m_playerInventory;

    [Header("Enemy Gold Bounty")]
    [SerializeField] private int m_maxBounty = 40;
    [SerializeField] private int m_minBounty = 34;
    [SerializeField] private int m_bountyIncreasePerRound = 1;
    private int m_bountyAmount;

    [Header("Enemy Spawn Properties")]
    [SerializeField] Transform[] m_spawnLocations;
    private ObjectPool m_enemyObjectPool;

    [SerializeField] float m_maxSpawnTime = 8f;
    [SerializeField] float m_minSpawnTime = 4f;

    private int m_randomSpawnIndex;
    private int m_randomElement;
    private float m_randomSpawnTime = 4f;

    [Header("Enemy Stats Increment")]
    [SerializeField] private float m_healthIncrease = 5.0f;
    [SerializeField] private float m_movementSpeedIncrease = 0.01f;
    [SerializeField] private float m_attackDamageIncrease = 2.0f;
    private float m_currentHealthIncrease = 0.0f;
    private float m_currentMovementSpeedIncrease = 0.0f;
    private float m_currentAttackDamageIncrease = 0.0f;

    private GameObject m_instantiatedEnemy;
    private EnemyController m_enemyController;

    [HideInInspector] public int m_enemiesLeft = 0;
    [HideInInspector] public bool m_isSpawning = false;

    private int m_elementCount;

    // Start is called before the first frame update
    void Start()
    {
        //Initialisations
        m_enemyObjectPool = GetComponent<ObjectPool>();

        m_playerInventory = PlayerInventory.instance;
        m_elementCount = Elements.GetNames(typeof(Elements)).Length - 1;

        //To stop increment at wave 1
        m_currentHealthIncrease = -m_healthIncrease;
        m_currentMovementSpeedIncrease = -m_movementSpeedIncrease;
        m_currentAttackDamageIncrease = -m_attackDamageIncrease;


    }

    public void InitiateEnemySpawn(int numOfEnemies, int enemiesPerSpawn)
    {
        IncrementBounty();
        UpdateStatsIncrement();
        StartCoroutine(SpawnEnemies(numOfEnemies, enemiesPerSpawn));
    }

    IEnumerator SpawnEnemies(int numOfEnemies, int enemiesPerSpawn)
    {
        m_isSpawning = true;
        int enemiesSpawned = 0;

        while (enemiesSpawned < numOfEnemies)
        {
            m_randomSpawnTime = Random.Range(m_minSpawnTime, m_maxSpawnTime);
            
            yield return new WaitForSeconds(m_randomSpawnTime);

            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                m_randomElement = Random.Range(0, m_elementCount);
                m_randomSpawnIndex = Random.Range(0, m_spawnLocations.Length);

                m_instantiatedEnemy = m_enemyObjectPool.GetAvailableObject();
                m_instantiatedEnemy.transform.position = m_spawnLocations[m_randomSpawnIndex].position;

                m_enemyController = m_instantiatedEnemy.GetComponent<EnemyController>();
                m_enemyController.ChangeElement((Elements)m_randomElement);
                m_enemyController.IncrementStats(m_currentHealthIncrease, m_currentMovementSpeedIncrease, m_currentAttackDamageIncrease);

                m_instantiatedEnemy.SetActive(true);

                enemiesSpawned++;
                m_enemiesLeft++;

                if(enemiesSpawned >= numOfEnemies)
                {
                    break;
                }
            }
        }

        m_isSpawning = false;
    }

    private void UpdateStatsIncrement()
    {
        m_currentHealthIncrease += m_healthIncrease;
        m_currentMovementSpeedIncrease += m_movementSpeedIncrease;
        m_currentAttackDamageIncrease += m_attackDamageIncrease;
    }

    private void IncrementBounty()
    {
        m_maxBounty += m_bountyIncreasePerRound;
        m_minBounty += m_bountyIncreasePerRound;
    }

    //Triggers when an enemy dies
    public void DecreaseEnemyCount()
    {
        m_enemiesLeft--;

        //Handling gold bounty here for efficiency instead of having each enemy handle it
        m_bountyAmount = Random.Range(m_minBounty, m_maxBounty + 1);
        m_playerInventory.GivePlayerGold(m_bountyAmount);
    }
}
