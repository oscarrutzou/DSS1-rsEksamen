using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;

namespace FørsteÅrsEksamen.Factory
{
    public class CellFactory : Factory
    {
        public override GameObject Create()
        {
            throw new Exception("Need to get the ");
        }

        public static GameObject Create(Grid grid, Point gridPos)
        {
            GameObject cellGo = new()
            {
                Type = GameObjectTypes.Cell
            };

            cellGo.AddComponent<Cell>(grid, gridPos);

            SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LAYERDEPTH.WorldBackground);
            sr.SetSprite(TextureNames.Cell);

            return cellGo;
        }

        public static GameObject Create(Grid grid, Point gridPos, CellWalkableType cellType, int roomNr)
        {
            GameObject cellGo = new()
            {
                Type = GameObjectTypes.Cell
            };

            cellGo.AddComponent<Cell>(grid, gridPos, cellType, roomNr);

            SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LAYERDEPTH.WorldBackground);
            sr.SetSprite(TextureNames.Cell);

            return cellGo;
        }
    }
}