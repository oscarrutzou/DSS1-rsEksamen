using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.DB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.CommandPattern
{
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

        private Dictionary<Keys, List<ICommand>> keybindsUpdate = new();
        private Dictionary<Keys, List<ICommand>> keybindsButtonDown = new();
        private Dictionary<MouseCmdState, List<ICommand>> mouseButtonUpdateCommands = new();
        private Dictionary<MouseCmdState, List<ICommand>> mouseButtonDownCommands = new();
        private Dictionary<ScrollWheelState, List<ICommand>> scrollWheelCommands = new();

        public Vector2 MouseInWorld, MouseOnUI;
        public bool MouseOutOfBounds, DebugMode = true;

        #endregion Properties

        private InputHandler()
        {
            SetBaseKeys();
        }

        public void StartInputThead()
        {
            while (true) // The update for input should always run.
            {
                Update();
            }
        }

        private void SetBaseKeys()
        {
            AddMouseUpdateCommand(MouseCmdState.Left, new CheckButtonCmd());

            AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(() => { GridManager.Instance.DrawOnCells(); }));
            AddMouseUpdateCommand(MouseCmdState.Right, new CustomCmd(() => { GridManager.Instance.SetDefaultOnCell(); }));

            AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { GridManager.Instance.ChangeRoomNrIndex(-1); }));
            AddKeyButtonDownCommand(Keys.E, new CustomCmd(() => { GridManager.Instance.ChangeRoomNrIndex(1); }));

            AddKeyButtonDownCommand(Keys.T, new CustomCmd(() => { DBMethods.DeleteSave(Data.CurrentSaveID); }));
        }

        #region Command

        #region Add/Remove

        public void AddKeyUpdateCommand(Keys inputKey, ICommand command)
        {
            if (!keybindsUpdate.ContainsKey(inputKey))
            {
                keybindsUpdate[inputKey] = new List<ICommand>();
            }
            keybindsUpdate[inputKey].Add(command);
        }

        public void AddKeyButtonDownCommand(Keys inputKey, ICommand command)
        {
            if (!keybindsButtonDown.ContainsKey(inputKey))
            {
                keybindsButtonDown[inputKey] = new List<ICommand>();
            }
            keybindsButtonDown[inputKey].Add(command);
        }

        public void AddMouseUpdateCommand(MouseCmdState inputButton, ICommand command)
        {
            if (!mouseButtonUpdateCommands.ContainsKey(inputButton))
            {
                mouseButtonUpdateCommands[inputButton] = new List<ICommand>();
            }
            mouseButtonUpdateCommands[inputButton].Add(command);
        }

        public void AddMouseButtonDownCommand(MouseCmdState inputButton, ICommand command)
        {
            if (!mouseButtonDownCommands.ContainsKey(inputButton))
            {
                mouseButtonDownCommands[inputButton] = new List<ICommand>();
            }
            mouseButtonDownCommands[inputButton].Add(command);
        }

        public void AddScrollWheelCommand(ScrollWheelState scrollWheelState, ICommand command)
        {
            if (!scrollWheelCommands.ContainsKey(scrollWheelState))
            {
                scrollWheelCommands[scrollWheelState] = new List<ICommand>();
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

            SetBaseKeys();
        }

        #endregion Add/Remove

        public KeyboardState KeyState;
        public MouseState MouseState;
        private KeyboardState previousKeyState;
        private MouseState previousMouseState;

        private void Update()
        {
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            MouseInWorld = GetMousePositionInWorld(MouseState);
            MouseOnUI = GetMousePositionOnUI(MouseState);

            UpdateKeyCommands(KeyState);
            UpdateMouseCommands(MouseState);

            previousKeyState = KeyState;
            previousMouseState = MouseState;
        }

        private void UpdateKeyCommands(KeyboardState keyState)
        {
            foreach (var pressedKey in keyState.GetPressedKeys())
            {
                if (keybindsUpdate.TryGetValue(pressedKey, out List<ICommand> cmds)) // Commands that happen every update
                {
                    foreach (var cmd in cmds)
                    {
                        cmd.Execute();
                    }
                }
                if (!previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey)) // Commands that only happens once every time the button gets pressed
                {
                    if (keybindsButtonDown.TryGetValue(pressedKey, out List<ICommand> cmdsBd))
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
                && mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Left, out List<ICommand> cmdsLeft))
            {
                foreach (var cmdLeft in cmdsLeft)
                {
                    cmdLeft.Execute();
                }
            }

            // Left mouse button down commands
            if (previousMouseState.LeftButton == ButtonState.Released
                && mouseState.LeftButton == ButtonState.Pressed
                && mouseButtonDownCommands.TryGetValue(MouseCmdState.Left, out List<ICommand> cmdsBdLeft))
            {
                foreach (var cmdBdLeft in cmdsBdLeft)
                {
                    cmdBdLeft.Execute();
                }
            }

            // Right mouse button update commands
            if (mouseState.RightButton == ButtonState.Pressed
                && mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Right, out List<ICommand> cmdsRight))
            {
                foreach (var cmdRight in cmdsRight)
                {
                    cmdRight.Execute();
                }
            }

            // Right mouse button down commands
            if (previousMouseState.RightButton == ButtonState.Released
                && mouseState.RightButton == ButtonState.Pressed
                && mouseButtonDownCommands.TryGetValue(MouseCmdState.Right, out List<ICommand> cmdsBdRight))
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
                    ? ScrollWheelState.Up : ScrollWheelState.Down, out List<ICommand> cmdsScroll))
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
}