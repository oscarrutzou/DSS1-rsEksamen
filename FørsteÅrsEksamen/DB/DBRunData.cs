using System;

namespace FørsteÅrsEksamen.DB
{
    public static class DBRunData
    {
        public static RunData SaveLoadRunData(SaveFileData saveFileData)
        {
            using var fileHasRunDataLinkDB = new DataBase(CollectionName.SaveFileHasRunData);
            using var runDataDB = new DataBase(CollectionName.RunData);

            DeleteRunData(saveFileData, fileHasRunDataLinkDB, runDataDB);

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

        public static void DeleteRunData(SaveFileData saveFileData, DataBase fileHasRunDataLinkDB, DataBase runDataDB)
        {
            // Get the existing link
            SaveFileHasRunData existingLink = fileHasRunDataLinkDB.GetCollection<SaveFileHasRunData>()
                                                                  .FindOne(link => link.Save_ID == saveFileData.Save_ID);

            if (existingLink != null)
            {
                // Delete the existing RunData and the link
                runDataDB.Delete<RunData>(existingLink.Run_ID);
                fileHasRunDataLinkDB.Delete<SaveFileHasRunData>(existingLink.Run_ID);
            }
        }

        private static RunData SaveRunData(DataBase runDataDB)
        {
            RunData runData = new()
            {
                Run_ID = SaveData.CurrentSaveID,
                Room_Reached = SaveData.Room_Reached,
                Time_Left = SaveData.Time_Left,
            };

            runDataDB.SaveOverrideSingle(runData, runData.Run_ID, x => x.Run_ID == runData.Run_ID);
            return runData;
        }

        public static PlayerData SavePlayer(RunData runData)
        {
            using var runDataHasPlayerLinkDB = new DataBase(CollectionName.RunDataHasPlayerData);
            using var playerDB = new DataBase(CollectionName.PlayerData);

            DeletePlayer(runData, runDataHasPlayerLinkDB, playerDB);

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

        public static void DeletePlayer(RunData runData, DataBase runDataHasPlayerLinkDB, DataBase playerDB)
        {
            // Get the existing link
            RunDataHasPlayerData existingLink = runDataHasPlayerLinkDB.GetCollection<RunDataHasPlayerData>()
                                                                  .FindOne(link => link.Run_ID == runData.Run_ID);

            // Delete the existing RunData and the link
            if (existingLink != null)
            {
                PlayerData currentPlayerData = playerDB.GetCollection<PlayerData>()
                                                       .FindOne(data => data.Player_ID == existingLink.Player_ID);
                if (currentPlayerData != null)
                {
                    playerDB.Delete<PlayerData>(currentPlayerData.Player_ID);
                }

                runDataHasPlayerLinkDB.Delete<RunDataHasPlayerData>(existingLink.Run_ID);
            }
        }

        private static PlayerData MakePlayer(DataBase playerDB)
        {
            string potionName = SaveData.Player.ItemInInventory == null ? string.Empty : SaveData.Player.ItemInInventory.Name;

            PlayerData playerData = new()
            {
                Player_ID = Guid.NewGuid(),
                Health = SaveData.Player.CurrentHealth,
                Potion_Name = potionName,
                Class_Type = SaveData.Player.ClassType,
                Weapon_Type = SaveData.Player.WeaponType,
            };

            playerDB.SaveSingle(playerData);

            return playerData;
        }
    }
}