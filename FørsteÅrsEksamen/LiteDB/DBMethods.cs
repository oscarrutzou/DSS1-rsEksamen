using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern.WorldObjects;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using LiteDB;
using Microsoft.Xna.Framework;
using SharpDX.Multimedia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.LiteDB
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
            SaveFileData saveFileData = DBSaveFile.LoadFileData(SaveData.CurrentSaveID); // Load Save File
            RunData runData = DBRunData.SaveLoadRunData(saveFileData); // Save Run File
            
            if (SaveData.Player == null) return;
            
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
            player.ItemInInventory = ItemFactory.Create(playerGo).GetComponent<Potion>();
            player.ItemInInventory.Name = playerData.Potion_Name;

            return playerGo;
        }

        public static void SavePlayer()
        {
            RunData runData = DBRunData.LoadRunData(SaveData.CurrentSaveID);
            DBRunData.SavePlayer(runData);
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

        public static void DeleteRun(int saveID)
        {
            SaveData.SetBaseValues();

            using var saveFileDB = new DataBase(CollectionName.SaveFile);
            SaveFileData data = saveFileDB.FindOne<SaveFileData>(x => x.Save_ID == saveID);

            if (data == null) return; // Has deleted data

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

        public static async void CheckChangeDungeonScene()
        {
            // After it has saved the palyer it will change scene
            await Task.Run(SavePlayer);

            int newRoomNr = SaveData.Room_Reached + 1;

            if (newRoomNr <= SaveData.MaxRooms)
            {
                // Change to next dungeon scene
                GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, newRoomNr);
            }
            else // Player won
            {
                AddCurrency(100); // 100 for winning
                SaveData.HasWon = true;
                OnChangeSceneEnd();
            }
        }

        public static void OnChangeSceneEnd()
        {
            // Delete the run
            DeleteRun(SaveData.CurrentSaveID);
            GameWorld.Instance.ChangeScene(SceneNames.EndMenu);
        }

        public static void UnlockClass(ClassTypes classType)
        {
            if (SaveData.UnlockedClasses.Contains(classType)) return;

            SaveData.UnlockedClasses.Add(classType);

            SaveFileData saveFileData = DBSaveFile.LoadFileData(SaveData.CurrentSaveID);

            SaveData.UnlockedClasses = DBSaveFile.LoadSaveClassType(saveFileData, true);
        }

        public static void UnlockWeapon(WeaponTypes unlockedWeapon)
        {
            // Only add the weapon if it's not already in the list
            if (SaveData.UnlockedWeapons.Contains(unlockedWeapon)) return;

            SaveData.UnlockedWeapons.Add(unlockedWeapon);

            SaveFileData saveFileData = DBSaveFile.LoadFileData(SaveData.CurrentSaveID);

            SaveData.UnlockedWeapons = DBSaveFile.LoadSaveWeaponType(saveFileData, true);
        }

        public static void AddCurrency(int amount)
        {
            SaveData.Currency += amount;

            DBSaveFile.LoadSaveFileData(SaveData.CurrentSaveID, true); // Override current data
        }

        /// <summary>
        /// Overrides the data if the player has enough currency
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>To check if you can remove that much</returns>
        public static bool RemoveCurrency(int amount)
        {
            if (SaveData.Currency - amount < 0)
            {
                return false;
            }
            SaveData.Currency -= amount;
         
            DBSaveFile.LoadSaveFileData(SaveData.CurrentSaveID, true); // Override current data
            return true;
        }

        /// <summary>
        /// This should be called after changing stuff like, room reached.
        /// </summary>
        public static void RegularSave()
        {
            // Override current data
            SaveFileData saveFileData = DBSaveFile.LoadSaveFileData(SaveData.CurrentSaveID, true); 

            DBSaveFile.LoadSaveWeaponType(saveFileData, true);
            DBSaveFile.LoadSaveClassType(saveFileData, true);

            RunData runData = DBRunData.SaveLoadRunData(saveFileData); // Save Run File

            if (SaveData.Player == null) return;

            DBRunData.SavePlayer(runData);
        }
    }
}