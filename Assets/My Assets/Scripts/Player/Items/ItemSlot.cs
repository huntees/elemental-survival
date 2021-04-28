using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerController m_playerController;
    private Item m_item;

    [Header("Slot Key")]
    [SerializeField] private KeyCode m_key = KeyCode.Alpha1;

    [Header("UI Elements")]
    [SerializeField] private Image m_icon;
    [SerializeField] private Image m_cooldownImage;
    [SerializeField] private TMP_Text m_cooldownText;

    private GameObject m_cooldownTextObject;
    private float m_currentItemCooldown = 0.0f;
    private float m_itemCooldownDuration;

    private string m_tooltipText = "";

    private void Start()
    {
        //Initialisation
        m_playerController = PlayerController.instance;

        m_cooldownTextObject = m_cooldownText.gameObject;

        m_cooldownTextObject.SetActive(false);
        m_cooldownImage.fillAmount = 0.0f;
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(m_key) && m_item != null)
        {
            UseItem();
        }

        if (m_currentItemCooldown > 0.0f)
        {
            m_currentItemCooldown -= Time.deltaTime;
            m_cooldownText.text = Mathf.RoundToInt(m_currentItemCooldown).ToString();
            m_cooldownImage.fillAmount = m_currentItemCooldown / m_itemCooldownDuration;
            m_item.currentCooldown = m_currentItemCooldown;
        }
        else if (m_cooldownTextObject.activeInHierarchy)
        {
            m_cooldownTextObject.SetActive(false);
            m_cooldownImage.fillAmount = 0.0f;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_item == null) { return ; }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        } 
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SellItem();
        }
    }

    private void UseItem()
    {
        if (m_item.hasActive && m_currentItemCooldown <= 0.0f && m_playerController.HasEnoughMana(m_item.manaCost))
        {
            m_playerController.UseItem(m_item.itemCode);
            m_playerController.SpendMana(m_item.manaCost);

            if (!m_cooldownTextObject.activeInHierarchy)
            {
                m_cooldownTextObject.SetActive(true);
            }

            m_currentItemCooldown = m_itemCooldownDuration;
        }
        else if (m_item.isConsumable)
        {
            m_playerController.UseItem(m_item.itemCode);
            ConsumeItem();
        }
    }

    public void AddItem(Item item)
    {
        m_item = item;

        m_icon.sprite = item.icon;
        m_icon.enabled = true;

        //caches item cooldown duration
        m_itemCooldownDuration = m_item.cooldownDuration;

        //get current cooldown of the item, used when slots are rearranged
        m_currentItemCooldown = m_item.currentCooldown;

        //if the item has an ongoing cooldown, make text active, used when slots are rearranged
        if (m_currentItemCooldown > 0.0f)
        {
            m_cooldownTextObject.SetActive(true);
        }

        //Sets tooltip text
        m_tooltipText = m_item.GetTooltipText();
    }

    public void ClearSlot()
    {
        m_item = null;

        m_icon.sprite = null;
        m_icon.enabled = false;

        m_currentItemCooldown = 0.0f;
    }

    public void SellItem()
    {
        if (m_item != null)
        {
            PlayerInventory.instance.SellItem(m_item);

            Tooltip.instance.HideTooltip();
        }
    }

    public void ConsumeItem()
    {
        if (m_item != null)
        {
            PlayerInventory.instance.ConsumeItem(m_item);

            Tooltip.instance.HideTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_item != null)
        {
            Tooltip.instance.SetTooltipText(m_tooltipText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_item != null)
        {
            Tooltip.instance.HideTooltip();
        }
    }
}
