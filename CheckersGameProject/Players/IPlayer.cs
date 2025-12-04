using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersGameProject.Game;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.Players
{
    public interface IPlayer
    {
        string Name { get; }
        Pieces.PieceColor Color { get; }
        Move GetMove(Board board);
    }
}
