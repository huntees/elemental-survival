using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Component References")]
    [SerializeField] private SpawnManager m_spawnManager;
    [SerializeField] private HUDManager m_HUDManager;
    [SerializeField] private PauseMenu m_pauseMenu;
    private PlayerController m_playerController;
    private AudioSource m_audioSource;

    [Header("Properties")]
    [SerializeField] private float m_cooloffPeriod = 60;
    private float m_cooloffTimer = 0;
    [SerializeField] private int m_goldAwardedPerRound = 100;
    [SerializeField] private int m_roundToGiveElement = 1;
    [SerializeField] private int m_waveToIncreaseEnemiesPerSpawn = 3;

    [Header("Spawn Manager")]
    [SerializeField] private int m_enemiesSpawnPerWave = 5;
    [SerializeField] private int m_maxAdditionalEnemy = 3;

    private int m_enemiesToSpawn = 0;
    private int m_enemiesPerSpawn = 1;
    private int m_waveCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Initialisation
        m_playerController = PlayerController.instance;
        m_audioSource = GetComponent<AudioSource>();
        m_playerController.triggerGameOver += TriggerGameOver;

        m_HUDManager.action_skipRestingPeriod += SkipRestingPeriod;
        m_playerController.RandomiseElementForPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //If no more enemies left and spawnManager is not spawning
        if (m_spawnManager.m_enemiesLeft <= 0 && !m_spawnManager.m_isSpawning)
        {
            //If resting timer reaches 0, begin next wave
            if(m_cooloffTimer <= 0f)
            {
                m_HUDManager.UpdateStatusText("");
                m_HUDManager.ShowShop(false);
                Tooltip.instance.HideTooltip();

                InitiateNextWave();
                m_cooloffTimer = m_cooloffPeriod;
            }
            //Start counting down resting timer and trigger OnRoundEnd as well as make shop appear
            else
            {
                m_HUDManager.UpdateStatusText("Next Wave In " + Mathf.Round(m_cooloffTimer) + " Seconds!");

                //During Round End
                if(!m_HUDManager.m_isShopActive)
                {
                    m_HUDManager.ShowShop(true);
                    m_audioSource.Play();

                    //Put here for efficiency and piggyback off whether shop is active which signifies round ended
                    OnRoundEnd();
                }

                m_cooloffTimer -= Time.deltaTime;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_pauseMenu.TogglePauseMenu();
        }

        //cheat
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            PlayerInventory.instance.GivePlayerGold(10000);
        }
    }

    private void InitiateNextWave()
    {
        m_waveCount++;
        m_HUDManager.UpdateWaveText(m_waveCount);

        //Spawns a set amount of enemies + a random amount
        m_enemiesToSpawn += m_enemiesSpawnPerWave + Random.Range(0, m_maxAdditionalEnemy);   

        //Increases enemies per spawn every 3rd wave, i.e. spawns faster
        if(m_waveCount % m_waveToIncreaseEnemiesPerSpawn == 0)
        {
            m_enemiesPerSpawn++;
        }

        m_spawnManager.InitiateEnemySpawn(m_enemiesToSpawn, m_enemiesPerSpawn);
    }

    private void OnRoundEnd()
    {
        //give player gold on round end
        PlayerInventory.instance.GivePlayerGold(m_waveCount * m_goldAwardedPerRound);

        //give player a random element on certain round
        if (m_waveCount % m_roundToGiveElement == 0)
        {
            m_playerController.RandomiseElementForPlayer();
        }
    }

    private void SkipRestingPeriod()
    {
        m_cooloffTimer = 0.0f;
    }

    private void TriggerGameOver()
    {
        m_pauseMenu.TriggerGameOver();
    }

}
