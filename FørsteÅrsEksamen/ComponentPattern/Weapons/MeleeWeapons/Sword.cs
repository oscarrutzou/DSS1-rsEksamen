using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

// Erik
public class Sword : MeleeWeapon
{
    public Sword(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        AttackSoundNames = new SoundNames[]
        {
            SoundNames.SwipeSlow1,
        };

        Animations = new()
        {
            { WeaponAnimTypes.Light, new WeaponAnimation(1.5f, MathHelper.Pi, 25, BaseMath.EaseInOutBack, WeaponAnimTypes.Light)},
        };

        CurrentAnim = WeaponAnimTypes.Light;

        MinimumTimeBetweenHits = 0.3f;


        base.Start();
    }

    protected override void PlayerWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.WoodSword);
        SetStartColliders(new Vector2(7.5f, 38), 5, 5, 0, 6); // Gets set in each of the weapons insted of here.
    }

    protected override void EnemyWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneSword);
        SetStartColliders(new Vector2(7.5f, 38), 5, 5, 0, 5); // Gets set in each of the weapons insted of here.
    }
}