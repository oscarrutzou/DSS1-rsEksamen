using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{
    public class TestRotateComponent : Component
    {
        public TestRotateComponent(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();
            //sprite = GlobalTextures.Textures[TextureNames.WoodArrow];
            GameObject.Transform.Scale = new Vector2(10, 10);
        }

        public override void Update()
        {
            base.Update();

            GameObject.Transform.Rotation += (float)GameWorld.DeltaTime;
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    // Draw center
        //    Vector2 spriteOrigin = Vector2.Zero;
        //    int width, height;

        //    //width = sprite.Width * (int)GameObject.Transform.Scale.X;
        //    //height = sprite.Height * (int)GameObject.Transform.Scale.Y;
        //    //width = sprite.Width / 2;
        //    //height = sprite.Height / 2;

        //    //spriteOrigin = new Vector2(width, height);

        //    //spriteBatch.Draw(sprite, Vector2.Zero, null, Color.White, GameObject.Transform.Rotation, spriteOrigin, GameObject.Transform.Scale, SpriteEffects.None, 1f);
        //}
    }
}
