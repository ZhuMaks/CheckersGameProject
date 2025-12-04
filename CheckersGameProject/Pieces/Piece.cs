using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersGameProject.Game;

namespace CheckersGameProject.Pieces
{
    public abstract class Piece
    {
        private readonly PieceColor _color;
        private int _row;
        private int _column;

        protected Piece(PieceColor color, int row, int column)
        {
            _color = color;
            _row = row;
            _column = column;
        }

        public PieceColor Color
        {
            get
            {
                return _color;
            }
        }
        public int Row 
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
            }
        }
        public int Column 
        {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
            }
        }
        public abstract List<Move> GetPotentialMoves(Board board);

        public abstract Piece Clone();

    }
}
