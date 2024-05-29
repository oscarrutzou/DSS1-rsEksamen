namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons
{
    // Erik
    public class Arrow : Projectile
    {
        private Weapon weapon;

        public Arrow(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            //SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>();
            //sr.SetSprite(TextureNames.WoodArrow);
            ////weapon.SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4);
        }
    }
}