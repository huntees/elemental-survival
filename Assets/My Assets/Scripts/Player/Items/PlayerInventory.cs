using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{

    #region Singleton 
    public static PlayerInventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("PlayerInventory already has an instance");
            return;
        }

        instance = this;
    }

    #endregion

    private PlayerController m_playerController;

    public List<Item> m_items = new List<Item>();

    [Header("Player Gold")]
    private int m_playerGold = 0;

    [Header("Inventory Space")]
    [SerializeField] private int m_space = 6;

    private bool m_playerHasBoots = false;

    public event Action HUD_updateInventory;
    public event Action HUD_updatePlayerGold;
    public event Action<string> HUD_showInventoryStatusMessage;

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
    }

    public void AddItem(Item item)
    {
        m_items.Add(item);

        //give player stats if there are any
        if (item.hasStats)
        {
            m_playerController.ApplyItemStats(item.movementSpeedAmount, item.attackDamageAmount, item.attackSpeedAmount,
                item.healthAmount, item.manaAmount, item.manaRegenAmount);
        }

        HUD_updateInventory?.Invoke();
    }

    public void SellItem(Item item)
    {
        //If item sold is boots
        if (item.name.Contains("Boots"))
        {
            m_playerHasBoots = false;
        }

        m_items.Remove(item);

        //remove player stats if there are any
        if (item.hasStats)
        {
            m_playerController.RemoveItemStats(item.movementSpeedAmount, item.attackDamageAmount, item.attackSpeedAmount,
                item.healthAmount, item.manaAmount, item.manaRegenAmount);
        }

        //return 70% item cost
        GivePlayerGold((int)(item.cost * 0.7f));

        HUD_updateInventory?.Invoke();
    }

    public void ConsumeItem(Item item)
    {
        m_items.Remove(item);

        //remove player stats if there are any
        if (item.hasStats)
        {
            m_playerController.RemoveItemStats(item.movementSpeedAmount, item.attackDamageAmount, item.attackSpeedAmount,
                item.healthAmount, item.manaAmount, item.manaRegenAmount);
        }

        HUD_updateInventory?.Invoke();
    }

    public void PurchaseItem(Item item)
    {
        //If player does not have enough gold
        if (m_playerGold < item.cost)
        {
            HUD_showInventoryStatusMessage?.Invoke("Insufficient Gold!");
            return;
        }

        //If player does not have enough space
        if (m_items.Count >= m_space)
        {
            HUD_showInventoryStatusMessage?.Invoke("Inventory is full!");
            return;
        }

        //If the item being purchased are boots and player already own one
        if (item.name.Contains("Boots"))
        {
            if (m_playerHasBoots)
            {
                HUD_showInventoryStatusMessage?.Invoke("Already own boots!");
                return;
            }

            m_playerHasBoots = true;
        } 

        AddItem(Instantiate(item));
        TakePlayerGold(item.cost);

    }

    #region Player Gold

    public int GetPlayerGold()
    {
        return m_playerGold;
    }

    public void GivePlayerGold(int amount)
    {
        m_playerGold += amount;
        HUD_updatePlayerGold?.Invoke();
    }

    public void TakePlayerGold(int amount)
    {
        m_playerGold -= amount;
        HUD_updatePlayerGold?.Invoke();
    }

    #endregion
}
