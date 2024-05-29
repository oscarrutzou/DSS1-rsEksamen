﻿using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.Factory
{
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
            sr.SetLayerDepth(LayerDepth.WorldBackground);
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
            sr.SetLayerDepth(LayerDepth.WorldBackground);
            sr.SetSprite(TextureNames.Cell);

            return cellGo;
        }
    }
}