using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform m_projectileBarrel;
    private float m_nextAttackTime = 0.0f;

    private GameObject m_projectile;
    private ProjectileLogic m_projectileLogic;
    private GameObject m_primaryProjectile;
    private GameObject m_secondaryProjectile;

    private Vector3 direction;

    //Spells
    [Header("Spells")]
    [SerializeField] private GameObject m_earthShatterPrefab;
    [SerializeField] private float m_earthShatterCooldown;
    private float m_earthShatterCooldownTimer;

    [SerializeField] private GameObject m_steamBlastPrefab;
    private ParticleSystem m_steamBlastParticle;
    private SteamBlast m_steamBlastComponent;
    private bool m_isSteamBlastPlaying = false;

    [SerializeField] private GameObject m_geyserPrefab;
    [SerializeField] private float m_geyserCooldown;
    private float m_geyserCooldownTimer;


    private string m_activeSpell = "Blank";
    private Action CastSpell;


    //Update HUD 
    public event Action<float, float> HUD_updateHP;
    public event Action<float, float> HUD_updateMana;

    public event Action<Elements, Elements> HUD_updateElements;
    public event Action<Elements, int> HUD_updateElementTable;
    public event Action<string> HUD_updateSpell;
    public event Action<float, float> HUD_updateSpellCooldown;
    public event Action HUD_activateSpellCooldownText;

    private Transform myTransform;

    private Ray m_mouseRay;
    private RaycastHit m_mouseRayHit;
    private Vector3 m_mouseRayPosition = new Vector3(0, 0, 0);
    private Vector3 m_mouseRayPositionWithoutY = new Vector3(0, 0, 0);

    [SerializeField] private EventSystem m_eventSystem;

    private void Initialisation()
    {
        //Initialisation
        m_playerStats = GetComponent<PlayerStats>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        
        m_steamBlastParticle = m_steamBlastPrefab.gameObject.GetComponent<ParticleSystem>();
        m_steamBlastComponent = m_steamBlastPrefab.gameObject.GetComponent<SteamBlast>();
}

    void Start()
    {
        //Remove on release
        Application.targetFrameRate = 145;

        myTransform = transform;

        //HUD initialisation
        UpdateHealthHUD();
        UpdateManaHUD();

    }

    void FixedUpdate()
    {
        //Movement 
        m_Move = (Input.GetAxisRaw("Vertical") * Vector3.forward + Input.GetAxisRaw("Horizontal") * Vector3.right).normalized;

        m_animator.SetFloat("Move Speed", m_Move.magnitude, 0f, Time.deltaTime);

        m_rigidbody.MovePosition(myTransform.position + m_Move * m_playerStats.m_movementSpeed * Time.deltaTime);

        //Look at mouse cursor
        myTransform.LookAt(m_mouseRayPositionWithoutY);
        myTransform.rotation = Quaternion.Euler(0f, myTransform.eulerAngles.y, 0f);  //Stops x and z rotation to keep upright

    }

    void Update()
    {
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
                    m_nextAttackTime = Time.time + (1f - (m_playerStats.m_attackSpeed * 0.01f));
                }
                else if (Input.GetKey(KeyCode.Mouse1))
                {
                    Shoot(1);
                    m_nextAttackTime = Time.time + (1f - (m_playerStats.m_attackSpeed * 0.01f));
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
            m_projectile = Instantiate(m_primaryProjectile, m_projectileBarrel.position, Quaternion.identity);
            m_projectileLogic = m_projectile.GetComponent<ProjectileLogic>();
            m_projectileLogic.SetElementLevel(m_playerStats.m_primaryElement.elementLevel);
        }
        else if (primaryOrSecondary == 1)
        {
            m_projectile = Instantiate(m_secondaryProjectile, m_projectileBarrel.position, Quaternion.identity);
            m_projectileLogic = m_projectile.GetComponent<ProjectileLogic>();
            m_projectileLogic.SetElementLevel(m_playerStats.m_secondaryElement.elementLevel);
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
            m_steamBlastParticle.Stop();
            m_isSteamBlastPlaying = false;
        }
    }

    public void TakeDamage(float damage)
    {
        m_animator.SetTrigger("Take Damage");
        m_playerStats.m_currentHealth -= damage;


        if (m_playerStats.m_currentHealth <= 0)
        {
            m_animator.SetTrigger("Die");
        }

        UpdateHealthHUD();
    }

    #endregion

    #region Spells

    private void CastEarthShatter()
    {
        if (m_earthShatterCooldownTimer <= 0.0f)
        {
            var spell = Instantiate(m_earthShatterPrefab, m_projectileBarrel.position + new Vector3(0, -0.7f, 0), m_projectileBarrel.transform.rotation);
            spell.GetComponentInChildren<EarthShatter>().SetElementLevel(m_playerStats.fireElement.elementLevel, m_playerStats.earthElement.elementLevel);

            m_earthShatterCooldownTimer = m_earthShatterCooldown;
            UpdateSpellCooldownHUD(m_earthShatterCooldownTimer, m_earthShatterCooldown);
        }
    }

    private void CastSteamBlast()
    {
        if (!m_isSteamBlastPlaying)
        {
            m_steamBlastParticle.Play();
            m_steamBlastComponent.SetElementLevel(m_playerStats.fireElement.elementLevel, m_playerStats.waterElement.elementLevel);
            m_isSteamBlastPlaying = true;
        }
    }

    private void CastGeyser()
    {
        if (m_geyserCooldownTimer <= 0.0f)
        {
            var spell = Instantiate(m_geyserPrefab, m_mouseRayPosition + new Vector3(0, 0.1f, 0), Quaternion.identity);
            spell.GetComponentInChildren<Geyser>().SetElementLevel(m_playerStats.waterElement.elementLevel, m_playerStats.earthElement.elementLevel);

            m_geyserCooldownTimer = m_geyserCooldown;
            UpdateSpellCooldownHUD(m_geyserCooldownTimer, m_geyserCooldown);
        }
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

        if (elementQueue.Contains(m_playerStats.fireElement) && elementQueue.Contains(m_playerStats.earthElement))
        {
            CastSpell = CastEarthShatter;
            m_activeSpell = "EarthShatter";
            UpdateSpellCooldownHUD(m_earthShatterCooldownTimer, m_earthShatterCooldown);
        }
        else if (elementQueue.Contains(m_playerStats.fireElement) && elementQueue.Contains(m_playerStats.waterElement))
        {
            CastSpell = CastSteamBlast;
            m_activeSpell = "SteamBlast";
            UpdateSpellCooldownHUD(0.0f, 0.0f);
        }
        else if (elementQueue.Contains(m_playerStats.waterElement) && elementQueue.Contains(m_playerStats.earthElement))
        {
            CastSpell = CastGeyser;
            m_activeSpell = "Geyser";
            UpdateSpellCooldownHUD(m_geyserCooldownTimer, m_geyserCooldown);
        }
        else
        {
            CastSpell = null;
            m_activeSpell = "Blank";
            UpdateSpellCooldownHUD(0.0f, 0.0f);
        }

        //turns off steamblast if it is still being used after switching
        if (m_isSteamBlastPlaying && CastSpell != CastSteamBlast)
        {
            m_steamBlastParticle.Stop();
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

            default:
                break;
        }

        UpdateElementsHUD(m_playerStats.m_primaryElement.elementType, m_playerStats.m_secondaryElement.elementType);
    }

    private void SwitchElementListener()
    {

        if(Input.GetKey(KeyCode.LeftShift))
        {
            return ;
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
    }

    #endregion

    #region Items

    public void UseItem(UsableItemCode usableItemCode)
    {
        switch (usableItemCode)
        {
            case UsableItemCode.Health_Potion: 
                RestoreHealth(50.0f);
                break;

            case UsableItemCode.Mana_Potion:
                RestoreMana(50.0f);
                break;

            case UsableItemCode.Force_Staff: 
                UseForceStaff();
                break;

            default:
                break;
        }
    }

    //---------------------------------------------------------Consumables---------------------------------------------------------
    private void RestoreHealth(float amount)
    {
        m_playerStats.RestoreHealth(amount);
        //play effect on restore

        UpdateHealthHUD();
    }

    private void RestoreMana(float amount)
    {
        m_playerStats.RestoreMana(amount);
        //play effect on restore

        UpdateManaHUD();
    }

    //---------------------------------------------------------Actives--------------------------------------------------------------
    private void UseForceStaff()
    {
        m_rigidbody.AddForce(GetPlayerDirection() * 10f, ForceMode.Impulse);
        //m_rigidbody.velocity = Vector3.zero;
    }

    public void ApplyItemStats(float movement, float attackDamage, float attackSpeed, float health, float mana, float manaRegen)
    {
        m_playerStats.ApplyItemStats(movement, attackDamage, attackSpeed, health, mana, manaRegen);
        UpdateHealthHUD();
        UpdateManaHUD();
    }

    public void RemoveItemStats(float movement, float attackDamage, float attackSpeed, float health, float mana, float manaRegen)
    {
        m_playerStats.RemoveItemStats(movement, attackDamage, attackSpeed, health, mana, manaRegen);
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

    private void UpdateSpellHUD(string spellName)
    {
        HUD_updateSpell?.Invoke(spellName);
    }

    private void UpdateSpellCooldownHUD(float cooldownTimer, float cooldown)
    {
        HUD_updateSpellCooldown?.Invoke(cooldownTimer, cooldown);
    }

    private void ActivateSpellCooldownText()
    {
        HUD_activateSpellCooldownText?.Invoke();
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
        direction = (m_mouseRayPositionWithoutY - transform.position).normalized;
        direction = new Vector3(direction.x, 0, direction.z);

        return direction;
    }

    public void GiveElement(Elements element)
    {
        m_playerStats.GiveElement(element);
        UpdateElementTableHUD(element, m_playerStats.GetElementLevel(element));
    }
}