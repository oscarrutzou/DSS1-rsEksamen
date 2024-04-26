using FørsteÅrsEksamen.GameManagement;
using System;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        // Problemet med at GameWorld noglegange ikke have en GraphicsDevice var fordi vi kaldte instance
        // men instance var aldrig sat før, og dermed lavede den en ny GameWorld.
        // Vi sætter derfor Instance når vi laver GameWorld her
        using (var game = new GameWorld())
        {
            GameWorld.Instance = game;
            game.Run();
        }
    }
}