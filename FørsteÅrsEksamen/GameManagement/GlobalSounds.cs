using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement;

public enum SoundNames
{
    // UI -----------------------------------
    ButtonHover,
    ButtonClicked,

    // Weapon  ------------------------------
    //SwipeFast1,
    //SwipeFast2,
    //SwipeFast3,
    SwipeFast4,
    SwipeFast5,

    SwipeHeavy1,
    SwipeHeavy2,
    SwipeHeavy3,
    //SwipeHeavy4,

    // Weapon Hit  --------------------------
    WoodHitFlesh,
    WoodHitLeather,
    WoodHitMetal,

    // Enemy Hit   --------------------------
    EnemyHit1,
    EnemyHit2,
    EnemyHit3,
    EnemyHit4,
    EnemyHit5,
    EnemyHit6,
    EnemyHit7,

    // Player Hit  --------------------------
    PlayerHit1,
    PlayerHit2,
    PlayerHit3,
    PlayerHit4,
    PlayerHit5,
    PlayerHit6,
    PlayerHit7,
    PlayerHit8,
}

// Oscar
public static class GlobalSounds
{
    #region Properties

    //Sound effects
    public static Dictionary<SoundNames, SoundEffect> Sounds { get; private set; }

    private static Dictionary<SoundNames, List<SoundEffectInstance>> _soundInstancesPool;
    private static int _maxInstanceOfOneSound = 5; // There can only be 2 of the same sounds playing, otherwise it wont play.
    //private static int maxInstanceOfGunSound = 10;

    public static bool InMenu { get; set; } = true;

    private static SoundEffect _menuMusic;
    private static SoundEffect _gameMusic;

    private static SoundEffectInstance _instanceMenuMusic;
    private static SoundEffectInstance _instanceGameMusic;

    private static int _musicVolDivide = 4; //Makes the song less loud by dividing the real volume
    private static Random _rnd = new();
    public static float MusicVolume = 0.0f;
    public static float SfxVolume = 0.5f;
    private static bool _musicCountDown;
    private static bool _sfxCountDown;

    #endregion Properties

