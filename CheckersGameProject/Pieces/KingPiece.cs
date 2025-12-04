using System;
using System.Collections.Generic;
using CheckersGameProject.Game;

namespace CheckersGameProject.Pieces
{
    public class KingPiece : Piece
    {
        public KingPiece(PieceColor color, int row, int column)
            : base(color, row, column) { }

        public override List<Move> GetPotentialMoves(Board board)
        {
            List<Move> moves = new List<Move>();

            int[] dirs = { -1, 1 };

            foreach (int dRow in dirs)
            {
                foreach (int dCol in dirs)
                {
                    int r = Row + dRow;
                    int c = Column + dCol;

                    Piece enemy = null;

                    while (Board.IsInsideBoard(r, c))
                    {
                        var piece = board.GetPiece(r, c);

                        if (piece == null)
                        {
                            if (enemy == null)
                            {
                                moves.Add(new Move(Row, Column, r, c, this));
                            }
                            else
                            {
                                Move m = new Move(Row, Column, r, c, this);
                                m.AddCapturedPiece(enemy);
                                moves.Add(m);

                                break;
                            }
                        }
                        else
                        {
                            if (piece.Color == Color)
                                break;

                            if (enemy == null)
                            {
                                enemy = piece;
                                r += dRow;
                                c += dCol;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }

                        r += dRow;
                        c += dCol;
                    }
                }
            }

            return moves;
        }

        public override Piece Clone()
        {
            return new KingPiece(Color, Row, Column);
        }
    }
}
