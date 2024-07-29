using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Particles;
using System;

namespace DoctorsDungeon.ObjectPoolPattern
{
    public class ParticlePool : ObjectPool
    {

        public override GameObject CreateObject(params object[] args)
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Particle;
            
            go.AddComponent<Particle>();
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.EnemyOver);

            return go;
        }
    }
}
