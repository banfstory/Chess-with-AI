using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    class King : Piece
    {
        public King(PictureBox[][] board, int sourceY, int sourceX, int destinationY, int destinationX, History history, bool turn)
        {
            InitalizePiece(board, turn, sourceY, sourceX, destinationY, destinationX, history);
        }

        override public bool Move(PictureBox[][] board) // move king - can only move one block in each direction
        {
            double diffYX = diffX != 0 ? ((double)diffY / (double)diffX) : 0;  
            if (diffY == 0) // moving east or west
            {
                if (diffX != -1 && diffX != 1) // if king can't move east or west than the it is an invalid move
                    return false;
            }
            else if (diffX == 0) // moving north or south
            {
                if (diffY != -1 && diffY != 1) // if king can't move north or south than the it is an invalid move
                    return false;
            }
            else if (diffYX == 1) // move south-east or north-west
            {
                if (!(diffY == 1 && diffX == 1) && !(diffY == -1 && diffX == -1)) // if king can't move south-east or north-westthan the it is an invalid move
                    return false;
            }
            else if (diffYX == -1) // move north-east or south-west
            {
                if (!(diffY == -1 && diffX == 1) && !(diffY == 1 && diffX == -1)) // if king can't move north-east or south-west than the it is an invalid move
                    return false;
            }
            else
                return false;

            if (GameState(board))  // determine if piece can be moved without their king being checked         
            {
                if (destination != null) // if piece is eatting another piece
                {
                    destination.Visible = false;
                    return PieceDetails.movePiece(destinationY, destinationX, source);
                }
                else // if piece is moving to an empty square
                {
                    return PieceDetails.movePiece(destinationY, destinationX, source);
                }
            }
            return false;
        }
    }
}
