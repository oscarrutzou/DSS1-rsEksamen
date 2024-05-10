using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Characters
{
    // Oscar
    public class Player : Component, ISubject
    {
        private float speed;
        private Vector2 velocity = Vector2.Zero;
        public Vector2 targetVelocity = Vector2.Zero;
        private float turnSpeed = 40f; // Adjust as needed

        public Player(GameObject gameObject) : base(gameObject)
        {
            speed = 300;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>();
            sr.SetLayerDepth(LAYERDEPTH.Player);

            Animator animator = GameObject.GetComponent<Animator>();
            animator.AddAnimation(AnimNames.KnightIdle); //Set all the animations
            animator.PlayAnimation(AnimNames.KnightIdle);
        }

        private Vector2 totalInput = Vector2.Zero;

        public void AddInput(Vector2 input)
        {
            totalInput += input;
        }

        public void Move(Vector2 velocity)
        {
            // Add the additionalVelocity to the current targetVelocity
            targetVelocity += Vector2.Normalize(velocity);

            // Normalize the targetVelocity in case it's length is more than 1
            if (velocity != Vector2.Zero)
            {
                targetVelocity = Vector2.Normalize(targetVelocity);
            }
            else
            {
                targetVelocity = Vector2.Zero;
            }

            this.velocity = Vector2.Lerp(this.velocity, targetVelocity, turnSpeed * GameWorld.DeltaTime);
            GameObject.Transform.Translate(this.velocity * speed * GameWorld.DeltaTime);
            GameWorld.Instance.WorldCam.Move(this.velocity * speed * GameWorld.DeltaTime);

            Notify();
        }

        public override void Update(GameTime gameTime)
        {
            if (totalInput != Vector2.Zero)
            {
                totalInput = Vector2.Normalize(totalInput);
            }

            Move(totalInput);
            totalInput = Vector2.Zero;
        }

        private IObserver testSceneObserver;

        public void Attach(IObserver observer)
        {
            testSceneObserver = observer;
        }

        public void Detach(IObserver observer)
        {
        }

        public void Notify()
        {
            testSceneObserver.UpdateObserver();
        }
    }
}