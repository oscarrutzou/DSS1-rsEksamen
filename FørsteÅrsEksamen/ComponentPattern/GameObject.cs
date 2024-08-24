using DoctorsDungeon.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.ComponentPattern;

/// <summary>
/// <para>Uses the type to make different lists in the SceneData.</para>
/// <para>Only make a new type, if its a object type that there is a lot of, and we need to get references to them e.g for collsion.</para>
/// </summary>
public enum GameObjectTypes
{
    Background,
    Cell,
    Player,
    Enemy,
    Weapon,
    Projectile,
    Gui,
    Items,
    Particle,
    Emitter,
    Default, //Not set
}

// Oscar
public class GameObject : ICloneable
{
    private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();
    public Transform Transform { get; private set; } = new Transform();

    public GameObjectTypes Type { get; set; } = GameObjectTypes.Default;
    public bool IsEnabled { get; set; } = true;
    public List<Effect> Effects { get; set;} = new List<Effect>(); // Can use a initial Capacity, and then use TrimExess.
    /// <summary>
    /// Adds a component to the GameObject, with a empty contructer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T AddComponent<T>() where T : Component
    {
        Type componentType = typeof(T);
        // Finds the contructers, and returns the empty contructer.
        var constructor = componentType.GetConstructors().FirstOrDefault(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(GameObject);
        });

        if (constructor != null)
        {
            // Makes a component and adds "this" gameobject to the params.
            T component = (T)Activator.CreateInstance(componentType, this);
            _components[componentType] = component; // Adds the component to the GameObject
            return component;
        }
        else
        {
            throw new InvalidOperationException($"The class {componentType.Name} must have a constructor with one parameter of type GameObject.");
        }
    }

    /// <summary>
    /// <para>Adds a component to the GameObject. The GameObject shouldnt be in here too, only the params after in the contructer.</para>
    /// <para>Remember there still need to be a empty contructer</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="additionalParams"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public T AddComponent<T>(params object[] additionalParams) where T : Component
    {
        Type componentType = typeof(T);
        try
        {
            object[] allParams = new object[1 + additionalParams.Length]; // Generates a array with the correct params lenght.
            allParams[0] = this; // Since the base first param is the parent gameobject, we can set the first param to this Gameobject.
            Array.Copy(additionalParams, 0, allParams, 1, additionalParams.Length); // Copies extra params into the array.

            T component = (T)Activator.CreateInstance(componentType, allParams); // Creates a component with the correct params
            _components[componentType] = component; // Adds the component to the GameObject
            return component;
        }
        catch (Exception)
        {
            throw new Exception($"The class {componentType.Name} does not have a constructor that matches the provided parameters.");
        }
    }

    /// <summary>
    /// <para>Can get the component from the GameObject. Also works with calling Superclasses</para>
    /// <para>When used in other scripts, remember to first call this in the Awake or Start, so it dosent return null</para>
    /// </summary>
    /// <typeparam name="T">The specific component class or a superclass of the specific component</typeparam>
    /// <returns></returns>
    public T GetComponent<T>() where T : Component
    {
        Type componentType = typeof(T);

        // First check if the component is in the GameObject
        if (_components.TryGetValue(componentType, out Component component))
        {
            return (T)component;
        }

        // Make a check to see if "T" is a subclass of one of the components.
        foreach (Component componentVal in _components.Values)
        {
            if (componentType.IsAssignableFrom(componentVal.GetType()))
            {
                return (T)componentVal;
            }
        }

        // Cant find the class T
        return null;
    }

    public void Awake()
    {
        foreach (var component in _components.Values)
        {
            component.Awake();
        }
    }

    public void Start()
    {
        foreach (var component in _components.Values)
        {
            component.Start();
        }
    }

    public void Update()
    {
        if (!IsEnabled) return;
        foreach (var component in _components.Values)
        {
            component.Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsEnabled) return;
        foreach (var component in _components.Values)
        {
            component.Draw(spriteBatch);
        }
    }

    public void OnCollisionEnter(Collider collider)
    {
        if (!IsEnabled) return;
        foreach (var component in _components.Values)
        {
            component.OnCollisionEnter(collider);
        }
    }

    private Component AddComponentWithExistingValues(Component component)
    {
        _components[component.GetType()] = component;
        return component;
    }

    /// <summary>
    /// How we can check on each of the gameobjects what they should collide with.
    /// </summary>
    /// <param name="gameobjectType"></param>
    /// <returns></returns>
    public bool CollidesWithGameObject(GameObjectTypes gameobjectType)
    {
        if (!IsEnabled) throw new Exception("The current gameobject should be enabled");

        Collider thisGoCollider = GetComponent<Collider>() ?? throw new Exception("This Gameobject need a collider to check for collision");

        foreach (GameObject otherGo in SceneData.GameObjectLists[gameobjectType])
        {
            if (!otherGo.IsEnabled) continue;

            Collider otherCollider = otherGo.GetComponent<Collider>();

            if (otherCollider == null) continue;

            if (thisGoCollider.CollisionBox.Intersects(otherCollider.CollisionBox))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Clones the current gameObject, with the existing values of the component.
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        GameObject go = new GameObject();
        go.Transform = Transform; // Sets the transform to be the same
        go.Type = Type;
        foreach (Component component in _components.Values)
        {
            Component newComponent = go.AddComponentWithExistingValues(component.Clone() as Component);
            newComponent.SetNewGameObject(go);
        }
        return go;
    }

    /// <summary>
    /// Need a SpriteRenderer to work, and sets the layer though that component.
    /// </summary>
    /// <param name="layerDepth"></param>
    /// <exception cref="Exception"></exception>
    public void SetLayerDepth(LayerDepth layerDepth)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.SetLayerDepth(layerDepth);
            return;
        }

        throw new Exception($"You need to add a SpriteRenderer to set the layerdepth");
    }
}