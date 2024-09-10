using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.GameManagement;
using Microsoft.Xna.Framework;

namespace ShamansDungeon.Factory;

// Oscar
public static class CellFactory
{
    public static GameObject Create(Grid grid, Point gridPos)
    {
        GameObject cellGo = new()
        {
            Type = GameObjectTypes.Cell
        };

        cellGo.AddComponent<Cell>(grid, gridPos);

        SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
        sr.SetLayerDepth(LayerDepth.Cells);
        sr.SetSprite(TextureNames.Cell);

        return cellGo;
    }

    public static GameObject Create(Grid grid, Point gridPos, CellWalkableType cellType, int collisionNr, int roomNr)
    {
        GameObject cellGo = new()
        {
            Type = GameObjectTypes.Cell
        };

        cellGo.AddComponent<Cell>(grid, gridPos, cellType, collisionNr, roomNr);

        SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
        sr.SetLayerDepth(LayerDepth.Cells);
        sr.SetSprite(TextureNames.Cell);

        return cellGo;
    }
}