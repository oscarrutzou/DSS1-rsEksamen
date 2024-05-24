using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.LiteDB;
using Microsoft.Xna.Framework;
using FørsteÅrsEksamen.ComponentPattern.Classes.RangedClasses;
using FørsteÅrsEksamen.ComponentPattern.Classes.MeleeClasses;
using FørsteÅrsEksamen.ComponentPattern.Weapons;

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

            GameObject movementColliderGo = CreatePlayerMovementCollider();
            GameWorld.Instance.Instantiate(movementColliderGo); // Makes the collider


            // remove the hands from the constructer
            playerGo = AddClassComponent(playerGo, playerClass);

            Player player = playerGo.GetComponent<Player>();
            // Adds hands and the collider
            player.HandsGo = hands;
            player.MovementColliderGo = movementColliderGo;

            //Weapon
            GameObject weaponGo = WeaponFactory.Create(weaponType,false);
            weaponGo.GetComponent<Weapon>().WeaponUser = player;
            GameWorld.Instance.Instantiate(weaponGo);

            // Add weapon to player
            player.WeaponGo = weaponGo;
            player.WeaponType = weaponType;
            player.ClassType = playerClass;

            // Set the reference to this player.
            SaveData.Player = player;

            return playerGo;
        }

        private static GameObject AddClassComponent(GameObject playerGo, ClassTypes playerClass)
        {
            switch (playerClass)
            {
                case ClassTypes.Warrior:
                    playerGo.AddComponent<Warrior>();
                    break;

                case ClassTypes.Archer:
                    playerGo.AddComponent<Archer>();
                    break;

                case ClassTypes.Mage:
                    playerGo.AddComponent<Mage>();
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
            go.AddComponent<SpriteRenderer>().SetLayerDepth(LayerDepth.Player);
            Collider collider = go.AddComponent<Collider>();

            collider.SetCollisionBox(13, 15);
            collider.DebugColor = Color.Aqua;

            return go;
        }
    }
}