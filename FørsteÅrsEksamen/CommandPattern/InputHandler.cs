using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern.Path;
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

    private Dictionary<Keys, List<Command>> keybindsUpdate = new();
    private Dictionary<Keys, List<Command>> keybindsButtonDown = new();
    private Dictionary<MouseCmdState, List<Command>> mouseButtonUpdateCommands = new();
    private Dictionary<MouseCmdState, List<Command>> mouseButtonDownCommands = new();
    private Dictionary<ScrollWheelState, List<Command>> scrollWheelCommands = new();

    public Vector2 MouseInWorld, MouseOnUI;
    public bool MouseOutOfBounds, DebugMode;
    public bool IsUpdating { get; private set; }

    #endregion Properties

    private InputHandler()
    {
        SetBaseKeys();
    }

    private void SetBaseKeys()
    {
        AddMouseButtonDownCommand(MouseCmdState.Left, new CheckButtonCmd());

        if (!GameWorld.DebugAndCheats) return;

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
        if (!keybindsUpdate.ContainsKey(inputKey))
        {
            keybindsUpdate[inputKey] = new List<Command>();
        }
        keybindsUpdate[inputKey].Add(command);
    }

    public void AddKeyButtonDownCommand(Keys inputKey, Command command)
    {
        if (!keybindsButtonDown.ContainsKey(inputKey))
        {
            keybindsButtonDown[inputKey] = new List<Command>();
        }
        keybindsButtonDown[inputKey].Add(command);
    }

    public void AddMouseUpdateCommand(MouseCmdState inputButton, Command command)
    {
        if (!mouseButtonUpdateCommands.ContainsKey(inputButton))
        {
            mouseButtonUpdateCommands[inputButton] = new List<Command>();
        }
        mouseButtonUpdateCommands[inputButton].Add(command);
    }

    public void AddMouseButtonDownCommand(MouseCmdState inputButton, Command command)
    {
        if (!mouseButtonDownCommands.ContainsKey(inputButton))
        {
            mouseButtonDownCommands[inputButton] = new List<Command>();
        }
        mouseButtonDownCommands[inputButton].Add(command);
    }

    public void AddScrollWheelCommand(ScrollWheelState scrollWheelState, Command command)
    {
        if (!scrollWheelCommands.ContainsKey(scrollWheelState))
        {
            scrollWheelCommands[scrollWheelState] = new List<Command>();
        }
        scrollWheelCommands[scrollWheelState].Add(command);
    }

    public void RemoveKeyUpdateCommand(Keys inputKey)
    {
        if (keybindsUpdate.ContainsKey(inputKey))
        {
            keybindsUpdate[inputKey].Clear();
        }
    }

    public void RemoveKeyButtonDownCommand(Keys inputKey)
    {
        if (keybindsButtonDown.ContainsKey(inputKey))
        {
            keybindsButtonDown[inputKey].Clear();
        }
    }

    public void RemoveMouseUpdateCommand(MouseCmdState inputButton)
    {
        if (mouseButtonUpdateCommands.ContainsKey(inputButton))
        {
            mouseButtonUpdateCommands[inputButton].Clear();
        }
    }

    public void RemoveMouseButtonDownCommand(MouseCmdState inputButton)
    {
        if (mouseButtonDownCommands.ContainsKey(inputButton))
        {
            mouseButtonDownCommands[inputButton].Clear();
        }
    }

    public void RemoveScrollWheelCommand(ScrollWheelState scrollWheelState)
    {
        if (scrollWheelCommands.ContainsKey(scrollWheelState))
        {
            scrollWheelCommands[scrollWheelState].Clear();
        }
    }

    /// <summary>
    /// Base Commands are the ones in the InputHandler, in the SetBaseKeys() method.
    /// </summary>
    public void RemoveAllExeptBaseCommands()
    {
        keybindsUpdate.Clear();
        keybindsButtonDown.Clear();
        mouseButtonUpdateCommands.Clear();
        mouseButtonDownCommands.Clear();
        scrollWheelCommands.Clear();

        allCommands.Clear();
        firstUpdate = true;

        SetBaseKeys();
    }

    #endregion Add/Remove

    public KeyboardState KeyState;
    public MouseState MouseState;
    private KeyboardState previousKeyState;
    private MouseState previousMouseState;

    List<Command> allCommands = new List<Command>();
    private void SetAllCommands()
    {
        allCommands.AddRange(keybindsButtonDown.Values.SelectMany(cmdList => cmdList));
        allCommands.AddRange(mouseButtonUpdateCommands.Values.SelectMany(cmdList => cmdList));
        allCommands.AddRange(mouseButtonDownCommands.Values.SelectMany(cmdList => cmdList));
        allCommands.AddRange(scrollWheelCommands.Values.SelectMany(cmdList => cmdList));
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

        UpdateAllCommands();

        UpdateKeyCommands(KeyState);
        UpdateMouseCommands(MouseState);

        previousKeyState = KeyState;
        previousMouseState = MouseState;
    }

    private void UpdateAllCommands()
    {
        // Updates each command
        foreach (var cmd in allCommands)
        {
            cmd.Update();
        }
    }

    private void UpdateKeyCommands(KeyboardState keyState)
    {
        foreach (var pressedKey in keyState.GetPressedKeys())
        {
            if (keybindsUpdate.TryGetValue(pressedKey, out List<Command> cmds)) // Commands that happen every update
            {
                foreach (var cmd in cmds)
                {
                    cmd.Execute();
                }
            }
            if (!previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey)) // Commands that only happens once every time the button gets pressed
            {
                if (keybindsButtonDown.TryGetValue(pressedKey, out List<Command> cmdsBd))
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
            && mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Left, out List<Command> cmdsLeft))
        {
            foreach (var cmdLeft in cmdsLeft)
            {
                cmdLeft.Execute();
            }
        }

        // Left mouse button down commands
        if (previousMouseState.LeftButton == ButtonState.Released
            && mouseState.LeftButton == ButtonState.Pressed
            && mouseButtonDownCommands.TryGetValue(MouseCmdState.Left, out List<Command> cmdsBdLeft))
        {
            foreach (var cmdBdLeft in cmdsBdLeft)
            {
                cmdBdLeft.Execute();
            }
        }

        // Right mouse button update commands
        if (mouseState.RightButton == ButtonState.Pressed
            && mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Right, out List<Command> cmdsRight))
        {
            foreach (var cmdRight in cmdsRight)
            {
                cmdRight.Execute();
            }
        }

        // Right mouse button down commands
        if (previousMouseState.RightButton == ButtonState.Released
            && mouseState.RightButton == ButtonState.Pressed
            && mouseButtonDownCommands.TryGetValue(MouseCmdState.Right, out List<Command> cmdsBdRight))
        {
            foreach (var cmdBdRight in cmdsBdRight)
            {
                cmdBdRight.Execute();
            }
        }

        // Checks the Scroll wheel and gets the appropriately command
        if (previousMouseState.ScrollWheelValue != mouseState.ScrollWheelValue
            && scrollWheelCommands.TryGetValue(
                mouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue
                ? ScrollWheelState.Up : ScrollWheelState.Down, out List<Command> cmdsScroll))
        {
            foreach (var cmdScroll in cmdsScroll)
            {
                cmdScroll.Execute();
            }
        }

        previousMouseState = mouseState;
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
        Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
        Matrix invMatrix = Matrix.Invert(GameWorld.Instance.UiCam.GetMatrix());
        Vector2 returnValue = Vector2.Transform(pos, invMatrix);
        MouseOutOfBounds = (returnValue.X < 0 || returnValue.Y < 0 || returnValue.X > GameWorld.Instance.GfxManager.PreferredBackBufferWidth || returnValue.Y > GameWorld.Instance.GfxManager.PreferredBackBufferHeight);
        return returnValue;
    }
}