using ShamansDungeon.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

public class DmgBoostPotion : Potion
{
    public DmgBoostPotion(GameObject gameObject) : base(gameObject)
    {
    }

    public DmgBoostPotion(GameObject gameObject, GameObject player) : base(gameObject, player)
    {
    }

    public override void SwitchBetweenSimilarPotions()
    {
        switch (PotionType)
        {
            case PotionTypes.SmallDmgBoost:
                SpriteRenderer.SetSprite(TextureNames.BlueSmallHalfPotion);
                AmountToAdd = 0.2f; 
                break;
        }

        PotionText = $"Permanent {AmountToAdd * 100}% damage boost";
    }

    public override void Use()
    {
        Player.DamageMultiplier += AmountToAdd;
        Player.DamageMultiplier = (float)Math.Round(Player.DamageMultiplier, 2);
        // The amount
        base.Use();
    }
}
