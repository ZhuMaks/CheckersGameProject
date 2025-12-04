using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersGameProject.Game;

namespace CheckersGameProject.Pieces
{
    public class CheckerPiece : Piece
    {
        public CheckerPiece(PieceColor color, int row, int column) : base(color, row, column)
        {
        }

        public override List<Move> GetPotentialMoves(Board board)
        {
            List<Move> potentialMoves = new List<Move>();
            int direction;
            if (Color == PieceColor.White)
                direction = -1; 
            else
                direction = 1;  

            int[] columnOffsets = { -1, 1 }; 

            for (int i = 0; i < columnOffsets.Length; i++)
            {
                int newRow = Row + direction;
                int newColumn = Column + columnOffsets[i];
                if (Board.IsInsideBoard(newRow, newColumn) && board.GetPiece(newRow, newColumn) == null)
                {
                    potentialMoves.Add(new Move(Row, Column, newRow, newColumn, this));
                }
            }

            columnOffsets = new int[] { -2, 2 };
            for (int i = 0; i < columnOffsets.Length; i++)
            {
                int newRow = Row + 2 * direction;
                int newColumn = Column + columnOffsets[i];
                int midRow = Row + direction;
                int midColumn = Column + columnOffsets[i] / 2;
                if (Board.IsInsideBoard(newRow, newColumn) && board.GetPiece(newRow, newColumn) == null)
                {
                    Piece midPiece = board.GetPiece(midRow, midColumn);
                    if (midPiece != null && midPiece.Color != this.Color)
                    {
                        Move captureMove = new Move(Row, Column, newRow, newColumn, this);
                        captureMove.AddCapturedPiece(midPiece);
                        potentialMoves.Add(captureMove);
                    }
                }
            }
            return potentialMoves;
        }

        public override Piece Clone()
        {
            return new CheckerPiece(Color, Row, Column);
        }
    }
}
