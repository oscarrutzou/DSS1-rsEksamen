using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal class Projectile : Component
    {
        private float speed;
        private Vector2 direction;

        public Projectile(GameObject gameObject) : base(gameObject)
        {
            this.speed = 400;
            SetDirection();
        }

        public void SetDirection()
        {
            direction = (InputHandler.Instance.MouseOnUI) - GameObject.Transform.Position;
            if (direction != Vector2.Zero) direction.Normalize();

            //regner punktet ud mellem x og y aksen
            float angle = (float)Math.Atan2(direction.X, direction.Y);

            // tilføjer π/2 til vinklen så den kan roteres 90 grader med uret
            angle += MathHelper.PiOver2;

            // sikre at vinklen er mellem 0 til 2π
            if (angle < 0) angle += MathHelper.TwoPi;
            else if (angle > MathHelper.TwoPi) angle -= MathHelper.TwoPi;

            GameObject.Transform.Rotation = angle;


        }

        public override void Update(GameTime gameTime)
        {
            Move();
        }

        private void Move()
        {
            GameObject.Transform.Translate(direction * speed * GameWorld.DeltaTime);

            // skal rettes fra skærm til gridsize
            Vector2 screenPosition = Vector2.Transform(GameObject.Transform.Position, GameWorld.Instance.WorldCam.GetMatrix());
            if (screenPosition.X<0 
                || screenPosition.Y <0
                || screenPosition.X > GameWorld.Instance.GfxManager.PreferredBackBufferWidth
                || screenPosition.Y > GameWorld.Instance.GfxManager.PreferredBackBufferHeight)
            {
                GameWorld.Instance.Destroy(GameObject);
            }
        }

        private bool colCalled;
        public override void OnCollisionEnter(Collider collider)
        {
            if (colCalled) return;
            if (collider.GameObject.GetComponent<Enemy>() !=null)
            {
                GameWorld.Instance.Destroy(GameObject);
                colCalled = true;
            }
            

            
        }


    }
}
