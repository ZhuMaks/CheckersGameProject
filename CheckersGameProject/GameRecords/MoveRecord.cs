using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.GameRecords
{
    [Serializable]
    public class MoveRecord
    {
        public string PlayerName { get; set; }
        public PieceColor PlayerColor { get; set; }
        public int StartRow { get; set; }
        public int StartColumn { get; set; }
        public int EndRow { get; set; }
        public int EndColumn { get; set; }
        public List<(int Row, int Col)> Captured { get; set; } = new List<(int, int)>();
    }
}
