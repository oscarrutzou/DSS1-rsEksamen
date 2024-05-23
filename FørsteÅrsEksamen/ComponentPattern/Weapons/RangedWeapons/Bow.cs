namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal class Bow : RangedWeapon
    {
        public Bow(GameObject gameObject) : base(gameObject)
        {
        }

        public Bow(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            this.enemyWeapon = enemyWeapon;
        }


    }
}