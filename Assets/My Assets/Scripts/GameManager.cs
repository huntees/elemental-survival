using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private SpawnManager m_spawnManager;
    [SerializeField] private HUDManager m_HUDManager;
    private PlayerController m_playerController;

    [Header("Properties")]
    [SerializeField] private float m_cooloffPeriod = 60;
    private float m_cooloffTimer = 0;
    [SerializeField] private int m_goldAwardedPerRound = 100;

    [Header("Spawn Manager")]
    [SerializeField] private int m_enemiesSpawnPerWave = 5;
    [SerializeField] private int m_maxAdditionalEnemy = 3;

    private int m_enemiesToSpawn = 0;
    private int m_waveCount = 0;

    //Player Stuff
    private int m_elementCount;
    private int m_randomElement;
    

    // Start is called before the first frame update
    void Start()
    {
        //Initialisation
        m_playerController = PlayerController.instance;

        // -1 because theres "neutral element"
        m_elementCount = Elements.GetNames(typeof(Elements)).Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_spawnManager.m_enemiesLeft <= 0 && !m_spawnManager.m_isSpawning)
        {
            if(m_cooloffTimer <= 0f)
            {
                m_HUDManager.UpdateStatusText("");
                //m_HUDManager.ShowShop(false);
                InitiateNextWave();
                m_cooloffTimer = m_cooloffPeriod;
            }
            else
            {
                m_HUDManager.UpdateStatusText("Next Wave In " + Mathf.Round(m_cooloffTimer) + " Seconds!");

                if(!m_HUDManager.m_isShopActive)
                {
                    m_HUDManager.ShowShop(true);

                    //give player gold on round end
                    PlayerInventory.instance.GivePlayerGold(m_waveCount * m_goldAwardedPerRound);
                }
                m_cooloffTimer -= 1f * Time.deltaTime;
            }
        }
        
        //remove on release
        if(Input.GetKeyDown(KeyCode.Equals))
        {
            RandomiseElementForPlayer();
        }

    }

    private void InitiateNextWave()
    {
        m_waveCount++;
        m_HUDManager.UpdateWaveText(m_waveCount);

        m_enemiesToSpawn += m_enemiesSpawnPerWave + Random.Range(0, m_maxAdditionalEnemy);   
        m_spawnManager.InitiateEnemySpawn(m_enemiesToSpawn);

    }

    private void RandomiseElementForPlayer()
    {
        m_randomElement = Random.Range(0, m_elementCount);
        m_playerController.GiveElement((Elements)m_randomElement);
    }
}
