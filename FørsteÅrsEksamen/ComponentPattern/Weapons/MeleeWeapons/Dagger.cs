using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

public class Dagger : MeleeWeapon
{
    public Dagger(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        AttackSoundNames = new SoundNames[]
        {
            SoundNames.SwipeFast1,
            SoundNames.SwipeFast2,
        };

        Animations = new()
        {
            { WeaponAnimTypes.Light, new WeaponAnimation(0.85f, MathHelper.PiOver4 * 3, 15, BaseMath.EaseOutQuart, WeaponAnimTypes.Medium, 2)},
            { WeaponAnimTypes.Medium, new WeaponAnimation(1.2f, MathHelper.PiOver4 * 6, 30, BaseMath.EaseInOutQuint, WeaponAnimTypes.Light)},
        };

        CurrentAnim = WeaponAnimTypes.Light;

        if (EnemyUser != null)
        {
            EnemyUser.CellPlayerMoveBeforeNewTarget = 2;
        }
        
        base.Start();
    }

    protected override void PlayerWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.WoodDagger);
        SetStartColliders(new Vector2(7.5f, 21), 5, 5, 2, 3); // Gets set in each of the weapons insted of here.
    }

    protected override void EnemyWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneDagger);
        SetStartColliders(new Vector2(7.5f, 21), 5, 5, 2, 3); // Gets set in each of the weapons insted of here.
    }
}