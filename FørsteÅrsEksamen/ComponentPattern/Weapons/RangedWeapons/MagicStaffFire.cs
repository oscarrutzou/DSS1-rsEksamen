namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal class MagicStaffFire : RangedWeapon
    {
        public MagicStaffFire(GameObject gameObject) : base(gameObject)
        {
        }

        public MagicStaffFire(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            this.enemyWeapon = enemyWeapon;
        }
    }
}