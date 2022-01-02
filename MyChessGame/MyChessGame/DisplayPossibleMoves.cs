using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MyChessGame
{
    class DisplayPossibleMoves
    {
        int sizeOfBox = 45;
        Color backcolor;
        Graphics gObject;
        Brush brush;

        public DisplayPossibleMoves(Graphics GObject, Color color)
        {
            backcolor = color;
            gObject = GObject;
            brush = new SolidBrush(color);
        }

        public void DisplayMoves(int Y, int X, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
        {
            switch (sourcePieceType) 
            {
                case ChessBoard.pieceName.Pawn:
                    PawnPossibleMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                    return;
                case ChessBoard.pieceName.Rook:
                    RookPossibleMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                    return;
                case ChessBoard.pieceName.Knight:
                    KnightPossibleMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                    return;
                case ChessBoard.pieceName.Bishop:
                    BishopPossibleMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                    return;
                case ChessBoard.pieceName.Queen:
                    QueenPossibleMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                    return;
                default:
                    KingPossibleMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                    return;
            }
        }

        private void RookPossibleMoves(int y, int x, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
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
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y), sizeOfBox, sizeOfBox);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)))
                    {
                        PossiblePieceToTake.Add(board[Y][X]);
                        board[Y][X].BackColor = backcolor;
                        pieceDirection[j] = true;
                    }
                    else
                        pieceDirection[j] = true;
                }
            }
        }

        private void BishopPossibleMoves(int y, int x, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
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
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y), sizeOfBox, sizeOfBox);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)))
                    {
                        PossiblePieceToTake.Add(board[Y][X]);
                        board[Y][X].BackColor = backcolor;
                        pieceDirection[j] = true;
                    }
                    else
                        pieceDirection[j] = true;
                }
            }           
        }

        private void QueenPossibleMoves(int y, int x, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
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
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y), sizeOfBox, sizeOfBox);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name)))
                    {
                        PossiblePieceToTake.Add(board[Y][X]);
                        board[Y][X].BackColor = backcolor;
                        pieceDirection[j] = true;
                    }
                    else
                        pieceDirection[j] = true;
                }
            }
          
        }

        private void KingPossibleMoves(int Y, int X, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 && j == 0) || Y + i >= 8 || Y + i < 0 || X + j >= 8 || X + j < 0) continue;
                    if (board[Y + i][X + j] == null)
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X + j), PieceDetails.ToCoordinate(Y + i), sizeOfBox, sizeOfBox);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y + i][X + j].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y + i][X + j].Name)))
                    {
                        PossiblePieceToTake.Add(board[Y + i][X + j]);
                        board[Y + i][X + j].BackColor = backcolor;
                    }
                }
            }
        }

        private void PawnPossibleMoves(int Y, int X, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
        {
            if (turn)
            {
                if (Y - 1 >= 0)
                {
                    if (board[Y - 1][X] == null)
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y - 1), sizeOfBox, sizeOfBox);
                    if (X - 1 >= 0 && board[Y - 1][X - 1] != null && !PieceDetails.IsPieceBlackorWhite(board[Y - 1][X - 1].Name))
                    {
                        PossiblePieceToTake.Add(board[Y - 1][X - 1]);
                        board[Y - 1][X - 1].BackColor = backcolor;
                    }
                    if (X + 1 < 8 && board[Y - 1][X + 1] != null && !PieceDetails.IsPieceBlackorWhite(board[Y - 1][X + 1].Name))
                    {
                        PossiblePieceToTake.Add(board[Y - 1][X + 1]);
                        board[Y - 1][X + 1].BackColor = backcolor;
                    }
                }
                if (Y == 6 && Y - 2 >= 0 && board[Y - 2][X] == null && board[Y - 1][X] == null)
                    gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y - 2), sizeOfBox, sizeOfBox);
            }
            else
            {
                if (Y + 1 < 8)
                {
                    if (board[Y + 1][X] == null)
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y + 1), sizeOfBox, sizeOfBox);
                    if (X - 1 >= 0 && board[Y + 1][X - 1] != null && PieceDetails.IsPieceBlackorWhite(board[Y + 1][X - 1].Name))
                    {
                        PossiblePieceToTake.Add(board[Y + 1][X - 1]);
                        board[Y + 1][X - 1].BackColor = backcolor;
                    }
                    if (X + 1 < 8 && board[Y + 1][X + 1] != null && PieceDetails.IsPieceBlackorWhite(board[Y + 1][X + 1].Name))
                    {
                        PossiblePieceToTake.Add(board[Y + 1][X + 1]);
                        board[Y + 1][X + 1].BackColor = backcolor;
                    }
                }
                if (Y == 1 && Y + 2 < 8 && board[Y + 2][X] == null && board[Y + 1][X] == null)
                    gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X), PieceDetails.ToCoordinate(Y + 2), sizeOfBox, sizeOfBox);
            }
        }

        private void KnightPossibleMoves(int Y, int X, PictureBox[][] board, List<PictureBox> PossiblePieceToTake, bool turn, ChessBoard.pieceName sourcePieceType)
        {
            foreach (int[] dir in PieceDetails.KnightDirection)  
            {
                if ((dir[0] < 0 ? Y + dir[0] >= 0 : Y + dir[0] < 8) && (dir[1] < 0 ? X + dir[1] >= 0 : X + dir[1] < 8))
                {
                    if (board[Y + dir[0]][X + dir[1]] == null)
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X + dir[1]), PieceDetails.ToCoordinate(Y + dir[0]), sizeOfBox, sizeOfBox);
                    else if ((turn && !PieceDetails.IsPieceBlackorWhite(board[Y + dir[0]][X + dir[1]].Name)) || (!turn && PieceDetails.IsPieceBlackorWhite(board[Y + dir[0]][X + dir[1]].Name)))
                    {
                        gObject.FillRectangle(brush, PieceDetails.ToCoordinate(X + dir[1]), PieceDetails.ToCoordinate(Y + dir[0]), sizeOfBox, sizeOfBox);
                        PossiblePieceToTake.Add(board[Y + dir[0]][X + dir[1]]);
                        board[Y + dir[0]][X + dir[1]].BackColor = backcolor;
                    }
                }
            }         
        }
    }
}
