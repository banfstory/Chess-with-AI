using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MyChessGame
{
    class CapturedSquares
    {
        int capturedBoard = 0;

        public int CaptureCount(PictureBox[][] board)
        {
            for (int i = 0; i < board.Length; i++) 
            {
                for (int j = 0; j < board[i].Length; j++) 
                {
                    if (board[i][j] == null)  
                        continue;
                    bool turn = PieceDetails.IsPieceBlackorWhite(board[i][j].Name);
                    ChessBoard.pieceName selected = PieceDetails.selectedPiece(board[i][j].Name);
                    switch (selected)
                    {
                        case ChessBoard.pieceName.Pawn:
                            PawnSquaresTaken(i, j, board, turn);
                            break;
                        case ChessBoard.pieceName.Rook:
                            RookSquaresTaken(i, j, board, turn);
                            break;
                        case ChessBoard.pieceName.Knight:
                            KnightSquaresTaken(i, j, board, turn);
                            break;
                        case ChessBoard.pieceName.Bishop:
                            BishopSquaresTaken(i, j, board, turn);
                            break;
                        case ChessBoard.pieceName.Queen:
                            QueenSquaresTaken(i, j, board, turn);
                            break;
                        default:
                            KingSquaresTaken(i, j, board, turn);
                            break;
                    }
                }
            }
            return capturedBoard;
        }

        private void capturedIncrDecr(bool t) 
        {
            if (t)
                capturedBoard++;
            else
                capturedBoard--;
        }

        private void RookSquaresTaken(int y, int x, PictureBox[][] board, bool turn)
        {
            bool[] pieceDirection = new bool[4]; // this array represents north, east, south, west and will reduce the processing time
            for (int i = 1; i < 8; i++)
            {
                // rook cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.RookDirection.Length; j++)
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.RookDirection[j][0];
                    int X = x + i * PieceDetails.RookDirection[j][1];
                    if (Y < 0 || Y > 7 || X < 0 || X > 7)
                        pieceDirection[j] = true;
                    else if (board[Y][X] == null) 
                        capturedIncrDecr(turn);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)))
                    {
                        capturedIncrDecr(turn);
                        pieceDirection[j] = true;
                    }
                    else
                        pieceDirection[j] = true;
                }
            }
        }

        private void BishopSquaresTaken(int y, int x, PictureBox[][] board, bool turn)
        {
            bool[] pieceDirection = new bool[4]; // this array represents northeast, southeast, southwest, northwest and will reduce the processing time
            for (int i = 1; i < 8; i++)
            {
                // bishop cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.BishopDirection.Length; j++)
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.BishopDirection[j][0];
                    int X = x + i * PieceDetails.BishopDirection[j][1];
                    if (Y < 0 || Y > 7 || X < 0 || X > 7)
                        pieceDirection[j] = true;
                    else if (board[Y][X] == null)
                        capturedIncrDecr(turn);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)))
                    {
                        capturedIncrDecr(turn);
                        pieceDirection[j] = true;
                    }
                    else
                        pieceDirection[j] = true;
                }
            }
        }

        private void QueenSquaresTaken(int y, int x, PictureBox[][] board, bool turn)
        {
            bool[] pieceDirection = new bool[8]; // this array represents north, east, south, west, northeast, southeast, southwest, northwest and will reduce the processing time
            for (int i = 1; i < 8; i++)
            {
                // bishop cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.QueenDirection.Length; j++)
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.QueenDirection[j][0];
                    int X = x + i * PieceDetails.QueenDirection[j][1];
                    if (Y < 0 || Y > 7 || X < 0 || X > 7)
                        pieceDirection[j] = true;
                    else if (board[Y][X] == null)
                        capturedIncrDecr(turn);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)))
                    {
                        capturedIncrDecr(turn);
                        pieceDirection[j] = true;
                    }
                    else
                        pieceDirection[j] = true;
                }
            }

        }

        private void KingSquaresTaken(int Y, int X, PictureBox[][] board, bool turn)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 && j == 0) || Y + i >= 8 || Y + i < 0 || X + j >= 8 || X + j < 0) continue;
                    if (board[Y + i][X + j] == null)
                        capturedIncrDecr(turn);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y + i][X + j].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y + i][X + j].Name)))
                        capturedIncrDecr(turn);
                }
            }
        }

        private void PawnSquaresTaken(int Y, int X, PictureBox[][] board, bool turn)
        {
            if (turn)
            {
                if (Y - 1 >= 0)
                {
                    if (board[Y - 1][X] == null)
                        capturedIncrDecr(turn);
                    if (X - 1 >= 0 && board[Y - 1][X - 1] != null && !PieceDetails.IsPieceBlackorWhite(board[Y - 1][X - 1].Name))
                        capturedIncrDecr(turn);
                    if (X + 1 < 8 && board[Y - 1][X + 1] != null && !PieceDetails.IsPieceBlackorWhite(board[Y - 1][X + 1].Name))
                        capturedIncrDecr(turn);
                }
                if (Y == 6 && Y - 2 >= 0 && board[Y - 2][X] == null && board[Y - 1][X] == null)
                    capturedIncrDecr(turn);
            }
            else
            {
                if (Y + 1 < 8)
                {
                    if (board[Y + 1][X] == null)
                        capturedIncrDecr(turn);
                    if (X - 1 >= 0 && board[Y + 1][X - 1] != null && PieceDetails.IsPieceBlackorWhite(board[Y + 1][X - 1].Name))
                        capturedIncrDecr(turn);
                    if (X + 1 < 8 && board[Y + 1][X + 1] != null && PieceDetails.IsPieceBlackorWhite(board[Y + 1][X + 1].Name))
                        capturedIncrDecr(turn);
                }
                if (Y == 1 && Y + 2 < 8 && board[Y + 2][X] == null && board[Y + 1][X] == null)
                    capturedIncrDecr(turn);
            }
        }

        private void KnightSquaresTaken(int Y, int X, PictureBox[][] board, bool turn)
        {
            foreach (int[] dir in PieceDetails.KnightDirection)
            {
                if ((dir[0] < 0 ? Y + dir[0] >= 0 : Y + dir[0] < 8) && (dir[1] < 0 ? X + dir[1] >= 0 : X + dir[1] < 8))
                {
                    if (board[Y + dir[0]][X + dir[1]] == null)
                        capturedIncrDecr(turn);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y + dir[0]][X + dir[1]].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y + dir[0]][X + dir[1]].Name)))
                        capturedIncrDecr(turn);
                }
            }
        }
    }
}
