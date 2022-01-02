using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    class Check
    {
        public bool IsChecked(PictureBox[][] board, int targetY, int targetX, bool turn) // Determine if king is checked
        {
            for (int y = 0; y < board.Length; y++) // loop board to determine if move is valid
            {
                for (int x = 0; x < board[y].Length; x++) 
                {
                    // only select pieces based on black or white's turn
                    if (board[y][x] == null || (y == targetY && x == targetX)) continue;
                    ChessBoard.pieceName selected;
                    if (turn && PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        selected = PieceDetails.selectedWhitePiece(board[y][x].Name);
                    else if (!turn && !PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        selected = PieceDetails.selectedBlackPiece(board[y][x].Name);
                    else continue;
                    switch (selected) // select a piece that will be moved that may result in a check
                    {
                        case ChessBoard.pieceName.Pawn:
                            if (PawnCheck(y, x, turn, targetY, targetX)) 
                                return true;
                            break;
                        case ChessBoard.pieceName.Rook:
                            if (RookCheck(board, y, x, targetY, targetX))
                                return true;
                            break;
                        case ChessBoard.pieceName.Knight:
                            if (KnightCheck(y, x, targetY, targetX))
                                return true;
                            break;
                        case ChessBoard.pieceName.Bishop:
                            if (BishopCheck(board, y, x, targetY, targetX))
                                return true;
                            break;
                        case ChessBoard.pieceName.Queen:
                            if (QueenCheck(board, y, x, targetY, targetX)) 
                                return true;
                            break;
                        case ChessBoard.pieceName.King:
                            if (KingCheck(y, x, targetY, targetX))
                                return true;
                            break;
                    }
                }
            }
            return false;
        }

        // used for stalemate where a piece needs to be moved where it is determined if by moving that piece will cause a check
        public bool IsAbleToMovePiece(PictureBox[][] board, int sourceY, int sourceX, int destinationY, int destinationX, bool turn, int targetY, int targetX)
        {
            bool IsMovable = false;
            // temporary change the board by moving the source to the destination and reverting back to its original state once result is found
            PictureBox source = board[sourceY][sourceX];
            PictureBox destination = board[destinationY][destinationX];
            board[sourceY][sourceX] = null;
            board[destinationY][destinationX] = source;
            if (IsChecked(board, targetY, targetX, turn)) // invalid move        
                IsMovable = false;
            else  // valid move
                IsMovable = true;
            board[sourceY][sourceX] = source;
            board[destinationY][destinationX] = destination;
            return IsMovable;
        }

        // store all the possible ways king can be checked in the current board 
        public void CheckForMultipleChecks(PictureBox[][] board, int targetY, int targetX, bool turn, List<int[]> checks) 
        {
            // loop through board to look if king has being checked
            for (int y = 0; y < board.Length; y++) 
            {
                for (int x = 0; x < board[y].Length; x++) 
               {
                    if (checks.Count > 1) return;
                    if (board[y][x] == null) continue;
                    ChessBoard.pieceName selected;
                    if (turn && PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        selected = PieceDetails.selectedWhitePiece(board[y][x].Name);
                    else if (!turn && !PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        selected = PieceDetails.selectedBlackPiece(board[y][x].Name);
                    else continue;
                    switch (selected) // select a piece that will be moved that may result in a check where results are stored as coordinates in an array
                    {
                        case ChessBoard.pieceName.Pawn:
                            if (PawnCheck(y, x, turn, targetY, targetX))
                                checks.Add(new int[] { y, x });                           
                            break;
                        case ChessBoard.pieceName.Rook:
                            if (RookCheck(board, y, x, targetY, targetX))
                                checks.Add(new int[] { y, x });
                            break;
                        case ChessBoard.pieceName.Knight:
                            if (KnightCheck(y, x, targetY, targetX))
                                checks.Add(new int[] { y, x });
                            break;
                        case ChessBoard.pieceName.Bishop:
                            if (BishopCheck(board, y, x, targetY, targetX))
                                checks.Add(new int[] { y, x });
                            break;
                        case ChessBoard.pieceName.Queen:
                            if (QueenCheck(board, y, x, targetY, targetX))
                                checks.Add(new int[] { y, x });
                            break;
                        case ChessBoard.pieceName.King:
                            if (KingCheck(y, x, targetY, targetX))
                                checks.Add(new int[] { y, x });
                            break;
                    }
                }
            }
        }

        private bool PawnCheck(int y, int x, bool turn, int targetY, int targetX) // pawn check king
        {
            // pawn must move diagonally north-east,north-west, south-east or south-west depending on the selected piece
            if (((turn && y - 1 == targetY) || (!turn && y + 1 == targetY)) && (x + 1 == targetX || x - 1 == targetX)) 
                return true;
            return false;
        }

        private bool RookCheck(PictureBox[][] board, int y, int x, int targetY, int targetX) // rook check king
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
                    if (Y == targetY && X == targetX)
                        return true;
                    else if (Y < 0 || Y > 7 || X < 0 || X > 7 || board[Y][X] != null) // rook cannot move anymore in this direction as it is either out of bound or there is another piece blocking it from its target
                        pieceDirection[j] = true;
                }
            }
            return false;
        }

        private bool KnightCheck(int y, int x, int targetY, int targetX) // knight check king
        {
            foreach (int[] dir in PieceDetails.KnightDirection) // loop through all different moves knight can make
            {
                if (y + dir[0] == targetY && x + dir[1] == targetX)
                    return true;
            }
            return false;
        }

        private bool BishopCheck(PictureBox[][] board, int y, int x, int targetY, int targetX) 
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
                    if (Y == targetY && X == targetX)
                        return true;
                    else if (Y < 0 || Y > 7 || X < 0 || X > 7 || board[Y][X] != null) // bishop cannot move anymore in this direction as it is either out of bound or there is another piece blocking it from its target
                        pieceDirection[j] = true;
                }
            }           
            return false;
        }

        private bool QueenCheck(PictureBox[][] board, int y, int x, int targetY, int targetX) 
        {
            bool[] pieceDirection = new bool[8]; // this array represents north, east, south, west, northeast, southeast, southwest, northwest and will reduce the processing time
            for (int i = 1; i < 8; i++)
            {
                // queen cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.QueenDirection.Length; j++)
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.QueenDirection[j][0];
                    int X = x + i * PieceDetails.QueenDirection[j][1];
                    if (Y == targetY && X == targetX)
                        return true;
                    else if (Y < 0 || Y > 7 || X < 0 || X > 7 || board[Y][X] != null) // queen cannot move anymore in this direction as it is either out of bound or there is another piece blocking it from its target
                        pieceDirection[j] = true;
                }
            }
            return false;
        }

        private bool KingCheck(int y, int x, int targetY, int targetX) // king check opposite king
        {
            // loop through all directions king can move by one square 
            for (int i = -1; i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++) 
                {
                    if (i == 0 && j == 0) continue; // this is ignored as this means that king has not moved
                    if (y + i == targetY && x + j == targetX)
                        return true;
                }
            }
            return false;
        }
    }
}
