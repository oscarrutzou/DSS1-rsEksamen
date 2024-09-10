using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;
using ShamansDungeon.GameManagement;
using System.Numerics;

namespace ShamansDungeon.Factory;

//Asser
public static class ItemFactory
{
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