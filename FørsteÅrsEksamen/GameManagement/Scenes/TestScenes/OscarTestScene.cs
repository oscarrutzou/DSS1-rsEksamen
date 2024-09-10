using ShamansDungeon.CommandPattern;
using ShamansDungeon.CommandPattern.Commands;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.Enemies.MeleeEnemies;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.Factory;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.LiteDB;
using ShamansDungeon.ObserverPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ShamansDungeon.GameManagement.Scenes.TestScenes;

public class OscarTestScene : Scene
{
    private GameObject _drawRoomBtn, _drawAstarPathBtn;
    private Point _playerSpawnPos;
    private GameObject _playerGo;

    public OscarTestScene()
    {
        _playerSpawnPos = new Point(6, 6);
    }

    public override void Initialize()
    {
        SetLevelBG();
        //First grid
        StartGrid();

        //Then player
        MakePlayer();

        // then enemies
        MakeEnemy();

        MakeItem();

        MakeButtons();

        OnPlayerChanged();
    }

    private void SetLevelBG()
    {
        GameObject go = new()
        {
            Type = GameObjectTypes.Background
        };

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.SetSprite(TextureNames.TestLevelBG);
        spriteRenderer.IsCentered = false;

        GameWorld.Instance.Instantiate(go);
    }

    private void MakeEnemy()
    {
        GameObject enemGo = EnemyFactory.Create(EnemyTypes.OrcWarrior, WeaponTypes.Sword);
        GameWorld.Instance.Instantiate(enemGo);

        if (GridManager.Instance.CurrentGrid != null)
        {
            SkeletonWarrior enemy = enemGo.GetComponent<SkeletonWarrior>();
            enemy.SetStartPosition(_playerGo, new Point(7, 13));
        }
    }

    private void MakeItem()
    {
        GameObject itemGo = ItemFactory.CreatePotion(_playerGo, ComponentPattern.WorldObjects.PickUps.PotionTypes.SmallHealth);
        GameWorld.Instance.Instantiate(itemGo);

        itemGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[new Point(3, 3)].Transform.Position;
    }

    private void MakePlayer()
    {
        _playerGo = PlayerFactory.Create(ClassTypes.Warrior, WeaponTypes.Sword);
        _playerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[_playerSpawnPos].Transform.Position;
        _playerGo.Transform.GridPosition = _playerSpawnPos;
        GameWorld.Instance.WorldCam.Position = _playerGo.Transform.Position;
        GameWorld.Instance.Instantiate(_playerGo);
    }

    private Player player;

    private void SetCommands()
    {
        player = _playerGo.GetComponent<Player>();
        InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.D1, new CustomCmd(player.UseItem));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Tab, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DB.Instance.SaveGrid(GridManager.Instance.CurrentGrid); }));
    }

    public override void OnPlayerChanged()
    {
        InputHandler.Instance.RemoveAllExeptBaseCommands();
        SetCommands();
    }

    private void Attack()
    {
        //player.weapon.Attack();
    }

    private void StartGrid()
    {
        GameObject gridGo = new();
        Grid grid = gridGo.AddComponent<Grid>("Test1", new Vector2(0, 0), 24, 18);
        grid.GenerateGrid();
        GridManager.Instance.SaveLoadGrid(grid);
    }

    private void MakeButtons()
    {
        Camera uiCam = GameWorld.Instance.UiCam;

        _drawRoomBtn = ButtonFactory.Create("Draw Room", true, () => { });
        _drawRoomBtn.Transform.Translate(uiCam.TopRight + new Vector2(-100, 50));

        GameWorld.Instance.Instantiate(_drawRoomBtn);

        _drawAstarPathBtn = ButtonFactory.Create("Draw Valid Path", true, () => { });
        _drawAstarPathBtn.Transform.Translate(uiCam.TopRight + new Vector2(-100, 120));
        GameWorld.Instance.Instantiate(_drawAstarPathBtn);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void DrawInWorld(SpriteBatch spriteBatch)
    {
        if (GridManager.Instance.CurrentGrid != null)
        {
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GridManager.Instance.GetCornerPositionOfCell(new Point(3, 1)), null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
        }

        base.DrawInWorld(spriteBatch);
    }

    private List<GameObject> list; //For test

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameWorld.Instance.UiCam.TopLeft, null, Color.WhiteSmoke, 0f, Vector2.Zero, new Vector2(350, 150), SpriteEffects.None, 0f);

        Vector2 mousePos = InputHandler.Instance.MouseOnUI;
        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"MousePos UI {mousePos}", GameWorld.Instance.UiCam.TopLeft, Color.Black);

        DrawCellPos(spriteBatch);

        SceneData.Instance.GameObjectLists.TryGetValue(GameObjectTypes.Cell, out list);
        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell GameObjects in scene {list.Count}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 90), Color.Black);

        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"RoomNr {GridManager.Instance.ColliderNrIndex}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 120), Color.Black);

        base.DrawOnScreen(spriteBatch);
    }

    private void DrawCellPos(SpriteBatch spriteBatch)
    {
        GameObject cellGo = GridManager.Instance.GetCellAtPos(InputHandler.Instance.MouseInWorld);

        if (cellGo == null) return;

        Point cellGridPos = cellGo.Transform.GridPosition;
        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell Point from MousePos: {cellGridPos}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 30), Color.Black);
    }
}