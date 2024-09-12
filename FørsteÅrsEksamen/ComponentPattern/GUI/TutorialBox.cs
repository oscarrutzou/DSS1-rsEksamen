using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShamansDungeon.CommandPattern;
using ShamansDungeon.CommandPattern.Commands;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.GameManagement;
using ShamansDungeon.GameManagement.Scenes.Rooms;
using ShamansDungeon.LiteDB;
using ShamansDungeon.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ComponentPattern.GUI
{
    public class TutorialBox : Component
    {
        private Collider _col;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _textStartPos;

        public int TutorialNumberReached = 0;
        public int MaxAmountOfBaseTutorials = 4;
        public int MaxAmountOfTutorials = 5;

        public static readonly string[] TutorialText = new[]
        {
            "To move: W A S D",
            "Left click to attack",
            "Press ESC to pause",
            "Space to Dash\nGain brief damage immunity",
            "Hold TAB to show stats",
            "Find a red potion\nPress E to use and heal",

            "Completed base tutorial",                      // Tutorial
            "Here's 100 gold\nSpend them wisely...",    
        };
        // Always last two for the tutorialSystem.ArgumentException: 'Text contains characters that cannot be resolved by this SpriteFont. Arg_ParamName_Name'

        private bool _hasRemovedChecks;
        private CustomCmd completed0Base, completed1Base, completed2Base, completed3Base, completed4Base, potionTutorialCmd;

        private string _tutorialText;
        private bool _startRemoveTimer { get; set; }
        private double _tutorialRemoveTimer, _tutorialHowLongOnScreen = 5f;
        private bool _hasCompletedTutorialBefore;
        public TutorialBox(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            GameObject.Transform.Position = GameWorld.Instance.UiCam.TopRight + new Vector2(-260, 205);

            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.SetSprite(TextureNames.QuestUnder);
            
            _col = GameObject.GetComponent<Collider>();
            _textStartPos = _col.LeftTopCollisionPosition;

            _hasCompletedTutorialBefore = SaveData.HasCompletedFullTutorial;
            if (_hasCompletedTutorialBefore)
            {
                return;
            }
            _tutorialText = TutorialText[SaveData.TutorialReached]; // Start with base or loaded text
            TutorialNumberReached = SaveData.TutorialReached;

            ResetTimer();
            AddListenerCommands();
        }

        private void AddListenerCommands()
        {
            completed0Base = new(() => { NextBaseTutorial(1); });
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.DownMovementKey, completed0Base);
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.UpMovementKey, completed0Base);
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.LeftMovementKey, completed0Base);
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.RightMovementKey, completed0Base);

            // A bool for each check
            completed1Base = new(() => { NextBaseTutorial(2); });
            InputHandler.Instance.AddMouseUpdateCommand(RoomBase.AttackSimpelAttackKey, completed1Base);

            completed2Base = new(() => { NextBaseTutorial(3); });
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.TogglePauseMenuKey, completed2Base);

            completed3Base = new(() => { NextBaseTutorial(4); });
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.DashKey, completed3Base);

            completed4Base = new(() => { NextBaseTutorial(); });
            InputHandler.Instance.AddKeyUpdateCommand(RoomBase.ToggleStatsMenuKey, completed4Base);

            // We need to have another way to start the potion tutorial, and we use a Action here to do that
            _playerHealth = SaveData.Player.GameObject.GetComponent<Health>();
            _playerHealth.On50orUnder += ChangeTextToHealthReminder;

            potionTutorialCmd = new(FinnishedPotionTutorial); // Last tutorial
            InputHandler.Instance.AddKeyButtonDownCommand(RoomBase.UseItem, potionTutorialCmd);

        }
        private Health _playerHealth;
        private void NextBaseTutorial(int nextTutorialNmb = -1)
        {
            if (TutorialNumberReached == MaxAmountOfBaseTutorials && nextTutorialNmb == -1)
            {
                // The last nmb
                TutorialNumberReached = TutorialText.Length - 2;
                _tutorialText = TutorialText[TutorialNumberReached];

                _startRemoveTimer = true;
                return;
            }

            if (TutorialNumberReached + 1 == nextTutorialNmb)  // Dont change tutorial
            {
                TutorialNumberReached = nextTutorialNmb;
                _tutorialText = TutorialText[nextTutorialNmb];
                SaveTutorial();
            }
        }


        // Gets set when player health is low
        private void ChangeTextToHealthReminder()
        {
            // Que the stuff so it will get called if it has not changed
            if (_tutorialText == "" || TutorialNumberReached == TutorialText.Length - 2) // The completed base tutorial
            {
                TutorialNumberReached = 5; // Add it to the tutorial
                _tutorialText = TutorialText[TutorialNumberReached];
                SaveTutorial();
                ResetTimer();
                _playerHealth.On50orUnder -= ChangeTextToHealthReminder;
            }
        }

        private void SaveTutorial()
        {
            SaveData.TutorialReached = TutorialNumberReached;
            DB.Instance.SaveGame(SaveData.CurrentSaveID, false);
        }

        private void FinnishedPotionTutorial()
        {
            if (SaveData.HasCompletedFullTutorial || !SaveData.Player.HasUsedItem || TutorialNumberReached != 5) return;
            _tutorialText = TutorialText[^1];
            SaveData.HasCompletedFullTutorial = true;
            SaveTutorial();
            DB.Instance.AddCurrency(100);
            _startRemoveTimer = true;
            GlobalSounds.PlaySound(SoundNames.Reward, 1, 0.8f, true, 0.1f, 0.2f);
        }
        private void ResetTimer()
        {
            _startRemoveTimer = false;
            _tutorialRemoveTimer = 0f;
            _spriteRenderer.ShouldDrawSprite = true;
        }

        public override void Update()
        {
            base.Update();

            //CheckIfShouldDeleteMoveChecks();

            if (_startRemoveTimer)
                _tutorialRemoveTimer += GameWorld.DeltaTime;
        }


        #region Remove Commands
        // Only remove if it has completed all tutorials
        private void CheckIfShouldDeleteMoveChecks()
        {
            if (_hasRemovedChecks || TutorialNumberReached != MaxAmountOfTutorials) return;

            _hasRemovedChecks = true;
            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.DownMovementKey, completed0Base);
            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.UpMovementKey, completed0Base);
            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.LeftMovementKey, completed0Base);
            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.RightMovementKey, completed0Base);

            InputHandler.Instance.RemoveMouseUpdateCommand(RoomBase.AttackSimpelAttackKey, completed1Base);

            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.DashKey, completed2Base);
        
            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.TogglePauseMenuKey, completed3Base);

            InputHandler.Instance.RemoveKeyUpdateCommand(RoomBase.ToggleStatsMenuKey, completed4Base);

            InputHandler.Instance.RemoveKeyButtonDownCommand(RoomBase.UseItem, potionTutorialCmd);
        }

        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_hasCompletedTutorialBefore)
            {
                _spriteRenderer.ShouldDrawSprite = false;
                return;
            }

            if (_startRemoveTimer)
            {
                double normalizedTime = _tutorialRemoveTimer / _tutorialHowLongOnScreen;
                if (normalizedTime >= 1)
                {
                    _spriteRenderer.ShouldDrawSprite = false;
                    return;
                }
            }

            //Vector2 textPos = _textStartPos + new Vector2(40, 20);
            Vector2 textPos = _textStartPos + new Vector2(_col.CollisionBox.Width / 2, 30);

            //spriteBatch.DrawString(GlobalTextures.DefaultFont, _tutorialText, textPos, BaseMath.TransitionColor(GameWorld.TextColor), 0, Vector2.Zero, 1, SpriteEffects.None, SpriteRenderer.GetLayerDepth(LayerDepth.Text));

            GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, textPos, _tutorialText, BaseMath.TransitionColor(GameWorld.TextColor));
        }
    }
}
