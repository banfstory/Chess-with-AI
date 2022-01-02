using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    class Pawn : Piece
    {
        public Pawn(PictureBox[][] board, int sourceY, int sourceX, int destinationY, int destinationX, History history, bool turn)
        {
            InitalizePiece(board, turn, sourceY, sourceX, destinationY, destinationX, history);
        }

        public override bool Move(PictureBox[][] board) // move pawn
        {
            if ((turn && diffY == -1) || (!turn && diffY == 1)) // pawn can only move forward and not backwards
            {
                if (destination != null && (diffX == 1 || diffX == -1)) // if moving diagonally pawn must not land on an empty square
                {
                    if (GameState(board)) // determine if piece can be moved without their king being checked  
                    {
                        destination.Visible = false;
                        return PieceDetails.movePiece(destinationY, destinationX, source);
                    }
                }
                else if (destination == null && sourceX == destinationX) // if moving forward but only one square forward
                {
                    if (GameState(board)) // determine if piece can be moved without their king being checked  
                    {
                        return PieceDetails.movePiece(destinationY, destinationX, source);
                    }
                }
            }
            else if (destination == null && sourceX == destinationX && ((turn && diffY == -2 && board[sourceY - 1][sourceX] == null && sourceY == 6) || (!turn && diffY == 2 && board[sourceY + 1][sourceX] == null && sourceY == 1))) 
            {
                // moving pawn forward two blocks must land on an empty square and must not be moved before
                if (GameState(board)) // determine if piece can be moved without their king being checked  
                {
                    return PieceDetails.movePiece(destinationY, destinationX, source);
                }
            }
            return false;
        }
    }
}
