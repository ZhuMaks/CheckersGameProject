using System;
using System.Collections.Generic;
using System.Linq;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.Game
{
    public class Board
    {
        private const int BoardSizeConst = 8;
        private readonly Piece[,] _cells;

        public Board() => _cells = new Piece[BoardSizeConst, BoardSizeConst];

        public static int BoardSize => BoardSizeConst;

        #region Initialization

        public void InitializeBoard()
        {
            InitializePieces(PieceColor.Black, 0, 3);
            InitializePieces(PieceColor.White, BoardSize - 3, BoardSize);
        }

        private void InitializePieces(PieceColor color, int startRow, int endRow)
        {
            for (int row = startRow; row < endRow; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    if ((row + col) % 2 == 1)
                        _cells[row, col] = new CheckerPiece(color, row, col);
                }
            }
        }

        #endregion

        #region Piece Access

        public Piece GetPiece(int row, int column)
        {
            EnsureInsideBoard(row, column);
            return _cells[row, column];
        }

        public void SetPiece(int row, int column, Piece piece)
        {
            EnsureInsideBoard(row, column);
            _cells[row, column] = piece;
            if (piece != null)
            {
                piece.Row = row;
                piece.Column = column;
            }
        }

        public static bool IsInsideBoard(int row, int column)
            => row >= 0 && row < BoardSizeConst && column >= 0 && column < BoardSizeConst;

        private static void EnsureInsideBoard(int row, int col)
        {
            if (!IsInsideBoard(row, col))
                throw new ArgumentOutOfRangeException($"Position ({row},{col}) is outside the board.");
        }

        #endregion

        #region Moving Pieces

        public void MovePiece(Move move)
        {
            if (move == null) throw new ArgumentNullException(nameof(move));

            ValidateMovePosition(move.StartRow, move.StartColumn, "Move.Start");
            ValidateMovePosition(move.EndRow, move.EndColumn, "Move.End");

            move.CapturedPieces ??= new List<Piece>();

            var movedPiece = GetPiece(move.StartRow, move.StartColumn)
                             ?? throw new InvalidOperationException(
                                 $"No piece to move at start position ({move.StartRow},{move.StartColumn}).");

            CaptureMiddlePieceIfJump(move);

            ApplyMove(move, movedPiece);

            RemoveCapturedPieces(move);

            PromoteIfEligible(move, movedPiece);
        }

        private void ValidateMovePosition(int row, int col, string positionName)
        {
            if (!IsInsideBoard(row, col))
                throw new ArgumentOutOfRangeException(positionName, $"{positionName} position is outside the board.");
        }

        private void CaptureMiddlePieceIfJump(Move move)
        {
            int dRow = move.EndRow - move.StartRow;
            int dCol = move.EndColumn - move.StartColumn;

            if (Math.Abs(dRow) == 2 && Math.Abs(dCol) == 2 && move.CapturedPieces.Count == 0)
            {
                int midRow = move.StartRow + dRow / 2;
                int midCol = move.StartColumn + dCol / 2;
                var middle = GetPiece(midRow, midCol);
                if (middle != null) move.CapturedPieces.Add(middle);
            }
        }

        private void ApplyMove(Move move, Piece movedPiece)
        {
            SetPiece(move.EndRow, move.EndColumn, movedPiece);
            SetPiece(move.StartRow, move.StartColumn, null);
        }

        private void RemoveCapturedPieces(Move move)
        {
            foreach (var piece in move.CapturedPieces ?? Enumerable.Empty<Piece>())
            {
                if (piece == null) continue;

                if (IsInsideBoard(piece.Row, piece.Column))
                    SetPiece(piece.Row, piece.Column, null);
                else
                    RemovePieceFromBoard(piece);
            }
        }

        private void RemovePieceFromBoard(Piece piece)
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (ReferenceEquals(_cells[r, c], piece))
                    {
                        _cells[r, c] = null;
                        return;
                    }
                }
            }
        }

        private void PromoteIfEligible(Move move, Piece movedPiece)
        {
            if (movedPiece is not CheckerPiece) return;

            bool reachedPromotionRank =
                (movedPiece.Color == PieceColor.White && move.EndRow == 0) ||
                (movedPiece.Color == PieceColor.Black && move.EndRow == BoardSize - 1);

            bool shouldPromote = reachedPromotionRank || move.IsPromotion;

            if (!shouldPromote) return;

            SetPiece(move.EndRow, move.EndColumn, new KingPiece(movedPiece.Color, move.EndRow, move.EndColumn));
            move.IsPromotion = true;
            Console.WriteLine("King! The figure was raised.");
        }

        #endregion

        #region Cloning

        public Board Clone()
        {
            var clone = new Board();
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    var piece = _cells[row, col];
                    if (piece != null)
                        clone.SetPiece(row, col, piece.Clone());
                }
            }
            return clone;
        }

        public Board CloneAndApply(Move move)
        {
            if (move == null) throw new ArgumentNullException(nameof(move));

            var clone = Clone();

            ValidateMovePosition(move.StartRow, move.StartColumn, "Move.Start");
            ValidateMovePosition(move.EndRow, move.EndColumn, "Move.End");

            var movedPiece = clone.GetPiece(move.StartRow, move.StartColumn)
                             ?? throw new InvalidOperationException(
                                 $"CloneAndApply: no piece to move at ({move.StartRow},{move.StartColumn}) on cloned board.");

            clone.CaptureMiddlePieceIfJump(move);
            clone.ApplyMove(move, movedPiece);
            clone.RemoveCapturedPieces(move);
            clone.PromoteIfEligible(move, movedPiece);

            return clone;
        }

        #endregion

        #region Utility

        public List<Piece> GetAllPiecesOfColor(PieceColor color)
        {
            var piecesList = new List<Piece>();
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    var piece = _cells[row, col];
                    if (piece != null && piece.Color == color)
                        piecesList.Add(piece);
                }
            }
            return piecesList;
        }

        public void PrintBoard(List<(int, int)> highlightPositions = null)
        {
            Console.WriteLine("\n-----------------------------------\n");

            Console.Write("     ");
            for (int col = 0; col < BoardSize; col++) Console.Write($" {col} ");
            Console.WriteLine("\n");

            for (int row = 0; row < BoardSize; row++)
            {
                Console.Write($"  {row}  ");
                for (int col = 0; col < BoardSize; col++)
                {
                    bool dark = (row + col) % 2 == 1;
                    bool highlight = highlightPositions?.Any(p => p.Item1 == row && p.Item2 == col) ?? false;

                    Console.BackgroundColor = highlight ? ConsoleColor.DarkYellow :
                                              dark ? ConsoleColor.DarkGray : ConsoleColor.Gray;

                    var piece = GetPiece(row, col);
                    string symbol = piece switch
                    {
                        KingPiece kp when kp.Color == PieceColor.White => " K ",
                        KingPiece kp when kp.Color == PieceColor.Black => " Q ",
                        CheckerPiece cp when cp.Color == PieceColor.White => " @ ",
                        CheckerPiece cp when cp.Color == PieceColor.Black => " O ",
                        _ => "   "
                    };

                    Console.Write(symbol);
                    Console.ResetColor();
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("-----------------------------------\n");
        }

        #endregion
    }
}
