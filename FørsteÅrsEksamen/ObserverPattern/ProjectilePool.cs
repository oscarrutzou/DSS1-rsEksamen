using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.Factory;
using DoctorsDungeon.ObjectPoolPattern;

namespace DoctorsDungeon.ObserverPattern
{
    // Stefan
    public class ProjectilePool : ObjectPool
    {
        private static ProjectilePool instance;

        public static ProjectilePool Instance
        { get { return instance ??= new ProjectilePool(); } }

        public override void CleanUp(GameObject gameObject)
        {
        }

        public override GameObject CreateObject()
        {
            return ProjectileFactory.Create();
        }
    }
}