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

    public class DataBase : IDisposable
    {
        private readonly LiteDatabase db;

        private readonly CollectionName currentCollection;

        public DataBase(CollectionName connectionString)
        {
            currentCollection = connectionString;
            db = new LiteDatabase(GetConnectionString(currentCollection));
        }

        public ILiteCollection<T> GetCollection<T>()
        {
            return db.GetCollection<T>(currentCollection.ToString());
        }

        /// <summary>
        /// Save a single data into the db
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="input"></param>
        /// <returns>Returns a bool on if it has saved the data</returns>
        public BsonValue SaveSingle<T1>(T1 input)
        {
            var collection = GetCollection<T1>();

            return collection.Insert(input);
        }

        public void SaveAll<T>(IEnumerable<T> items)
        {
            var collection = GetCollection<T>();
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
            var collection = GetCollection<T>();

            var existingItem = FindOne(predicate);
            if (existingItem == null)
            {
                return collection.Insert(input);
            }

            return null;
        }

        public void SaveOverrideSingle<T>(T input, BsonValue id, Expression<Func<T, bool>> findPredicate)
        {
            T file = FindOne(findPredicate);

            if (file != null)
            {
                Delete<T>(id);
            }

            SaveSingle(input);
        }

        public void SaveAll<T>(IEnumerable<T> items, Expression<Func<T, bool>> predicate)
        {
            var collection = GetCollection<T>();

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
            var collection = GetCollection<T>();
            return collection.FindOne(predicate);
        }

        public List<T> GetAll<T>()
        {
            var collection = GetCollection<T>();
            var returnData = collection.Query().ToList();
            return returnData;
        }

        public void EnsureIndex<T>(Expression<Func<T, object>> field)
        {
            var collection = GetCollection<T>();
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
            var collection = GetCollection<T>();
            var existingData = collection.FindById(id);
            if (existingData != null)
            {
                updateAction(existingData);
                collection.Update(existingData);
            }
        }

        public void UpdateReplaceData<T>(BsonValue id, T updatedData)
        {
            var collection = GetCollection<T>();
            collection.Update(id, updatedData);
        }

        public void Delete<T>(BsonValue id)
        {
            var collection = GetCollection<T>();
            collection.Delete(id);
        }

        public void DeleteAll<T>()
        {
            var collection = GetCollection<T>();
            collection.DeleteAll();
        }

        public static string GetConnectionString(CollectionName collectionName)
        {
            string pathAppData = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(pathAppData, "data");
            Directory.CreateDirectory(path);

            return Path.Combine(path, $"{collectionName}.db");
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}