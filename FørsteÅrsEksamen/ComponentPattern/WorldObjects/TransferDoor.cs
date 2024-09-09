using ShamansDungeon.ComponentPattern.Particles.BirthModifiers;
using ShamansDungeon.ComponentPattern.Particles.Modifiers;
using ShamansDungeon.ComponentPattern.Particles.Origins;
using ShamansDungeon.ComponentPattern.Particles;
using ShamansDungeon.GameManagement;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework;
using ShamansDungeon.GameManagement.Scenes.Menus;

namespace ShamansDungeon.ComponentPattern.WorldObjects;

// Oscar
public class TransferDoor : Component
{
    public bool CanTranser { get; set; }    
    
    private Collider _collider, _playerCollider;
    private Health _playerHealth;
    private double _timer;
    private readonly double _timeTillActivation = 2f;
    public ParticleEmitter emitter;

    public TransferDoor(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        MakeEmitter();
    }

    public override void Start()
    {
        _playerCollider = SaveData.Player.GameObject.GetComponent<Collider>();
        _playerHealth = SaveData.Player.GameObject.GetComponent<Health>();
        _collider = GameObject.GetComponent<Collider>();
    }

    private void MakeEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", GameObject.Transform.Position, new Interval(50, 80), new Interval(-MathHelper.Pi, MathHelper.Pi), 20, new Interval(1000, 2000), 100, -1, new Interval(-MathHelper.Pi, MathHelper.Pi), new Interval(-0.01, 0.01));

        emitter = go.GetComponent<ParticleEmitter>();
        emitter.LayerName = LayerDepth.EnemyUnder;

        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(0.5, 2)));

        emitter.AddModifier(new InwardModifier(2));

        emitter.AddModifier(new ColorRangeModifier(IndependentBackground.RoomColors));

        int width = 32 * 4;
        int height = 48 * 4;
        RectangleOrigin origin = new RectangleOrigin(width, height);
        origin.OffCenter(emitter);
        emitter.Origin = origin;

        GameWorld.Instance.Instantiate(go);
    }

    public override void Update()
    {
        if (!CanTranser) return; //Shouldnt open door before quest is done
        

        OnCollisionEnter(_collider);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        // Skal kun fjerne item ved player position, ikke alle items.
        // Stop the door from changing scenes if the player died.
        if (_playerHealth.IsDead) return;

        if (collider.CollisionBox.Intersects(_playerCollider.CollisionBox))
        {
            _timer += GameWorld.DeltaTime;

            // Darken screen (Remeber to brighten screen if player goes out of the collision box

            if (_timer >= _timeTillActivation)
            {
                GlobalSounds.PlaySound(SoundNames.Teleport, 1, 0.8f, true);
                DB.Instance.CheckChangeDungeonScene();
                _timer = 0f;
            }
        }
    }
}