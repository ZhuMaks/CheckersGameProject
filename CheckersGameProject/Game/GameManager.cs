using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CheckersGameProject.GameRecords;
using CheckersGameProject.Pieces;

namespace CheckersGameProject.Game
{
    public class GameManager
    {
        private readonly Board _board;
        private Players.IPlayer _player1;
        private Players.IPlayer _player2;
        private Players.IPlayer _currentPlayer;
        public Players.IPlayer Winner { get; private set; } = null;

        private List<MoveRecord> _moveHistory = new List<MoveRecord>();

        public delegate void MoveMadeHandler(Move move);
        public delegate void PlayerChangedHandler(Players.IPlayer newPlayer);
        public delegate void GameEndedHandler(Players.IPlayer winner);

        public event MoveMadeHandler MoveMade;
        public event PlayerChangedHandler PlayerChanged;
        public event GameEndedHandler GameEnded;

        public GameManager(Players.IPlayer player1, Players.IPlayer player2)
        {
            _board = new Board();
            _board.InitializeBoard();
            _player1 = player1;
            _player2 = player2;
            _currentPlayer = _player1;
        }

        public void Run()
        {
            Console.Clear();
            Console.WriteLine($"Current player: {_currentPlayer.Name} ({_currentPlayer.Color})");
            _board.PrintBoard();

            var legalMoves = MoveValidator.GetLegalMoves(_board, _currentPlayer.Color);
            if (legalMoves.Count == 0)
            {
                EndGame(_currentPlayer == _player1 ? _player2 : _player1);
                return;
            }

            Move move = _currentPlayer.GetMove(_board);
            if (move == null)
            {
                EndGame(_currentPlayer == _player1 ? _player2 : _player1);
                return;
            }

            _board.MovePiece(move);

            _moveHistory.Add(new MoveRecord
            {
                PlayerName = _currentPlayer.Name,
                PlayerColor = _currentPlayer.Color,
                StartRow = move.StartRow,
                StartColumn = move.StartColumn,
                EndRow = move.EndRow,
                EndColumn = move.EndColumn,
                Captured = move.CapturedPieces.Select(c => (c.Row, c.Column)).ToList()
            });

            MoveMade?.Invoke(move);

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();

            SwitchPlayer();
            PlayerChanged?.Invoke(_currentPlayer);


            SwitchPlayer();
            PlayerChanged?.Invoke(_currentPlayer);

            Run();
        }

        private void EndGame(Players.IPlayer winner)
        {
            Winner = winner;
            Console.WriteLine($"{_currentPlayer.Name} has no valid moves. Game over.");
            Console.WriteLine($"Winner: {Winner.Name} ({Winner.Color})");
            GameEnded?.Invoke(Winner);
            SaveMoveHistory();
        }


        private void SwitchPlayer()
        {
            if (_currentPlayer == _player1)
                _currentPlayer = _player2;
            else
                _currentPlayer = _player1;
        }

        private void SaveMoveHistory()
        {
            string json = JsonSerializer.Serialize(_moveHistory,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("last_game_moves.json", json);
            Console.WriteLine("Move history saved to last_game_moves.json");
        }

        public void LoadMoveHistory(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            string json = File.ReadAllText(filePath);
            var history = JsonSerializer.Deserialize<List<MoveRecord>>(json);
            if (history == null || history.Count == 0)
            {
                Console.WriteLine("No moves in file.");
                return;
            }

            Board tempBoard = new Board();
            tempBoard.InitializeBoard();

            foreach (var record in history)
            {
                var movedPiece = tempBoard.GetPiece(record.StartRow, record.StartColumn);
                if (movedPiece == null)
                {
                    Console.WriteLine($"No piece found at ({record.StartRow},{record.StartColumn})");
                    continue;
                }

                var move = Move.FromRecord(record, tempBoard);

                Console.Clear();
                Console.WriteLine($"{record.PlayerName} ({record.PlayerColor})" +
                    $" moves from ({record.StartRow},{record.StartColumn}) to ({record.EndRow},{record.EndColumn})");
                tempBoard.MovePiece(move);
                tempBoard.PrintBoard();

                Console.WriteLine("Press Enter for next move...");
                Console.ReadLine();
            }
        }
    }
}
