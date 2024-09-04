using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.GUI
{
    public class TriggerRectanglePlayer : Component
    {
        public Action OnEnter;
        private Collider _collider, _playerCollider;
        private Point _topLeftCorner, _bottomRightCorner;

        public TriggerRectanglePlayer(GameObject gameObject) : base(gameObject)
        {
        }

        public TriggerRectanglePlayer(GameObject gameObject, Point topLeftCorner, Point bottomRightCorner, Action onEnter) : base(gameObject)
        {
            OnEnter = onEnter;
            _topLeftCorner = topLeftCorner;
            _bottomRightCorner = bottomRightCorner;
        }

        public override void Start()
        {
            _collider = GameObject.GetComponent<Collider>();    
            // Find pos of the corners
            Vector2 leftPos = GridManager.Instance.GetCornerPositionOfCell(_topLeftCorner);
            GameObject.Transform.Position = leftPos;

            Vector2 rightBottom = GridManager.Instance.GetCornerPositionOfCell(_bottomRightCorner) + new Vector2(Cell.Dimension * Cell.Scale, Cell.Dimension * Cell.Scale);

            Vector2 totalDistance = new (Math.Abs(leftPos.X- rightBottom.X), Math.Abs(leftPos.Y - rightBottom.Y));

            _collider.SetCollisionBox((int)totalDistance.X, (int)totalDistance.Y);
        }
        
        // Could make this so it would check the objects in a Gameobject type.
        public override void Update()
        {
            if (SaveData.Player == null || OnEnter == null) return;

            _playerCollider = SaveData.Player.movementCollider;

            if (!_collider.CollisionBox.Intersects(_playerCollider.CollisionBox)) return;
            
            OnEnter?.Invoke();
            OnEnter = null;
        }
    }
}
