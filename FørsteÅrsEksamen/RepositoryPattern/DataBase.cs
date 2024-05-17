using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

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


        public ILiteCollection<T> GetCollection<T>(CollectionName collectionName)
        {
            return db.GetCollection<T>(collectionName.ToString());
        }

        /// <summary>
        /// Save a single data into the db
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="input"></param>
        /// <returns>Returns a bool on if it has saved the data</returns>
        public BsonValue SaveSingle<T1>(T1 input)
        {
            var collection = GetCollection<T1>(currentCollection);

            return collection.Insert(input);
        }

        public void SaveAll<T>(IEnumerable<T> items)
        {
            var collection = GetCollection<T>(currentCollection);
            collection.InsertBulk(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <returns>Returns a bool on if it has saved the data</returns>
        public BsonValue SaveSingle<T>(T input, Expression<Func<T, bool>> predicate)
        {
            var collection = GetCollection<T>(currentCollection);

            var existingItem = FindOne(predicate);
            if (existingItem == null)
            {
                return collection.Insert(input);
            }

            return null;
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

        public void Delete<T>(BsonValue id)
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
    }
}
