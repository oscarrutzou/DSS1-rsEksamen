using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public static class DBRunData
    {
        public static RunData RunDataWithSaveFileData(SaveFileData saveFileData)
        {
            using var fileHasRunDataLinkDB = new DataBase(CollectionName.SaveFileHasRunData);
            using var runDataDB = new DataBase(CollectionName.RunData);

            // Get the existing link
            SaveFileHasRunData existingLink = fileHasRunDataLinkDB.GetCollection<SaveFileHasRunData>()
                                                                  .FindOne(link => link.Save_ID == saveFileData.Save_ID);

            if (existingLink != null)
            {
                // Delete the existing RunData and the link
                runDataDB.Delete<RunData>(existingLink.Run_ID);
                fileHasRunDataLinkDB.Delete<SaveFileHasRunData>(existingLink.Run_ID);
            }

            // Create and save the new RunData
            RunData newRunData = SaveRunData(runDataDB);

            // Create and save the new link
            SaveFileHasRunData newLink = new()
            {
                Run_ID = newRunData.Run_ID,
                Save_ID = saveFileData.Save_ID,
            };

            fileHasRunDataLinkDB.SaveSingle(newLink);

            return newRunData;
        }

        private static RunData SaveRunData(DataBase runDataDB)
        {
            RunData runData = new()
            {
                Run_ID = SaveFileManager.CurrentSaveID,
                Room_Reached = SaveFileManager.Room_Reached,
                Time_Left = SaveFileManager.Time_Left,
            };

            runDataDB.SaveOverrideSingle(runData, runData.Run_ID, x => x.Run_ID == runData.Run_ID);
            return runData;
        }

        public static PlayerData SavePlayer(RunData runData)
        {
            using var runDataHasPlayerLinkDB = new DataBase(CollectionName.RunDataHasPlayerData);
            using var playerDB = new DataBase(CollectionName.PlayerData);

            // Get the existing link
            RunDataHasPlayerData existingLink = runDataHasPlayerLinkDB.GetCollection<RunDataHasPlayerData>()
                                                                  .FindOne(link => link.Run_ID == runData.Run_ID);

            if (existingLink != null)
            {
                // Delete the existing RunData and the link
                playerDB.Delete<PlayerData>(existingLink.Run_ID);
                runDataHasPlayerLinkDB.Delete<RunDataHasPlayerData>(existingLink.Run_ID);
            }

            // Create and save the new RunData
            PlayerData playerData = MakePlayer(playerDB);

            // Create and save the new link
            RunDataHasPlayerData newLink = new()
            {
                Run_ID = runData.Run_ID,
                Player_ID = playerData.Player_ID,
            };
            
            runDataHasPlayerLinkDB.SaveSingle(newLink);

            return playerData;
        }

        private static PlayerData MakePlayer(DataBase playerDB)
        {
            string potionName = SaveFileManager.Player.ItemInInventory == null ? string.Empty: SaveFileManager.Player.ItemInInventory.Name;

            PlayerData playerData = new()
            {
                Player_ID = Guid.NewGuid(),
                Health = SaveFileManager.Player.CurrentHealth,
                Potion_Name = potionName,
                Class_Type = SaveFileManager.Player.ClassType,
                Weapon_Type = SaveFileManager.Player.WeaponType,
            };

            playerDB.SaveSingle(playerData);

            return playerData;
        }
    }
}
          