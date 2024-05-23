using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.DB;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.Factory
{
    public enum ClassTypes
    {
        Archer,
        Warrior,
        Mage,
    }

    public static class PlayerFactory
    {
        public static GameObject Create(ClassTypes playerClass, WeaponTypes weaponType)
        {
            GameObject playerGo = new GameObject();
            playerGo.Transform.Scale = new Vector2(4, 4);
            playerGo.Type = GameObjectTypes.Player;
            playerGo.AddComponent<SpriteRenderer>();
            playerGo.AddComponent<Animator>();
            playerGo.AddComponent<Collider>();

            GameObject hands = CreateHands();
            GameWorld.Instance.Instantiate(hands); // Makes hands

            GameObject movementCollider = CreatePlayerMovementCollider();
            GameWorld.Instance.Instantiate(movementCollider); // Makes the collider

            // remove the hands from the constructer
            playerGo = AddClassComponent(playerGo, hands, movementCollider, playerClass);

            //Weapon
            GameObject weapon = WeaponFactory.Create(weaponType);
            GameWorld.Instance.Instantiate(weapon);

            // Add weapon to player
            Player player = playerGo.GetComponent<Player>();
            player.WeaponGo = weapon;
            player.WeaponType = weaponType;
            player.ClassType = playerClass;

            // Set the data that will be downloaded to this player
            Data.Player = player;

            return playerGo;
        }

        private static GameObject AddClassComponent(GameObject playerGo, GameObject handsGo, GameObject movementColliderGo, ClassTypes playerClass)
        {
            switch (playerClass)
            {
                case ClassTypes.Warrior:
                    playerGo.AddComponent<Warrior>(handsGo, movementColliderGo);
                    break;

                case ClassTypes.Archer:
                    playerGo.AddComponent<Archer>(handsGo, movementColliderGo);
                    break;

                case ClassTypes.Mage:
                    playerGo.AddComponent<Mage>(handsGo, movementColliderGo);
                    break;
            }

            return playerGo;
        }

        private static GameObject CreateHands()
        {
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Animator>();
            return go;
        }

        private static GameObject CreatePlayerMovementCollider()
        {
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            go.AddComponent<SpriteRenderer>().SetLayerDepth(LAYERDEPTH.Player);
            Collider collider = go.AddComponent<Collider>();

            collider.SetCollisionBox(13, 15);
            collider.DebugColor = Color.Aqua;

            return go;
        }
    }
}