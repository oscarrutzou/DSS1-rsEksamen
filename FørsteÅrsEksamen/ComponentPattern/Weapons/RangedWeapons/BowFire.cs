namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons
{
    public class BowFire : RangedWeapon
    {
        public BowFire(GameObject gameObject) : base(gameObject)
        {
        }

        public BowFire(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
        {
        }
    }
}