using LiteDB;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern.Classes;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public static class DBMethods
    {
        private readonly static List<CollectionName> deleteRunCollections = new() {
            CollectionName.RunData,
            CollectionName.SaveFileHasRunData,
            CollectionName.PlayerData,
            CollectionName.RunDataHasPlayerData,
        };

        public static void SaveGame()
        {
            SaveFileData saveFileData = DBSaveFile.OverrideSaveFileData();
            DBSaveFile.SaveWeaponData(saveFileData);
            DBSaveFile.SaveClassData(saveFileData);
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
                using LiteDatabase db = new(DataBase.GetConnectionString(name));
                db.DropCollection(name.ToString());
            }
        }


        /*
         * Methods
         * update potion on player
         * Rundata
         * Delete ids
         * How to get the save id?
         */





    }
}
