namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    public class Bow : RangedWeapon
    {
        public Bow(GameObject gameObject) : base(gameObject)
        {
        }

        public Bow(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
        {
        }


    }
}