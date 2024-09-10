using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ObserverPattern;
using System;
using System.Collections.Generic;

namespace ShamansDungeon.GameManagement.Scenes;

// Oscar
public class SceneData : ILayerDepthObserver
{
    private static SceneData instance;
    public static SceneData Instance
    { get { return instance ??= instance = new SceneData(); } }   
   
    /// <summary>
    /// Every GameObject will be in these lists. There is a default type if its not important where the GameObject is placed
    /// </summary>
    public Dictionary<GameObjectTypes, List<GameObject>> GameObjectLists { get; set; }
    public Dictionary<ColliderLayer, List<GameObject>> ColliderMeshLists { get; set; }
    private SortedList<float, List<SpriteRenderer>> _sortedGameObjects = new SortedList<float, List<SpriteRenderer>>();

    /// <summary>
    /// Generatates lists based on GameObjectTypes Enum
    /// Should only be called once in the GameWorld.
    /// </summary>
    public void GenereateGameObjectDicionary()
    {
        GameObjectLists = new();
        foreach (GameObjectTypes type in Enum.GetValues(typeof(GameObjectTypes)))
        {
            GameObjectLists.Add(type, new List<GameObject>());
        }

        ColliderMeshLists = new();
        foreach (ColliderLayer type in Enum.GetValues(typeof(ColliderLayer)))
        {
            ColliderMeshLists.Add(type, new List<GameObject>());
        }
    }

    /// <summary>
    /// Clear all the lists of GameObjects
    /// </summary>
    public void DeleteAllGameObjects()
    {
        foreach (List<GameObject> list in GameObjectLists.Values)
        {
            list.Clear();
        }

        foreach (List<GameObject> list in ColliderMeshLists.Values)
        {
            list.Clear();
        }
        _sortedGameObjects.Clear();
    }


    public void AddGameObject(GameObject gameObject)
    {
        if (gameObject.Type == GameObjectTypes.Gui) return;

        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        sr.AddObserver(this);
        AddToSortedList(sr);
    }

    public void OnLayerDepthChanged(SpriteRenderer spriteRenderer)
    {
        // Remove from old depth list
        if (_sortedGameObjects.TryGetValue(spriteRenderer.OldLayerDepth, out var oldList))
        {
            oldList.Remove(spriteRenderer);
            if (oldList.Count == 0)
            {
                _sortedGameObjects.Remove(spriteRenderer.OldLayerDepth);
            }
        }

        // Add to new depth list
        AddToSortedList(spriteRenderer);

        //spriteRenderer.OldLayerDepth = spriteRenderer.LayerDepth;
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        if (gameObject.Type == GameObjectTypes.Gui) return; // Not in the list of layer depths
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return; // Already not in the list

        if (_sortedGameObjects.TryGetValue(spriteRenderer.LayerDepth, out var list))
        {
            list.Remove(spriteRenderer);
            if (list.Count == 0)
            {
                _sortedGameObjects.Remove(spriteRenderer.LayerDepth);
            }
        }
    }

    private void AddToSortedList(SpriteRenderer spriteRenderer)
    {
        if (!_sortedGameObjects.ContainsKey(spriteRenderer.LayerDepth))
        {
            _sortedGameObjects[spriteRenderer.LayerDepth] = new List<SpriteRenderer>();
        }
        _sortedGameObjects[spriteRenderer.LayerDepth].Add(spriteRenderer);
    }

    public IEnumerable<SpriteRenderer> GetSortedGameObjects()
    {
        foreach (var kvp in _sortedGameObjects)
        {
            foreach (var gameObject in kvp.Value)
            {
                yield return gameObject;
            }
        }
    }
}