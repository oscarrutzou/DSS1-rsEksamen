using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FørsteÅrsEksamen.ComponentPattern
{
    // Oscar
    public abstract class Component : ICloneable
    {
        public GameObject GameObject { get; private set; }

        public Component(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void OnCollisionEnter(Collider collider)
        {
        }

        public virtual object Clone()
        {
            // Clones componentet and sets the new GameObject
            Component component = (Component)MemberwiseClone();
            component.GameObject = GameObject;
            return component;
        }

        public virtual void SetNewGameObject(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
}