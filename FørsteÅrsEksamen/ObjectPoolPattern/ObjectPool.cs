using DoctorsDungeon.ComponentPattern;
using System.Collections.Generic;

namespace DoctorsDungeon.ObjectPoolPattern
{
    //Stefan
    public abstract class ObjectPool
    {
        public List<GameObject> active = new List<GameObject>();

        public Stack<GameObject> inactive { get; protected set; } = new Stack<GameObject>();
        public int maxAmount = 10;

        public virtual GameObject GetObject()
        {
            if (inactive.Count == 0)
            {
                return CreateObject();
            }
            GameObject go = inactive.Pop();
            active.Add(go);
            return go;
        }

        public virtual void ReleaseObject(GameObject gameObject)
        {
            active.Remove(gameObject);
            inactive.Push(gameObject);
            GameWorld.Instance.Destroy(gameObject); //Removes gameobject
            CleanUp(gameObject);
        }

        public abstract GameObject CreateObject();

        public abstract void CleanUp(GameObject gameObject);
    }
}