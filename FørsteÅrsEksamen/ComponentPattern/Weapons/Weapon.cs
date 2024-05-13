using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons
{
    internal abstract class Weapon : Component
    {
       public int damage;
       public int range;
       public int attackSpeed;
       public string weaponName;
       public float velocity, targetVelocity;
       
        
        private Texture2D sprite;
        private float weaponRotation;

        protected Weapon(GameObject gameObject) : base(gameObject)
        {
            attackSpeed = 10; 
            
        }

        public void Create()
        {
           

            
        }

        public void Move(float rotation)
        {
            targetVelocity += rotation;

            //this.velocity = float.Lerp(this.velocity, targetVelocity, attackSpeed * GameWorld.DeltaTime);
            GameObject.Transform.Rotation = this.velocity * attackSpeed * GameWorld.DeltaTime; 
           
            
        }

        public void SetSprite(String weaponName)
        {
            sprite = GlobalTextures.Textures[TextureNames.Cell];

        }

        public void Attack()
        {
            //GameObject.Transform.Rotation += 30f;


        }





        

     
     

            

    }
}
