using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

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

        // Keyboard commands
        private Dictionary<Keys, ICommand> keybindsUpdate = new();

        private Dictionary<Keys, ICommand> keybindsButtonDown = new();

        // Mouse Commands
        private Dictionary<MouseCmdState, ICommand> mouseButtonUpdateCommands = new();

        private Dictionary<MouseCmdState, ICommand> mouseButtonDownCommands = new();
        private Dictionary<ScrollWheelState, ICommand> scrollWheelCommands = new();

        public Vector2 MouseInWorld, MouseOnUI;
        public bool MouseOutOfBounds;

        #endregion Properties

        private InputHandler()
        {
            SetBaseKeys();
        }

        public void SetBaseKeys()
        {
            AddKeyButtonDownCommand(Keys.Escape, new QuitCmd());
            AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(() => { GridManager.Instance.DrawOnCells(); }));
            AddMouseUpdateCommand(MouseCmdState.Right, new CustomCmd(() => { GridManager.Instance.SetDefaultOnCell(); }));

            AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { GridManager.Instance.ChangeRoomNrIndex(-1); }));
            AddKeyButtonDownCommand(Keys.E, new CustomCmd(() => { GridManager.Instance.ChangeRoomNrIndex(1); }));
        }

        //Make a mark in the right corner that just is a bool that check if there have been made any changes to the data (for debug) so we can save it.
        //Maybe make a ctrl z + x command.
        // Multiple command inputs?
        // Make it save the new grid.
        // Change it so the grid manager only shows 1 grid, since thats what our design is made.
        // Change all foreach to just check the grid != null.
        // Make commen commands to the contains and stuff.

        #region Command

        #region Add/Remove

        public void AddKeyUpdateCommand(Keys inputKey, ICommand command) => keybindsUpdate.Add(inputKey, command);

        public void AddKeyButtonDownCommand(Keys inputKey, ICommand command) => keybindsButtonDown.Add(inputKey, command);

        public void AddMouseUpdateCommand(MouseCmdState inputButton, ICommand command) => mouseButtonUpdateCommands.Add(inputButton, command);

        public void AddMouseButtonDownCommand(MouseCmdState inputButton, ICommand command) => mouseButtonDownCommands.Add(inputButton, command);

        public void AddScrollWheelCommand(ScrollWheelState scrollWheelState, ICommand command) => scrollWheelCommands.Add(scrollWheelState, command);

        public void RemoveKeyUpdateCommand(Keys inputKey) => keybindsUpdate.Remove(inputKey);

        public void RemoveKeyButtonDownCommand(Keys inputKey) => keybindsButtonDown.Remove(inputKey);

        public void RemoveMouseUpdateCommand(MouseCmdState inputButton) => mouseButtonUpdateCommands.Remove(inputButton);

        public void RemoveMouseButtonDownCommand(MouseCmdState inputButton) => mouseButtonDownCommands.Remove(inputButton);

        public void RemoveScrollWheelCommand(ScrollWheelState scrollWheelState) => scrollWheelCommands.Remove(scrollWheelState);

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

        private KeyboardState previousKeyState;
        private MouseState previousMouseState;

        public void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            MouseInWorld = GetMousePositionInWorld(mouseState);
            MouseOnUI = GetMousePositionOnUI(mouseState);

            Camera worldCam = GameWorld.Instance.WorldCam;
            if (float.IsNaN(MouseInWorld.X) || float.IsNaN(MouseOnUI.X))
            {
                Debug.WriteLine("ERROR WORLD CAM IS NAN");
            }

            UpdateKeyCommands(keyState);
            UpdateMouseCommands(mouseState);

            previousKeyState = keyState;
            previousMouseState = mouseState;
        }

        private void UpdateKeyCommands(KeyboardState keyState)
        {
            foreach (var pressedKey in keyState.GetPressedKeys())
            {
                if (keybindsUpdate.TryGetValue(pressedKey, out ICommand cmd)) // Commands that happend every update
                {
                    cmd.Execute();
                }
                if (!previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey)) // Commands that only happens once every time the button gets pressed
                {
                    if (keybindsButtonDown.TryGetValue(pressedKey, out ICommand cmdBd))
                    {
                        cmdBd.Execute();
                    }
                }
            }
        }

        private void UpdateMouseCommands(MouseState mouseState)
        {
            // Left mouse button update commands
            if (mouseState.LeftButton == ButtonState.Pressed
                && mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Left, out ICommand cmdLeft))
            {
                cmdLeft.Execute();
            }

            // Left mouse button down commands
            if (previousMouseState.LeftButton == ButtonState.Released
                && mouseState.LeftButton == ButtonState.Pressed
                && mouseButtonDownCommands.TryGetValue(MouseCmdState.Left, out ICommand cmdBdLeft))
            {
                cmdBdLeft.Execute();
            }

            // Right mouse button update commands
            if (mouseState.RightButton == ButtonState.Pressed
                && mouseButtonUpdateCommands.TryGetValue(MouseCmdState.Right, out ICommand cmdRight))
            {
                cmdRight.Execute();
            }

            // Right mouse button down commands
            if (previousMouseState.RightButton == ButtonState.Released
                && mouseState.RightButton == ButtonState.Pressed
                && mouseButtonDownCommands.TryGetValue(MouseCmdState.Right, out ICommand cmdBdRight))
            {
                cmdBdRight.Execute();
            }

            // Checks the Schoold wheel and gets the appropriately command
            if (previousMouseState.ScrollWheelValue != mouseState.ScrollWheelValue
                && scrollWheelCommands.TryGetValue(
                    mouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue
                    ? ScrollWheelState.Up : ScrollWheelState.Down, out ICommand cmdScroll))
            {
                cmdScroll.Execute();
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