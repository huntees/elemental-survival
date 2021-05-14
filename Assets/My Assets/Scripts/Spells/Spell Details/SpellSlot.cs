using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpellDetails m_spell;

    [Header("UI Elements")]
    [SerializeField] private Image m_icon;
    [SerializeField] private Image m_spellFrame;
    [SerializeField] private Image m_cooldownImage;
    [SerializeField] private TMP_Text m_cooldownText;

    private GameObject m_cooldownTextObject;
    private float m_currentSpellCooldown;
    private float m_spellCooldownDuration;

    [Header("Spell Frame Sprites")]
    [SerializeField] private Sprite m_activeFrameSprite;
    [SerializeField] private Sprite m_disabledFrameSprite;

    private string m_tooltipText = "";

    private void Start()
    {
        //Initialisations
        m_cooldownTextObject = m_cooldownText.gameObject;

        m_cooldownTextObject.SetActive(false);
        m_cooldownImage.fillAmount = 0.0f;
    }

    private void Update()
    {
        if (m_currentSpellCooldown > 0.0f)
        {
            m_currentSpellCooldown -= Time.deltaTime;
            m_cooldownText.text = Mathf.RoundToInt(m_currentSpellCooldown).ToString();
            m_cooldownImage.fillAmount = m_currentSpellCooldown / m_spellCooldownDuration;
        }
        else if (m_cooldownTextObject.activeInHierarchy)
        {
            m_cooldownTextObject.SetActive(false);
            m_cooldownImage.fillAmount = 0.0f;
        }
    }

    public void UpdateSpellCooldown(float cooldownTimer, float cooldown)
    {

        if (!m_cooldownTextObject.activeInHierarchy && cooldownTimer > 0.0f)
        {
            m_cooldownTextObject.SetActive(true);
        }
        m_currentSpellCooldown = cooldownTimer;
        m_spellCooldownDuration = cooldown;

    }

    public void AddSpell(SpellDetails spell)
    {
        if(m_spell == spell)
        {
            return ;
        }

        m_spell = spell;

        m_icon.sprite = spell.icon;
        m_icon.enabled = true;

        m_spellFrame.sprite = m_activeFrameSprite;

        //Sets tooltip text
        m_tooltipText = m_spell.GetTooltipText();

        Tooltip.instance.HideTooltip();
    }

    public void ClearSpell()
    {
        m_spell = null;

        m_icon.sprite = null;
        m_icon.enabled = false;

        m_spellFrame.sprite = m_disabledFrameSprite;

        Tooltip.instance.HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_spell != null)
        {
            Tooltip.instance.SetTooltipText(m_tooltipText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_spell != null)
        {
            Tooltip.instance.HideTooltip();
        }
    }
}
