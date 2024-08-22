using DoctorsDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoctorsDungeon.ComponentPattern.WorldObjects;

// Potions could be like a buff. 
/*
 * Need amount of time buff is on = -1 for permeant.
 * Need value to contain the previous value
 * Need to save the different buffs given, if loading a save
 * Need to have the name on.
 */

// Asser
public class Potion : Component
{
    private SpriteRenderer spriteRenderer;
    private Collider collider, playerCollider;
    private Player player;
    private Health health;
    private GameObject playerGo;

    public string Name = "Strong Health Potion";
    private int healAmount = 100;

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

    public override void Update()
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
        if (health.IsDead || !health.AddHealth(healAmount)) return; // Already full health
        player.ItemInInventory = null;
        GameWorld.Instance.Destroy(GameObject);
    }
}