namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal class BowFire : RangedWeapon
    {
        public BowFire(GameObject gameObject) : base(gameObject)
        {
        }

        public BowFire(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            this.enemyWeapon = enemyWeapon;
        }
    }
}