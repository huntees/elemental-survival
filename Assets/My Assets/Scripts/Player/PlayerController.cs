using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Singleton 
    public static PlayerController instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("PlayerController already has an instance");
            return;
        }

        instance = this;
        Initialisation();
    }

    #endregion

    private PlayerStats m_playerStats;

    //Movement
    private Rigidbody m_rigidbody;
    private Animator m_animator;

    private Vector3 m_Move;

    //Shooting
    [Header("Projectiles")]
    [SerializeField] private GameObject m_fireProjectilePrefab;
    [SerializeField] private GameObject m_waterProjectilePrefab;
    [SerializeField] private GameObject m_earthProjectilePrefab;
    [SerializeField] private GameObject m_natureProjectilePrefab;
    [SerializeField] private GameObject m_airProjectilePrefab;
    [SerializeField] private Transform m_projectileBarrel;

    private float m_attackTime = 1.0f;
    private float m_nextAttackTime = 0.0f;

    private GameObject m_projectile;
    private ProjectileLogic m_projectileLogic;
    private GameObject m_primaryProjectile;
    private GameObject m_secondaryProjectile;

    private Vector3 direction;

    #region Spell References
    [Header("Spells")]
    //Earth Shatter
    [SerializeField] private GameObject m_earthShatterPrefab;
    [SerializeField] private SpellDetails m_earthShatterDetails;
    private float m_earthShatterCooldownTimer = 0.0f;

    //Steam Blast
    [SerializeField] private GameObject m_steamBlastObject;
    private SteamBlast m_steamBlastComponent;
    [SerializeField] private SpellDetails m_steamBlastDetails;
    private bool m_isSteamBlastPlaying = false;

    //Geyser
    [SerializeField] private GameObject m_geyserPrefab;
    [SerializeField] private SpellDetails m_geyserDetails;
    private float m_geyserCooldownTimer = 0.0f;

    //Inner Fire
    [SerializeField] private GameObject m_innerFireObject;
    private InnerFire m_innerFireComponent;
    [SerializeField] private SpellDetails m_innerFireDetails;
    private float m_innerFireCooldownTimer = 0.0f;

    //Natures Rejuvenation
    [SerializeField] private GameObject m_naturesRejuvenationObject;
    private NaturesRejuvenation m_naturesRejuvenationComponent;
    [SerializeField] private SpellDetails m_naturesRejuvenationDetails;
    private float m_naturesRejuvenationCooldownTimer = 0.0f;

    //Living Armor
    [SerializeField] private GameObject m_livingArmorObject;
    private LivingArmor m_livingArmorComponent;
    [SerializeField] private SpellDetails m_livingArmorDetails;
    private float m_livingArmorCooldownTimer = 0.0f;

    //Meteor Strike
    [SerializeField] private GameObject m_meteorStrikePrefab;
    [SerializeField] private SpellDetails m_meteorStrikeDetails;
    private float m_meteorStrikeCooldownTimer = 0.0f;

    //Tornado
    [SerializeField] private GameObject m_tornadoPrefab;
    [SerializeField] private SpellDetails m_tornadoDetails;
    private float m_tornadoCooldownTimer = 0.0f;

    //Sand Storm
    [SerializeField] private GameObject m_sandStormPrefab;
    [SerializeField] private SpellDetails m_sandStormDetails;
    private float m_sandStormCooldownTimer = 0.0f;

    //Overcharge
    [SerializeField] private GameObject m_overchargeObject;
    private Overcharge m_overchargeComponent;
    [SerializeField] private SpellDetails m_overchargeDetails;
    private float m_overchargeCooldownTimer = 0.0f;

    #endregion

    private SpellCode m_activeSpell = SpellCode.None;
    private Action CastSpell;
    private float m_nextManaRegenTime = 0.0f;

    //Update HUD 
    public event Action<float, float> HUD_updateHP;
    public event Action<float, float> HUD_updateMana;

    public event Action<Elements, Elements> HUD_updateElements;
    public event Action<Elements, int> HUD_updateElementTable;
    public event Action<SpellCode> HUD_updateSpell;
    public event Action<float, float> HUD_updateSpellCooldown;

    private Ray m_mouseRay;
    private RaycastHit m_mouseRayHit;
    private Vector3 m_mouseRayPosition = new Vector3(0, 0, 0);
    private Vector3 m_mouseRayPositionWithoutY = new Vector3(0, 0, 0);

    [Header ("Item Effects")]
    [SerializeField] private ParticleSystem m_restoreHealthParticle;
    [SerializeField] private ParticleSystem m_restoreManaParticle;
    [SerializeField] private ParticleSystem m_shurikenStormParticle;
    [SerializeField] private GameObject m_lichsSpears;

    [Header("Audio")]
    [SerializeField] private AudioClip m_drinkPotionSound;
    [SerializeField] private AudioClip m_blinkDaggerSound;
    [SerializeField] private AudioClip m_forceStaffSound;
    [SerializeField] private AudioClip m_essenceRingSound;
    [SerializeField] private AudioClip m_mendersCharmSound;
    [SerializeField] private AudioClip m_shadowsongsShurikenSound;
    [SerializeField] private AudioClip m_ankhOfImmortalitySound;

    [SerializeField] private AudioClip m_takeDamageSound;
    [SerializeField] private AudioClip m_switchElementSound;
    [SerializeField] private AudioClip m_noManaSound;

    private AudioSource m_audioSource;

    private EventSystem m_eventSystem;

    public event Action triggerGameOver;
    private bool m_isDead = false;
    
    private void Initialisation()
    {
        //Initialisations
        m_eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        m_playerStats = GetComponent<PlayerStats>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();

        m_steamBlastComponent = m_steamBlastObject.gameObject.GetComponent<SteamBlast>();
        m_innerFireComponent = m_innerFireObject.gameObject.GetComponent<InnerFire>();
        m_naturesRejuvenationComponent = m_naturesRejuvenationObject.gameObject.GetComponent<NaturesRejuvenation>();
        m_livingArmorComponent = m_livingArmorObject.gameObject.GetComponent<LivingArmor>();
        m_overchargeComponent = m_overchargeObject.gameObject.GetComponent<Overcharge>();

    }

    void Start()
    {
        //Remove on release
        Application.targetFrameRate = 145;
        GiveElement(Elements.Nature);
        GiveElement(Elements.Fire);
        GiveElement(Elements.Water);
        GiveElement(Elements.Earth);
        GiveElement(Elements.Air);
        PlayerInventory.instance.GivePlayerGold(500000);

        CalculateAttackTime();

        //HUD initialisation
        UpdateHealthHUD();
        UpdateManaHUD();
    }

    void FixedUpdate()
    {
        if (m_isDead)
        {
            //To stop flying behvaiour after death
            transform.LookAt(m_mouseRayPositionWithoutY);
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);  //Stops x and z rotation to keep upright

            return;
        }

        //Movement 
        m_Move = (Input.GetAxisRaw("Vertical") * Vector3.forward + Input.GetAxisRaw("Horizontal") * Vector3.right).normalized;

        m_animator.SetFloat("Move Speed", m_Move.magnitude, 0f, Time.deltaTime);

        m_rigidbody.MovePosition(transform.position + m_Move * m_playerStats.m_movementSpeed * Time.deltaTime);

        //Look at mouse cursor
        transform.LookAt(m_mouseRayPositionWithoutY);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);  //Stops x and z rotation to keep upright
    }

    void Update()
    {
        if (m_isDead)
        {
            return;
        }

        GetCursorPosition();

        SwitchElementListener();

        AttackListener();

        CastSpellListener();

        HandleSpellCooldowns();
    }

    #region Combat

    private void AttackListener()
    {
        if (Time.time >= m_nextAttackTime)
        {
            //makes sure mouse is not over UI
            if (!m_eventSystem.IsPointerOverGameObject())
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Shoot(0);
                    //m_nextAttackTime = Time.time + (1f - (m_playerStats.m_attackSpeed * 0.01f));
                    m_nextAttackTime = Time.time + m_attackTime;
                }
                else if (Input.GetKey(KeyCode.Mouse1))
                {
                    Shoot(1);
                    //m_nextAttackTime = Time.time + (1f - (m_playerStats.m_attackSpeed * 0.01f));
                    m_nextAttackTime = Time.time + m_attackTime;
                }
            }
        }
    }

    private void Shoot(int primaryOrSecondary)
    {
        if (m_primaryProjectile == null || m_secondaryProjectile == null)
        {
            return;
        }

        if (primaryOrSecondary == 0)
        {
            m_projectile = Instantiate(m_primaryProjectile, m_projectileBarrel.position, m_projectileBarrel.rotation);
            m_projectileLogic = m_projectile.GetComponent<ProjectileLogic>();
            m_projectileLogic.SetElementLevel(m_playerStats.m_primaryElement.elementLevel, m_playerStats.m_attackDamage);
        }
        else if (primaryOrSecondary == 1)
        {
            m_projectile = Instantiate(m_secondaryProjectile, m_projectileBarrel.position, m_projectileBarrel.rotation);
            m_projectileLogic = m_projectile.GetComponent<ProjectileLogic>();
            m_projectileLogic.SetElementLevel(m_playerStats.m_secondaryElement.elementLevel, m_playerStats.m_attackDamage);
        }

        m_animator.SetTrigger("Projectile Right Attack 01");
        m_projectileLogic.FireProjectile(GetPlayerDirection());
    }

    private void CastSpellListener()
    {
        //Cast whenever key is held
        if (CastSpell == CastSteamBlast && Input.GetKey(KeyCode.Space))
        {
            CastSpell?.Invoke();
        }
        //Cast only when key is pressed
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            CastSpell?.Invoke();
        }

        if (m_isSteamBlastPlaying && Input.GetKeyUp(KeyCode.Space))
        {
            m_steamBlastComponent.StopSteamBlast();
            m_isSteamBlastPlaying = false;
        }

        if (Time.time >= m_nextManaRegenTime)
        {
            RestoreMana(m_playerStats.m_manaRegen);
            m_nextManaRegenTime = Time.time + 1.0f;
        }
    }

    public void TakeDamage(float damage)
    {
        if(m_isDead)
        {
            return ;
        }

        m_audioSource.PlayOneShot(m_takeDamageSound);
        m_animator.SetTrigger("Take Damage");

        m_playerStats.m_currentHealth -= damage - (damage * m_playerStats.m_damageBlock);
        UpdateHealthHUD();

        if (m_playerStats.m_currentHealth <= 0)
        {
            if(PlayerInventory.instance.UseAnkhOfReincarnation())
            {
                m_audioSource.PlayOneShot(m_ankhOfImmortalitySound);
                m_restoreHealthParticle.Play();
                m_playerStats.m_currentHealth = 10.0f;
                UpdateHealthHUD();

                return ;
            }

            m_animator.SetTrigger("Die");
            //send player died event
            m_isDead = true;
            triggerGameOver?.Invoke();
        }
    }

    #endregion

    #region Spells

    private void CastEarthShatter()
    {
        if (m_earthShatterCooldownTimer <= 0.0f && HasEnoughMana(m_earthShatterDetails.manaCost))
        {
            m_animator.SetTrigger("Projectile Right Attack 01");
            SpendMana(m_earthShatterDetails.manaCost);

            var spell = Instantiate(m_earthShatterPrefab, m_projectileBarrel.position + new Vector3(0, -0.7f, 0), m_projectileBarrel.rotation);
            spell.GetComponentInChildren<EarthShatter>().SetValueIncrease(m_playerStats.m_fireElement.elementLevel, 
                m_playerStats.m_earthElement.elementLevel, m_playerStats.m_spellAmp);

            m_earthShatterCooldownTimer = m_earthShatterDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_earthShatterCooldownTimer, m_earthShatterDetails.cooldownDuration);
        }
    }

    private void CastSteamBlast()
    {
        if(!HasEnoughManaNoSound(m_steamBlastDetails.manaCost))
        {
            if (m_isSteamBlastPlaying)
            {
                m_steamBlastComponent.StopSteamBlast();
                m_isSteamBlastPlaying = false;
            }

            return ;
        }

        if (!m_isSteamBlastPlaying)
        {
            m_steamBlastComponent.PlaySteamBlast();
            m_steamBlastComponent.SetValueIncrease(m_playerStats.m_waterElement.elementLevel, m_playerStats.m_fireElement.elementLevel, m_playerStats.m_spellAmp);
            m_isSteamBlastPlaying = true;
        }

        SpendMana(m_steamBlastDetails.manaCost);
    }

    private void CastGeyser()
    {
        if (m_geyserCooldownTimer <= 0.0f && HasEnoughMana(m_geyserDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_geyserDetails.manaCost);

            var spell = Instantiate(m_geyserPrefab, m_mouseRayPosition + new Vector3(0, 0.1f, 0), Quaternion.identity);
            spell.GetComponentInChildren<Geyser>().SetValueIncrease(m_playerStats.m_waterElement.elementLevel, 
                m_playerStats.m_earthElement.elementLevel, m_playerStats.m_spellAmp);

            m_geyserCooldownTimer = m_geyserDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_geyserCooldownTimer, m_geyserDetails.cooldownDuration);
        }
    }

    private void CastInnerFire()
    {
        if (m_innerFireCooldownTimer <= 0.0f && !m_innerFireComponent.isActiveAndEnabled && HasEnoughMana(m_innerFireDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_innerFireDetails.manaCost);

            m_innerFireComponent.Activate();
            m_innerFireComponent.SetValueIncrease(m_playerStats.m_fireElement.elementLevel, m_playerStats.m_natureElement.elementLevel, m_playerStats.m_spellAmp);
            StartCoroutine(InnerFireCountdown(m_innerFireComponent.GetDuration()));

            m_innerFireCooldownTimer = m_innerFireDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_innerFireCooldownTimer, m_innerFireDetails.cooldownDuration);
        }
    }

    IEnumerator InnerFireCountdown(float seconds)
    {
        m_playerStats.m_attackDamage += m_innerFireComponent.GetDamageAmount();
        yield return new WaitForSeconds(seconds);
        m_innerFireComponent.Deactivate();
        m_playerStats.m_attackDamage -= m_innerFireComponent.GetDamageAmount();
    }

    private void CastNaturesRejuvenation()
    {
        if (m_naturesRejuvenationCooldownTimer <= 0.0f && !m_naturesRejuvenationComponent.isActiveAndEnabled && HasEnoughMana(m_naturesRejuvenationDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_naturesRejuvenationDetails.manaCost);

            m_naturesRejuvenationComponent.SetValueIncrease(m_playerStats.m_waterElement.elementLevel, m_playerStats.m_natureElement.elementLevel, m_playerStats.m_spellAmp);
            m_naturesRejuvenationComponent.Activate();

            m_naturesRejuvenationCooldownTimer = m_naturesRejuvenationDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_naturesRejuvenationCooldownTimer, m_naturesRejuvenationDetails.cooldownDuration);
        }
    }

    private void CastLivingArmor()
    {
        if (m_livingArmorCooldownTimer <= 0.0f && !m_livingArmorComponent.isActiveAndEnabled && HasEnoughMana(m_livingArmorDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_livingArmorDetails.manaCost);

            m_livingArmorComponent.Activate();
            m_livingArmorComponent.SetValueIncrease(m_playerStats.m_earthElement.elementLevel, m_playerStats.m_natureElement.elementLevel, m_playerStats.m_spellAmp);
            StartCoroutine(LivingArmorCountdown(m_livingArmorComponent.GetDuration()));

            m_livingArmorCooldownTimer = m_livingArmorDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_livingArmorCooldownTimer, m_livingArmorDetails.cooldownDuration);
        }
    }

    IEnumerator LivingArmorCountdown(float seconds)
    {
        m_playerStats.m_damageBlock += m_livingArmorComponent.GetDamageBlockAmount();
        yield return new WaitForSeconds(seconds);
        m_livingArmorComponent.Deactivate();
        m_playerStats.m_damageBlock -= m_livingArmorComponent.GetDamageBlockAmount();
    }

    private void CastMeteorStrike()
    {
        if (m_meteorStrikeCooldownTimer <= 0.0f && HasEnoughMana(m_meteorStrikeDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_meteorStrikeDetails.manaCost);

            var spell = Instantiate(m_meteorStrikePrefab, m_mouseRayPosition + new Vector3(0, 0.2f, 0), m_projectileBarrel.rotation);
            spell.GetComponentInChildren<MeteorStrike>().SetValueIncrease(m_playerStats.m_fireElement.elementLevel,
                m_playerStats.m_airElement.elementLevel, m_playerStats.m_spellAmp);

            m_meteorStrikeCooldownTimer = m_meteorStrikeDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_meteorStrikeCooldownTimer, m_meteorStrikeDetails.cooldownDuration);
        }
    }

    private void CastTornado()
    {
        if (m_tornadoCooldownTimer <= 0.0f && HasEnoughMana(m_tornadoDetails.manaCost))
        {
            m_animator.SetTrigger("Projectile Right Attack 01");
            SpendMana(m_tornadoDetails.manaCost);

            var spell = Instantiate(m_tornadoPrefab, m_projectileBarrel.position + new Vector3(0, -0.7f, 0), m_projectileBarrel.rotation);
            spell.GetComponentInChildren<Tornado>().SetValueIncrease(m_playerStats.m_airElement.elementLevel, 
                m_playerStats.m_waterElement.elementLevel, m_playerStats.m_spellAmp);

            m_tornadoCooldownTimer = m_tornadoDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_tornadoCooldownTimer, m_tornadoDetails.cooldownDuration);
        }
    }

    private void CastSandStorm()
    {
        if (m_sandStormCooldownTimer <= 0.0f && HasEnoughMana(m_sandStormDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_sandStormDetails.manaCost);

            var spell = Instantiate(m_sandStormPrefab, m_mouseRayPosition + new Vector3(0, 1f, 0), m_projectileBarrel.rotation);
            spell.GetComponentInChildren<SandStorm>().SetValueIncrease(m_playerStats.m_earthElement.elementLevel, 
                m_playerStats.m_airElement.elementLevel, m_playerStats.m_spellAmp);

            m_sandStormCooldownTimer = m_sandStormDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_sandStormCooldownTimer, m_sandStormDetails.cooldownDuration);
        }
    }

    private void CastOverCharge()
    {
        if (m_overchargeCooldownTimer <= 0.0f && !m_overchargeComponent.isActiveAndEnabled && HasEnoughMana(m_overchargeDetails.manaCost))
        {
            m_animator.SetTrigger("Cast Spell");
            SpendMana(m_overchargeDetails.manaCost);

            m_overchargeComponent.Activate();
            m_overchargeComponent.SetValueIncrease(m_playerStats.m_airElement.elementLevel, m_playerStats.m_natureElement.elementLevel, m_playerStats.m_spellAmp);
            StartCoroutine(OverchargeCountdown(m_overchargeComponent.GetDuration()));

            m_overchargeCooldownTimer = m_overchargeDetails.cooldownDuration;
            UpdateSpellCooldownHUD(m_overchargeCooldownTimer, m_overchargeDetails.cooldownDuration);
        }
    }

    IEnumerator OverchargeCountdown(float seconds)
    {
        m_playerStats.m_attackSpeed += m_overchargeComponent.GetAttackSpeedAmount();
        CalculateAttackTime();
        yield return new WaitForSeconds(seconds);
        m_overchargeComponent.Deactivate();
        m_playerStats.m_attackSpeed -= m_overchargeComponent.GetAttackSpeedAmount();
        CalculateAttackTime();
    }

    private void HandleSpellCooldowns()
    {
        if (m_earthShatterCooldownTimer > 0.0f)
        {
            m_earthShatterCooldownTimer -= Time.deltaTime;
        }

        if (m_geyserCooldownTimer > 0.0f)
        {
            m_geyserCooldownTimer -= Time.deltaTime;
        }

        if (m_innerFireCooldownTimer > 0.0f)
        {
            m_innerFireCooldownTimer -= Time.deltaTime;
        }

        if (m_naturesRejuvenationCooldownTimer > 0.0f)
        {
            m_naturesRejuvenationCooldownTimer -= Time.deltaTime;
        }

        if (m_livingArmorCooldownTimer > 0.0f)
        {
            m_livingArmorCooldownTimer -= Time.deltaTime;
        }

        if (m_meteorStrikeCooldownTimer > 0.0f)
        {
            m_meteorStrikeCooldownTimer -= Time.deltaTime;
        }

        if (m_tornadoCooldownTimer > 0.0f)
        {
            m_tornadoCooldownTimer -= Time.deltaTime;
        }        
        
        if (m_sandStormCooldownTimer > 0.0f)
        {
            m_sandStormCooldownTimer -= Time.deltaTime;
        }

        if (m_overchargeCooldownTimer > 0.0f)
        {
            m_overchargeCooldownTimer -= Time.deltaTime;
        }
    }

    #endregion

    #region Element Switching

    private void UpdateElementSpell()
    {
        if (m_playerStats.m_activeElementQueue.Count == 0)
        {
            return;
        }

        var elementQueue = m_playerStats.m_activeElementQueue;

        if (elementQueue.Contains(m_playerStats.m_fireElement) && elementQueue.Contains(m_playerStats.m_earthElement))
        {
            CastSpell = CastEarthShatter;
            m_activeSpell = SpellCode.EarthShatter;
            UpdateSpellCooldownHUD(m_earthShatterCooldownTimer, m_earthShatterDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_fireElement) && elementQueue.Contains(m_playerStats.m_waterElement))
        {
            CastSpell = CastSteamBlast;
            m_activeSpell = SpellCode.SteamBlast;
            UpdateSpellCooldownHUD(0.0f, 0.0f);
        }
        else if (elementQueue.Contains(m_playerStats.m_waterElement) && elementQueue.Contains(m_playerStats.m_earthElement))
        {
            CastSpell = CastGeyser;
            m_activeSpell = SpellCode.Geyser;
            UpdateSpellCooldownHUD(m_geyserCooldownTimer, m_geyserDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_fireElement) && elementQueue.Contains(m_playerStats.m_natureElement))
        {
            CastSpell = CastInnerFire;
            m_activeSpell = SpellCode.InnerFire;
            UpdateSpellCooldownHUD(m_innerFireCooldownTimer, m_innerFireDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_waterElement) && elementQueue.Contains(m_playerStats.m_natureElement))
        {
            CastSpell = CastNaturesRejuvenation;
            m_activeSpell = SpellCode.NaturesRejuvenation;
            UpdateSpellCooldownHUD(m_naturesRejuvenationCooldownTimer, m_naturesRejuvenationDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_earthElement) && elementQueue.Contains(m_playerStats.m_natureElement))
        {
            CastSpell = CastLivingArmor;
            m_activeSpell = SpellCode.LivingArmor;
            UpdateSpellCooldownHUD(m_livingArmorCooldownTimer, m_livingArmorDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_airElement) && elementQueue.Contains(m_playerStats.m_fireElement))
        {
            CastSpell = CastMeteorStrike;
            m_activeSpell = SpellCode.MeteorStrike;
            UpdateSpellCooldownHUD(m_meteorStrikeCooldownTimer, m_meteorStrikeDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_airElement) && elementQueue.Contains(m_playerStats.m_waterElement))
        {
            CastSpell = CastTornado;
            m_activeSpell = SpellCode.Tornado;
            UpdateSpellCooldownHUD(m_tornadoCooldownTimer, m_tornadoDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_airElement) && elementQueue.Contains(m_playerStats.m_earthElement))
        {
            CastSpell = CastSandStorm;
            m_activeSpell = SpellCode.SandStorm;
            UpdateSpellCooldownHUD(m_sandStormCooldownTimer, m_sandStormDetails.cooldownDuration);
        }
        else if (elementQueue.Contains(m_playerStats.m_airElement) && elementQueue.Contains(m_playerStats.m_natureElement))
        {
            CastSpell = CastOverCharge;
            m_activeSpell = SpellCode.Overcharge;
            UpdateSpellCooldownHUD(m_overchargeCooldownTimer, m_overchargeDetails.cooldownDuration);
        }
        else
        {
            CastSpell = null;
            m_activeSpell = SpellCode.None;
            UpdateSpellCooldownHUD(0.0f, 0.0f);
        }

        //turns off steamblast if it is still being used after switching
        if (m_isSteamBlastPlaying && CastSpell != CastSteamBlast)
        {
            m_steamBlastComponent.StopSteamBlast();
            m_isSteamBlastPlaying = false;
        }

        UpdateSpellHUD(m_activeSpell);
    }

    private void UpdateElementProjectile()
    {
        if (m_playerStats.m_activeElementQueue.Count == 0)
        {
            return;
        }

        m_audioSource.PlayOneShot(m_switchElementSound);

        switch (m_playerStats.m_primaryElement.elementType)
        {
            case Elements.Fire:
                m_primaryProjectile = m_fireProjectilePrefab;
                break;

            case Elements.Water:
                m_primaryProjectile = m_waterProjectilePrefab;
                break;

            case Elements.Earth:
                m_primaryProjectile = m_earthProjectilePrefab;
                break;

            case Elements.Nature:
                m_primaryProjectile = m_natureProjectilePrefab;
                break;

            case Elements.Air:
                m_primaryProjectile = m_airProjectilePrefab;
                break;

            default:
                break;
        }

        switch (m_playerStats.m_secondaryElement.elementType)
        {
            case Elements.Fire:
                m_secondaryProjectile = m_fireProjectilePrefab;
                break;

            case Elements.Water:
                m_secondaryProjectile = m_waterProjectilePrefab;
                break;

            case Elements.Earth:
                m_secondaryProjectile = m_earthProjectilePrefab;
                break;

            case Elements.Nature:
                m_secondaryProjectile = m_natureProjectilePrefab;
                break;

            case Elements.Air:
                m_secondaryProjectile = m_airProjectilePrefab;
                break;

            default:
                break;
        }

        UpdateElementsHUD(m_playerStats.m_primaryElement.elementType, m_playerStats.m_secondaryElement.elementType);
    }

    private void SwitchElementListener()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_playerStats.ActivateElement(Elements.Fire);
            UpdateElementProjectile();
            UpdateElementSpell();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_playerStats.ActivateElement(Elements.Water);
            UpdateElementProjectile();
            UpdateElementSpell();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_playerStats.ActivateElement(Elements.Earth);
            UpdateElementProjectile();
            UpdateElementSpell();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_playerStats.ActivateElement(Elements.Nature);
            UpdateElementProjectile();
            UpdateElementSpell();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            m_playerStats.ActivateElement(Elements.Air);
            UpdateElementProjectile();
            UpdateElementSpell();
        }
    }

    #endregion

    #region Items

    public void UseItem(ItemCode itemCode)
    {
        switch (itemCode)
        {
            //Consumables
            case ItemCode.HealthPotion:
                m_audioSource.PlayOneShot(m_drinkPotionSound);
                m_restoreHealthParticle.Play();
                RestoreHealth(50.0f);
                break;

            case ItemCode.ManaPotion:
                m_audioSource.PlayOneShot(m_drinkPotionSound);
                m_restoreManaParticle.Play();
                RestoreMana(50.0f);
                break;

            case ItemCode.GreaterHealthPotion:
                m_audioSource.PlayOneShot(m_drinkPotionSound);
                m_restoreHealthParticle.Play();
                RestoreHealth(100.0f);
                break;

            case ItemCode.GreaterManaPotion:
                m_audioSource.PlayOneShot(m_drinkPotionSound);
                m_restoreManaParticle.Play();
                RestoreMana(100.0f);
                break;

            case ItemCode.RestorationPotion:
                m_audioSource.PlayOneShot(m_drinkPotionSound);
                m_restoreHealthParticle.Play();
                m_restoreManaParticle.Play();
                RestoreHealth(200.0f);
                RestoreMana(200.0f);
                break;

            case ItemCode.ForceStaff:
                m_audioSource.PlayOneShot(m_forceStaffSound);
                UseForceStaff();
                break;

            case ItemCode.BlinkDagger:
                m_audioSource.PlayOneShot(m_blinkDaggerSound);
                UseBlinkDagger(12.0f);
                break;

            case ItemCode.EssenceRing:
                m_audioSource.PlayOneShot(m_essenceRingSound);
                m_restoreHealthParticle.Play();
                StartCoroutine(UseEssenceRing(100.0f, 10.0f));
                break;

            case ItemCode.MendersCharm:
                m_audioSource.PlayOneShot(m_mendersCharmSound);
                m_restoreHealthParticle.Play();
                RestoreHealth(100.0f);
                break;

            case ItemCode.ShadowsongsShurikens:
                m_audioSource.PlayOneShot(m_shadowsongsShurikenSound);
                m_shurikenStormParticle.Play();
                break;

            case ItemCode.LichsPike:
                UseLichsSpear();
                break;

            default:
                break;
        }
    }

    //---------------------------------------------------------Consumables---------------------------------------------------------
    public void RestoreHealth(float amount)
    {
        m_playerStats.RestoreHealth(amount);
        UpdateHealthHUD();
    }

    public void RestoreMana(float amount)
    {
        m_playerStats.RestoreMana(amount);
        UpdateManaHUD();
    }

    //---------------------------------------------------------Actives--------------------------------------------------------------
    private void UseForceStaff()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.AddForce(GetPlayerDirection() * 15f, ForceMode.Impulse);
        //m_rigidbody.velocity = Vector3.zero;
    }

    private void UseBlinkDagger(float maxDistance)
    {
        Vector3 distance = m_mouseRayPosition - base.transform.position;

        if (distance.magnitude <= maxDistance)
        {
            base.transform.position = m_mouseRayPosition;
        }
        else
        {
            base.transform.Translate((distance.normalized * maxDistance) + Vector3.up * 3.0f, Space.World);
        }

    }

    IEnumerator UseEssenceRing(float health, float seconds)
    {
        m_playerStats.AddHealth(health);
        UpdateHealthHUD();
        yield return new WaitForSeconds(seconds);
        m_playerStats.AddHealth(-health);
        UpdateHealthHUD();
    }

    private void UseLichsSpear()
    {
        Instantiate(m_lichsSpears, m_projectileBarrel.position, m_projectileBarrel.rotation);
    }

    public void ApplyItemStats(float movement, float attackDamage, float attackSpeed, float health, float mana, float manaRegen, float spellAmp)
    {
        m_playerStats.ApplyItemStats(movement, attackDamage, attackSpeed, health, mana, manaRegen, spellAmp);

        CalculateAttackTime();

        UpdateHealthHUD();
        UpdateManaHUD();
    }

    public void RemoveItemStats(float movement, float attackDamage, float attackSpeed, float health, float mana, float manaRegen, float spellAmp)
    {
        m_playerStats.RemoveItemStats(movement, attackDamage, attackSpeed, health, mana, manaRegen, spellAmp);

        CalculateAttackTime();

        UpdateHealthHUD();
        UpdateManaHUD();
    }

    #endregion

    #region HUD Updates

    private void UpdateHealthHUD()
    {
        HUD_updateHP?.Invoke(m_playerStats.m_currentHealth, m_playerStats.m_maxHealth);
    }

    private void UpdateManaHUD()
    {
        HUD_updateMana?.Invoke(m_playerStats.m_currentMana, m_playerStats.m_maxMana);
    }

    private void UpdateElementsHUD(Elements primary, Elements secondary)
    {
        HUD_updateElements?.Invoke(primary, secondary);
    }

    private void UpdateElementTableHUD(Elements primary, int level)
    {
        HUD_updateElementTable?.Invoke(primary, level);
    }

    private void UpdateSpellHUD(SpellCode spell)
    {
        HUD_updateSpell?.Invoke(spell);
    }

    private void UpdateSpellCooldownHUD(float cooldownTimer, float cooldown)
    {
        HUD_updateSpellCooldown?.Invoke(cooldownTimer, cooldown);
    }

    #endregion

    private void GetCursorPosition()
    {
        m_mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(m_mouseRay, out m_mouseRayHit))
        {
            // y omitted to prevent projectile from going up/down and similarly to character rotation
            m_mouseRayPositionWithoutY = new Vector3(m_mouseRayHit.point.x, 0, m_mouseRayHit.point.z);

            // y needed for spell spawning on the right y level
            m_mouseRayPosition = new Vector3(m_mouseRayHit.point.x, m_mouseRayHit.point.y, m_mouseRayHit.point.z);
        }
    }

    private Vector3 GetPlayerDirection()
    {
        direction = (m_mouseRayPositionWithoutY - base.transform.position).normalized;

        return new Vector3(direction.x, 0, direction.z);
    }

    private void CalculateAttackTime()
    {
        m_attackTime = (1f - (m_playerStats.m_attackSpeed / (m_playerStats.m_attackSpeed + 100)));
    }

    public void SpendMana(float manaCost)
    {
        m_playerStats.m_currentMana -= manaCost;
        UpdateManaHUD();
    }

    public bool HasEnoughMana(float amount)
    {
        if(m_playerStats.m_currentMana < amount)
        {
            m_audioSource.PlayOneShot(m_noManaSound);
            return false;
        }

        return true;
    }

    public bool HasEnoughManaNoSound(float amount)
    {
        return m_playerStats.m_currentMana >= amount;
    }

    public void GiveElement(Elements element)
    {
        m_playerStats.GiveElement(element);
        UpdateElementTableHUD(element, m_playerStats.GetElementLevel(element));
    }

    public void RandomiseElementForPlayer()
    {
        GiveElement((Elements)UnityEngine.Random.Range(0, Elements.GetNames(typeof(Elements)).Length - 1));
    }
}