    public static void LoadContent()
    {
        _musicCountDown = MusicVolume != 0; // For the method to change up and down on volume
        _sfxCountDown = SfxVolume != 0;

        ContentManager content = GameWorld.Instance.Content;

        _soundInstancesPool = new Dictionary<SoundNames, List<SoundEffectInstance>>();

        // Loads music
        _menuMusic = content.Load<SoundEffect>("Sound\\Music\\MenuTrack");
        _gameMusic = content.Load<SoundEffect>("Sound\\Music\\GameTrack");

        // Loads SFX's
        Sounds = new Dictionary<SoundNames, SoundEffect>
        {
            {SoundNames.ButtonHover, content.Load<SoundEffect>("Sound\\Gui\\HoverButton") },
            {SoundNames.ButtonClicked, content.Load<SoundEffect>("Sound\\Gui\\PressedButton2") },


            //{SoundNames.SwipeFast1, content.Load<SoundEffect>("Sound\\Attack\\Slash_Attack_Light_1") },
            //{SoundNames.SwipeFast2, content.Load<SoundEffect>("Sound\\Attack\\Slash_Attack_Light_2") },
            //{SoundNames.SwipeFast3, content.Load<SoundEffect>("Sound\\Attack\\Slash_Attack_Light_3") },
            {SoundNames.SwipeFast4, content.Load<SoundEffect>("Sound\\Attack\\SwipeFast1") },
            {SoundNames.SwipeFast5, content.Load<SoundEffect>("Sound\\Attack\\SwipeFast2") },

            {SoundNames.SwipeHeavy1, content.Load<SoundEffect>("Sound\\Attack\\Slash_Attack_Heavy_1") },
            {SoundNames.SwipeHeavy2, content.Load<SoundEffect>("Sound\\Attack\\Slash_Attack_Heavy_2") },
            {SoundNames.SwipeHeavy3, content.Load<SoundEffect>("Sound\\Attack\\Slash_Attack_Heavy_3") },
            //{SoundNames.SwipeHeavy4, content.Load<SoundEffect>("Sound\\Attack\\SwipeSlow1") },


            {SoundNames.WoodHitFlesh, content.Load<SoundEffect>("Sound\\Hit\\Hit_Wood_on_flesh") },
            {SoundNames.WoodHitLeather, content.Load<SoundEffect>("Sound\\Hit\\Hit_Wood_on_leather") },
            {SoundNames.WoodHitMetal, content.Load<SoundEffect>("Sound\\Hit\\Hit_Wood_on_metal") },


            {SoundNames.EnemyHit1, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (1)") },
            {SoundNames.EnemyHit2, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (2)") },
            {SoundNames.EnemyHit3, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (3)") },
            {SoundNames.EnemyHit4, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (4)") },
            {SoundNames.EnemyHit5, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (5)") },
            {SoundNames.EnemyHit6, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (6)") },
            {SoundNames.EnemyHit7, content.Load<SoundEffect>("Sound\\EnemyHurt\\Hit (7)") },


            {SoundNames.PlayerHit1, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (1)") },
            {SoundNames.PlayerHit2, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (2)") },
            {SoundNames.PlayerHit3, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (3)") },
            {SoundNames.PlayerHit4, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (4)") },
            {SoundNames.PlayerHit5, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (5)") },
            {SoundNames.PlayerHit6, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (6)") },
            {SoundNames.PlayerHit7, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (7)") },
            {SoundNames.PlayerHit8, content.Load<SoundEffect>("Sound\\PlayerHurt\\Hit (8)") },
        };

        //Create sound instances for the sound pool
        foreach (var sound in Sounds)
        {
            _soundInstancesPool[sound.Key] = new List<SoundEffectInstance>();
            int max = _maxInstanceOfOneSound;
            //if (sound.Key == SoundNames.Shot || sound.Key == SoundNames.Shotgun) max = maxInstanceOfGunSound;

            for (int i = 0; i < max; i++)
            {
                _soundInstancesPool[sound.Key].Add(sound.Value.CreateInstance());
            }
        }
    }

    public static void MusicUpdate()
    {
        if (_instanceGameMusic == null || _instanceMenuMusic == null)
        {
            _instanceMenuMusic = _menuMusic.CreateInstance();
            _instanceGameMusic = _gameMusic.CreateInstance();
        }

        // Make sure the volume is lower that the SFX's, since the SFX are more impactfull.
        _instanceMenuMusic.Volume = Math.Clamp(MusicVolume, 0, 1) / _musicVolDivide;
        _instanceGameMusic.Volume = Math.Clamp(MusicVolume, 0, 1) / _musicVolDivide;

        //Check if the music should be playing
        if (InMenu)
        {
            _instanceGameMusic.Stop(); // Stops it once and does nothing if its already stopped
        }
        else
        {
            _instanceMenuMusic.Stop(); // Stops it once and does nothing if its already stopped
        }

        if (_instanceMenuMusic.State == SoundState.Stopped && InMenu)
        {
            _instanceMenuMusic.Play(); // Play only plays it once and does nothing if it already plays
        }

        if (_instanceGameMusic.State == SoundState.Stopped && !InMenu)
        {
            _instanceGameMusic.Play();// Play only plays it once and does nothing if it already plays
        }
    }

    /// <summary>
    /// Changes the volume of music up or down with 25% each time.
    /// </summary>
    public static void ChangeMusicVolume()
    {
        // If the bool musicCountDown is true, we go down in volume towards 0, if its false we can go up until we hit 1.
        MusicVolume = _musicCountDown ? Math.Max(0, MusicVolume - 0.25f) : Math.Min(1, MusicVolume + 0.25f);
        if (MusicVolume == 0 || MusicVolume == 1) _musicCountDown = !_musicCountDown; // Reverse the change direction
    }

    /// <summary>
    /// Changes the volume of sfx up or down with 25% each time.
    /// </summary>
    public static void ChangeSfxVolume()
    {
        // If the bool sfxCountDown is true, we go down in volume towards 0, if its false we can go up until we hit 1.
        SfxVolume = _sfxCountDown ? Math.Max(0, SfxVolume - 0.25f) : Math.Min(1, SfxVolume + 0.25f);
        if (SfxVolume == 0 || SfxVolume == 1) _sfxCountDown = !_sfxCountDown;// Reverse the change direction
    }

    public static bool IsAnySoundPlaying(SoundNames[] soundArray)
    {
        //Check if any sound is playing
        foreach (SoundNames name in soundArray)
        {
            foreach (SoundEffectInstance inst in _soundInstancesPool[name])
            {
                if (inst.State == SoundState.Playing)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public static bool IsAnySoundPlaying(SoundNames soundName)
    {
        //Check if any sound is playing
        SoundEffectInstance instance = GetAvailableInstance(soundName);

        if (instance == null) return false;
        if (instance.State == SoundState.Playing) return true;

        return false;
    }

    public static void ChangeSoundVolumeDistance(Vector2 soundPosition, int minDistance, int maxDistance, float maxVolume, SoundEffectInstance soundEffect)
    {
        Vector2 playerPos = Vector2.Zero;
        if (SaveData.Player != null) playerPos = SaveData.Player.GameObject.Transform.Position;

        float distance = Vector2.Distance(playerPos, soundPosition);

        // Calculate the volume based on distance
        float normalizedDistance = MathHelper.Clamp((distance - (float)minDistance) / ((float)maxDistance - (float)minDistance), 0f, 1f);
        float lerpedVolume = MathHelper.Lerp(maxVolume, 0f, normalizedDistance);

        soundEffect.Volume = lerpedVolume;
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="soundName">The sound to play</param>
    /// <param name="soundVolDivided">Can change how loud the sound is</param>
    /// <param name="enablePitch">If it should add a random pitch to the sounds</param>
    public static SoundEffectInstance PlaySound(SoundNames soundName, float soundVolDivided = 1f, bool enablePitch = false)
    {
        // Play a sound with an optional random pitch
        float pitch = enablePitch ? GenerateRandomPitch() : 0f; // Base pitch is 0f

        // Play a sound
        SoundEffectInstance instance = GetAvailableInstance(soundName);
        if (instance == null)
        {
            // All instances are playing, so stop and reuse the oldest one.
            instance = _soundInstancesPool[soundName][0];
            instance.Stop();
        }

        instance.Pitch = pitch;
        instance.Volume = SfxVolume / soundVolDivided;
        instance.Play();

        return instance;
    }

    public static SoundEffectInstance PlaySound(SoundNames soundName, int maxAmountPlaying, float soundVolDivided = 1f, bool enablePitch = false)
    {
        int index = CountPlayingInstances(soundName);

        if (index >= maxAmountPlaying) return null;

        return PlaySound(soundName, soundVolDivided, enablePitch);
    }

    /// <summary>
    /// Can play a random sound in a array
    /// </summary>
    /// <param name="soundArray">The array of different sound effets that can be played</param>
    /// <param name="maxAmountPlaying">How many of the sounds that can play at once</param>
    /// <param name="soundVolDivided">Can change how loud the sound is</param>
    /// <param name="enablePitch">If it should add a random pitch to the sounds</param>
    public static void PlayRandomizedSound(SoundNames[] soundArray, int maxAmountPlaying, float soundVolDivided = 1f, bool enablePitch = false)
    {
        // Play a random sound from the array
        int soundIndex = _rnd.Next(0, soundArray.Length);
        SoundNames soundName = soundArray[soundIndex];

        int index = CountPlayingInstances(soundName);
        if (index >= maxAmountPlaying)
        {
            return;
        }

        PlaySound(soundName, soundVolDivided, enablePitch);
    }




    // Helper method
    private static SoundEffectInstance GetAvailableInstance(SoundNames soundName)
    {
        foreach (var inst in _soundInstancesPool[soundName])
        {
            if (inst.State != SoundState.Playing)
            {
                return inst;
            }
        }
        return null;
    }

    // Helper method
    private static int CountPlayingInstances(SoundNames soundName)
    {
        int count = 0;
        foreach (SoundEffectInstance inst in _soundInstancesPool[soundName])
        {
            if (inst.State == SoundState.Playing)
            {
                count++;
            }
        }
        return count;
    }

    // Helper method
    private static float GenerateRandomPitch(float minPitch = -0.3f, float maxPitch = 0.3f)
    {
        // Generate a random pitch within the specified range
        float pitch = (float)_rnd.NextDouble() * (maxPitch - minPitch) + minPitch;
        return pitch;
    }
}