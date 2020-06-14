using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    abstract class Piece
    {
        History History;
        protected bool turn;
        protected PictureBox source;
        protected PictureBox destination;
        protected int sourceY;
        protected int sourceX;
        protected int destinationY;
        protected int destinationX;
        protected int diffY; // difference between destination to source for Y which determine its direction and how far
        protected int diffX; // difference between destination to source for X which determine its direction and how far

        protected void InitalizePiece(PictureBox[][] board, bool Turn, int SourceY, int SourceX, int DestinationY, int DestinationX, History history)
        {
            History = history;
            turn = Turn;
            source = board[SourceY][SourceX];
            destination = board[DestinationY][DestinationX];
            sourceY = SourceY;
            sourceX = SourceX;
            destinationY = DestinationY;
            destinationX = DestinationX;
            diffY = destinationY - sourceY;
            diffX = destinationX - sourceX;
        }

        protected bool GameState(PictureBox[][] board) // determine the game state whether its check, checkmate, stalemate or normal
        {
            Check check = new Check();
            board[sourceY][sourceX] = null;
            board[destinationY][destinationX] = source;
            int[] kingCoord = PieceDetails.FindKing(board, turn);

            if (check.IsChecked(board, kingCoord[0], kingCoord[1], !turn)) // determine if this is an illegal move by check
            {
                board[sourceY][sourceX] = source;
                board[destinationY][destinationX] = destination;
                return false;
            }
            else
            {
                CurrentStatus status = new CurrentStatus();
                ChessBoard.gameState state = status.TurnResult(board, turn);

                if (state == ChessBoard.gameState.Check)
                {
                    setHistory(ChessBoard.gameState.Check);           
                }
                else if (state == ChessBoard.gameState.Checkmate)
                {
                    setHistory(ChessBoard.gameState.Checkmate);
                }
                else if (state == ChessBoard.gameState.Stalemate)
                {
                    setHistory(ChessBoard.gameState.Stalemate);
                }
                else
                {
                    setHistory(ChessBoard.gameState.Normal);
                }
            }
            return true;
        }

        private void setHistory(ChessBoard.gameState state) // set history variables with values if turn was valid for the undo or redo feature
        {
            History currentHistory = new History(turn, sourceY, sourceX, destinationY, destinationX, source, destination, state);
            History.Next = currentHistory;
            currentHistory.Prev = History;
        }

        public abstract bool Move(PictureBox[][] board); // this method must be overriden by derived class for consistency
    }
}
