using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
    [Header ("Stats")]
    public float m_movementSpeed = 5.0f;

    public float m_attackDamage = 0.0f;
    public float m_attackSpeed = 1.0f;

    public float m_maxHealth = 100f;
    [HideInInspector] public float m_currentHealth;
    public float m_maxMana = 100f;
    [HideInInspector] public float m_currentMana;
    public float m_manaRegen = 1.0f;

    public float m_spellAmp = 0.0f;

    public float m_damageBlock = 0.0f;

    public class PlayerElement
    {
        public Elements elementType;
        public int elementLevel;

        public PlayerElement(Elements element)
        {
            elementType = element;
            elementLevel = 0;
        }
    }

    public Queue<PlayerElement> m_activeElementQueue = new Queue<PlayerElement>();
    public PlayerElement m_primaryElement;
    public PlayerElement m_secondaryElement;

    public PlayerElement m_fireElement = new PlayerElement(Elements.Fire);
    public PlayerElement m_waterElement = new PlayerElement(Elements.Water);
    public PlayerElement m_earthElement = new PlayerElement(Elements.Earth);
    public PlayerElement m_natureElement = new PlayerElement(Elements.Nature);
    public PlayerElement m_airElement = new PlayerElement(Elements.Air);

    void Awake()
    {
        m_currentHealth = m_maxHealth;
        m_currentMana = m_maxMana;
    }

    public void ActivateElement(Elements element)
    {
        switch(element)
        {
            case Elements.Fire:
                EnqueueElement(m_fireElement);
                break;

            case Elements.Water:
                EnqueueElement(m_waterElement);
                break;

            case Elements.Earth:
                EnqueueElement(m_earthElement);
                break;

            case Elements.Nature:
                EnqueueElement(m_natureElement);
                break;

            case Elements.Air:
                EnqueueElement(m_airElement);
                break;

            default:
                break;
        }
    }

    private void EnqueueElement(PlayerElement playerElement)
    {
        if (playerElement.elementLevel == 0)
        {
            return ;
        }

        if (m_activeElementQueue.Count >= 2)
        {
            m_activeElementQueue.Dequeue();
        }

        m_activeElementQueue.Enqueue(playerElement);


        //Update player elements
        m_primaryElement = playerElement;
        m_secondaryElement = m_activeElementQueue.Peek();
    }

    public void GiveElement(Elements element)
    {
        switch (element)
        {
            case Elements.Fire:
                m_fireElement.elementLevel++;
                break;

            case Elements.Water:
                m_waterElement.elementLevel++;
                break;

            case Elements.Earth:
                m_earthElement.elementLevel++;
                break;

            case Elements.Nature:
                m_natureElement.elementLevel++;
                break;

            case Elements.Air:
                m_airElement.elementLevel++;
                break;

            default:
                break;
        }
    }

    public int GetElementLevel(Elements element)
    {
        switch (element)
        {
            case Elements.Fire:
                return m_fireElement.elementLevel;

            case Elements.Water:
                return m_waterElement.elementLevel;

            case Elements.Earth:
                return m_earthElement.elementLevel;

            case Elements.Nature:
                return m_natureElement.elementLevel;

            case Elements.Air:
                return m_airElement.elementLevel;

            default:
                return 0;
        }
    }

    public void RestoreHealth(float amount)
    {
        m_currentHealth += amount;

        if (m_currentHealth > m_maxHealth)
        {
            m_currentHealth = m_maxHealth;
        }
    }

    public void RestoreMana(float amount)
    {
        m_currentMana += amount;

        if (m_currentMana > m_maxMana)
        {
            m_currentMana = m_maxMana;
        }
    }

    public bool HaveEnoughMana(float amount)
    {
        return m_currentMana >= amount;
    }

    public void ApplyItemStats(float movement, float attackDamage, float attackSpeed, float health, float mana, float manaRegen, float spellAmp)
    {
        m_movementSpeed += movement;

        m_attackDamage += attackDamage;
        m_attackSpeed += attackSpeed;

        m_currentHealth += m_currentHealth * (health / m_maxHealth);
        m_maxHealth += health;

        m_currentMana += m_currentMana * (mana / m_maxMana);
        m_maxMana += mana;

        m_manaRegen += manaRegen;

        m_spellAmp += spellAmp;
    }

    public void RemoveItemStats(float movement, float attackDamage, float attackSpeed, float health, float mana, float manaRegen, float spellAmp)
    {
        m_movementSpeed -= movement;

        m_attackDamage -= attackDamage;
        m_attackSpeed -= attackSpeed;

        m_currentHealth -= m_currentHealth * (health / m_maxHealth);
        m_maxHealth -= health;

        m_currentMana -= m_currentMana * (mana / m_maxMana);
        m_maxMana -= mana;

        m_manaRegen -= manaRegen;

        m_spellAmp -= spellAmp;
    }
}
