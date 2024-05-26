namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons
{
    public class MagicStaffFire : RangedWeapon
    {
        public MagicStaffFire(GameObject gameObject) : base(gameObject)
        {
        }

        public MagicStaffFire(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
        {
        }
    }
}