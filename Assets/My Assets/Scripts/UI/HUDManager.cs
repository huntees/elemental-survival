using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private PlayerController m_playerController;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI m_waveText;
    [SerializeField] private TextMeshProUGUI m_statusText;

    [Header("Health & Mana")]
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private Slider m_manaSlider;

    [Header("Elements")]
    [SerializeField] private Image m_primaryElementSlot;
    [SerializeField] private Image m_secondaryElementSlot;
    [SerializeField] private TMP_Text m_fireLevelText;
    [SerializeField] private TMP_Text m_waterLevelText;
    [SerializeField] private TMP_Text m_earthLevelText;
    [SerializeField] private TMP_Text m_natureLevelText;
    [SerializeField] private TMP_Text m_lightLevelText;
    [SerializeField] private TMP_Text m_darkLevelText;

    [Header("Element Sprites")]
    [SerializeField] private Sprite m_fireElementSprite;
    [SerializeField] private Sprite m_waterElementSprite;
    [SerializeField] private Sprite m_earthElementSprite;

    [Header("Spell")]
    [SerializeField] private Image m_spellSlot;

    [Header("Spell Sprites")]
    [SerializeField] private Sprite m_emptySprite;
    [SerializeField] private Sprite m_earthShatterSprite;
    [SerializeField] private Sprite m_steamBlastSprite;
    [SerializeField] private Sprite m_geyserSprite;

    [Header("Spell Frame")]
    [SerializeField] private Image m_spellFrame;
    [SerializeField] private Image m_cooldownImage;
    [SerializeField] private TMP_Text m_cooldownText;
    private GameObject m_cooldownTextObject;
    private float m_spellCooldownText;
    private float m_spellCooldown;

    [Header("Spell Frame Sprites")]
    [SerializeField] private Sprite m_activeFrameSprite;
    [SerializeField] private Sprite m_disabledFrameSprite;

    // Start is called before the first frame update
    void Awake()
    {
        m_playerController.HUD_updateHP += UpdateHealth;
        m_playerController.HUD_updateMana += UpdateMana;
        m_playerController.HUD_updateElements += UpdateElements;
        m_playerController.HUD_updateElementTable += UpdateElementTable;
        m_playerController.HUD_updateSpell += UpdateSpell;
        m_playerController.HUD_updateSpellCooldown += UpdateSpellCooldown;
        m_playerController.HUD_activateSpellCooldownText += ActivateSpellCooldownText;

        m_cooldownTextObject = m_cooldownText.gameObject;
    }

    void Update()
    {
        if(m_spellCooldownText > 0)
        {
            m_spellCooldownText -= Time.deltaTime;
            m_cooldownText.text = Mathf.RoundToInt(m_spellCooldownText).ToString();
            m_cooldownImage.fillAmount = m_spellCooldownText / m_spellCooldown;
        }
        else if(m_cooldownTextObject.activeInHierarchy)
        {
            m_cooldownTextObject.SetActive(false);
            m_cooldownImage.fillAmount = 0.0f;
        }
    }

    public void UpdateWaveText(int wave)
    {
        m_waveText.text = "Wave " + wave;
    }

    public void UpdateStatusText(string text)
    {
        m_statusText.text = text;
    }

    #region Player Stuff
    private void UpdateElementTable(Elements element, int level)
    {
        switch (element)
        {
            case (Elements.Fire):
                m_fireLevelText.text = level.ToString();
                break;

            case (Elements.Water):
                m_waterLevelText.text = level.ToString();
                break;

            case (Elements.Earth):
                m_earthLevelText.text = level.ToString();
                break;

            default:
                break;
        }
    }


    private void UpdateHealth(float currentHealth, float maxHealth)
    {
        m_healthSlider.maxValue = maxHealth;
        m_healthSlider.value = currentHealth;
    }

    private void UpdateMana(float currentMana, float maxMana)
    {
        m_manaSlider.maxValue = maxMana;
        m_manaSlider.value = currentMana;
    }

    private void UpdateElements(Elements primary, Elements secondary)
    {
        switch (primary)
        {
            case (Elements.Fire):
                m_primaryElementSlot.sprite = m_fireElementSprite;
                break;

            case (Elements.Water):
                m_primaryElementSlot.sprite = m_waterElementSprite;
                break;

            case (Elements.Earth):
                m_primaryElementSlot.sprite = m_earthElementSprite;
                break;

            default:
                break;
        }

        switch (secondary)
        {
            case (Elements.Fire):
                m_secondaryElementSlot.sprite = m_fireElementSprite;
                break;

            case (Elements.Water):
                m_secondaryElementSlot.sprite = m_waterElementSprite;
                break;

            case (Elements.Earth):
                m_secondaryElementSlot.sprite = m_earthElementSprite;
                break;

            default:
                break;
        }
    }

    private void UpdateSpell(string spellName)
    {
        switch (spellName)
        {
            case ("EarthShatter"):
                m_spellSlot.sprite = m_earthShatterSprite;
                m_spellFrame.sprite = m_activeFrameSprite;
                break;

            case ("SteamBlast"):
                m_spellSlot.sprite = m_steamBlastSprite;
                m_spellFrame.sprite = m_activeFrameSprite;
                break;

            case ("Geyser"):
                m_spellSlot.sprite = m_geyserSprite;
                m_spellFrame.sprite = m_activeFrameSprite;
                break;

            default:
                m_spellSlot.sprite = m_emptySprite;
                m_spellFrame.sprite = m_disabledFrameSprite;
                break;
        }
    }

    private void UpdateSpellCooldown(float cooldownTimer, float cooldown)
    {

        if (!m_cooldownTextObject.activeInHierarchy && cooldownTimer > 0.0f)
        {
          m_cooldownTextObject.SetActive(true);
        }
        m_spellCooldownText = cooldownTimer;
        m_spellCooldown = cooldown;

    }

    private void ActivateSpellCooldownText()
    {
        m_cooldownTextObject.SetActive(true);
    }

    #endregion
}
