using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement;

public enum SoundNames
{
    SwipeFast1,
    SwipeFast2,
    SwipeSlow1,
}
// Oscar
public static class GlobalSounds
{
    #region Properties

    //Sound effects
    public static Dictionary<SoundNames, SoundEffect> Sounds { get; private set; }

    private static Dictionary<SoundNames, List<SoundEffectInstance>> soundInstancesPool;
    private static int maxInstanceOfOneSound = 5; // There can only be 2 of the same sounds playing, otherwise it wont play.
    //private static int maxInstanceOfGunSound = 10;

    public static bool InMenu { get; set; } = true;

    private static SoundEffect menuMusic;
    private static SoundEffect gameMusic;

    private static SoundEffectInstance instanceMenuMusic;
    private static SoundEffectInstance instanceGameMusic;

    private static int musicVolDivide = 4; //Makes the song less loud by dividing the real volume
    private static Random rnd = new();
    public static float MusicVolume = 0.5f;
    public static float SfxVolume = 0.5f;
    private static bool musicCountDown;
    private static bool sfxCountDown;

    #endregion Properties

    public static void LoadContent()
    {
        musicCountDown = MusicVolume != 0; // For the method to change up and down on volume
        sfxCountDown = SfxVolume != 0;

        ContentManager content = GameWorld.Instance.Content;

        soundInstancesPool = new Dictionary<SoundNames, List<SoundEffectInstance>>();

        // Loads music
        menuMusic = content.Load<SoundEffect>("Sound\\MenuTrack");
        gameMusic = content.Load<SoundEffect>("Sound\\GameTrack");

        // Loads SFX's
        Sounds = new Dictionary<SoundNames, SoundEffect>
        {
            {SoundNames.SwipeFast1, content.Load<SoundEffect>("Sound\\SwipeFast1") },
            {SoundNames.SwipeFast2, content.Load<SoundEffect>("Sound\\SwipeFast2") },
            {SoundNames.SwipeSlow1, content.Load<SoundEffect>("Sound\\SwipeSlow1") },
        };

        //Create sound instances for the sound pool
        foreach (var sound in Sounds)
        {
            soundInstancesPool[sound.Key] = new List<SoundEffectInstance>();
            int max = maxInstanceOfOneSound;
            //if (sound.Key == SoundNames.Shot || sound.Key == SoundNames.Shotgun) max = maxInstanceOfGunSound;

            for (int i = 0; i < max; i++)
            {
                soundInstancesPool[sound.Key].Add(sound.Value.CreateInstance());
            }
        }
    }

    public static void MusicUpdate()
    {
        if (instanceGameMusic == null || instanceMenuMusic == null)
        {
            instanceMenuMusic = menuMusic.CreateInstance();
            instanceGameMusic = gameMusic.CreateInstance();
        }

        // Make sure the volume is lower that the SFX's, since the SFX are more impactfull.
        instanceMenuMusic.Volume = Math.Clamp(MusicVolume, 0, 1) / musicVolDivide;
        instanceGameMusic.Volume = Math.Clamp(MusicVolume, 0, 1) / musicVolDivide;

        //Check if the music should be playing
        if (InMenu)
        {
            instanceGameMusic.Stop(); // Stops it once and does nothing if its already stopped
        }
        else
        {
            instanceMenuMusic.Stop(); // Stops it once and does nothing if its already stopped
        }

        if (instanceMenuMusic.State == SoundState.Stopped && InMenu)
        {
            instanceMenuMusic.Play(); // Play only plays it once and does nothing if it already plays
        }

        if (instanceGameMusic.State == SoundState.Stopped && !InMenu)
        {
            instanceGameMusic.Play();// Play only plays it once and does nothing if it already plays
        }
    }

    /// <summary>
    /// Changes the volume of music up or down with 25% each time.
    /// </summary>
    public static void ChangeMusicVolume()
    {
        // If the bool musicCountDown is true, we go down in volume towards 0, if its false we can go up until we hit 1.
        MusicVolume = musicCountDown ? Math.Max(0, MusicVolume - 0.25f) : Math.Min(1, MusicVolume + 0.25f);
        if (MusicVolume == 0 || MusicVolume == 1) musicCountDown = !musicCountDown; // Reverse the change direction
    }

    /// <summary>
    /// Changes the volume of sfx up or down with 25% each time.
    /// </summary>
    public static void ChangeSfxVolume()
    {
        // If the bool sfxCountDown is true, we go down in volume towards 0, if its false we can go up until we hit 1.
        SfxVolume = sfxCountDown ? Math.Max(0, SfxVolume - 0.25f) : Math.Min(1, SfxVolume + 0.25f);
        if (SfxVolume == 0 || SfxVolume == 1) sfxCountDown = !sfxCountDown;// Reverse the change direction
    }

    public static bool IsAnySoundPlaying(SoundNames[] soundArray)
    {
        //Check if any sound is playing
        foreach (SoundNames name in soundArray)
        {
            foreach (SoundEffectInstance inst in soundInstancesPool[name])
            {
                if (inst.State == SoundState.Playing)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="soundName">The sound to play</param>
    /// <param name="soundVolDivided">Can change how loud the sound is</param>
    /// <param name="enablePitch">If it should add a random pitch to the sounds</param>
    public static void PlaySound(SoundNames soundName, float soundVolDivided = 1f, bool enablePitch = false)
    {
        // Play a sound with an optional random pitch
        float pitch = enablePitch ? GenerateRandomPitch() : 0f; // Base pitch is 0f

        // Play a sound
        SoundEffectInstance instance = GetAvailableInstance(soundName);
        if (instance == null)
        {
            // All instances are playing, so stop and reuse the oldest one.
            instance = soundInstancesPool[soundName][0];
            instance.Stop();
        }

        instance.Pitch = pitch;
        instance.Volume = SfxVolume / soundVolDivided;
        instance.Play();
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
        int soundIndex = rnd.Next(0, soundArray.Length);
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
        foreach (var inst in soundInstancesPool[soundName])
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
        foreach (SoundEffectInstance inst in soundInstancesPool[soundName])
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
        float pitch = (float)rnd.NextDouble() * (maxPitch - minPitch) + minPitch;
        return pitch;
    }
}