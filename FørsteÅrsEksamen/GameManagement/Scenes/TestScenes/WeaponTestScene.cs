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
using DoctorsDungeon.GameManagement.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DoctorsDungeon.GameManagement.Scenes.TestScenes;

public class WeaponTestScene : Scene
{
    private GameObject _playerGo;
    private Player _player;
    private MeleeWeapon _weapon;
    private ParticleEmitter _emitter;

    public override void Initialize()
    {
        GameWorld.Instance.ShowBG = false;

        MakePlayer();
        MakeEmitters();
        MakeHealthBar();

        SetCommands();
    }
    #region Make
    private void MakePlayer()
    {
        _playerGo = PlayerFactory.Create(ClassTypes.Rogue, WeaponTypes.Sword);
        _player = _playerGo.GetComponent<Player>();
        _weapon = _player.WeaponGo.GetComponent<MeleeWeapon>();

        GameWorld.Instance.WorldCam.Position = _playerGo.Transform.Position;
        GameWorld.Instance.Instantiate(_playerGo);
    }

    private void MakeHealthBar()
    {
        GameObject go = ScalableBarFactory.CreateHealthBar(_playerGo, true);
        GameWorld.Instance.Instantiate(go);
    }


    private void MakeEmitters()
    {
        //FairyEmitter();

        BlodEmitter();
        //SpaceEmitter();

        //DoorEmitter();
    }

