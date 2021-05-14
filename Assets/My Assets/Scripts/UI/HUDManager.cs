using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    [SerializeField] private TMP_Text m_healthText;
    [SerializeField] private TMP_Text m_manaText;

    [Header("Elements")]
    [SerializeField] private Image m_primaryElementSlot;
    [SerializeField] private Image m_secondaryElementSlot;
    [SerializeField] private TMP_Text m_fireLevelText;
    [SerializeField] private TMP_Text m_waterLevelText;
    [SerializeField] private TMP_Text m_earthLevelText;
    [SerializeField] private TMP_Text m_natureLevelText;
    [SerializeField] private TMP_Text m_airLevelText;
    [SerializeField] private TMP_Text m_darkLevelText;

    [Header("Element Sprites")]
    [SerializeField] private Sprite m_fireElementSprite;
    [SerializeField] private Sprite m_waterElementSprite;
    [SerializeField] private Sprite m_earthElementSprite;
    [SerializeField] private Sprite m_natureElementSprite;
    [SerializeField] private Sprite m_airElementSprite;

    [Header("Spell")]
    [SerializeField] private SpellSlot m_spellSlot;
    [SerializeField] private SpellDetails m_earthShatterDetails;
    [SerializeField] private SpellDetails m_steamBlastDetails;
    [SerializeField] private SpellDetails m_geyserDetails;
    [SerializeField] private SpellDetails m_innerFireDetails;
    [SerializeField] private SpellDetails m_naturesRejuvenationDetails;
    [SerializeField] private SpellDetails m_livingArmorDetails;
    [SerializeField] private SpellDetails m_meteorStrikeDetails;
    [SerializeField] private SpellDetails m_tornadoDetails;
    [SerializeField] private SpellDetails m_sandStormDetails;
    [SerializeField] private SpellDetails m_overchargeDetails;
    
    [Header("Shop")]
    [SerializeField] private GameObject m_shopObject;
    [HideInInspector] public bool m_isShopActive = false;

    public event Action action_skipRestingPeriod;

    // Start is called before the first frame update
    void Awake()
    {
        m_playerController.HUD_updateHP += UpdateHealth;
        m_playerController.HUD_updateMana += UpdateMana;
        m_playerController.HUD_updateElements += UpdateElements;
        m_playerController.HUD_updateElementTable += UpdateElementTable;
        m_playerController.HUD_updateSpell += UpdateSpell;
        m_playerController.HUD_updateSpellCooldown += UpdateSpellCooldown;

    }

    public void UpdateWaveText(int wave)
    {
        m_waveText.text = "Wave " + wave;
    }

    public void UpdateStatusText(string text)
    {
        m_statusText.text = text;
    }

    public void ShowShop(bool show)
    {
        m_shopObject.SetActive(show);
        m_isShopActive = show;
    }

    public void SkipRestingPeriod()
    {
        action_skipRestingPeriod?.Invoke();
    }

    #region Player Stuff

    private void UpdateHealth(float currentHealth, float maxHealth)
    {
        m_healthSlider.maxValue = maxHealth;
        m_healthSlider.value = currentHealth;
        m_healthText.text = Mathf.RoundToInt(currentHealth).ToString() + " / " + maxHealth.ToString();
    }

    private void UpdateMana(float currentMana, float maxMana)
    {
        m_manaSlider.maxValue = maxMana;
        m_manaSlider.value = currentMana;
        m_manaText.text = Mathf.RoundToInt(currentMana).ToString() + " / " + maxMana.ToString();
    }

    #region Elements
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

            case (Elements.Nature):
                m_natureLevelText.text = level.ToString();
                break;

            case (Elements.Air):
                m_airLevelText.text = level.ToString();
                break;

            default:
                break;
        }
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

            case (Elements.Nature):
                m_primaryElementSlot.sprite = m_natureElementSprite;
                break;

            case (Elements.Air):
                m_primaryElementSlot.sprite = m_airElementSprite;
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

            case (Elements.Nature):
                m_secondaryElementSlot.sprite = m_natureElementSprite;
                break;

            case (Elements.Air):
                m_secondaryElementSlot.sprite = m_airElementSprite;
                break;

            default:
                break;
        }
    }
    #endregion

    #region Spells
    private void UpdateSpell(SpellCode spell)
    {
        switch (spell)
        {
            case SpellCode.EarthShatter:
                m_spellSlot.AddSpell(m_earthShatterDetails);
                break;

            case SpellCode.SteamBlast:
                m_spellSlot.AddSpell(m_steamBlastDetails);
                break;

            case SpellCode.Geyser:
                m_spellSlot.AddSpell(m_geyserDetails);
                break;

            case SpellCode.InnerFire:
                m_spellSlot.AddSpell(m_innerFireDetails);
                break;

            case SpellCode.NaturesRejuvenation:
                m_spellSlot.AddSpell(m_naturesRejuvenationDetails);
                break;

            case SpellCode.LivingArmor:
                m_spellSlot.AddSpell(m_livingArmorDetails);
                break;

            case SpellCode.MeteorStrike:
                m_spellSlot.AddSpell(m_meteorStrikeDetails);
                break;

            case SpellCode.Tornado:
                m_spellSlot.AddSpell(m_tornadoDetails);
                break;

            case SpellCode.SandStorm:
                m_spellSlot.AddSpell(m_sandStormDetails);
                break;

            case SpellCode.Overcharge:
                m_spellSlot.AddSpell(m_overchargeDetails);
                break;

            default:
                m_spellSlot.ClearSpell();
                break;
        }
    }

    private void UpdateSpellCooldown(float cooldownTimer, float cooldown)
    {
        m_spellSlot.UpdateSpellCooldown(cooldownTimer, cooldown);
    }

    #endregion

    #endregion
}
