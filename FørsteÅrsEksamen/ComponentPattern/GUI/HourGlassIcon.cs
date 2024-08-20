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

        private Animator animator;
        private float timer;
        private float resetCooldown = 2f;
        private bool startRotate;

        public HourGlassIcon(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            animator = GameObject.GetComponent<Animator>();

            GameObject.Transform.Position += new Vector2(270f, 0);
            
            PlayNormalAnim();
        }

        public override void Update()
        {
            if (!startRotate) return;

            timer += (float)GameWorld.DeltaTime;

            float normalized = timer / resetCooldown;

            normalized = BaseMath.EaseOutCubic(normalized);
            GameObject.Transform.Rotation = MathHelper.Lerp(0, MathHelper.Pi, normalized);

            if (timer < resetCooldown) return;

            timer = 0;
            startRotate = false;
            GameObject.Transform.Rotation = 0;

            animator.PlayAnimation(AnimNames.HourGlassReset);
            animator.CurrentAnimation.OnAnimationDone += PlayNormalAnim;
        }

        private void PlayNormalAnim()
        {
            animator.PlayAnimation(AnimNames.HourGlass);
            animator.StopCurrentAnimationAtLastSprite();
            animator.CurrentAnimation.OnAnimationDone += () => { startRotate = true; };
        }
    }
}
