using ShamansDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShamansDungeon.GameManagement;

namespace ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

// Potions could be like a buff. 
/*
 * Need amount of time buff is on = -1 for permeant.
 * Need value to contain the previous value
 * Need to save the different buffs given, if loading a save
 * Need to have the name on.
 */

// Asser
public enum PotionTypes
{
    SmallHealth,
    BigHealth,
    SmallDmgBoost,
    SmallSpeedBoost,
}

public abstract class Potion : Component
{
    public PotionTypes PotionType;
    public float AmountToAdd { get; protected set; }
    protected string PotionText;
    // Will get shown over the mouse
    public string FullPotionText
    {
        get
        {
            return $"{PotionText}\nAmount of uses = {MaxAmountOfUses}";
        }
    }
    public int MaxAmountOfUses { get; protected set; } = 1;
    
    protected SpriteRenderer SpriteRenderer;
    protected Player Player;
    
    private Collider _collider, _playerCollider;
    
    private GameObject _playerGo;

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

        SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder);
        _playerCollider = _playerGo.GetComponent<Collider>();
        Player = _playerGo.GetComponent<Player>();

        SwitchBetweenSimilarPotions();
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
            if (Player.CanPickUpItem(this))
            {
                GameObject.IsEnabled = false;
                GlobalSounds.PlaySound(SoundNames.PickUp, 1, 1f);
            }
        }
    }

    public abstract void SwitchBetweenSimilarPotions();

    public virtual void Use()
    {
        GlobalSounds.PlaySound(SoundNames.DrinkingPotion, 1);

        DestoryGameObject();
    }

    public void DestoryGameObject()
    {
        Player.ItemInInventory = null;
        Player.HasUsedItem = true;
        GameWorld.Instance.Destroy(GameObject);
    }
}