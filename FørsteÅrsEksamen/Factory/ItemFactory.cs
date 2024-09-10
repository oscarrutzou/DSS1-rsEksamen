using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;
using ShamansDungeon.GameManagement;
using System.Collections.Generic;
using System;
using System.Numerics;
using ShamansDungeon.ComponentPattern.PlayerClasses;

namespace ShamansDungeon.Factory;

//Asser
public static class ItemFactory
{
    private static readonly Random _random = new();

    public static GameObject CreatePotionWithRandomType(GameObject playerGo, List<PotionTypes> spawnableTypes)
    {
        PotionTypes randomType = spawnableTypes[_random.Next(0, spawnableTypes.Count)];

        return CreatePotion(playerGo, randomType);
    }

    public static GameObject CreatePotion(GameObject playerGo, PotionTypes type)
    {
        GameObject itemGo = new GameObject();
        itemGo.Type = GameObjectTypes.Items;
        SpriteRenderer sr = itemGo.AddComponent<SpriteRenderer>();
        sr.SetLayerDepth(LayerDepth.WorldBackground);
        itemGo.AddComponent<Collider>();
        AddPotionScript(ref itemGo, playerGo, type);

        return itemGo;
    }

    private static void AddPotionScript(ref GameObject itemGo, GameObject playerGo, PotionTypes type)
    {
        Potion potion = null;
        switch (type)
        {
            case PotionTypes.SmallHealth:
            case PotionTypes.BigHealth:
                 potion = itemGo.AddComponent<HealthPotion>(playerGo);
                break;
            case PotionTypes.SmallDmgBoost:
                potion = itemGo.AddComponent<DmgBoostPotion>(playerGo);
                break;
            case PotionTypes.SmallSpeedBoost:
                 potion = itemGo.AddComponent<SpeedBoostPotion>(playerGo);
                break;
        }

        if (potion != null)
            potion.PotionType = type;
    }
}