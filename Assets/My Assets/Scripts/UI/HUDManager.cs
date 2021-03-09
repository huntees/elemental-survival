using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header ("Controllers")] 
    [SerializeField] private PlayerController m_playerController;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI m_waveText;
    [SerializeField] private TextMeshProUGUI m_statusText;

    [Header("Health & Mana")]
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private Slider m_manaSlider;

    [Header("Elements")]
    [SerializeField] private Image m_primaryElement;
    [SerializeField] private Image m_secondaryElement;

    [Header("Element Sprites")]
    [SerializeField] private Sprite m_fireElementSprite;
    [SerializeField] private Sprite m_waterElementSprite;
    [SerializeField] private Sprite m_earthElementSprite;


    // Start is called before the first frame update
    void Awake()
    {
        m_playerController.HUD_updateHP += UpdateHealth;
        m_playerController.HUD_updateMana += UpdateMana;
        m_playerController.HUD_updateElements += UpdateElements;
    }

    public void UpdateWaveText(int wave)
    {
        m_waveText.text = "Wave " + wave;
    }

    public void UpdateStatusText(string text)
    {
        m_statusText.text = text;
    }

    private void UpdateHealth(int currentHealth, int maxHealth)
    {
        m_healthSlider.maxValue = maxHealth;
        m_healthSlider.value = currentHealth;   
    }    
    
    private void UpdateMana(int currentMana, int maxMana)
    {
        m_manaSlider.maxValue = maxMana;
        m_manaSlider.value = currentMana;   
    }

    private void UpdateElements(Elements primary, Elements secondary)
    {
        switch (primary)
        {
            case (Elements.Fire):
                m_primaryElement.sprite = m_fireElementSprite;
                break;

            case (Elements.Water):
                m_primaryElement.sprite = m_waterElementSprite;
                break;

            case (Elements.Earth):
                m_primaryElement.sprite = m_earthElementSprite;
                break;

            default:
                break;
        }

        switch(secondary)
        {
            case (Elements.Fire):
                m_secondaryElement.sprite = m_fireElementSprite;
                break;

            case (Elements.Water):
                m_secondaryElement.sprite = m_waterElementSprite;
                break;

            case (Elements.Earth):
                m_secondaryElement.sprite = m_earthElementSprite;
                break;

            default:
                break;
        }
    }
}
