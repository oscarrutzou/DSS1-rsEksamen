namespace FørsteÅrsEksamen.ComponentPattern.Weapons.MeleeWeapons
{
    internal class Axe : MeleeWeapon
    {
        public Axe(GameObject gameObject) : base(gameObject)
        {
        }

        public Axe(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            this.enemyWeapon = enemyWeapon;
        }
    }
}