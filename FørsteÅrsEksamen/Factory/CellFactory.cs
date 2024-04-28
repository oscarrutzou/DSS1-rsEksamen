using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Grid;
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

            if ((gridPos.X + gridPos.Y) % 2 == 0) sr.Color = new Color(30, 150, 20); // Set color so every second one is colored

            return cellGo;
        }

        public static GameObject Create(Grid grid, Point gridPos, CellWalkableType cellType)
        {
            GameObject cellGo = new()
            {
                Type = GameObjectTypes.Cell
            };

            cellGo.AddComponent<Cell>(grid, gridPos, cellType);

            SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LAYERDEPTH.WorldBackground);
            sr.SetSprite(TextureNames.Cell);

            if ((gridPos.X + gridPos.Y) % 2 == 0) sr.Color = new Color(30, 150, 20); // Set color so every second one is colored

            return cellGo;
        }
    }
}