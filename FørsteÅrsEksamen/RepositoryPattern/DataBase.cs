

using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using LiteDB;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public enum CollectionName
    {
        SaveFile,
        UnlockedClass,
        UnlockedWeapon,
        SaveFileHasClass,
        SaveFileHasWeapon,
        RunData,
        SaveFileHasRunData,
        PlayerData,
        RunDataHasPlayerData,
        Grids,
        Cells,
        GridHasCells,
    }

    public class DataBase: IDisposable
    {
        public readonly static Dictionary<CollectionName, Type> CollectionTypeMap = new Dictionary<CollectionName, Type>
        {
            { CollectionName.Cells, typeof(CellData) },
            { CollectionName.Grids, typeof(GridData) },
            { CollectionName.SaveFile, typeof(SaveFile) },
            { CollectionName.UnlockedClass, typeof(UnlockedClass) },
            { CollectionName.UnlockedWeapon, typeof(UnclockedWeapon) },
            { CollectionName.GridHasCells, typeof(GridHasCells) },
            { CollectionName.SaveFileHasClass, typeof(SaveFileHasUnlockedClass) },
            { CollectionName.SaveFileHasWeapon, typeof(SaveFileHasUnclockedWeapon) },
            { CollectionName.RunData, typeof(RunData) },
            { CollectionName.SaveFileHasRunData, typeof(SaveFileHasRunData) },
            { CollectionName.PlayerData, typeof(PlayerData) },
            { CollectionName.RunDataHasPlayerData, typeof(RunDataHasPlayerData) },
        };

        private readonly static List<CollectionName> deleteRunCollections = new() {
            CollectionName.RunData,
            CollectionName.SaveFileHasRunData,
            CollectionName.PlayerData,
            CollectionName.RunDataHasPlayerData,
        };

        private readonly LiteDatabase db;

        private readonly CollectionName currentCollection;

        public DataBase(CollectionName connectionString)
        {
            currentCollection = connectionString;
            db = new LiteDatabase(GetConnectionString(currentCollection));
        }


        /// <summary>
        /// A method that deletes all data related to each run.
        /// </summary>
        public static void DeleteRunData()
        {
            // This is ineffecient since we open and close connections,
            // but since we are just dropping the collection, we are fine with some of the code being less effient.
            // This metod will also only be called very rarely, so it wont make a difference, if we optimize this method.
            foreach (CollectionName name in deleteRunCollections)
            {
                using LiteDatabase db = new (GetConnectionString(name));
                db.DropCollection(name.ToString());
            }
        }

        #region Grid
        public GameObject GetGrid(string description)
        {
            GameObject gridGo = new();

            //Get grid with same 

            return gridGo;
        }
        #endregion

        #region Generic Methods
        public ILiteCollection<T> GetCollection<T>(CollectionName collectionName)
        {
            return db.GetCollection<T>(collectionName.ToString());
        }

        public void SaveSingle<T1>(T1 input)
        {
            var collection = GetCollection<T1>(currentCollection);
            
            collection.Insert(input);
        }

        public void SaveAll<T>(IEnumerable<T> items)
        {
            var collection = GetCollection<T>(currentCollection);
            collection.InsertBulk(items);
        }

        public void SaveSingle<T>(T input, Expression<Func<T, bool>> predicate)
        {
            var collection = GetCollection<T>(currentCollection);

            var existingItem = FindOne(predicate);
            if (existingItem == null)
            {
                collection.Insert(input);
            }
        }

        public void SaveAll<T>(IEnumerable<T> items, Expression<Func<T, bool>> predicate)
        {
            var collection = GetCollection<T>(currentCollection);

            foreach (var item in items)
            {
                var existingItem = FindOne(predicate);
                if (existingItem == null)
                {
                    collection.Insert(item);
                }
            }
        }


        public T FindOne<T>(Expression<Func<T, bool>> predicate)
        {
            var collection = GetCollection<T>(currentCollection);
            return collection.FindOne(predicate);
        }

        public List<T> GetAll<T>()
        {
            var collection = GetCollection<T>(currentCollection);
            var returnData = collection.Query().ToList();
            return returnData;
        }

        public void EnsureIndex<T>(Expression<Func<T, object>> field)
        {
            var collection = GetCollection<T>(currentCollection);
            collection.EnsureIndex(field);
        }

        /// <summary>
        /// Updates a single value if the id is in the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">Id of the saved</param>
        /// <param name="updateAction">Use lamda expression to set new value (cell => cell.SomeProperty = newValue) </param>
        public void UpdateSingleValue<T>(BsonValue id, Action<T> updateAction)
        {
            var collection = GetCollection<T>(currentCollection);
            var existingData = collection.FindById(id);
            if (existingData != null)
            {
                updateAction(existingData);
                collection.Update(existingData);
            }
        }

        public void UpdateReplaceData<T>(BsonValue id, T updatedData)
        {
            var collection = GetCollection<T>(currentCollection);
            collection.Update(id, updatedData);
        }

        public void Delete<T>(CollectionName collectionName, BsonValue id)
        {
            var collection = GetCollection<T>(currentCollection);
            collection.Delete(id);
        }

        public void DeleteAll<T>()
        {
            var collection = GetCollection<T>(currentCollection);
            collection.DeleteAll();
        }

        private static string GetConnectionString(CollectionName collectionName)
        {
            var pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var path = Path.Combine(pathAppData, "DoctorsDungeon");
            Directory.CreateDirectory(path);

            return Path.Combine(path, $"{collectionName}.db");
        }

        public void Dispose()
        {
            db?.Dispose();
        }
        #endregion
    }

    #region Data Classes
    public class SaveFile
    {
        [BsonId]
        public int Save_ID { get; set; }
        public DateTime Last_Login { get; set; }
        public int Currency { get; set; }
    }

    public class UnlockedClass
    {
        [BsonId]
        public ClassTypes Class_Type { get; set; }
    }
    public class SaveFileHasUnlockedClass
    {
        public int Save_ID { get; set; }
        public ClassTypes Class_Type { get; set; }
    }
    public class UnclockedWeapon
    {
        [BsonId]
        public WeaponTypes Weapon_Type { get; set; }
    }
    public class SaveFileHasUnclockedWeapon
    {
        public int Save_ID { get; set; }
        public WeaponTypes Weapon_Type { get; set; }
    }

    public class SaveFileHasRunData
    {
        public int Save_ID { get; set; }
        public string Run_ID { get; set; }
    }

    public class RunData
    {
        [BsonId]
        public string Run_ID { get; set; }
        public int Room_Reached { get; set; }
        public float Time_Left { get; set; }
    }

    public class RunDataHasPlayerData
    {
        public string Run_ID { get; set; }
        public int Player_ID { get; set; }
    }

    public class PlayerData
    {
        [BsonId]
        public int Player_ID { get; set; }
        public int Health { get; set; }
        public string Potion_Name { get; set; }
        public ClassTypes Class_Type { get; set; }
        public WeaponTypes Weapon_Type { get; set; }
    }

    public class GridData
    {
        [BsonId]
        public string Grid_Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Start_SizeX { get; set; }
        public int Start_SizeY { get; set; }
    }

    public class GridHasCells
    {
        public string Grid_Name { get; set; }
        public int Cell_ID { get; set; }
    }

    public class CellData
    {
        [BsonId]
        public int Cell_ID { get; set; }
        public int PointPositionX { get; set; }
        public int PointPositionY { get; set; }
        public int Room_Nr { get; set; }
        public CellWalkableType Cell_Type { get; set; }
    }
    #endregion
}
