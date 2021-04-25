using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [Header("Name & Icon")]
    new public string name = "New Item";
    public ItemCode itemCode = ItemCode.NotUsable;
    public Sprite icon = null;

    [Header("Cost")]
    public int cost = 9999;

    [Header("Active")]
    public bool hasActive = false;
    public float cooldownDuration = 0.0f;
    [HideInInspector] public float currentCooldown = 0.0f;
    public string activeDescriptionLine1 = "";
    public string activeDescriptionLine2 = "";

    [Header("Consumable")]
    public bool isConsumable = false;
    public string consumableDescription = "";

    [Header("Stats")]
    public bool hasStats = false;

    public float movementSpeedAmount = 0.0f;

    public float attackDamageAmount = 0.0f;
    public float attackSpeedAmount = 0.0f;

    public float healthAmount = 0.0f;
    public float manaAmount = 0.0f;
    public float manaRegenAmount = 0.0f;

    public float spellAmplificationAmount = 0.0f;

    private string tooltipText = "";

    public string GetTooltipText()
    {
        tooltipText = name + "\n";
        tooltipText += "Cost: " + cost + "\n";

        if (hasActive)
        {
            tooltipText += "\n";

            tooltipText += "Cooldown: " + cooldownDuration + "\n";
            tooltipText += activeDescriptionLine1 + "\n";

            if (activeDescriptionLine2 != "")
            {
                tooltipText += activeDescriptionLine2 + "\n";
            }
        }

        if (isConsumable)
        {
            tooltipText += "\n";

            tooltipText += consumableDescription + "\n";
        }

        if (hasStats)
        {
            tooltipText += "\n";

            tooltipText += GetMovementSpeedAmountText() + GetAttackDamageAmountText() + GetAttackSpeedAmountText() + 
                GetHealthAmountText() + GetManaAmountText() + GetManaRegenAmountText() + GetSpellAmplificationAmountText();
        }

        return tooltipText;
    }

    private string GetMovementSpeedAmountText()
    {
        if (movementSpeedAmount == 0.0f)
        {
            return "";
        }

        return "+" + movementSpeedAmount + " Movement Speed" + "\n";
    }

    private string GetAttackDamageAmountText()
    {
        if (attackDamageAmount == 0.0f)
        {
            return "";
        }

        return "+" + attackDamageAmount + " Damage" + "\n";
    }

    private string GetAttackSpeedAmountText()
    {
        if (attackSpeedAmount == 0.0f)
        {
            return "";
        }

        return "+" + attackSpeedAmount + " Attack Speed" + "\n";
    }

    private string GetHealthAmountText()
    {
        if(healthAmount == 0.0f)
        {
            return "";
        }

        return "+" + healthAmount + " Health" + "\n";
    }

    private string GetManaAmountText()
    {
        if (manaAmount == 0.0f)
        {
            return "";
        }

        return "+" + manaAmount + " Mana" + "\n";
    }

    private string GetManaRegenAmountText()
    {
        if (manaRegenAmount == 0.0f)
        {
            return "";
        }

        return "+" + manaRegenAmount + " Mana Regen" + "\n";
    }

    private string GetSpellAmplificationAmountText()
    {
        if (spellAmplificationAmount == 0.0f)
        {
            return "";
        }

        return "+" + spellAmplificationAmount + " Spell Amplification" + "\n";
    }
}
