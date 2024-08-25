using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.CommandPattern;

public enum ScrollWheelState
{
    Up, Down
}

public enum MouseCmdState
{
    Left, Right
}

// Oscar
public class InputHandler
{
    #region Properties

    private static InputHandler instance;
    public static InputHandler Instance
    { get { return instance ??= instance = new InputHandler(); } }

    public KeyboardState KeyState;
    public MouseState MouseState;
    public Vector2 MouseInWorld, MouseOnUI;
    public bool MouseOutOfBounds, DebugMode;
    public GameObject MouseGo;

    private Dictionary<Keys, List<Command>> _keybindsUpdate = new();
    private Dictionary<Keys, List<Command>> _keybindsButtonDown = new();
    private Dictionary<MouseCmdState, List<Command>> _mouseButtonUpdateCommands = new();
    private Dictionary<MouseCmdState, List<Command>> _mouseButtonDownCommands = new();
    private Dictionary<ScrollWheelState, List<Command>> _scrollWheelCommands = new();

    private KeyboardState _previousKeyState;
    private MouseState _previousMouseState;

    private List<Command> _allCommands = new List<Command>();

    #endregion Properties

    private InputHandler()
    {
        SetBaseKeys();
    }

    private void SetBaseKeys()
    {
        AddMouseButtonDownCommand(MouseCmdState.Left, new CheckButtonCmd());

        if (!GameWorld.DebugAndCheats) return;


        AddKeyButtonDownCommand(Keys.Left, new CustomCmd(() => {
            GameWorld.Instance.GaussianBlurEffect_BlurAmount += 0.5f;
            GlobalTextures.GaussianBlurEffect.Parameters["blurAmount"].SetValue(GameWorld.Instance.GaussianBlurEffect_BlurAmount);
        }));

        AddKeyButtonDownCommand(Keys.Right, new CustomCmd(() => {
            GameWorld.Instance.GaussianBlurEffect_BlurAmount -= 0.5f;
            GlobalTextures.GaussianBlurEffect.Parameters["blurAmount"].SetValue(GameWorld.Instance.GaussianBlurEffect_BlurAmount);
        }));

        AddKeyButtonDownCommand(Keys.Up, new CustomCmd(() => {
            GameWorld.Instance.HighlightsEffect_Threshold += 0.005f;
            GlobalTextures.HighlightsEffect.Parameters["threshold"].SetValue(GameWorld.Instance.HighlightsEffect_Threshold);
        }));

        AddKeyButtonDownCommand(Keys.Down, new CustomCmd(() => {
            GameWorld.Instance.HighlightsEffect_Threshold -= 0.005f;
            GlobalTextures.HighlightsEffect.Parameters["threshold"].SetValue(GameWorld.Instance.HighlightsEffect_Threshold);
        }));

        // For debugging
        AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { GridManager.Instance.ChangeNumberIndex(-1); }));
        AddKeyButtonDownCommand(Keys.E, new CustomCmd(() => { GridManager.Instance.ChangeNumberIndex(1); }));

        AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(() => { GridManager.Instance.DrawOnCells(); }));
        AddMouseUpdateCommand(MouseCmdState.Right, new CustomCmd(() => { GridManager.Instance.SetDefaultOnCell(); }));

        AddKeyButtonDownCommand(Keys.I, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));

        AddKeyButtonDownCommand(Keys.U, new CustomCmd(() => { DB.Instance.SaveGame(SaveData.CurrentSaveID); }));
        AddKeyButtonDownCommand(Keys.Z, new CustomCmd(() => { GridManager.Instance.ChangeSelectedDraw(DrawMapSelecter.DrawRoomColliders); }));
        AddKeyButtonDownCommand(Keys.X, new CustomCmd(() => { GridManager.Instance.ChangeSelectedDraw(DrawMapSelecter.DrawBlackedOutRooms); }));
    }

    #region Command

    #region Add/Remove

    public void AddKeyUpdateCommand(Keys inputKey, Command command)
    {
        if (!_keybindsUpdate.ContainsKey(inputKey))
        {
            _keybindsUpdate[inputKey] = new List<Command>();
        }
        _keybindsUpdate[inputKey].Add(command);
    }

    public void AddKeyButtonDownCommand(Keys inputKey, Command command)
    {
        if (!_keybindsButtonDown.ContainsKey(inputKey))
        {
            _keybindsButtonDown[inputKey] = new List<Command>();
        }
        _keybindsButtonDown[inputKey].Add(command);
    }

    public void AddMouseUpdateCommand(MouseCmdState inputButton, Command command)
    {
        if (!_mouseButtonUpdateCommands.ContainsKey(inputButton))
        {
            _mouseButtonUpdateCommands[inputButton] = new List<Command>();
        }
        _mouseButtonUpdateCommands[inputButton].Add(command);
    }

    public void AddMouseButtonDownCommand(MouseCmdState inputButton, Command command)
    {
        if (!_mouseButtonDownCommands.ContainsKey(inputButton))
        {
            _mouseButtonDownCommands[inputButton] = new List<Command>();
        }
        _mouseButtonDownCommands[inputButton].Add(command);
    }

    public void AddScrollWheelCommand(ScrollWheelState scrollWheelState, Command command)
    {
        if (!_scrollWheelCommands.ContainsKey(scrollWheelState))
        {
            _scrollWheelCommands[scrollWheelState] = new List<Command>();
        }
        _scrollWheelCommands[scrollWheelState].Add(command);
    }

    public void RemoveKeyUpdateCommand(Keys inputKey)
    {
        if (_keybindsUpdate.ContainsKey(inputKey))
        {
            _keybindsUpdate[inputKey].Clear();
        }
    }

    public void RemoveKeyButtonDownCommand(Keys inputKey)
    {
        if (_keybindsButtonDown.ContainsKey(inputKey))
        {
            _keybindsButtonDown[inputKey].Clear();
        }
    }

    public void RemoveMouseUpdateCommand(MouseCmdState inputButton)
    {
        if (_mouseButtonUpdateCommands.ContainsKey(inputButton))
        {
            _mouseButtonUpdateCommands[inputButton].Clear();
        }
    }

    public void RemoveMouseButtonDownCommand(MouseCmdState inputButton)
    {
        if (_mouseButtonDownCommands.ContainsKey(inputButton))
        {
            _mouseButtonDownCommands[inputButton].Clear();
        }
    }

    public void RemoveScrollWheelCommand(ScrollWheelState scrollWheelState)
    {
        if (_scrollWheelCommands.ContainsKey(scrollWheelState))
        {
            _scrollWheelCommands[scrollWheelState].Clear();
        }
    }

    /// <summary>
    /// Base Commands are the ones in the InputHandler, in the SetBaseKeys() method.
    /// </summary>
    public void RemoveAllExeptBaseCommands()
    {
        _keybindsUpdate.Clear();
        _keybindsButtonDown.Clear();
        _mouseButtonUpdateCommands.Clear();
        _mouseButtonDownCommands.Clear();
        _scrollWheelCommands.Clear();

        _allCommands.Clear();
        firstUpdate = true;

        SetBaseKeys();
    }

    #endregion Add/Remove


    private void SetAllCommands()
    {
        _allCommands.AddRange(_keybindsButtonDown.Values.SelectMany(cmdList => cmdList));
        _allCommands.AddRange(_mouseButtonUpdateCommands.Values.SelectMany(cmdList => cmdList));
        _allCommands.AddRange(_mouseButtonDownCommands.Values.SelectMany(cmdList => cmdList));
        _allCommands.AddRange(_scrollWheelCommands.Values.SelectMany(cmdList => cmdList));
    }

    private bool firstUpdate = true;

    public void Update()
    {
        if (firstUpdate)
        {
            SetAllCommands();
            firstUpdate = false;
        }

        KeyState = Keyboard.GetState();
        MouseState = Mouse.GetState();

        MouseInWorld = GetMousePositionInWorld(MouseState);
        MouseOnUI = GetMousePositionOnUI(MouseState);

        if (MouseOutOfBounds) return; // Dont update the commands, so the player e.g dont press attack, when the game is not running

        UpdateAllCommands();

        UpdateKeyCommands(KeyState);
        UpdateMouseCommands(MouseState);

        _previousKeyState = KeyState;
        _previousMouseState = MouseState;
    }

    private void UpdateAllCommands()
    {
        // Updates each command
        foreach (var cmd in _allCommands)
        {
            cmd.Update();
        }
    }

    private void UpdateKeyCommands(KeyboardState keyState)
    {
        foreach (var pressedKey in keyState.GetPressedKeys())
        {
            if (_keybindsUpdate.TryGetValue(pressedKey, out List<Command> cmds)) // Commands that happen every update
            {
                foreach (var cmd in cmds)
                {
                    cmd.Execute();
                }
            }
            if (!_previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey)) // Commands that only happens once every time the button gets pressed
            {
                if (_keybindsButtonDown.TryGetValue(pressedKey, out List<Command> cmdsBd))
                {
                    foreach (var cmdBd in cmdsBd)
                    {
                        cmdBd.Execute();
                    }
                }
            }
        }
    }

    private void UpdateMouseCommands(MouseState mouseState)
    {
        // Left mouse button update commands
        if (mouseState.LeftButton == ButtonState.Pressed
            && _mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Left, out List<Command> cmdsLeft))
        {
            foreach (var cmdLeft in cmdsLeft)
            {
                cmdLeft.Execute();
            }
        }

        // Left mouse button down commands
        if (_previousMouseState.LeftButton == ButtonState.Released
            && mouseState.LeftButton == ButtonState.Pressed
            && _mouseButtonDownCommands.TryGetValue(MouseCmdState.Left, out List<Command> cmdsBdLeft))
        {
            foreach (var cmdBdLeft in cmdsBdLeft)
            {
                cmdBdLeft.Execute();
            }
        }

        // Right mouse button update commands
        if (mouseState.RightButton == ButtonState.Pressed
            && _mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Right, out List<Command> cmdsRight))
        {
            foreach (var cmdRight in cmdsRight)
            {
                cmdRight.Execute();
            }
        }

        // Right mouse button down commands
        if (_previousMouseState.RightButton == ButtonState.Released
            && mouseState.RightButton == ButtonState.Pressed
            && _mouseButtonDownCommands.TryGetValue(MouseCmdState.Right, out List<Command> cmdsBdRight))
        {
            foreach (var cmdBdRight in cmdsBdRight)
            {
                cmdBdRight.Execute();
            }
        }

        // Checks the Scroll wheel and gets the appropriately command
        if (_previousMouseState.ScrollWheelValue != mouseState.ScrollWheelValue
            && _scrollWheelCommands.TryGetValue(
                mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue
                ? ScrollWheelState.Up : ScrollWheelState.Down, out List<Command> cmdsScroll))
        {
            foreach (var cmdScroll in cmdsScroll)
            {
                cmdScroll.Execute();
            }
        }

        _previousMouseState = mouseState;
    }

    #endregion Command

    private Vector2 GetMousePositionInWorld(MouseState mouseState)
    {
        Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
        Matrix invMatrix = Matrix.Invert(GameWorld.Instance.WorldCam.GetMatrix());
        return Vector2.Transform(pos, invMatrix);
    }

    private Vector2 GetMousePositionOnUI(MouseState mouseState)
    {
        Camera uiCam = GameWorld.Instance.UiCam;
        Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
        Matrix invMatrix = Matrix.Invert(uiCam.GetMatrix());
        Vector2 returnValue = Vector2.Transform(pos, invMatrix);
        MouseOutOfBounds = (returnValue.X < uiCam.TopLeft.X || returnValue.Y < uiCam.TopLeft.Y || returnValue.X > uiCam.BottomRight.X || returnValue.Y > uiCam.BottomRight.Y);
        return returnValue;
    }
}