using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern.Enemies;
using ShamansDungeon.GameManagement;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using System;

namespace ShamansDungeon.ComponentPattern.Weapons.RangedWeapons;

// Erik
public class Projectile : Component
{
    private float _speed;
    private Vector2 _direction;
    private float _range;
    private Vector2 _startPos;
    private Vector2 _lerpTo;
    private Vector2 _targetPos;

    public Projectile(GameObject gameObject) : base(gameObject)
    {
        this._speed = 100;
        this._range = 200;
    }

    public override void Start()
    {
        SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>();
        sr.SetLayerDepth(LayerDepth.Player);

        sr.SetSprite(TextureNames.WoodArrow);
    }

    public void SetValues(float rotation)
    {
        GameObject.Transform.Rotation = rotation;
        _startPos = GameObject.Transform.Position;

        _lerpTo = BaseMath.Rotate(_startPos + new Vector2(0, _range), rotation);
        SetDirection();
    }

    public void SetDirection()
    {
        _targetPos = InputHandler.Instance.MouseOnUI;

        _direction = BaseMath.SafeNormalize(_targetPos - GameObject.Transform.Position);

        float angle = (float)Math.Atan2(_direction.Y, _direction.X);
        GameObject.Transform.Rotation = angle;
    }

    public override void Update()
    {
        Move();
    }

    private void Move()
    {
        //if (targetPos == Vector2.Zero) return;

        //GameObject.Transform.Position = Vector2.Lerp(startPos, targetPos, GameWorld.DeltaTime * speed);

        //if (Math.Abs(GameObject.Transform.Position.X - targetPos.X) < 0.01f &&
        //    Math.Abs(GameObject.Transform.Position.Y - targetPos.Y) < 0.01f)
        //{
        //    GameWorld.Instance.Destroy(GameObject);

        //}

        double distance = _speed * GameWorld.DeltaTime;
        Vector2 step = _direction * (float)distance;

        GameObject.Transform.Position += step;

        if (Vector2.Distance(_startPos, GameObject.Transform.Position) >= _range)
        {
            GameWorld.Instance.Destroy(GameObject);
        }
    }

    public override void OnCollisionEnter(Collider collider)
    {
        if (collider.GameObject.GetComponent<Enemy>() != null)
        {
            GameWorld.Instance.Destroy(GameObject);
        }
    }

    public void Attack()
    {
    }
}