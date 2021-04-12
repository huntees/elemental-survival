using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
    public float m_maxHealth = 20;
    [HideInInspector] public float m_currentHealth;
    public float m_maxMana = 100;
    [HideInInspector] public float m_currentMana;
    public float m_attackSpeed = 1f;
    public float m_movementSpeed = 5f;

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

    public PlayerElement fireElement = new PlayerElement(Elements.Fire);
    public PlayerElement waterElement = new PlayerElement(Elements.Water);
    public PlayerElement earthElement = new PlayerElement(Elements.Earth);

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
                EnqueueElement(fireElement);
                break;

            case Elements.Water:
                EnqueueElement(waterElement);
                break;

            case Elements.Earth:
                EnqueueElement(earthElement);
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
                fireElement.elementLevel++;
                break;

            case Elements.Water:
                waterElement.elementLevel++;
                break;

            case Elements.Earth:
                earthElement.elementLevel++;
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
                return fireElement.elementLevel;

            case Elements.Water:
                return waterElement.elementLevel;

            case Elements.Earth:
                return earthElement.elementLevel;

            default:
                return 0;
        }
    }
}
