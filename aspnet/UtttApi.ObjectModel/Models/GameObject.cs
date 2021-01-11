using UtttApi.ObjectModel.Abstracts;

namespace UtttApi.ObjectModel.Models
{
    /// <summary>
    /// Class to control to overall game
    /// </summary>
    public class GameObject : AEntity
    {
        private GlobalBoard _board = new GlobalBoard();
        public GlobalBoard Board { get => _board; set => _board = value; }
        // public Player XPlayer { get; set; }
        // public Player OPlayer { get; set; }
        public GameStatus Status { get; set; }
        public MarkShape CurrentPlayer { get; set; }

        public GameObject()
        {
            CurrentPlayer = MarkShape.X;
        }

        /// <summary>
        /// Make a move and mark boards/update focus and playablity accordingly
        /// </summary>
        /// <param name="move"></param>
        public void MakeMove(MoveObject move)
        {
            Board.MakeMove(move);
            UpdateGameStatus();
            Board.UpdateFocus(move, Status);
            if (Status == GameStatus.IN_PROGRESS)
            {
                SwitchCurrentPlayer();
            }
        }

        /// <summary>
        /// Check if a move has been played already and lb is in focus
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public bool IsValidMove(MoveObject move) => Board.IsValidMove(move);

        /// <summary>
        /// Switch the current player after a turn
        /// </summary>
        private void SwitchCurrentPlayer()
        {
            if (CurrentPlayer == MarkShape.X)
            {
                CurrentPlayer = MarkShape.O;
            }
            else
            {
                CurrentPlayer = MarkShape.X;
            }
        }

        /// <summary>
        /// Check if the game has ended, and update status accordingly
        /// </summary>
        private void UpdateGameStatus()
        {
            if (Board.HasTicTacToe(MarkShape.X))
            {
                Status = GameStatus.X_WINS;
            }
            else if (Board.HasTicTacToe(MarkShape.O))
            {
                Status = GameStatus.O_WINS;
            }
            else if (Board.IsFull())
            {
                Status = GameStatus.DRAW;
            }
        }
    }
}