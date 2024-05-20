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
            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(SaveFileManager.CurrentSaveID);

            SaveFileManager.UnlockedWeapons = DBSaveFile.LoadSaveWeaponType(saveFileData, false);
            SaveFileManager.UnlockedClasses = DBSaveFile.LoadSaveClassType(saveFileData, false);

            UnlockWeapon(WeaponTypes.Axe);
            UnlockWeapon(WeaponTypes.MagicStaffFire);
            UnlockWeapon(WeaponTypes.Bow);

            UnlockClass(ClassTypes.Archer);
            UnlockClass(ClassTypes.Warrior);
            UnlockClass(ClassTypes.Mage);

            SavePlayer(); // Right now the player gets saved multiple times in PlayerData. Same fix as SaveUnlockedClass!!
        }

        public static void DeleteSave()
        {

        }

        public static void SavePlayer()
        {
            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(SaveFileManager.CurrentSaveID);
            RunData runData = DBRunData.RunDataWithSaveFileData(saveFileData);
            PlayerData playerData = DBRunData.SavePlayer(runData);

            // Add player. Also set other variables so this player go becomes the "real player"

            //GameObject playerGo = PlayerFactory.Create(playerData.Class_Type, playerData.Weapon_Type);
            //Player player = playerGo.GetComponent<Player>();
            //player.CurrentHealth = playerData.Health;
            //player.ItemInInventory = ItemFactory.Create(playerGo).GetComponent<PickupableItem>();
            //player.ItemInInventory.Name = playerData.Potion_Name;
            //GameWorld.Instance.Instantiate(playerGo);
        }

        public static void UnlockClass(ClassTypes classType)
        {
            if (SaveFileManager.UnlockedClasses.Contains(classType)) return;

            SaveFileManager.UnlockedClasses.Add(classType);

            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(SaveFileManager.CurrentSaveID);

            SaveFileManager.UnlockedClasses = DBSaveFile.LoadSaveClassType(saveFileData, true);
        }

        public static void UnlockWeapon(WeaponTypes unlockedWeapon)
        {
            // Only add the weapon if it's not already in the list
            if (SaveFileManager.UnlockedWeapons.Contains(unlockedWeapon)) return;

            SaveFileManager.UnlockedWeapons.Add(unlockedWeapon);

            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(SaveFileManager.CurrentSaveID);

            SaveFileManager.UnlockedWeapons = DBSaveFile.LoadSaveWeaponType(saveFileData, true);
        }

        public static void AddCurrency(int amount)
        {
            // Get current amount
            // add it with the amount
            // Update value
        }

        public static void AddRemove(int amount)
        {
            // Get current amount
            // remove it with the amount
            // Update value
        }

        public static void RegularSave()
        {

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
