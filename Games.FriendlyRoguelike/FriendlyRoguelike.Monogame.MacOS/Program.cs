using System;

namespace FriendlyRoguelike.Client.Monogame.MacOS
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = RoguelikeGameFactory.Create().Construct())
            {
                game.Run();
            }
        }
    }
}