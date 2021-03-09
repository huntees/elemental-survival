using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager m_spawnManager;
    [SerializeField] private HUDManager m_HUDManager;

    [SerializeField] private float m_cooloffPeriod = 60;
    private float m_cooloffTimer = 0;

    [SerializeField] private int m_enemiesSpawnPerWave = 5;
    [SerializeField] private int m_maxAdditionalEnemy = 3;

    private int m_enemiesToSpawn = 0;
    private int m_waveCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_spawnManager.enemiesLeft <= 0 && !m_spawnManager.isSpawning)
        {
            if(m_cooloffTimer <= 0f)
            {
                m_HUDManager.UpdateStatusText("");
                InitiateNextWave();
                m_cooloffTimer = m_cooloffPeriod;
            }
            else
            {
                m_HUDManager.UpdateStatusText("Next Wave In " + Mathf.Round(m_cooloffTimer) + " Seconds!");
            }

            m_cooloffTimer -= 1f * Time.deltaTime;
        }
        
        

    }

    private void InitiateNextWave()
    {
        m_waveCount++;
        m_HUDManager.UpdateWaveText(m_waveCount);

        m_enemiesToSpawn += m_enemiesSpawnPerWave + Random.Range(0, m_maxAdditionalEnemy);   
        m_spawnManager.InitiateEnemySpawn(m_enemiesToSpawn);

    }
}
