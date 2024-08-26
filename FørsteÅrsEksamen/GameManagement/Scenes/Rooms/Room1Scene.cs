using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room1Scene : RoomBase
{
    public override void Initialize()
    {
        GridName = "Level1";
        GridWidth = 40;
        GridHeight = 28;

        SaveData.Level_Reached = 1;

        BackGroundTexture = TextureNames.Level1BG;
        ForeGroundTexture = TextureNames.Level1FG;

        base.Initialize();
    }


    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(10, 3);
        EndPointSpawnPos = new Point(33, 2);

        EnemySpawnPoints = new() {
        new Point(10, 21),
        new Point(25, 21),
        new Point(37, 12),};

        PotionSpawnPoints = new() {
        new Point(7, 4),
        new Point(29, 9),};

        MiscGameObjectsInRoom = new()
        {
            { new Point(13, 5), TraningDummyFactory.Create() }
        };

    }

    public override void Update()
    {
        base.Update();
        _soundPos = GridManager.Instance.CurrentGrid.PosFromGridPos(new Point(10, 21));

        TestSoundDistance();
    }

    private SoundEffectInstance _currentPlayingSoundEffect;
    private Vector2 _soundPos;
    private void TestSoundDistance()
    {
        
        if (_currentPlayingSoundEffect != null && _currentPlayingSoundEffect.State == SoundState.Playing)
        {
            // Update sound
            GlobalSounds.ChangeSoundVolumeDistance(_soundPos, 200, 500, 1, _currentPlayingSoundEffect);
        }
        else
        {
            if (GlobalSounds.IsAnySoundPlaying(SoundNames.ButtonClicked)) return;

            _currentPlayingSoundEffect = GlobalSounds.PlaySound(SoundNames.ButtonClicked, 1, 0.5f, true);
        }
    }


}