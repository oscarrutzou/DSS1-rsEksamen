using ShamansDungeon.ComponentPattern.Particles.Origins;
using ShamansDungeon.ComponentPattern.Particles;
using ShamansDungeon.GameManagement;
using Microsoft.Xna.Framework;
using ShamansDungeon.Factory;
using System.Collections.Generic;
using System;
using ShamansDungeon.LiteDB;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

namespace ShamansDungeon.ComponentPattern.WorldObjects;

public class BreakableItem : Component
{
    private SpriteRenderer _sr;
    private Health _health;
    private Collider _collider;
    private ParticleEmitter _damageTakenEmitter;
    private BreakableItemType _type;
    private static Vector2 _drawPosOffset = new Vector2(0, -32);

    private TextureNames _startTexture, _50pctTexture, _finalTexture;
    private SoundNames[] _hitSounds, _brokenSounds;
    private Random _rnd = new();

    private static int _maxAmountBetweenDropSpawns = 3;
    private static int _lastSpawn = 0;

    private static List<PotionTypes> _spawnableTypes = new()
    {
        PotionTypes.SmallSpeedBoost,
        PotionTypes.SmallDmgBoost,
    };
    private static SoundNames[] _vaseHit = new SoundNames[]
    {
        SoundNames.VaseHit,
    };

    private static SoundNames[] _vaseBroken = new SoundNames[]
    {
        SoundNames.VaseHit,
    };

    private static SoundNames[] _woodItemHit = new SoundNames[]
    {
        SoundNames.WoodItemHit,
    };
    private static SoundNames[] _woodItemBroken = new SoundNames[]
    {
        SoundNames.WoodItemBroken1,
        SoundNames.WoodItemBroken2,
        SoundNames.WoodItemBroken3,
    };

    public BreakableItem(GameObject gameObject) : base(gameObject)
    {
    }
    public BreakableItem(GameObject gameObject, BreakableItemType type) : base(gameObject)
    {
        _type = type;
    }

    public override void Awake()
    {
        SetBasedOnType();

        _sr = GameObject.GetComponent<SpriteRenderer>();
        _sr.SetLayerDepth(LayerDepth.BackgroundDecoration); // Set it based on the player pos.
        _sr.SetSprite(_startTexture);
        _sr.DrawPosOffSet = _drawPosOffset;

        _collider = GameObject.GetComponent<Collider>();
        _collider.SetColliderLayer(ColliderLayer.BreakalbeItems);
        _collider.SetCollisionBox(16, 16);

        _health = GameObject.GetComponent<Health>();
        _health.SetHealth(100);

        _health.On50Hp += () =>
        {
            _sr.SetSprite(_50pctTexture);
            _sr.DrawPosOffSet = _drawPosOffset;
        };

        _health.OnZeroHealth += OnZeroHealth;
        _health.AmountDamageTaken += OnDamageTakenText;

        MakeEmitters();
    }

    private void SetBasedOnType()
    {
        switch (_type)
        {
            case BreakableItemType.FatVase:
                _startTexture = TextureNames.FatVase1;
                _50pctTexture = TextureNames.FatVase2;
                _finalTexture = TextureNames.FatVase3;
                _hitSounds = _vaseHit;
                _brokenSounds = _vaseBroken;

                break;

            case BreakableItemType.FatVaseBlue:
                _startTexture = TextureNames.FatVaseBlue;
                _50pctTexture = TextureNames.FatVase2;
                _finalTexture = TextureNames.FatVase3;
                _hitSounds = _vaseHit;
                _brokenSounds = _vaseBroken;

                break;
            
            case BreakableItemType.FatVaseRed:
                _startTexture = TextureNames.FatVaseRed;
                _50pctTexture = TextureNames.FatVase2;
                _finalTexture = TextureNames.FatVase3;
                _hitSounds = _vaseHit;
                _brokenSounds = _vaseBroken;

                break;
            
            case BreakableItemType.LongVase:
                _startTexture = TextureNames.LongVase1;
                _50pctTexture = TextureNames.LongVase2;
                _finalTexture = TextureNames.LongVase3;
                _hitSounds = _vaseHit;
                _brokenSounds = _vaseBroken;

                break;
            
            case BreakableItemType.LongVaseBlue:
                _startTexture = TextureNames.LongVaseBlue;
                _50pctTexture = TextureNames.LongVase2;
                _finalTexture = TextureNames.LongVase3;
                _hitSounds = _vaseHit;
                _brokenSounds = _vaseBroken;

                break;
            
            case BreakableItemType.LongVaseRed:
                _startTexture = TextureNames.LongVaseRed;
                _50pctTexture = TextureNames.LongVase2;
                _finalTexture = TextureNames.LongVase3;
                _hitSounds = _vaseHit;
                _brokenSounds = _vaseBroken;

                break;
            
            case BreakableItemType.Barrel:
                _startTexture = TextureNames.Barrel1;
                _50pctTexture = TextureNames.Barrel2;
                _finalTexture = TextureNames.Barrel3;
                _hitSounds = _woodItemHit;
                _brokenSounds = _woodItemBroken;
              
                break;
            
            case BreakableItemType.Crate:
                _startTexture = TextureNames.Crate1;
                _50pctTexture = TextureNames.Crate2;
                _finalTexture = TextureNames.Crate3;
                _hitSounds = _woodItemHit;
                _brokenSounds = _woodItemBroken;
                
                break;
        }
    }


    private void OnZeroHealth()
    {
        _sr.SetSprite(_finalTexture);
        _sr.DrawPosOffSet = _drawPosOffset;

        GlobalSounds.PlayRandomizedSound(_brokenSounds, 1, 0.8f, true);

        if (_lastSpawn > 0)
        {
            _lastSpawn--;
            return;
        }
        // play destroy sound
        // pop out new item
        bool _shouldSpawn = _rnd.Next(3) == 0;
        if (!_shouldSpawn || SaveData.Player == null) return;
        
        GameObject go = ItemFactory.CreatePotionWithRandomType(SaveData.Player.GameObject, _spawnableTypes);
        go.Transform.Position = GameObject.Transform.Position;
        go.Transform.GridPosition = GameObject.Transform.GridPosition;
        GameWorld.Instance.Instantiate(go);

        _lastSpawn = _maxAmountBetweenDropSpawns;
    }

    private void MakeEmitters()
    {
        GameObject textDamageEmitterGo = EmitterFactory.TextDamageEmitter(new Color[] { Color.OrangeRed, Color.DarkRed, Color.Transparent }, GameObject, new Vector2(-20, -65), new RectangleOrigin(50, 5));
        _damageTakenEmitter = textDamageEmitterGo.GetComponent<ParticleEmitter>();

        GameWorld.Instance.Instantiate(textDamageEmitterGo);
    }

    private void OnDamageTakenText(int damage)
    {
        _damageTakenEmitter.LayerName = LayerDepth.DamageParticles;
        _damageTakenEmitter.SetParticleText(new TextOnSprite() { Text = damage.ToString() });
        _damageTakenEmitter.EmitParticles();

        GlobalSounds.PlayRandomizedSound(_hitSounds, 1, 1, true);
    }
}
