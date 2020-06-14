using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    static class PieceDetails
    {
        static private int[][] queenDirection = new int[][] { new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 1 }, new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, -1 } };
        // bishopDirection represents all the moves that bishop can make
        static private int[][] bishopDirection = new int[][] { new int[] { -1, 1 }, new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, -1 } };
        // rookDirection represents all the moves that rook can make
        static private int[][] rookDirection = new int[][] { new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 } };
        // knightDirection represents all the moves that knight can make
        static private int[][] knightDirection = new int[][] { new int[] { -2, 1 }, new int[] { -2, -1 }, new int[] { 2, 1 }, new int[] { 2, -1 }, new int[] { 1, 2 }, new int[] { 1, -2 }, new int[] { -1, 2 }, new int[] { -1, -2 } };
        // queenDirection represents all the moves that queen can make

        static public int[][] QueenDirection { get { return queenDirection; } }
        static public int[][] BishopDirection { get { return bishopDirection; } }
        static public int[][] RookDirection { get { return rookDirection; } }
        static public int[][] KnightDirection { get { return knightDirection; } }

        static public bool checkedAllDirections(bool[] directions) // determine if all valid piece directions has already been checked
        {
            foreach (bool dir in directions)
            {
                if (!dir)
                    return false;
            }
            return true;
        }

        static public bool movePiece(int destinationY, int destinationX, PictureBox source) // allows to move the picture box represented as a chess piece on the panel
        {
            source.Location = new Point(ToCoordinate(destinationX), ToCoordinate(destinationY));
            return true;
        }

        static public int FindCoordinate(int coord) 
        {
            for (int i = 0; i < 8; i++)
                if (coord >= 45 * i && coord < 45 * (i + 1))
                    return i;
            throw null;
        }

        static public int ToCoordinate(int axis) 
        {
            return axis * 45;
        }

        static public ChessBoard.pieceName selectedPiece(string piece)
        {
            switch (piece)
            {
                case "wp1":
                case "wp2":
                case "wp3":
                case "wp4":
                case "wp5":
                case "wp6":
                case "wp7":
                case "wp8":
                case "bp1":
                case "bp2":
                case "bp3":
                case "bp4":
                case "bp5":
                case "bp6":
                case "bp7":
                case "bp8":
                    return ChessBoard.pieceName.Pawn;
                case "wr1":
                case "wr2":
                case "br1":
                case "br2":
                    return ChessBoard.pieceName.Rook;
                case "wkn1":
                case "wkn2":
                case "bkn1":
                case "bkn2":
                    return ChessBoard.pieceName.Knight;
                case "wb1":
                case "wb2":
                case "bb1":
                case "bb2":
                    return ChessBoard.pieceName.Bishop;
                case "wq":
                case "bq":
                    return ChessBoard.pieceName.Queen;
                default:
                    return ChessBoard.pieceName.King;
            }
        }

        static public ChessBoard.pieceName selectedWhitePiece(string piece)
        {
            switch (piece)
            {
                case "wp1":
                case "wp2":
                case "wp3":
                case "wp4":
                case "wp5":
                case "wp6":
                case "wp7":
                case "wp8":
                    return ChessBoard.pieceName.Pawn;
                case "wr1":
                case "wr2":
                    return ChessBoard.pieceName.Rook;
                case "wkn1":
                case "wkn2":
                    return ChessBoard.pieceName.Knight;
                case "wb1":
                case "wb2":
                    return ChessBoard.pieceName.Bishop;
                case "wq":
                case "bb1":
                case "bb2":
                    return ChessBoard.pieceName.Queen;
                default:
                    return ChessBoard.pieceName.King;
            }
        }

        static public ChessBoard.pieceName selectedBlackPiece(string piece)
        {
            switch (piece)
            {
                case "bp1":
                case "bp2":
                case "bp3":
                case "bp4":
                case "bp5":
                case "bp6":
                case "bp7":
                case "bp8":
                    return ChessBoard.pieceName.Pawn;
                case "br1":
                case "br2":
                    return ChessBoard.pieceName.Rook;
                case "bkn1":
                case "bkn2":
                    return ChessBoard.pieceName.Knight;
                case "bb1":
                case "bb2":
                    return ChessBoard.pieceName.Bishop;
                case "bq":
                    return ChessBoard.pieceName.Queen;
                default:
                    return ChessBoard.pieceName.King;
            }
        }

        static public bool IsPieceBlackorWhite(string piece)
        {
            switch (piece)
            {
                case "wp1":
                case "wp2":
                case "wp3":
                case "wp4":
                case "wp5":
                case "wp6":
                case "wp7":
                case "wp8":
                case "wr1":
                case "wr2":
                case "wkn1":
                case "wkn2":
                case "wb1":
                case "wb2":
                case "wq":
                case "wk":
                    return true;
                default:
                    return false;
            }
        }

        static public int[] FindKing(PictureBox[][] board, bool turn) // find the location of the king to find if piece can be moved without being checked or if it may result in a check, checkmate or stalemate
        {
            for (int y = 0; y < board.Length; y++)
            {
                for (int x = 0; x < board[y].Length; x++)
                {
                    if (board[y][x] != null && ((turn && board[y][x].Name == "wk") || (!turn && board[y][x].Name == "bk")))
                        return new int[] { y, x };
                }
            }
            throw null;
        }
    }
}
