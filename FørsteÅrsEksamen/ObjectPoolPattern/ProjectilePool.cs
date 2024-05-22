using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.Factory;

namespace FørsteÅrsEksamen.ObjectPoolPattern
{//Stefan
    internal class ProjectilePool : ObjectPool
    {
        
        private static ProjectilePool instance;

        public static ProjectilePool Instance { get { return instance ??= new ProjectilePool(); } }

        public override void CleanUp(GameObject gameObject)
        {

        }

        public override GameObject CreateObject()
        {
            return ProjectileFactory.Create();
        }
    }
}