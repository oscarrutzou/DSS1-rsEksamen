using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.GameManagement;
using System.Numerics;

namespace ShamansDungeon.Factory;

//Asser
public static class ItemFactory
{
    public static GameObject Create(GameObject playerGo)
    {
        GameObject itemGo = new GameObject();
        SpriteRenderer sr = itemGo.AddComponent<SpriteRenderer>();
        sr.SetSprite(TextureNames.HealthPotionFull);
        sr.SetLayerDepth(LayerDepth.WorldBackground);
        itemGo.AddComponent<Collider>();
        itemGo.AddComponent<Potion>(playerGo);

        return itemGo;
    }
}