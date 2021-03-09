using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private PlayerStats m_playerStats;

    //Movement
    private Rigidbody m_rigidbody;
    private Animator m_animator;

    private Vector3 m_Move;

    //Shooting
    [SerializeField] private GameObject m_fireProjectilePrefab;
    [SerializeField] private GameObject m_waterProjectilePrefab;
    [SerializeField] private GameObject m_earthProjectilePrefab;
    [SerializeField] private Transform m_projectileBarrel;
    private float m_nextAttackTime = 0.0f;

    private GameObject m_projectile;
    private GameObject m_primaryProjectile;
    private GameObject m_secondaryProjectile;
    
    //Update HUD HP
    public event Action<int, int> HUD_updateHP;
    public event Action<int, int> HUD_updateMana;

    public event Action<Elements, Elements> HUD_updateElements;

    private Transform myTransform;
    private Vector3 m_mouseRayPosition = new Vector3(0, 0, 0);

    void Start()
    {
        //Remove on release
        Application.targetFrameRate = 145;

        //Initialisation
        m_playerStats = GetComponent<PlayerStats>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();

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

        m_rigidbody.MovePosition(myTransform.position + m_Move * m_playerStats.movementSpeed * Time.deltaTime);

        //Look at mouse cursor **moved to utilities**
        //m_mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(m_mouseRay, out m_mouseRayHit))
        //{
        //    m_mouseRayVector = new Vector3(m_mouseRayHit.point.x, m_mouseRayHit.point.y, m_mouseRayHit.point.z);
        //    myTransform.LookAt(m_mouseRayVector);
        //}

        //Look at mouse cursor
        myTransform.LookAt(m_mouseRayPosition);
        myTransform.rotation = Quaternion.Euler(0f, myTransform.eulerAngles.y, 0f);  //Stops x and z rotation to keep upright

    }

    void Update()
    {
        m_mouseRayPosition = Utils.GetCursorPosition();

        SwitchElementListener();

        if (Time.time >= m_nextAttackTime)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space))
            {
                Shoot(0);
                m_nextAttackTime = Time.time + (1f - (m_playerStats.attackSpeed * 0.01f));
            }
            else if (Input.GetKey(KeyCode.Mouse1))
            {
                Shoot(1);
                m_nextAttackTime = Time.time + (1f - (m_playerStats.attackSpeed * 0.01f));
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            CastSpell();
        }
    }

    private void Shoot(int primaryOrSecondary)
    {
        if(m_primaryProjectile == null || m_secondaryProjectile == null)
        {
            return ;
        }

        if(primaryOrSecondary == 0)
        {
            m_projectile = Instantiate(m_primaryProjectile, m_projectileBarrel.position, Quaternion.identity);
        }
        else if(primaryOrSecondary == 1)
        {
            m_projectile = Instantiate(m_secondaryProjectile, m_projectileBarrel.position, Quaternion.identity);
        }

        m_animator.SetTrigger("Projectile Right Attack 01");

        Vector3 direction = (m_mouseRayPosition - transform.position).normalized;
        direction = new Vector3(direction.x, 0, direction.z);
        m_projectile.GetComponent<ProjectileLogic>().FireProjectile(direction);
    }

    private void UpdateElementProjectile()
    {
        if(m_playerStats.activeElementQueue.Count == 0)
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

    }

    private void CastSpell()
    {
        if (m_playerStats.activeElementQueue.Count == 0)
        {
            return;
        }

        var elementQueue = m_playerStats.activeElementQueue;

        if(elementQueue.Contains(m_playerStats.fireElement) && elementQueue.Contains(m_playerStats.earthElement))
        {
            Debug.Log("Cast Fire + Earth!");
        }
        else if(elementQueue.Contains(m_playerStats.fireElement) && elementQueue.Contains(m_playerStats.waterElement))
        {
            Debug.Log("Cast Fire + Water!");
        }
        else if(elementQueue.Contains(m_playerStats.waterElement) && elementQueue.Contains(m_playerStats.earthElement))
        {
            Debug.Log("Cast Water + Earth!");
        }
    }

    private void SwitchElementListener()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_playerStats.ActivateElement(Elements.Fire);
            UpdateElementProjectile();
            UpdateElementsHUD(m_playerStats.m_primaryElement.elementType, m_playerStats.m_secondaryElement.elementType);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_playerStats.ActivateElement(Elements.Water);
            UpdateElementProjectile();
            UpdateElementsHUD(m_playerStats.m_primaryElement.elementType, m_playerStats.m_secondaryElement.elementType);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_playerStats.ActivateElement(Elements.Earth);
            UpdateElementProjectile();
            UpdateElementsHUD(m_playerStats.m_primaryElement.elementType, m_playerStats.m_secondaryElement.elementType);
        }
    }

    public void TakeDamage(int damage)
    {
        m_animator.SetTrigger("Take Damage");
        m_playerStats.currentHealth -= damage;


        if (m_playerStats.currentHealth < 1)
        {
            m_animator.SetTrigger("Die");
        }

        UpdateHealthHUD();
    }

    private void UpdateHealthHUD()
    {
        HUD_updateHP?.Invoke(m_playerStats.currentHealth, m_playerStats.maxHealth);
    }    
    
    private void UpdateManaHUD()
    {
        HUD_updateMana?.Invoke(m_playerStats.currentMana, m_playerStats.maxMana);
    }

    private void UpdateElementsHUD(Elements primary, Elements secondary)
    {
        HUD_updateElements?.Invoke(primary, secondary);
    }

}