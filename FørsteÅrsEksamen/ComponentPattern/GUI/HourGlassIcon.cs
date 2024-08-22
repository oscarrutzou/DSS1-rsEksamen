using DoctorsDungeon.GameManagement;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.GUI
{
    public class HourGlassIcon : Component
    {

        private Animator _animator;
        private float _timer;
        private float _resetCooldown = 2f;
        private bool _startRotate;

        public HourGlassIcon(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            _animator = GameObject.GetComponent<Animator>();

            GameObject.Transform.Position += new Vector2(270f, 0);
            
            PlayNormalAnim();
        }

        public override void Update()
        {
            if (!_startRotate) return;

            _timer += (float)GameWorld.DeltaTime;

            float normalized = _timer / _resetCooldown;

            normalized = BaseMath.EaseOutCubic(normalized);
            GameObject.Transform.Rotation = MathHelper.Lerp(0, MathHelper.Pi, normalized);

            if (_timer < _resetCooldown) return;

            _timer = 0;
            _startRotate = false;
            GameObject.Transform.Rotation = 0;

            _animator.PlayAnimation(AnimNames.HourGlassReset);
            _animator.CurrentAnimation.OnAnimationDone += PlayNormalAnim;
        }

        private void PlayNormalAnim()
        {
            _animator.PlayAnimation(AnimNames.HourGlass);
            _animator.StopCurrentAnimationAtLastSprite();
            _animator.CurrentAnimation.OnAnimationDone += () => { _startRotate = true; };
        }
    }
}
