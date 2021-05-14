using UnityEngine;

//For tooltip
[CreateAssetMenu(fileName = "New Spell Details", menuName = "Spell Details/Spell Detail")]
public class SpellDetails : ScriptableObject
{
    [Header("Name & Icon")]
    new public string name = "New Spell";
    public SpellCode spellCode = SpellCode.None;
    public Sprite icon = null;

    [Header("Mana Cost")]
    public float manaCost = 0;

    [Header("Description")]
    public bool showDamageType = false;
    public Elements damageType = Elements.Neutral;
    public float cooldownDuration = 0.0f;
    public string activeDescriptionLine1 = "";
    public string activeDescriptionLine2 = "";

    public string damageScaleInfoLine1 = "";
    public string damageScaleInfoLine2 = "";

    private string tooltipText = "";

    public string GetTooltipText()
    {
        tooltipText = name + "\n" + "\n";

        if (showDamageType)
        {
            tooltipText += "Damage Type: " + damageType.ToString() + "\n";
        }
        tooltipText += "Cooldown: " + cooldownDuration + "   Mana Cost: " + manaCost + "\n" + "\n";

        tooltipText += activeDescriptionLine1 + "\n";
        tooltipText += activeDescriptionLine2 + "\n" + "\n";

        tooltipText += damageScaleInfoLine1 + "\n";

        if(damageScaleInfoLine2 != "")
        {
            tooltipText += damageScaleInfoLine2 + "\n";
        }

        return tooltipText;
    }
}
