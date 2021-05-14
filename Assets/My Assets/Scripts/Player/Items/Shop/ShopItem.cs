using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header ("Item reference")]
    [SerializeField] private Item m_item;

    [Header ("Properties")]
    [SerializeField] private Image m_icon;

    private string m_tooltipText = "";

    private void Awake()
    {
        m_icon.sprite = m_item.icon;

        m_tooltipText = m_item.GetTooltipText();
    }

    public void PurchaseItem()
    {
        PlayerInventory.instance.PurchaseItem(m_item);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.instance.SetTooltipText(m_tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.HideTooltip();
    }

}
