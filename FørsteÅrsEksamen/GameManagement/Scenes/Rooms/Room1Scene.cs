using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies.MeleeEnemies;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

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

        GameObject trainingDummyGo = TraningDummyFactory.Create();
        _trainingDummy = trainingDummyGo.GetComponent<TrainingDummy>();

        MiscGameObjectsInRoom = new()
        {
            { new Point(13, 5), trainingDummyGo}
        };

    }
    private TrainingDummy _trainingDummy;
    public override void Update()
    {
        base.Update();
        _soundPos = GridManager.Instance.CurrentGrid.PosFromGridPos(new Point(10, 21));

        TestSoundDistance();
    }

    private SoundEffectInstance _currentPlayingSoundEffect;
    private Vector2 _soundPos;

    // Make a class that has the values for the sound, like a point for pos. Also has a centered rectangle for debug
    // Also a bool if it should keep playing
    private void TestSoundDistance()
    {
        
        if (_currentPlayingSoundEffect != null && _currentPlayingSoundEffect.State == SoundState.Playing)
        {
            // Update sound
            GlobalSounds.ChangeSoundVolumeDistance(_soundPos, 200, 500, 1, _currentPlayingSoundEffect);
        }
        else
        {
            //if (GlobalSounds.IsAnySoundPlaying(SoundNames.ButtonClicked)) return;

            _currentPlayingSoundEffect = GlobalSounds.PlaySound(SoundNames.ButtonClicked, 2, 0.5f, true);
        }
    }
    // Make a debug 

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        if (string.IsNullOrEmpty(_trainingDummy.Text)) return;
        spriteBatch.DrawString(GlobalTextures.DefaultFont, _trainingDummy.Text, _trainingDummy.TextPosition, BaseMath.TransitionColor(CurrentTextColor), 0f, Vector2.Zero, 1, SpriteEffects.None, SpriteRenderer.GetLayerDepth(LayerDepth.UI));
    }
}