using ShamansDungeon.ComponentPattern;
using System;
using System.Collections.Generic;

namespace ShamansDungeon.ObjectPoolPattern;

// Stefan
public abstract class ObjectPool
{
    public List<GameObject> Active = new List<GameObject>();

    public Stack<GameObject> InActive { get; protected set; } = new Stack<GameObject>();
    public int MaxAmount = 10;

    public virtual GameObject GetObjectAndMake()
    {
        if (Active.Count == MaxAmount)
        {
            // Already reached the maximum number of active objects
            return null;
        }

        GameObject go;

        if (InActive.Count == 0)
        {
            go = CreateObject();

        }
        else
        {
            go = InActive.Pop();
        }

        Active.Add(go);

        return go;
    }


    public virtual GameObject GetObject()
    {
        if (InActive.Count == 0) return null; // There is no more objects to take from

        GameObject go = InActive.Pop();
        Active.Add(go);
        GameWorld.Instance.Instantiate(go);

        return go;
    }

    public virtual GameObject AddObject()
    {
        if ((Active.Count + InActive.Count) < MaxAmount) // Limits the amount of objects created
        {
            return CreateObject();
        }
        return null;
    }

    public virtual void ReleaseObject(GameObject gameObject)
    {
        Active.Remove(gameObject);
        InActive.Push(gameObject);
        GameWorld.Instance.Destroy(gameObject); //Removes gameobject
        CleanUp(gameObject);
    }

    public void ReleaseAllObjects()
    {

    }

    public abstract GameObject CreateObject(params object[] args);

    public virtual void CleanUp(GameObject gameObject) { }
}