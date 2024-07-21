using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Weapons;
using DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;
using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoctorsDungeon.GameManagement.Scenes.TestScenes;

public class WeaponTestScene : Scene
{
    private GameObject playerGo;
    private Player player;

    
    public override void Initialize()
    {
        MakePlayer();

        SetCommands();
    }

    MeleeWeapon weapon;
    private void MakePlayer()
    {
        playerGo = PlayerFactory.Create(ClassTypes.Rogue, WeaponTypes.Dagger);
        player = playerGo.GetComponent<Player>();
        weapon = player.WeaponGo.GetComponent<MeleeWeapon>();

        GameWorld.Instance.WorldCam.Position = playerGo.Transform.Position;
        GameWorld.Instance.Instantiate(playerGo);
    }

    private void SetCommands()
    {
        InputHandler.Instance.AddMouseButtonDownCommand(MouseCmdState.Right, new CustomCmd(player.Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(GameWorld.Instance.Exit));
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        Vector2 pos = GameWorld.Instance.UiCam.TopLeft;
        Vector2 offset = new Vector2(0, 30);

        pos += offset;
        DrawString(spriteBatch, $"Current anim: {weapon.CurrentAnim} | Rot: {weapon.Animations[weapon.CurrentAnim].AmountOfRotation}", pos);

        pos += offset;
        DrawString(spriteBatch, $"Next anim: {weapon.NextAnim} | Rot: {weapon.Animations[weapon.NextAnim].AmountOfRotation}", pos);

        //pos += offset;
        //DrawString(spriteBatch, $"Weapon angle: {weapon.angle}", pos);
    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
    }
}