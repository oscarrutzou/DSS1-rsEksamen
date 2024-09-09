using ShamansDungeon.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ComponentPattern.Effects
{
    public class TeleportEffect : Component
    {
        private float _teleportEffectAmount;
        private float _dir;
        private float _startDir;
        private bool _stoppedTeleport;
        public float SpawnSpeed = 2f;
        public bool StartFromTop = false;
        public Action OnStopTeleport;
        private bool _playEffect = false;
        public bool PlayEffect
        {
            get => _playEffect;
            set
            {
                _playEffect = value;
                CheckIfCanPlayEffect();
            }
        }


        public TeleportEffect(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            MakeShader();
        }

        private void MakeShader()
        {
            GameObject.ShaderEffect = GlobalTextures.TeleportEffect.Clone();

            ResetEffect();
            CheckIfCanPlayEffect();

            GameObject.ShaderEffect.Parameters["amount"].SetValue(_teleportEffectAmount);
        }

        private void CheckIfCanPlayEffect()
        {
            if (GameObject.ShaderEffect == null) return;

            if (PlayEffect)
                GameObject.ShaderEffect.CurrentTechnique = GameObject.ShaderEffect.Techniques["Teleport"];
            else
                GameObject.ShaderEffect.CurrentTechnique = GameObject.ShaderEffect.Techniques["Basic"];
        }

        private void ResetEffect()
        {
            if (!StartFromTop)  // Dont change the start values
            {
                _teleportEffectAmount = 1;
                _dir = -1;
            }
            else
            {
                _teleportEffectAmount = 0;
                _dir = 1;
            }

            _startDir = _dir;
        }

        // If we want to change the direction multiple times, then use this:d
        public override void Update()
        {
            if (!PlayEffect || _stoppedTeleport) return; // Only play on the right times

            _teleportEffectAmount += (float)GameWorld.DeltaTime * _dir * SpawnSpeed;
            GameObject.ShaderEffect.Parameters["amount"].SetValue(_teleportEffectAmount);

            // Maybe a sound here? Ofc distance sound

            if (_teleportEffectAmount < 0 || _teleportEffectAmount > 1)
            {
                StopEffect();
            }
        }

        public void StartEffect()
        {
            PlayEffect = true;
            GlobalSounds.PlaySound(SoundNames.Teleport, 1, 1f, true, -0.2f, 0.2f);
        }

        public void StopEffect()
        {
            _dir = _startDir;
            _teleportEffectAmount = 0;
            _stoppedTeleport = true;
            GameObject.ShaderEffect.CurrentTechnique = GameObject.ShaderEffect.Techniques["Basic"];
            
            OnStopTeleport?.Invoke(); // E.g for the gameobject to disappear
        }
    }
}
