using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGameProject.Game
{
    public class Move
    {
        private readonly int _startRow;
        private int _endRow;
        private readonly int _startColumn;
        private int _endColumn;
        private readonly Pieces.Piece _movedPiece;
        private List<Pieces.Piece> _capturedPieces;
        private bool _isPromotion;

        public Move(int startRow, int startColumn, int endRow, int endColumn, Pieces.Piece movedPiece)
        {
            _startRow = startRow;
            _startColumn = startColumn;
            _endRow = endRow;
            _endColumn = endColumn;
            _movedPiece = movedPiece;
            _capturedPieces = new List<Pieces.Piece>();
        }
        public int StartRow
        {
            get { return _startRow; }
        }
        public int StartColumn
        {
            get { return _startColumn; }
        }
        public int EndRow
        {
            get { return _endRow; }
            set { _endRow = value; }
        }
        public int EndColumn
        {
            get { return _endColumn; }
            set { _endColumn = value; }
        }
        public List<Pieces.Piece> CapturedPieces
        {
            get { return _capturedPieces; }
            set { _capturedPieces = value; }
        }
        public bool IsPromotion
        {
            get { return _isPromotion; }
            set { _isPromotion = value; }
        }
        public Pieces.Piece MovedPiece
        {
            get { return _movedPiece; }
        }

        public void AddCapturedPiece(Pieces.Piece piece)
        {
            CapturedPieces.Add(piece);
        }

        public Move Clone()
        {
            var clone = new Move(StartRow, StartColumn, EndRow, EndColumn, MovedPiece);
            clone.CapturedPieces.AddRange(CapturedPieces);
            clone.IsPromotion = IsPromotion;
            return clone;
        }

        public static Move FromRecord(GameRecords.MoveRecord record, Board board)
        {
            var piece = board.GetPiece(record.StartRow, record.StartColumn);
            if (piece == null)
                throw new Exception($"No piece at {record.StartRow},{record.StartColumn}");

            var move = new Move(record.StartRow, record.StartColumn, record.EndRow, record.EndColumn, piece);
            foreach (var c in record.Captured)
            {
                var capturedPiece = board.GetPiece(c.Row, c.Col);
                if (capturedPiece != null)
                    move.AddCapturedPiece(capturedPiece);
            }
            return move;
        }
    }
}
