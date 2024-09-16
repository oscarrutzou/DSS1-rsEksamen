using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.Factory.Gui;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace ShamansDungeon.GameManagement.Scenes.Menus;

// Oscar
public class MainMenu : MenuScene
{
    protected override void InitFirstMenu()
    {
        GameObject startBtn = ButtonFactory.Create("Play", true,
                        () => { GameWorld.Instance.ChangeScene(SceneNames.SaveFileMenu); });

        FirstMenuObjects.Add(startBtn);

        GameObject settingsBtn = ButtonFactory.Create("Settings", true, ShowHideSecondMenu);
        FirstMenuObjects.Add(settingsBtn);

        GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
        FirstMenuObjects.Add(quitBtn);
    }

    protected override void InitSecondMenu()
    {
        GameObject musicVolGo = ButtonFactory.Create("", true, ChangeMusic, TextureNames.LongButton);
        MusicBtn = musicVolGo.GetComponent<Button>();
        MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
        SecondMenuObjects.Add(musicVolGo);

        GameObject sfxVolGo = ButtonFactory.Create("", true, ChangeSfx, TextureNames.LongButton);
        SfxBtn = sfxVolGo.GetComponent<Button>();
        SfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
        SecondMenuObjects.Add(sfxVolGo);

        GameObject quitBtn = ButtonFactory.Create("Back", true, ShowHideSecondMenu);
        SecondMenuObjects.Add(quitBtn);

        ShowHideGameObjects(SecondMenuObjects, false);

        SpawnAnimations();
        OnFirstCleanUp += () => { _idleAnim1.PlayAnimation(AnimNames.MainMenuAssassin); };
        OnFirstCleanUp += () => { _idleAnim2.PlayAnimation(AnimNames.MainMenuAssassin); };
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        if (ShowSecondMenu)
        {
            DrawMenuText(spriteBatch, "Settings", TextPos);
        }
        else
        {
            DrawMenuText(spriteBatch, "Shaman's Dungeon", TextPos);
        }
    }

    private void SpawnAnimations()
    {
        GameObject go1 = new();
        go1.Transform.Scale = new Vector2(8, 8);
        go1.Transform.Position = GameWorld.Instance.UiCam.LeftCenter + new Vector2(300, -200);
        go1.Type = GameObjectTypes.Player;
        go1.AddComponent<SpriteRenderer>();
        _idleAnim1 = go1.AddComponent<Animator>();
        GameWorld.Instance.Instantiate(go1);


        GameObject go2 = new();
        go2.Transform.Scale = new Vector2(8, 8);
        go2.Transform.Position = GameWorld.Instance.UiCam.RightCenter + new Vector2(-300, -200);
        go2.Type = GameObjectTypes.Player;
        go2.AddComponent<SpriteRenderer>().SpriteEffects = SpriteEffects.FlipHorizontally;
        _idleAnim2 = go2.AddComponent<Animator>();
        GameWorld.Instance.Instantiate(go2);
    }
    Animator _idleAnim1, _idleAnim2;
}