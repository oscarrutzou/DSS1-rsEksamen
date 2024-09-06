using ShamansDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.ComponentPattern.WorldObjects;

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
    private SpriteRenderer _spriteRenderer;
    private Collider _collider, _playerCollider;
    private Player _player;
    private Health _health;
    private GameObject _playerGo;

    public string Name = "Strong Health Potion";
    private int _healAmount = 100;

    //private int _maxAmountOfUses = 1;
    public Potion(GameObject gameObject) : base(gameObject)
    { 
    }

    public Potion(GameObject gameObject, GameObject player) : base(gameObject)
    {
        _playerGo = player;
    }

    public override void Awake()
    {
        base.Awake();
        _collider = GameObject.GetComponent<Collider>();
        _collider.SetCollisionBox(10, 15);
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        _playerCollider = _playerGo.GetComponent<Collider>();
        _player = _playerGo.GetComponent<Player>();
        _health = _playerGo.GetComponent<Health>();
    }

    public override void Update()
    {
        OnCollisionEnter(_collider);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        // Skal kun fjerne item ved player position, ikke alle items.
        if (collider.CollisionBox.Intersects(_playerCollider.CollisionBox))
        {
            if (_player.CanPickUpItem(this))
            {
                GameObject.IsEnabled = false;
            }
        }
    }

    public void Use()
    {
        if (_health.IsDead || !_health.AddHealth(_healAmount)) return; // Already full health
        // Destroy next frame
        //_removeFromInventoryNextFrame = true;
        _player.ItemInInventory = null;
        _player.HasUsedItem = true;
        GameWorld.Instance.Destroy(GameObject);
    }

}