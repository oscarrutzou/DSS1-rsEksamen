using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using LiteDB;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.DB
{
    public static class DBMethods
    {
        private static readonly List<CollectionName> deleteRunCollections = new() {
            CollectionName.RunData,
            CollectionName.SaveFileHasRunData,
            CollectionName.PlayerData,
            CollectionName.RunDataHasPlayerData,
        };

        //public static void SaveGame()
        //{
        //    SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(Data.CurrentSaveID, false);

        //    Data.UnlockedWeapons = DBSaveFile.LoadSaveWeaponType(saveFileData, false);
        //    Data.UnlockedClasses = DBSaveFile.LoadSaveClassType(saveFileData, false);

        //    // In buy menu.
        //    //UnlockWeapon(WeaponTypes.Axe);
        //    //UnlockWeapon(WeaponTypes.MagicStaffFire);
        //    //UnlockWeapon(WeaponTypes.Bow);

        //    //UnlockClass(ClassTypes.Archer);
        //    //UnlockClass(ClassTypes.Warrior);
        //    //UnlockClass(ClassTypes.Mage);

        //    SaveRunData();
        //}

        public static void SaveRunData()
        {
            SaveFileData saveFileData = DBSaveFile.LoadFileData(Data.CurrentSaveID); // Load Save File
            RunData runData = DBRunData.SaveLoadRunData(saveFileData); // Save Run File
            
            if (Data.Player == null) return;
            
            DBRunData.SavePlayer(runData);
        }

        /// <summary>
        /// Needs to be called in the Initialize after spawning a grid
        /// </summary>
        /// <param name="playerData"></param>
        public static GameObject SpawnPlayer(PlayerData playerData, Point spawnPos)
        {
            // Makes a new Player
            GameObject playerGo = PlayerFactory.Create(playerData.Class_Type, playerData.Weapon_Type);

            // Sets the saved data to the player
            Player player = playerGo.GetComponent<Player>();
            player.CurrentHealth = playerData.Health;
            player.ItemInInventory = ItemFactory.Create(playerGo).GetComponent<PickupableItem>();
            player.ItemInInventory.Name = playerData.Potion_Name;

            return playerGo;
        }

        private static void MakePlayer()
        {

        }

        public static void DeleteSave(int saveID)
        {
            using var saveFileDB = new DataBase(CollectionName.SaveFile);
            SaveFileData data = saveFileDB.FindOne<SaveFileData>(x => x.Save_ID == saveID);

            if (data == null) return; // Has deleted data

            DBSaveFile.DeleteWeapon(data);
            DBSaveFile.DeleteClass(data);

            using var fileHasRunDataLinkDB = new DataBase(CollectionName.SaveFileHasRunData);
            using var runDataDB = new DataBase(CollectionName.RunData);

            // Find the rundata for the
            SaveFileHasRunData existingLink = fileHasRunDataLinkDB.GetCollection<SaveFileHasRunData>()
                                                      .FindOne(link => link.Save_ID == data.Save_ID);

            // If there is no link, we can just delete the savefile and quit out of the method
            if (existingLink == null)
            {
                //Delete SaveFile.
                saveFileDB.Delete<SaveFileData>(data.Save_ID);
                return;
            }

            RunData runData = runDataDB.GetCollection<RunData>()
                                                    .FindOne(data => data.Run_ID == existingLink.Run_ID);

            using var runDataHasPlayerLinkDB = new DataBase(CollectionName.RunDataHasPlayerData);
            using var playerDB = new DataBase(CollectionName.PlayerData);
            DBRunData.DeletePlayer(runData, runDataHasPlayerLinkDB, playerDB);

            // Delete run data. Need to be last, since it needs to be used to get and delete the Player.
            DBRunData.DeleteRunData(data, fileHasRunDataLinkDB, runDataDB);
        }

        public static void UnlockClass(ClassTypes classType)
        {
            if (Data.UnlockedClasses.Contains(classType)) return;

            Data.UnlockedClasses.Add(classType);

            SaveFileData saveFileData = DBSaveFile.LoadFileData(Data.CurrentSaveID);

            Data.UnlockedClasses = DBSaveFile.LoadSaveClassType(saveFileData, true);
        }

        public static void UnlockWeapon(WeaponTypes unlockedWeapon)
        {
            // Only add the weapon if it's not already in the list
            if (Data.UnlockedWeapons.Contains(unlockedWeapon)) return;

            Data.UnlockedWeapons.Add(unlockedWeapon);

            SaveFileData saveFileData = DBSaveFile.LoadFileData(Data.CurrentSaveID);

            Data.UnlockedWeapons = DBSaveFile.LoadSaveWeaponType(saveFileData, true);
        }

        public static void AddCurrency(int amount)
        {
            Data.Currency += amount;

            DBSaveFile.LoadSaveFileData(Data.CurrentSaveID, true); // Override current data
        }

        /// <summary>
        /// Overrides the data if the player has enough currency
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>To check if you can remove that much</returns>
        public static bool AddRemove(int amount)
        {
            if (Data.Currency - amount < 0)
            {
                return false;
            }
            Data.Currency -= amount;
         
            DBSaveFile.LoadSaveFileData(Data.CurrentSaveID, true); // Override current data
            return true;
        }

        /// <summary>
        /// This should be called after changing stuff like, room reached.
        /// </summary>
        public static void RegularSave()
        {
            // Override current data
            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(Data.CurrentSaveID, true); 

            DBSaveFile.LoadSaveWeaponType(saveFileData, true);
            DBSaveFile.LoadSaveClassType(saveFileData, true);

            RunData runData = DBRunData.SaveLoadRunData(saveFileData); // Save Run File

            if (Data.Player == null) return;

            DBRunData.SavePlayer(runData);
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