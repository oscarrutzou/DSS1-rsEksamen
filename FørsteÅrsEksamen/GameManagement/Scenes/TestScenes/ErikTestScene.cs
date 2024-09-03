using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Weapons;
using DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons;
using DoctorsDungeon.Factory;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement.Scenes.TestScenes;

public class ErikTestScene : Scene
{
    private GameObject _weapon, _bow, _playerGo, _spawnerGameObject;
    private Point _playerSpawnPos;
    private Player _player;

    public override void Initialize()
    {
        SetLevelBG();
        StartGrid();
        _playerSpawnPos = new Point(5, 5);
        MakePlayer();

        OnPlayerChanged();

        //InitSpawner();

        MakeWeapon();
        GameWorld.Instance.WorldCam.Position = Vector2.Zero;

        AttackCommand();
        //spawner = new Spawner(new GameObject(), player);
        //GameWorld.Instance.Instantiate(spawner.GameObject);
    }

    public override void Update()
    {
        base.Update();
    }

    private List<Point> spawnPoints = new()
    {
        new Point(5, 5),
        new Point(7, 5),
        new Point(7, 7),
    };

    private void InitSpawner()
    {
        _spawnerGameObject = new GameObject();
        Spawner spawner = _spawnerGameObject.AddComponent<Spawner>();
        spawner.SpawnEnemies(spawnPoints, _playerGo);
    }

    private void SetLevelBG()
    {
        GameObject go = new();
        go.Type = GameObjectTypes.Background;

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.SetSprite(TextureNames.TestLevelBG);
        spriteRenderer.IsCentered = false;

        GameWorld.Instance.Instantiate(go);
    }

    private void MakePlayer()
    {
        _playerGo = PlayerFactory.Create(ClassTypes.Warrior, WeaponTypes.Sword);
        _playerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[_playerSpawnPos].Transform.Position;
        _playerGo.Transform.GridPosition = _playerSpawnPos;
        GameWorld.Instance.WorldCam.Position = _playerGo.Transform.Position;
        GameWorld.Instance.Instantiate(_playerGo);
    }

    private void StartGrid()
    {
        GameObject gridGo = new();
        Grid grid = gridGo.AddComponent<Grid>("Test1", new Vector2(0, 0), 24, 18);
        grid.GenerateGrid();
        GridManager.Instance.SaveLoadGrid(grid);
    }

    private void SetCommands()
    {
        _player = _playerGo.GetComponent<Player>();

        InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(_player, new Vector2(1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(_player, new Vector2(-1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(_player, new Vector2(0, -1)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(_player, new Vector2(0, 1)));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.D1, new CustomCmd(_player.UseItem));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Tab, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DB.Instance.SaveGrid(GridManager.Instance.CurrentGrid); }));
    }

    public override void OnPlayerChanged()
    {
        InputHandler.Instance.RemoveAllExeptBaseCommands();
        SetCommands();
    }

    private void MakeWeapon()
    {
        _weapon = WeaponFactory.Create(WeaponTypes.Sword);

        _bow = WeaponFactory.Create(WeaponTypes.Bow);

        GameWorld.Instance.Instantiate(_weapon);
        GameWorld.Instance.Instantiate(_bow);
    }

    private void Attack()
    {
        _weapon.GetComponent<Weapon>().StartAttack();

        //projectile.GetComponent<MagicStaff>().Attack();
    }

    private void AttackCommand()
    {
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space,
            new CustomCmd(Attack));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.B,
            new CustomCmd(Shoot));
    }

    public void Shoot()
    {
        var rangedWeapon = _bow.GetComponent<RangedWeapon>();

        //projectile.GetComponent<Projectile>().SetValues(MathHelper.Pi);

        if (rangedWeapon != null)
        {
            //rangedWeapon.Shoot();
        }
    }

    public override void DrawInWorld(SpriteBatch spriteBatch)
    {
        base.DrawInWorld(spriteBatch);

        //spriteBatch.Draw(GlobalTextures.Textures[TextureNames.WoodSword], Vector2.Zero, Color.White);
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);
    }
}