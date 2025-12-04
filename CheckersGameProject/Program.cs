using System;
using CheckersGameProject.Players;
using CheckersGameProject.Pieces;
using CheckersGameProject.Game;
using System.Linq;

namespace CheckersGameProject
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1: Human vs Human");
            Console.WriteLine("2: Human vs Bot");
            Console.WriteLine("3: Watch last game");

            int option;
            while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 3)
            {
                Console.WriteLine("Invalid choice. Enter 1, 2 or 3.");
            }

            if (option == 3)
            {
                GameManager replayGame = new GameManager(null, null);
                replayGame.LoadMoveHistory("last_game_moves.json");
                return;
            }

            IPlayer player1 = new HumanPlayer("Player1", PieceColor.White);
            IPlayer player2;

            if (option == 1)
            {
                player2 = new HumanPlayer("Player2", PieceColor.Black);
            }
            else 
            {
                Console.WriteLine("Select bot difficulty: 1-Easy, 2-Medium, 3-Hard, 4-Impossible");
                int diffChoice;
                while (!int.TryParse(Console.ReadLine(), out diffChoice) || diffChoice < 1 || diffChoice > 4)
                {
                    Console.WriteLine("Invalid choice. Enter 1,2,3 or 4.");
                }
                BotDifficulty difficulty = (BotDifficulty)(diffChoice - 1);
                player2 = new BotPlayer("Bot", PieceColor.Black, difficulty);
            }

            GameManager game = new GameManager(player1, player2);

            game.MoveMade += (move) =>
            {
                Console.WriteLine($"{move.MovedPiece.Color}" +
                    $" moved from ({move.StartRow},{move.StartColumn}) to ({move.EndRow},{move.EndColumn})");
                if (move.CapturedPieces.Count > 0)
                    Console.WriteLine($"Captured: " +
                        $"{string.Join(", ", move.CapturedPieces.Select(c => $"({c.Row},{c.Column})"))}");
            };

            game.PlayerChanged += (newPlayer) =>
            {
                Console.WriteLine($"Next player: {newPlayer.Name} ({newPlayer.Color})");
            };

            game.GameEnded += (winner) =>
            {
                Console.WriteLine($"Game over! Winner: {winner.Name} ({winner.Color})");
            };

            game.Run();
        }
    }
}
