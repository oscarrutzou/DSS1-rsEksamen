namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal class MagicStaff : RangedWeapon
    {
        public MagicStaff(GameObject gameObject) : base(gameObject)
        {
        }

        public MagicStaff(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            this.enemyWeapon = enemyWeapon;
        }
    }
}