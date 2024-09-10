using System;
using ShamansDungeon.GameManagement;

namespace ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

public class SpeedBoostPotion : Potion
{
    public SpeedBoostPotion(GameObject gameObject) : base(gameObject)
    {
    }

    public SpeedBoostPotion(GameObject gameObject, GameObject player) : base(gameObject, player)
    {
    }

    public override void SwitchBetweenSimilarPotions()
    {
        switch (PotionType)
        {
            case PotionTypes.SmallSpeedBoost:
                SpriteRenderer.SetSprite(TextureNames.GreenSmallHalfPotion);
                AmountToAdd = 0.1f; // 10% perm boost
                break;
        }
        PotionText = $"Permanent {AmountToAdd * 100}% speed boost";
    }

    public override void Use()
    {
        // The amount
        Player.SpeedMultiplier += AmountToAdd;
        base.Use();
    }
}
