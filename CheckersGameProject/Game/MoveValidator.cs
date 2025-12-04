using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.Game
{
    public static class MoveValidator
    {
        public static List<Move> GetLegalMoves(Board board, PieceColor color)
        {
            List<Move> allMoves = new List<Move>();
            List<Move> captureMoves = new List<Move>();

            foreach (var piece in board.GetAllPiecesOfColor(color))
            {
                var moves = piece.GetPotentialMoves(board);
                foreach (var move in moves)
                {
                    if (move.CapturedPieces.Count > 0)
                    {
                        captureMoves.Add(move);
                    }
                    else
                    {
                        allMoves.Add(move);
                    }
                }
            }
            if (captureMoves.Count > 0)
            {
                return ExpandMultiCaptureMoves(board, captureMoves);
            }
            else
            {
                return allMoves;
            }
        }

        private static List<Move> ExpandMultiCaptureMoves(Board board, List<Move> moves)
        {
            List<Move> expandedMoves = new List<Move>();
            foreach (var atomic in moves)
            {
                List<Move> results = new List<Move>();
                Move chainStart = atomic.Clone();
                Board simulated = board.CloneAndApply(atomic);
                Piece movedSim = null;
                try
                {
                    movedSim = simulated.GetPiece(atomic.EndRow, atomic.EndColumn);
                }
                catch
                {
                    movedSim = null;
                }
                if (movedSim == null)
                {
                    results.Add(chainStart);
                    expandedMoves.AddRange(results);
                    continue;
                }
                ExpandFromSimulated(board, simulated, chainStart, movedSim, results);
                if (results.Count == 0)
                    expandedMoves.Add(chainStart);
                else
                    expandedMoves.AddRange(results);
            }
            return expandedMoves;
        }

        private static void ExpandFromSimulated
            (Board originalBoard, Board simulatedBoard, Move chainSoFar, Piece simPiece, List<Move> result)
        {
            var nextMoves = simPiece.GetPotentialMoves(simulatedBoard)
                .Where(m => m.CapturedPieces.Count > 0).ToList();

            if (nextMoves.Count == 0)
            {
                result.Add(chainSoFar);
                return;
            }

            foreach (var next in nextMoves)
            {
                Move newChain = chainSoFar.Clone();
                newChain.EndRow = next.EndRow;
                newChain.EndColumn = next.EndColumn;

                foreach (var simCap in next.CapturedPieces)
                {
                    if (simCap == null) continue;
                    if (Board.IsInsideBoard(simCap.Row, simCap.Column))
                    {
                        var origCap = originalBoard.GetPiece(simCap.Row, simCap.Column);
                        if (origCap != null && !newChain.CapturedPieces.Contains(origCap))
                        {
                            newChain.CapturedPieces.Add(origCap);
                        }
                    }
                }

                Board newSim = simulatedBoard.CloneAndApply(next);
                Piece newSimPiece = null;
                try
                {
                    newSimPiece = newSim.GetPiece(next.EndRow, next.EndColumn);
                }
                catch
                {
                    newSimPiece = null;
                }

                if (newSimPiece == null)
                {
                    result.Add(newChain);
                    continue;
                }

                if (!(newSimPiece is CheckerPiece))
                {
                    newChain.IsPromotion = true;
                    result.Add(newChain);
                    continue;
                }

                ExpandFromSimulated(originalBoard, newSim, newChain, newSimPiece, result);
            }
        }
    }
}   
