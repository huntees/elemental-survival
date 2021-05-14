using UnityEngine;
using TMPro;

public class HUDInventory : MonoBehaviour
{

    private PlayerInventory m_playerInventory;

    [Header("Player Gold Text")]
    [SerializeField] private TextMeshProUGUI m_playerGoldText;

    [Header("Status Message 2")]
    [SerializeField] private TextMeshProUGUI m_inventoryStatusMessage;
    private float m_textTimer = 0;

    [Header("GameObject consisting the inventory slots")]
    [SerializeField] private Transform m_inventoryParent;

    private ItemSlot[] m_itemSlots;

    void Start()
    {
        m_playerInventory = PlayerInventory.instance;

        m_playerInventory.HUD_updateInventory += UpdateItems;
        m_playerInventory.HUD_updatePlayerGold += UpdatePlayerGoldText;
        m_playerInventory.HUD_showInventoryStatusMessage += ShowInventoryStatusMessage;

        m_itemSlots = m_inventoryParent.GetComponentsInChildren<ItemSlot>();
    }

    void Update()
    {
        if(m_textTimer > 0.0f)
        {
            m_textTimer -= Time.deltaTime;

            if(m_textTimer <= 0.0f)
            {
                m_inventoryStatusMessage.text = "";
            }
        }
    }

    private void UpdateItems()
    {
        for (int i = 0; i < m_itemSlots.Length; i++)
        {
            if(i < m_playerInventory.m_items.Count)
            {
                m_itemSlots[i].AddItem(m_playerInventory.m_items[i]);
            } 
            else
            {
                m_itemSlots[i].ClearSlot();
            }
        }
    }

    #region Text Updates

    private void UpdatePlayerGoldText()
    {
        m_playerGoldText.text = m_playerInventory.GetPlayerGold().ToString();
    }

    private void ShowInventoryStatusMessage(string text)
    {
        m_inventoryStatusMessage.text = text;
        m_textTimer = 1f;
    }

    #endregion
}
