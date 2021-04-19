using UnityEngine;
using TMPro;

/* This was implemented by following CodeMonkey's tutorial https://www.youtube.com/watch?v=d_qk7egZ8_c
 * Some modifications were made throughout.
 */

public class Tooltip : MonoBehaviour
{

    #region Singleton 
    public static Tooltip instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Tooltip already has an instance");
            return;
        }

        instance = this;
    }

    #endregion

    [SerializeField] private RectTransform m_canvasRectTransform;
    [SerializeField] private RectTransform m_parentTooltipObject;
    [SerializeField] private RectTransform m_backgroundRectTransform;
    [SerializeField] private TextMeshProUGUI m_tooltipText;

    private Vector2 m_anchoredPostion;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        m_anchoredPostion = Input.mousePosition / m_canvasRectTransform.localScale.x;

        if(m_anchoredPostion.x + m_backgroundRectTransform.rect.width > m_canvasRectTransform.rect.width)
        {
            m_anchoredPostion.x = m_canvasRectTransform.rect.width - m_backgroundRectTransform.rect.width;
        }
        if(m_anchoredPostion.y + m_backgroundRectTransform.rect.height > m_canvasRectTransform.rect.height)
        {
            m_anchoredPostion.y = m_canvasRectTransform.rect.height - m_backgroundRectTransform.rect.height;
        }

        m_parentTooltipObject.anchoredPosition = m_anchoredPostion;
    }

    public void SetTooltipText(string text)
    {
        gameObject.SetActive(true);

        m_tooltipText.text = text;
        m_tooltipText.ForceMeshUpdate();

        m_backgroundRectTransform.sizeDelta = m_tooltipText.GetRenderedValues(false) + new Vector2(8, 8);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
