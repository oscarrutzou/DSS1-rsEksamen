using ShamansDungeon.CommandPattern;
using ShamansDungeon.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.ComponentPattern.Weapons.RangedWeapons;

// Erik
public abstract class RangedWeapon : Weapon
{
    protected RangedWeapon(GameObject gameObject) : base(gameObject)
    {
    }



}   

//private GameObject _projectile;
//private bool _canShoot = true;
//private double _lastShot = 0;
//private double _shootTimer = 0.5f;
//public override void Update()
//{
//    base.Update();
//    _lastShot += GameWorld.DeltaTime;

//    if (_lastShot > _shootTimer)
//    {
//        _canShoot = true;
//    }
//}

//public void MakeProjectile()
//{
//    //ProjectileFactory projectileFactory = new ProjectileFactory();
//    _projectile = ProjectileFactory.Create();
//    _projectile.GetComponent<Projectile>().SetValues(GameObject.Transform.Rotation);

//    _projectile.Transform.Position = GameObject.Transform.Position;

//    GameWorld.Instance.Instantiate(_projectile);
//}

//public void Shoot()
//{
//    //projectile.GetComponent<Projectile>().SetValues(MathHelper.Pi);

//    if (_canShoot)
//    {
//        _canShoot = false;
//        _lastShot = 0;
//        MakeProjectile();
//    }
//}