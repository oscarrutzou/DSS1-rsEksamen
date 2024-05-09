using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.GameManagement;
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

        // Keyboard commands
        private Dictionary<Keys, ICommand> keybindsUpdate = new();

        private Dictionary<Keys, ICommand> keybindsButtonDown = new();

        // Mouse Commands
        private Dictionary<MouseCmdState, ICommand> mouseButtonUpdateCommands = new();

        private Dictionary<MouseCmdState, ICommand> mouseButtonDownCommands = new();
        private Dictionary<ScrollWheelState, ICommand> scrollWheelCommands = new();

        public Vector2 mouseInWorld, mouseOnUI;
        public bool mouseOutOfBounds;

        #endregion Properties

        private InputHandler()
        {
            AddKeyButtonDownCommand(Keys.Escape, new QuitCmd());
            AddMouseUpdateCommand(MouseCmdState.Left, new DrawTilesCmd());
            AddMouseUpdateCommand(MouseCmdState.Right, new ChangeSeletecDrawActionCmd());

        }

        #region Command

        public void AddKeyUpdateCommand(Keys inputKey, ICommand command) => keybindsUpdate.Add(inputKey, command);

        public void AddKeyButtonDownCommand(Keys inputKey, ICommand command) => keybindsButtonDown.Add(inputKey, command);

        public void AddMouseUpdateCommand(MouseCmdState inputButton, ICommand command) => mouseButtonUpdateCommands.Add(inputButton, command);

        public void AddMouseButtonDownCommand(MouseCmdState inputButton, ICommand command) => mouseButtonDownCommands.Add(inputButton, command);

        public void AddScrollWheelCommand(ScrollWheelState scrollWheelState, ICommand command) => scrollWheelCommands.Add(scrollWheelState, command);

        private KeyboardState previousKeyState;
        private MouseState previousMouseState;

        public void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            mouseInWorld = GetMousePositionInWorld(mouseState);
            mouseOnUI = GetMousePositionOnUI(mouseState);

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
            mouseOutOfBounds = (returnValue.X < 0 || returnValue.Y < 0 || returnValue.X > GameWorld.Instance.GfxManager.PreferredBackBufferWidth || returnValue.Y > GameWorld.Instance.GfxManager.PreferredBackBufferHeight);
            return returnValue;
        }
    }
}