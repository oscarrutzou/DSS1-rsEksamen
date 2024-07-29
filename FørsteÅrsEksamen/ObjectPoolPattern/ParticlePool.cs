using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Particles;
using Microsoft.Xna.Framework;
using System;

namespace DoctorsDungeon.ObjectPoolPattern
{
    public class ParticlePool : ObjectPool
    {

        public override GameObject CreateObject(params object[] args)
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Particle;
            go.Transform.Scale = new Vector2(1, 1);

            go.AddComponent<Particle>();
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            

            return go;
        }
    }
}
