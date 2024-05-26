using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;
using System.Numerics;

namespace DoctorsDungeon.Factory
{
    //Asser
    public static class ItemFactory
    {
        public static GameObject Create(GameObject playerGo)
        {
            GameObject itemGo = new GameObject();
            itemGo.Transform.Scale = new Vector2(4, 4);
            SpriteRenderer sr = itemGo.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.HealthPotionFull);
            sr.SetLayerDepth(LayerDepth.WorldBackground);
            itemGo.AddComponent<Collider>();
            itemGo.AddComponent<Potion>(playerGo);

            return itemGo;
        }
    }
}