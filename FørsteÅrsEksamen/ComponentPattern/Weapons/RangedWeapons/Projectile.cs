using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{

    // Erik
    internal class Projectile : Component
    {
        private float speed;
        private Vector2 direction;
        private float range;
        private Vector2 startPos;
        private Vector2 lerpTo;
        private Vector2 targetPos;


        public Projectile(GameObject gameObject) : base(gameObject)
        {
            this.speed = 100;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>();
            sr.SetLayerDepth(LAYERDEPTH.Player);

            sr.SetSprite(TextureNames.WoodSword);

        }
        

        public void SetValues(float rotation)
        {
            range = 200f;
            GameObject.Transform.Rotation = rotation;
            startPos = GameObject.Transform.Position;
            
            lerpTo = Rotate(startPos + new Vector2(0, range), rotation);
            SetDirection();
        }

        private Vector2 Rotate(Vector2 position, float rotation)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);

            float newX = position.X * cos - position.Y * sin;
            float newY = position.X * sin + position.Y * cos;

            return new Vector2(newX, newY);
        }

        public void SetDirection()
        {
            //direction = (InputHandler.Instance.MouseOnUI) - GameObject.Transform.Position;
            //if (direction != Vector2.Zero) direction.Normalize();

            ////regner punktet ud mellem x og y aksen
            //float angle = (float)Math.Atan2(direction.X, direction.Y);

            //// tilføjer π/2 til vinklen så den kan roteres 90 grader med uret
            //angle += MathHelper.PiOver2;

            //// sikre at vinklen er mellem 0 til 2π
            //if (angle < 0) angle += MathHelper.TwoPi;
            //else if (angle > MathHelper.TwoPi) angle -= MathHelper.TwoPi;

            //GameObject.Transform.Rotation = angle;

            targetPos = InputHandler.Instance.MouseOnUI;

            direction = Vector2.Normalize(targetPos - GameObject.Transform.Position);

            float angle = (float)Math.Atan2(direction.Y, direction.X);
            GameObject.Transform.Rotation = angle;
            


        }

       
        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
        }
        
        private void Move(GameTime gameTime)
        {
            //if (targetPos == Vector2.Zero) return;

            //GameObject.Transform.Position = Vector2.Lerp(startPos, targetPos, GameWorld.DeltaTime * speed);


            //if (Math.Abs(GameObject.Transform.Position.X - targetPos.X) < 0.01f &&
            //    Math.Abs(GameObject.Transform.Position.Y - targetPos.Y) < 0.01f)
            //{
            //    GameWorld.Instance.Destroy(GameObject);

            //}

            float distance = speed * GameWorld.DeltaTime;
            Vector2 step = direction * distance;

            GameObject.Transform.Position += step;



            // skal rettes fra skærm til gridsize
            //Vector2 screenPosition = Vector2.Transform(GameObject.Transform.Position, GameWorld.Instance.WorldCam.GetMatrix());
            //if (screenPosition.X<0 
            //    || screenPosition.Y <0
            //    || screenPosition.X > GameWorld.Instance.GfxManager.PreferredBackBufferWidth
            //    || screenPosition.Y > GameWorld.Instance.GfxManager.PreferredBackBufferHeight)
            //{
            //    GameWorld.Instance.Destroy(GameObject);
            //}5
        }

        private bool colCalled;
        public override void OnCollisionEnter(Collider collider)
        {
            //if (colCalled) return;
            if (collider.GameObject.GetComponent<Enemy>() !=null)
            {
                GameWorld.Instance.Destroy(GameObject);
                //colCalled = true;
            }
            



        }

        public void DestroyRange()
        {
            
        }

        public void Attack()
        {

        }


    }
}
