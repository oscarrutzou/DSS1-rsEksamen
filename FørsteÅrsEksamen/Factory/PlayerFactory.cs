using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;


namespace FørsteÅrsEksamen.Factory
{
    public enum PlayerClasses
    {
        Warrior,
        Archer,
        Mage
    }

    public class PlayerFactory : Factory
    {
        public override GameObject Create()
        {
            throw new System.Exception("Need to specify a class for the player");
        }

        public GameObject Create(PlayerClasses playerClass)
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

            playerGo = AddClassComponent(playerGo, hands, movementCollider, playerClass);

            return playerGo;
        }


        public GameObject Create(PlayerClasses playerClass, WeaponTypes weaponType)
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

            return playerGo;
        }
        private GameObject AddClassComponent(GameObject playerGo, GameObject handsGo, GameObject movementColliderGo, PlayerClasses playerClass)
        {
            switch (playerClass)
            {
                case PlayerClasses.Warrior:
                    playerGo.AddComponent<Warrior>(handsGo, movementColliderGo);
                    break;
                case PlayerClasses.Archer:
                    playerGo.AddComponent<Archer>(handsGo, movementColliderGo);
                    break;
                case PlayerClasses.Mage:
                    playerGo.AddComponent<Mage>(handsGo, movementColliderGo);
                    break;
            }

            return playerGo;
        }

        private GameObject CreateHands()
        {
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Animator>();
            return go;
        }

        private GameObject CreatePlayerMovementCollider()
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