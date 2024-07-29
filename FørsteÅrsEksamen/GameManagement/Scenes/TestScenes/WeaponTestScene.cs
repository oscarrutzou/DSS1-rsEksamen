using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Weapons;
using DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;
using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DoctorsDungeon.GameManagement.Scenes.TestScenes;

public class WeaponTestScene : Scene
{
    private GameObject playerGo;
    private Player player;

    
    public override void Initialize()
    {
        MakePlayer();
        MakeEmitters();

        SetCommands();
    }

    MeleeWeapon weapon;
    private void MakePlayer()
    {
        playerGo = PlayerFactory.Create(ClassTypes.Rogue, WeaponTypes.Sword);
        player = playerGo.GetComponent<Player>();
        weapon = player.WeaponGo.GetComponent<MeleeWeapon>();

        GameWorld.Instance.WorldCam.Position = playerGo.Transform.Position;
        GameWorld.Instance.Instantiate(playerGo);
    }
    ParticleEmitter emitter;
    private void MakeEmitters()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(200, -200), new Interval(50, 150), new Interval(-Math.PI, Math.PI), 400, new Interval(1000, 2000), 200);

        emitter = go.GetComponent<ParticleEmitter>();
        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel));

        emitter.StartEmitter();

        GameWorld.Instance.Instantiate(go);
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

        pos += offset;
        DrawString(spriteBatch, $"Active count: {emitter.ParticlePool.Active.Count}", pos);
        pos += offset;
        DrawString(spriteBatch, $"In Active count: {emitter.ParticlePool.InActive.Count}", pos);
    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
    }
}