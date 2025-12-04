using System;
using System.Collections.Generic;
using System.Linq;
using CheckersGameProject.Game;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.Players
{
    public class HumanPlayer : IPlayer
    {
        public string Name { get; private set; }
        public PieceColor Color { get; private set; }

        public HumanPlayer(string name, PieceColor color)
        {
            Name = name;
            Color = color;
        }

        public Move GetMove(Board board)
        {
            var allMoves = MoveValidator.GetLegalMoves(board, Color);
            if (allMoves.Count == 0) return null;

            var captureMoves = allMoves.Where(m => m.CapturedPieces.Count > 0).ToList();
            if (captureMoves.Count > 0)
                allMoves = captureMoves;

            var allowedPieces = allMoves
                .Select(m => (m.StartRow, m.StartColumn))
                .Distinct()
                .ToList();

            Console.Clear();
            Console.WriteLine($"Current player: {Name} ({Color})");
            board.PrintBoard();

            var (selectedRow, selectedCol) = PromptForPiece(allowedPieces, board);
            if (selectedRow == -1)
            {
                return GetMove(board);
            }

            var pieceMoves = allMoves
                .Where(m => m.StartRow == selectedRow && m.StartColumn == selectedCol)
                .ToList();

            if (pieceMoves.Count == 0)
            {
                Console.WriteLine("Selected piece has no legal moves.");
                Console.ReadLine();
                return GetMove(board);
            }

            var highlights = pieceMoves.Select(m => (m.EndRow, m.EndColumn)).ToList();
            Console.Clear();
            Console.WriteLine($"Current player: {Name} ({Color})");
            board.PrintBoard(highlights);

            return ChooseMoveFromList(pieceMoves, selectedRow, selectedCol);
        }


        private (int row, int col) PromptForPiece(List<(int row, int col)> allowedPieces, Board board)
        {
            Console.Write("Enter piece to move (row col): ");
            var input = Console.ReadLine()?.Split(' ');

            if (input == null || input.Length != 2 ||
                !int.TryParse(input[0], out int row) ||
                !int.TryParse(input[1], out int col) ||
                !Board.IsInsideBoard(row, col))
            {
                Console.WriteLine("Invalid coordinates.");
                Console.ReadLine();
                return (-1, -1);
            }

            if (!allowedPieces.Contains((row, col)))
            {
                Console.WriteLine("This piece cannot move (or another piece must capture).");
                Console.ReadLine();
                return (-1, -1);
            }

            return (row, col);
        }


        public Move ChooseMoveFromList(List<Move> moves, int pieceRow = -1, int pieceCol = -1)
        {
            Console.WriteLine();
            if (pieceRow != -1)
                Console.WriteLine($"Possible moves for piece at ({pieceRow},{pieceCol}):");

            for (int i = 0; i < moves.Count; i++)
            {
                var m = moves[i];
                Console.Write($"{i + 1}: To ({m.EndRow},{m.EndColumn})");
                if (m.CapturedPieces.Count > 0)
                {
                    Console.Write(" Captures:");
                    foreach (var c in m.CapturedPieces)
                        Console.Write($"({c.Row},{c.Column}) ");
                }
                Console.WriteLine();
            }

            int choice;
            bool moveSelected = false;

            do
            {
                Console.Write("Select move number: ");
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > moves.Count);

            return moves[choice - 1];
        }
    }
}
