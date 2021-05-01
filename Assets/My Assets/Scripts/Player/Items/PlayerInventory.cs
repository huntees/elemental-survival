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
    private AudioSource m_audioSource;

    public List<Item> m_items = new List<Item>();

    [Header("Player Gold")]
    private int m_playerGold = 0;

    [Header("Inventory Space")]
    [SerializeField] private int m_space = 6;

    [Header("Audio")]
    [SerializeField] private AudioClip m_purchaseSound;
    [SerializeField] private AudioClip m_sellSound;


    private bool m_playerHasBoots = false;
    private Item m_ankhOfImmortality = null;

    public event Action HUD_updateInventory;
    public event Action HUD_updatePlayerGold;
    public event Action<string> HUD_showInventoryStatusMessage;

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_audioSource = GetComponent<AudioSource>();
    }

    public void AddItem(Item item)
    {
        m_audioSource.PlayOneShot(m_purchaseSound, 1.0f);
        m_items.Add(item);

        //give player stats if there are any
        if (item.hasStats)
        {
            m_playerController.ApplyItemStats(item.movementSpeedAmount, item.attackDamageAmount, item.attackSpeedAmount,
                item.healthAmount, item.manaAmount, item.manaRegenAmount, item.spellAmplificationAmount);
        }

        HUD_updateInventory?.Invoke();
    }

    public void SellItem(Item item)
    {
        m_audioSource.PlayOneShot(m_sellSound, 1.0f);
        m_items.Remove(item);

        //remove player stats if there are any
        if (item.hasStats)
        {
            m_playerController.RemoveItemStats(item.movementSpeedAmount, item.attackDamageAmount, item.attackSpeedAmount,
                item.healthAmount, item.manaAmount, item.manaRegenAmount, item.spellAmplificationAmount);
        }

        //If item sold is boots 
        if (item.movementSpeedAmount != 0.0f)
        {
            m_playerHasBoots = false;
        }
        //If item sold is ankh
        if (item.itemCode == ItemCode.AnkhOfImmortality)
        {
            m_ankhOfImmortality = null;
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
                item.healthAmount, item.manaAmount, item.manaRegenAmount, item.spellAmplificationAmount);
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

        //Random element adds on purchase, does not need inventory space
        if (item.itemCode == ItemCode.RandomElement)
        {
            TakePlayerGold(item.cost);
            m_playerController.RandomiseElementForPlayer();

            return;
        }

        //If player does not have enough space
        if (m_items.Count >= m_space)
        {
            HUD_showInventoryStatusMessage?.Invoke("Inventory is full!");
            return;
        }

        //If the item being purchased are boots and player already own one
        if (item.movementSpeedAmount != 0.0f)
        {
            if (m_playerHasBoots)
            {
                HUD_showInventoryStatusMessage?.Invoke("Already own boots!");
                return;
            }

            m_playerHasBoots = true;
        }

        //If player already own Ankh
        if (item.itemCode == ItemCode.AnkhOfImmortality)
        {
            if(m_ankhOfImmortality != null)
            {
                HUD_showInventoryStatusMessage?.Invoke("Already own Ankh Of Reincarnation!");
                return;
            }

            m_ankhOfImmortality = item;

            AddItem(item);
            TakePlayerGold(item.cost);
            return ;
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

    public bool UseAnkhOfReincarnation()
    {
        if(m_ankhOfImmortality == null)
        {
            return false;
        }

        ConsumeItem(m_ankhOfImmortality);
        m_ankhOfImmortality = null;
        return true;
    }
}
