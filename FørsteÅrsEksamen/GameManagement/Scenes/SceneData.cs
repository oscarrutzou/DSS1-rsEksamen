using DoctorsDungeon.ComponentPattern;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement.Scenes;

// Oscar
public static class SceneData
{
    /// <summary>
    /// Every GameObject will be in these lists. There is a default type if its not important where the GameObject is placed
    /// </summary>
    public static Dictionary<GameObjectTypes, List<GameObject>> GameObjectLists { get; set; }

    /// <summary>
    /// Generatates lists based on GameObjectTypes Enum
    /// Should only be called once in the GameWorld.
    /// </summary>
    public static void GenereateGameObjectDicionary()
    {
        GameObjectLists = new Dictionary<GameObjectTypes, List<GameObject>>();

        foreach (GameObjectTypes type in Enum.GetValues(typeof(GameObjectTypes)))
        {
            GameObjectLists.Add(type, new List<GameObject>());
        }
    }

    /// <summary>
    /// Clear all the lists of GameObjects
    /// </summary>
    public static void DeleteAllGameObjects()
    {
        foreach (List<GameObject> list in GameObjectLists.Values)
        {
            list.Clear();
        }
    }
}