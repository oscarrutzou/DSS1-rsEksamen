

using FørsteÅrsEksamen.ComponentPattern.Path;
using LiteDB;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class DataBase
    {
        private const string cellCollectionName = "cells";

        public void Save(CellData cell)
        {
            using var db = new LiteDatabase(GetConnectionString(cellCollectionName));

            var cellCollection = db.GetCollection<CellData>(cellCollectionName);

            cellCollection.Insert(cell);

            cellCollection.EnsureIndex(x => x.Room_Nr);

        }

        public List<CellData> GetAll()
        {
            using var db = new LiteDatabase(GetConnectionString(cellCollectionName));

            var cellCollection = db.GetCollection<CellData>(cellCollectionName);

            var cells = cellCollection.Query().ToList(); //Can use .Where(x => condition)

            return cells;
        }

        private string GetConnectionString(string folderName)
        {
            var pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var path = Path.Combine(pathAppData, "DoctorsDungeon");
            Directory.CreateDirectory(path);

            return Path.Combine(path, $"{folderName}.db");
        }
    }


    /*
 * SaveFile
 * save_id (PK)
 * last_login = Time
 * currency = int
 * 
 * UnlockedClass
 * class_type (PK)
 * 
 * UnlockedWeapon
 * weapon_type (PK)
 * 
 * SaveFileHasUnlockedClass
 * save_id -> SaveFile.save_id (FK)
 * class_id -> UnlockedClass.class_type (FK)
 * 
 * SaveFileHasUnlockedWeapon
 * save_id -> SaveFile.save_id (FK)
 * weapon_id -> UnlockedWeapon.weapon_type (FK)
 * 
 * 
 * 
 * RunData
 * run_id (PK)
 * room_reached = int
 * time_left = float
 * 
 * SaveFileHasRunData
 * save_id -> SaveFile.save_id (FK)
 * run_id -> Rundata.run_id (FK)
 * 
 * 
 * Cell
 * cell_id = int (PK)
 * position = Vec2
 * room_nr = int
 * cell_type = string
 * 
 * Grid
 * grid_name = string (PK)
 * position = Vec2
 * start_size
 * 
 * 
 * RunDataHasGrids
 * run_id -> Rundata.run_id (FK)
 * grid_name -> Grid.grid_name (FK)
 * 
 * GridHasCells
 * grid_name -> Grid.grid_name (FK)
 * cell_id -> Cell.cell_id (FK)
 * 
 */

    public class CellData
    {
        [BsonId]
        public int Cell_ID { get; set; }
        public Point Position { get; set; }
        public int Room_Nr { get; set; }
        public CellWalkableType Cell_Type { get; set; }
    } 
}
