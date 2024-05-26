namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons
{
    public class MagicStaff : RangedWeapon
    {
        public MagicStaff(GameObject gameObject) : base(gameObject)
        {
        }

        public MagicStaff(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
        {
        }
    }
}