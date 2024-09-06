using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.Effects;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.ComponentPattern.Weapons;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace ShamansDungeon.Factory;

public enum ClassTypes
{
    Assassin,
    Warrior,
    Mage,
}

// Stefan
public static class PlayerFactory
{
    private static Vector2 _playerScale = new Vector2(4, 4);
    public static GameObject Create(ClassTypes playerClass, WeaponTypes weaponType)
    {
        GameObject playerGo = new GameObject();        
        playerGo.Transform.Scale = _playerScale;

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

        Health health = playerGo.AddComponent<Health>();

        //Weapon
        GameObject weaponGo = WeaponFactory.Create(weaponType);
        weaponGo.Transform.Scale = _playerScale;

        Weapon weapon = weaponGo.GetComponent<Weapon>();
        weapon.PlayerUser = player;
        weapon.UseAttackCooldown = false;
        GameWorld.Instance.Instantiate(weaponGo);

        // Add weapon to player
        player.WeaponGo = weaponGo;
        player.WeaponType = weaponType;
        player.ClassType = playerClass;

        if (GameWorld.DebugAndCheats)
        {
            health.SetHealth(999_999);
        }

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

            case ClassTypes.Assassin:
                playerGo.AddComponent<Assassin>();
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
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Animator>();
        return go;
    }

    private static GameObject CreatePlayerMovementCollider()
    {
        GameObject go = new();
        go.AddComponent<SpriteRenderer>().SetLayerDepth(LayerDepth.Player);
        Collider collider = go.AddComponent<Collider>();

        collider.SetCollisionBox(12, 13, new Vector2(0, -5));
        collider.DebugColor = Color.Aqua;

        return go;
    }
}