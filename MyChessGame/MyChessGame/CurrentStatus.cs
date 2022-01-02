﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    class CurrentStatus // game status result can either be check, checkmate, stalemate or normal
    {
        public ChessBoard.gameState TurnResult(PictureBox[][] board, bool turn) // turn will be represented as the current player that is making the move
        {
            Dictionary<int, HashSet<int>> targets = new Dictionary<int, HashSet<int>>(); // different places king can move are stored in a hash table
            int[] kingCoord = PieceDetails.FindKing(board, !turn); // represent the opposite king to determine status of game through "!turn"

            for (int i = -1; i <= 1; i++) // look for all the different places the king can move
            {
                for (int j = -1; j <= 1; j++) {
                    int y = kingCoord[0] + i;
                    int x = kingCoord[1] + j;
                    if ((i == 0 && j == 0) || y >= 8 || y < 0 || x >= 8 || x < 0) continue;
                    // check if selected king in the kingCoord can move to empty square or eat the opposite piece
                    if (board[y][x] == null || (turn && PieceDetails.IsPieceBlackorWhite(board[y][x].Name)) 
                        || (!turn && !PieceDetails.IsPieceBlackorWhite(board[y][x].Name)))
                    {
                        if (!targets.ContainsKey(y))
                            targets.Add(y, new HashSet<int>());
                        targets[y].Add(x);
                    }
                }
            }

            PictureBox originalPosition = board[kingCoord[0]][kingCoord[1]];
            board[kingCoord[0]][kingCoord[1]] = null;
            for (int y = 0; y < board.Length; y++) // determine if the king can move at any of these places without being checked by removing all invalid moves in target array
            {
                if (targets.Count == 0) // break out if no targets are left
                    break;
                for (int x = 0; x < board[y].Length; x++) 
                {
                    if (targets.Count == 0) // break out if no targets are left
                        break;
                    if (board[y][x] == null) continue;
                    ChessBoard.pieceName selected = ChessBoard.pieceName.None;
                    // piece being selected must be based on the current turn piece
                    if (turn && PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        selected = PieceDetails.selectedWhitePiece(board[y][x].Name);
                    else if (!turn && !PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        selected = PieceDetails.selectedBlackPiece(board[y][x].Name);
                    else continue;
                    switch (selected)
                    {
                        case ChessBoard.pieceName.Pawn:
                            PawnCheck(y, x, turn, targets);                          
                            break;
                        case ChessBoard.pieceName.Rook:
                            RookCheck(board, y, x, targets);
                            break;
                        case ChessBoard.pieceName.Knight:
                            KnightCheck(y, x, targets);
                            break;
                        case ChessBoard.pieceName.Bishop:
                            BishopCheck(board, y, x, targets);
                            break;
                        case ChessBoard.pieceName.Queen:
                            QueenCheck(board, y, x, targets);
                            break;
                        case ChessBoard.pieceName.King:
                            KingCheck(y, x, targets);
                            break;
                    }
                }
            }
            board[kingCoord[0]][kingCoord[1]] = originalPosition;
            List<int[]> checks = new List<int[]>(); // determine how many pieces are checking king
            Check IsChecked = new Check();
            IsChecked.CheckForMultipleChecks(board, kingCoord[0], kingCoord[1], turn, checks); // determine if king is checked

            /* 
                Checkmate Conditions:
                1. King must be checked
                2. King cannot move to any other squares without being checked again or an ally piece blocking it
                3. The piece checking the king cannot be blocked or eatten by another piece

                Stalemate Conditions:
                1. King must not be checked
                2. King cannot move to any other squares without being checked again or an ally piece blocking it
                3. No piece cannot be moved due to being checked if moved or blocked
                
                Check Conditions:
                1. King must be checked
                2. a) King can move to any other squares without being checked again or an ally piece blocking it
                   b) The piece checking the king can be blocked or eatten by another piece
                   
                Normal Conditions:
                1. King must not be checked
                2. a) King can move to other squares without being checked or blocked 
                   b) Other pieces can be moved without being blocked or king being checked 
            */

            if (checks.Count > 1 && targets.Count == 0) // this is checkmate
                return ChessBoard.gameState.Checkmate;
            else if (checks.Count == 0 && targets.Count == 0) // determine if stalemate
            {
                // determine if any other pieces can move aside from the king
                Movable canMove = new Movable();
                Check IsCheck = new Check();

                // determine if opposite player can move any piece to determine if its stalemate (for example, if it is currently the white's turn look for all the black pieces)
                for (int y = 0; y < board.Length; y++) 
                {
                    for (int x = 0; x < board[y].Length; x++)
                    {
                        if (board[y][x] == null) continue;
                        ChessBoard.pieceName piece = ChessBoard.pieceName.None;
                        if (!turn && PieceDetails.IsPieceBlackorWhite(board[y][x].Name)) 
                            piece = PieceDetails.selectedWhitePiece(board[y][x].Name);
                        else if (turn && !PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                            piece = PieceDetails.selectedBlackPiece(board[y][x].Name);
                        else continue;
                        switch (piece) // if opposite piece can be moved than it is not a stalemate without being checked
                        {
                            case ChessBoard.pieceName.Pawn:
                                if (canMove.PawnMove(board, y, x, !turn, kingCoord[0], kingCoord[1]))
                                    return ChessBoard.gameState.Normal;
                                break;
                            case ChessBoard.pieceName.Rook:
                                if (canMove.RookMove(board, y, x, !turn, kingCoord[0], kingCoord[1]))
                                    return ChessBoard.gameState.Normal;
                                break;
                            case ChessBoard.pieceName.Knight:
                                if (canMove.KnightMove(board, y, x, !turn, kingCoord[0], kingCoord[1]))
                                    return ChessBoard.gameState.Normal;
                                break;
                            case ChessBoard.pieceName.Bishop:
                                if (canMove.BishopMove(board, y, x, !turn, kingCoord[0], kingCoord[1]))
                                    return ChessBoard.gameState.Normal;
                                break;
                            case ChessBoard.pieceName.Queen:
                                if (canMove.QueenMove(board, y, x, !turn, kingCoord[0], kingCoord[1]))
                                    return ChessBoard.gameState.Normal;
                                break;
                            default:
                                break;
                        }
                    }
                }
                return ChessBoard.gameState.Stalemate;
            }
            else if (checks.Count == 1 && targets.Count == 0) // determine if checkmate
            {
                // check.Count must be 1 and not larger than 1 as two piece cannot be blocked at the same time to protect the king
                int Y = checks[0][0];
                int X = checks[0][1];
                PieceReach maxReach = new PieceReach();
                Dictionary<int, HashSet<int>> reach = new Dictionary<int, HashSet<int>>(); // the reach will determine how many different squares can be blocked to protect the king
                ChessBoard.pieceName selected = ChessBoard.pieceName.None;
                if (turn && PieceDetails.IsPieceBlackorWhite(board[Y][X].Name))
                    selected = PieceDetails.selectedWhitePiece(board[Y][X].Name);
                else if (!turn && !PieceDetails.IsPieceBlackorWhite(board[Y][X].Name))
                    selected = PieceDetails.selectedBlackPiece(board[Y][X].Name);
                reach.Add(Y, new HashSet<int>());
                reach[Y].Add(X); // this represents that piece being eaten rather than blocked
                switch (selected)
                {
                    case ChessBoard.pieceName.Rook:
                        maxReach.RookReach(Y, X, kingCoord[0], kingCoord[1], turn, reach);
                        break;
                    case ChessBoard.pieceName.Bishop:
                        maxReach.BishopReach(Y, X, kingCoord[0], kingCoord[1], turn, reach);
                        break;
                    case ChessBoard.pieceName.Queen:
                        maxReach.QueenReach(Y, X, kingCoord[0], kingCoord[1], turn, reach);
                        break;
                }
                BlockPiece block = new BlockPiece();
                if (!block.BlockMultipleTargets(board, !turn, reach)) // determine if piece checking king can be blocked or eaten
                    return ChessBoard.gameState.Checkmate;
            }
            return checks.Count > 0 ? ChessBoard.gameState.Check : ChessBoard.gameState.Normal;
        }

        private void RemoveTarget(int y, int x, Dictionary<int, HashSet<int>> targets) // remove target for invalid target which represents all the squares king can land on
        {
            targets[y].Remove(x);
            if (targets[y].Count == 0)
                targets.Remove(y);
        }

        private void PawnCheck(int y, int x, bool turn, Dictionary<int, HashSet<int>> targets) // pawn check king
        {
            if (turn) // when white pawn moves north-west or north-east
            {
                if (targets.ContainsKey(y - 1) && targets[y - 1].Contains(x + 1))
                    RemoveTarget(y - 1, x + 1, targets);
                if (targets.ContainsKey(y - 1) && targets[y - 1].Contains(x - 1))
                    RemoveTarget(y - 1, x - 1, targets);
            }
            else if (!turn) // when black pawn moves south-west or south-east
            {
                if (targets.ContainsKey(y + 1) && targets[y + 1].Contains(x + 1))
                    RemoveTarget(y + 1, x + 1, targets);
                if (targets.ContainsKey(y + 1) && targets[y + 1].Contains(x - 1))
                    RemoveTarget(y + 1, x - 1, targets);
            }
        }

        private void RookCheck(PictureBox[][] board, int y, int x, Dictionary<int, HashSet<int>> targets) // rook check king
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
                    if (targets.ContainsKey(Y) && targets[Y].Contains(X))
                        RemoveTarget(Y, X, targets);
                    if (Y < 0 || Y > 7 || X < 0 || X > 7 || board[Y][X] != null) // rook cannot move anymore in this direction as it is either out of bound or there is another piece blocking it from its target
                        pieceDirection[j] = true;
                }
            }
        }

        private void KnightCheck(int y, int x, Dictionary<int, HashSet<int>> targets) // knight check king
        {
            foreach (int[] dir in PieceDetails.KnightDirection) // loop through all the moves knight can make
            {
                if (targets.ContainsKey(y + dir[0]) && targets[y + dir[0]].Contains(x + dir[1]))
                    RemoveTarget(y + dir[0], x + dir[1], targets);
                if (targets.Count == 0) return; 
            }
        }

        private void BishopCheck(PictureBox[][] board, int y, int x, Dictionary<int, HashSet<int>> targets) // bishop check king
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
                    if (targets.ContainsKey(Y) && targets[Y].Contains(X))
                        RemoveTarget(Y, X, targets);
                    if (Y < 0 || Y > 7 || X < 0 || X > 7 || board[Y][X] != null) // bishop cannot move anymore in this direction as it is either out of bound or there is another piece blocking it from its target
                        pieceDirection[j] = true;
                }
            }        
        }

        private void QueenCheck(PictureBox[][] board, int y, int x, Dictionary<int, HashSet<int>> targets)
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
                    if (targets.ContainsKey(Y) && targets[Y].Contains(X))
                        RemoveTarget(Y, X, targets);
                    if (Y < 0 || Y > 7 || X < 0 || X > 7 || board[Y][X] != null) // queen cannot move anymore in this direction as it is either out of bound or there is another piece blocking it from its target
                        pieceDirection[j] = true;
                }
            }
        }

        private void KingCheck(int y, int x, Dictionary<int, HashSet<int>> targets) // king check opposite king
        {
            // loop through all directions king can move by one square 
            for (int i = -1; i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // this is ignored as this means that king has not moved
                    if (targets.ContainsKey(y + i) && targets[y + i].Contains(x + j))
                        RemoveTarget(y + i, x + j, targets);
                }               
            }
        }
    }
}
