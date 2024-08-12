using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.WorldObjects;

// Oscar
public class TransferDoor : Component
{
    public bool CanTranser { get; set; }    
    
    private Collider collider, playerCollider;
    private Health playerHealth;
    private double timer;
    private double timeTillActivation = 2f;
    public ParticleEmitter emitter;

    public TransferDoor(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        playerCollider = SaveData.Player.GameObject.GetComponent<Collider>();
        playerHealth = SaveData.Player.GameObject.GetComponent<Health>();
        collider = GameObject.GetComponent<Collider>();

        MakeEmitter();
    }

    private void MakeEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", GameObject.Transform.Position, new Interval(50, 80), new Interval(-MathHelper.Pi, MathHelper.Pi), 20, new Interval(1000, 2000), 100, -1, new Interval(-MathHelper.Pi, MathHelper.Pi), new Interval(-0.01, 0.01));

        emitter = go.GetComponent<ParticleEmitter>();
        emitter.LayerName = LayerDepth.EnemyUnder;

        emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(0.5, 2)));

        emitter.AddModifier(new InwardModifier(2));

        emitter.AddModifier(new ColorRangeModifier(GameWorld.Instance.roomColors));

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
        

        OnCollisionEnter(collider);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        // Skal kun fjerne item ved player position, ikke alle items.
        // Stop the door from changing scenes if the player died.
        if (playerHealth.CurrentHealth == 0) return;

        if (collider.CollisionBox.Intersects(playerCollider.CollisionBox))
        {
            timer += GameWorld.DeltaTime;

            // Darken screen (Remeber to brighten screen if player goes out of the collision box

            if (timer >= timeTillActivation)
            {
                DB.Instance.CheckChangeDungeonScene();
                timer = 0f;
            }
        }
    }
}