    private void DoorEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(0, 0), new Interval(100, 150), new Interval(100, 150), new Interval(-MathHelper.Pi, MathHelper.Pi), 50, new Interval(500, 1000), 1000, -1, new Interval(-MathHelper.Pi, MathHelper.Pi), new Interval(-0.01, 0.01));

        _emitter = go.GetComponent<ParticleEmitter>();

        _emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        _emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(0.5, 2)));

        _emitter.AddModifier(new InwardModifier(2));

        _emitter.AddModifier(new ColorRangeModifier(IndependentBackground.RoomColors));

        int width = 32 * 4;
        int height = 48 * 4;
        RectangleOrigin origin = new RectangleOrigin(width, height, true);
        origin.OffCenter(_emitter);
        _emitter.Origin = origin;

        GameWorld.Instance.Instantiate(go);
    }



    private void FairyEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(0, 0), new Interval(250, 550), new Interval(250, 550), new Interval(-MathHelper.Pi, 0), 300, new Interval(1500, 2000), 1000, -1, new Interval(-MathHelper.Pi, 0));

        _emitter = go.GetComponent<ParticleEmitter>();
        _emitter.LayerName = LayerDepth.EnemyUnder;
        _emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        _emitter.AddModifier(new ColorRangeModifier(new Color[] { Color.DarkViolet, Color.WhiteSmoke, Color.Transparent }));
        //emitter.AddModifier(new ColorRangeModifier(true));

        //emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(4, 4)));
        _emitter.AddBirthModifier(new OutwardBirthModifier());
        _emitter.LinearDamping = 1.0f;
        _emitter.AddModifier(new GravityModifier());
        _emitter.AddModifier(new ScaleModifier(4, 1));
        //emitter.Origin = new CircleOrigin(500);

        _emitter.Origin = new FairyDustAnimatedOrigin(new Rectangle((int)GameWorld.Instance.WorldCam.TopLeft.X, (int)GameWorld.Instance.WorldCam.TopLeft.Y, 1920, 1080), 200, 0.5);
        GameWorld.Instance.Instantiate(go);
    }

    private void SpaceEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Space Dust", new Vector2(0, 0), new Interval(50, 100), new Interval(50, 100), new Interval(-MathHelper.Pi, MathHelper.Pi), 200, new Interval(3000, 4000), 300, -1, new Interval(-MathHelper.Pi, MathHelper.Pi));

        _emitter = go.GetComponent<ParticleEmitter>();
        _emitter.LayerName = LayerDepth.WorldBackground;

        _emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));

        _emitter.AddModifier(new ColorRangeModifier(new Color[] { Color.DarkCyan, Color.DarkGray, Color.Gray, Color.Transparent }));
        _emitter.AddModifier(new ScaleModifier(0.5f, 2));

        _emitter.Origin = new RectangleOrigin(GameWorld.Instance.DisplayWidth, GameWorld.Instance.DisplayHeight);

        GameWorld.Instance.Instantiate(go);
    }
    #endregion



    private void BlodEmitter()
    {
        // Would need to make the direction a cone, so change that part. 
        GameObject go = EmitterFactory.CreateParticleEmitter("Blod Cloud", _playerGo.Transform.Position, _speed, _speed, _direction, 100, new Interval(1000, 5000), 1000, 0.1f, new Interval(-MathHelper.Pi, 0), new Interval(-0.001, 0.001));

        _emitter = go.GetComponent<ParticleEmitter>();
        _emitter.FollowGameObject(_playerGo, new Vector2(0, 25));
        _emitter.LayerName = LayerDepth.DamageParticles;
        _emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        _emitter.AddModifier(new ColorRangeModifier(new Color[] { new Color(100, 40, 40, 255), new Color(50, 10, 10, 255) }));

        // Should have a force birth modifere, and a speed modifier. 
        // Could change the birth speed
        _emitter.AddModifier(new GravityModifier(_gravity));

        _dragModifier = new DragModifier(_bounce, _friction, _stopAmount, _velocityDrag);
        _emitter.AddModifier(_dragModifier);

        _emitter.AddModifier(new ScaleModifier(4, 2));

        _emitter.Origin = new RectangleOrigin(40, 20);

        GameWorld.Instance.Instantiate(go);
    }


    private DragModifier _dragModifier;
    private Random _rnd = new();
    private float _bounce = 0.2f;
    private float _friction = 0.5f;
    private int _stopAmount = 55;
    private float _velocityDrag = 1f;
    private Interval _speed = new Interval(200, 500);
    private Interval _direction = new Interval(0, MathHelper.PiOver2);
    private int _gravity = 30;

    private float _angle;
    private float _cone = MathHelper.PiOver4;
    private void TestDirection()
    {
        Vector2 mousePos = InputHandler.Instance.MouseInWorld;
        Vector2 playerPos = _playerGo.Transform.Position;

        // From player to mouse
        Vector2 direction = playerPos - mousePos;
        _angle = (float)Math.Atan2(direction.Y, direction.X);

        _emitter.Direction = new Interval(_angle - _cone, _angle + _cone);
    }

    private void SetCommands()
    {
        //InputHandler.Instance.AddMouseButtonDownCommand(MouseCmdState.Right, new CustomCmd(player.Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.V, new CustomCmd(_emitter.StartEmitter));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.B, new CustomCmd(_emitter.StopEmitter));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { _bounce += -0.005f; }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.W, new CustomCmd(() => { _bounce += 0.005f; }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.E, new CustomCmd(() => { _friction += -0.005f; }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.R, new CustomCmd(() => { _friction += 0.005f; }));


        InputHandler.Instance.AddKeyButtonDownCommand(Keys.A, new CustomCmd(() => { _stopAmount += -1; }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.S, new CustomCmd(() => { _stopAmount += 1; }));


        InputHandler.Instance.AddKeyButtonDownCommand(Keys.D, new CustomCmd(() => { _velocityDrag += -0.05f; }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.F, new CustomCmd(() => { _velocityDrag += 0.05f; }));


        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Z, new CustomCmd(() => { _emitter.StartZVel += -1; }));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.X, new CustomCmd(() => { _emitter.StartZVel += 1; }));

        int speedChangeAmount = 20;
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.D1, new CustomCmd(() => {
            _emitter.SpeedX.Min += -speedChangeAmount;
            _emitter.SpeedX.Max += -speedChangeAmount;
        }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.D2, new CustomCmd(() => {
            _emitter.SpeedX.Min += speedChangeAmount;
            _emitter.SpeedX.Max += speedChangeAmount;
        }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.NumPad3, new CustomCmd(() => { _emitter.Direction = new Interval(0, MathHelper.PiOver2); }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.NumPad1, new CustomCmd(() => { _emitter.Direction = new Interval(MathHelper.PiOver2, MathHelper.Pi); }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.NumPad7, new CustomCmd(() => { _emitter.Direction = new Interval(MathHelper.Pi, MathHelper.Pi * 1.5); }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.NumPad9, new CustomCmd(() => { _emitter.Direction = new Interval(MathHelper.Pi * 1.5 + MathHelper.TwoPi, MathHelper.TwoPi * 2); }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(GameWorld.Instance.Exit));
    }


    public override void Update()
    {
        if (_dragModifier != null)
        {
            _dragModifier.UpdateValue(_bounce, _friction, _stopAmount, _velocityDrag);
        }
        TestDirection();
        base.Update();
    }


    public override void DrawInWorld(SpriteBatch spriteBatch)
    {
        base.DrawInWorld(spriteBatch);
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        Vector2 offset = new Vector2(0, 30);
        Vector2 pos = GameWorld.Instance.UiCam.TopLeft + offset * 2;

        //pos += offset;
        //DrawString(spriteBatch, $"Current anim: {weapon.CurrentAnim} | Rot: {weapon.Animations[weapon.CurrentAnim].AmountOfRotation}", pos);

        //pos += offset;
        //DrawString(spriteBatch, $"Next anim: {weapon.NextAnim} | Rot: {weapon.Animations[weapon.NextAnim].AmountOfRotation}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Bounce QW: {_bounce}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Friction ER: {_friction}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Stop amount AS: {_stopAmount}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Vel Drag DF: {_velocityDrag}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Start Z vel ZX: {_emitter.StartZVel}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Speed 12: Min:{_emitter.SpeedX.Min}, Max: {_emitter.SpeedX.Max}", pos);

        pos += offset;
        pos += offset;
        DrawString(spriteBatch, $"Mouse to player Direction: {_angle}", pos);
        pos += offset;
        DrawString(spriteBatch, $"Direction: Min:{_emitter.Direction.Min}, Max: {_emitter.Direction.Max}", pos);

        //pos += offset;
        //DrawString(spriteBatch, $"Active count: {_emitter.ParticlePool.Active.Count}", pos);
        //pos += offset;
        //DrawString(spriteBatch, $"In Active count: {_emitter.ParticlePool.InActive.Count}", pos);
        //pos += offset;
        //DrawString(spriteBatch, $"Mouse pos UI : {InputHandler.Instance.MouseOnUI}", pos);
        //pos += offset;
        //DrawString(spriteBatch, $"Mouse pos World: {InputHandler.Instance.MouseInWorld}", pos);

        // To check if its any of these that makes it from

    }


    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
    }
}