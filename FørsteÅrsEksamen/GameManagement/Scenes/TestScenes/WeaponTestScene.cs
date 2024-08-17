using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.Factory.Gui;
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
        GameWorld.Instance.ShowBG = false;

        MakePlayer();
        MakeEmitters();
        MakeHealthBar();

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

    private void MakeHealthBar()
    {
        GameObject go = ScalableBarFactory.CreateHealthBar(playerGo, true);
        GameWorld.Instance.Instantiate(go);
    }

    ParticleEmitter emitter;

    private void MakeEmitters()
    {
        //FairyEmitter();

        //DustEmitter();
        //SpaceEmitter();

        DoorEmitter();
    }

    private void DoorEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(0, 0), new Interval(100, 150), new Interval(-MathHelper.Pi, MathHelper.Pi), 50, new Interval(500, 1000), 1000, -1, new Interval(-MathHelper.Pi, MathHelper.Pi), new Interval(-0.01, 0.01));

        emitter = go.GetComponent<ParticleEmitter>();

        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(0.5, 2)));

        emitter.AddModifier(new InwardModifier(2));

        emitter.AddModifier(new ColorRangeModifier(GameWorld.Instance.roomColors));

        int width = 32 * 4;
        int height = 48 * 4;
        RectangleOrigin origin = new RectangleOrigin(width, height, true);
        origin.OffCenter(emitter);
        emitter.Origin = origin;

        GameWorld.Instance.Instantiate(go);
    }

    private void DustEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(200, -200), new Interval(100, 150), new Interval(-MathHelper.Pi, 0), 20, new Interval(2000, 2000), 1000, -1, new Interval(-MathHelper.Pi, 0), new Interval(-0.01, 0.01));

        emitter = go.GetComponent<ParticleEmitter>();
        emitter.FollowGameObject(playerGo, new Vector2(0, 25));
        emitter.LayerName = LayerDepth.EnemyUnder;
        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        emitter.AddModifier(new ColorRangeModifier(new Color[] { Color.DarkGray, Color.Moccasin, Color.Transparent }, new Color[] { Color.Yellow, Color.Transparent }));

        emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(4, 4)));
        //emitter.AddModifier(new GravityModifier());
        //emitter.AddModifier(new ScaleModifier(4, 10));

        GameWorld.Instance.Instantiate(go);
    }

    private void FairyEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(0, 0), new Interval(250, 550), new Interval(-MathHelper.Pi, 0), 300, new Interval(1500, 2000), 1000, -1, new Interval(-MathHelper.Pi, 0));

        emitter = go.GetComponent<ParticleEmitter>();
        emitter.LayerName = LayerDepth.EnemyUnder;
        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        emitter.AddModifier(new ColorRangeModifier(new Color[] { Color.DarkViolet, Color.WhiteSmoke, Color.Transparent }));
        //emitter.AddModifier(new ColorRangeModifier(true));

        //emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(4, 4)));
        emitter.AddBirthModifier(new OutwardBirthModifier());
        emitter.LinearDamping = 1.0f;
        emitter.AddModifier(new GravityModifier());
        emitter.AddModifier(new ScaleModifier(4, 1));
        //emitter.Origin = new CircleOrigin(500);

        emitter.Origin = new FairyDustAnimatedOrigin(new Rectangle((int)GameWorld.Instance.WorldCam.TopLeft.X, (int)GameWorld.Instance.WorldCam.TopLeft.Y, 1920, 1080), 200, 0.5);
        GameWorld.Instance.Instantiate(go);
    }

    private void SpaceEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Space Dust", new Vector2(0, 0), new Interval(50, 100), new Interval(-MathHelper.Pi, MathHelper.Pi), 200, new Interval(3000, 4000), 300, -1, new Interval(-MathHelper.Pi, MathHelper.Pi));

        emitter = go.GetComponent<ParticleEmitter>();
        emitter.LayerName = LayerDepth.WorldBackground;

        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));

        emitter.AddModifier(new ColorRangeModifier(new Color[] { Color.DarkCyan, Color.DarkGray, Color.Gray, Color.Transparent }));
        emitter.AddModifier(new ScaleModifier(0.5f, 2));

        emitter.Origin = new RectangleOrigin(GameWorld.Instance.DisplayWidth, GameWorld.Instance.DisplayHeight);

        GameWorld.Instance.Instantiate(go);
    }

    //int nmb = 100;
    Random rnd = new();
    private void SetCommands()
    {
        //InputHandler.Instance.AddMouseButtonDownCommand(MouseCmdState.Right, new CustomCmd(player.Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.V, new CustomCmd(emitter.StartEmitter));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.B, new CustomCmd(emitter.StopEmitter));

        //InputHandler.Instance.AddKeyButtonDownCommand(Keys.E, new CustomCmd(() => { player.GameObject.GetComponent<Health>().TakeDamage(rnd.Next(10, 50)); }));
        //InputHandler.Instance.AddKeyButtonDownCommand(Keys.N, new CustomCmd(() =>
        //{
        //    nmb++;
        //    emitter.SetParticleText(new TextOnSprite()
        //    {
        //        Text = nmb.ToString()
        //    });
        //}));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.W, new CustomCmd(() => { player.GameObject.GetComponent<Health>().TakeDamage(1000000); }));


        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(GameWorld.Instance.Exit));
    }

    public override void DrawInWorld(SpriteBatch spriteBatch)
    {
        base.DrawInWorld(spriteBatch);
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        Vector2 pos = GameWorld.Instance.UiCam.TopLeft;
        Vector2 offset = new Vector2(0, 30);

        //pos += offset;
        //DrawString(spriteBatch, $"Current anim: {weapon.CurrentAnim} | Rot: {weapon.Animations[weapon.CurrentAnim].AmountOfRotation}", pos);

        //pos += offset;
        //DrawString(spriteBatch, $"Next anim: {weapon.NextAnim} | Rot: {weapon.Animations[weapon.NextAnim].AmountOfRotation}", pos);

        pos += offset;
        DrawString(spriteBatch, $"Active count: {emitter.ParticlePool.Active.Count}", pos);
        pos += offset;
        DrawString(spriteBatch, $"In Active count: {emitter.ParticlePool.InActive.Count}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Mouse pos UI : {InputHandler.Instance.MouseOnUI}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Mouse pos World: {InputHandler.Instance.MouseInWorld}", pos);
    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
    }
}