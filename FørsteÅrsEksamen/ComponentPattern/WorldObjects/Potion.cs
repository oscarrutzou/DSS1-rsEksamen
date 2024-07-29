using DoctorsDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.WorldObjects;

// Asser
public class Potion : Component
{
    private SpriteRenderer spriteRenderer;
    private Collider collider, playerCollider;
    private Player player;
    private Health health;
    private GameObject playerGo;

    public string Name = "Health Potion";
    private int healAmount = 50;

    public Potion(GameObject gameObject) : base(gameObject)
    {
    }

    public Potion(GameObject gameObject, GameObject player) : base(gameObject)
    {
        playerGo = player;
    }

    public override void Awake()
    {
        base.Awake();
        collider = GameObject.GetComponent<Collider>();
        collider.SetCollisionBox(10, 15);
        spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        playerCollider = playerGo.GetComponent<Collider>();
        player = playerGo.GetComponent<Player>();
        health = playerGo.GetComponent<Health>();
    }

    public override void Update(GameTime gameTime)
    {
        OnCollisionEnter(collider);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        // Skal kun fjerne item ved player position, ikke alle items.
        if (collider.CollisionBox.Intersects(playerCollider.CollisionBox))
        {
            if (player.CanPickUpItem(this))
            {
                GameObject.IsEnabled = false;
            }
        }
    }

    public void Use()
    {
        if (!health.AddHealth(healAmount)) return; // Already full health
        player.ItemInInventory = null;
        GameWorld.Instance.Destroy(GameObject);
    }
}