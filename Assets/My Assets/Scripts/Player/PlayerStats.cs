using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
    public int maxHealth = 20;
    public int currentHealth;
    public int maxMana = 100;
    public int currentMana;
    public float attackSpeed = 1f;
    public float movementSpeed = 5f;

    public struct PlayerElement
    {
        public Elements elementType;
        public int elementLevel;

        public PlayerElement(Elements element)
        {
            elementType = element;
            elementLevel = 0;
        }
    }

    public Queue<PlayerElement> activeElementQueue = new Queue<PlayerElement>();
    public PlayerElement m_primaryElement;
    public PlayerElement m_secondaryElement;

    public PlayerElement fireElement = new PlayerElement(Elements.Fire);
    public PlayerElement waterElement = new PlayerElement(Elements.Water);
    public PlayerElement earthElement = new PlayerElement(Elements.Earth);

    void Awake()
    {
        fireElement.elementLevel++;
        waterElement.elementLevel++;
        earthElement.elementLevel++;
        currentHealth = maxHealth;
        currentMana = maxMana;
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

        if (activeElementQueue.Count >= 2)
        {
            activeElementQueue.Dequeue();
        }

        activeElementQueue.Enqueue(playerElement);


        //Update player elements
        m_primaryElement = playerElement;
        m_secondaryElement = activeElementQueue.Peek();
    }
}
