using System;
using System.Collections.Generic;
using System.Linq;
using CheckersGameProject.Game;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.Players
{
    public enum BotDifficulty { Easy, Medium, Hard, Impossible }

    public class BotPlayer : IPlayer
    {
        public string Name { get; private set; }
        public PieceColor Color { get; private set; }
        private BotDifficulty Difficulty;
        private Random rand = new Random();

        public BotPlayer(string name, PieceColor color, BotDifficulty difficulty = BotDifficulty.Medium)
        {
            Name = name;
            Color = color;
            Difficulty = difficulty;
        }

        public Move GetMove(Board board)
        {
            List<Move> possibleMoves = MoveValidator.GetLegalMoves(board, Color);
            if (possibleMoves.Count == 0) return null;

            switch (Difficulty)
            {
                case BotDifficulty.Easy:
                    return possibleMoves[rand.Next(possibleMoves.Count)];

                case BotDifficulty.Medium:
                    var captureMoves = possibleMoves.Where
                        (m => m.CapturedPieces.Count > 0).ToList();
                    if (captureMoves.Count > 0)
                        return captureMoves[rand.Next(captureMoves.Count)];
                    return possibleMoves[rand.Next(possibleMoves.Count)];

                case BotDifficulty.Hard:
                    int maxCapture = possibleMoves.Max(m => m.CapturedPieces.Count);
                    var bestMoves = possibleMoves.Where
                        (m => m.CapturedPieces.Count == maxCapture).ToList();
                    return bestMoves[rand.Next(bestMoves.Count)];

                case BotDifficulty.Impossible:
                    Move bestMove = possibleMoves[0];
                    int bestScore = int.MinValue;
                    foreach (var move in possibleMoves)
                    {
                        Board tempBoard = board.CloneAndApply(move);

                        var opponentMoves = MoveValidator.GetLegalMoves
                            (tempBoard, Color == PieceColor.White ? PieceColor.Black : PieceColor.White);
                        int score = -opponentMoves.Count + move.CapturedPieces.Count * 10;
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = move;
                        }
                    }
                    return bestMove;

                default:
                    return possibleMoves[rand.Next(possibleMoves.Count)];
            }
        }
    }
}
