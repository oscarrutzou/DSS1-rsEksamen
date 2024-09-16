using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.WorldObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.Factory
{
    public enum BreakableItemType
    {
        FatVase,
        FatVaseBlue,
        FatVaseRed,
        
        LongVase,
        LongVaseBlue,
        LongVaseRed,

        Barrel,
        Crate,
    }

    public static class BreakableItemFactory
    {
        public static GameObject Create(BreakableItemType item)
        {
            GameObject go = new();
            go.Type = GameObjectTypes.BreakableItems;
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Collider>();
            go.AddComponent<Health>();
            go.AddComponent<BreakableItem>(item);

            GameWorld.Instance.Instantiate(go);
            return go;
        }
    }
}
