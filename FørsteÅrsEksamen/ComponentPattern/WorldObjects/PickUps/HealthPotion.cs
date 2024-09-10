using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShamansDungeon.GameManagement;

namespace ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

public class HealthPotion: Potion
{
    private Health _health;

    public HealthPotion(GameObject gameObject) : base(gameObject)
    {
    }

    public HealthPotion(GameObject gameObject, GameObject player) : base(gameObject, player)
    {
    }

    public override void Awake()
    {
        base.Awake();

        _health = Player.GameObject.GetComponent<Health>();
    }

    public override void SwitchBetweenSimilarPotions()
    {
        switch (PotionType)
        {
            case PotionTypes.SmallHealth:
                SpriteRenderer.SetSprite(TextureNames.RedSmallHalfPotion);
                AmountToAdd = 50;
                break;
            case PotionTypes.BigHealth:
                SpriteRenderer.SetSprite(TextureNames.RedBigPotionHalfFull);
                AmountToAdd = 100;
                break;
        }

        PotionText = $"Heals for {AmountToAdd}";
    }

    public override void Use()
    {
        if (_health.IsDead || !_health.AddHealth((int)AmountToAdd)) return; // Already full health

        base.Use();
    }
}
