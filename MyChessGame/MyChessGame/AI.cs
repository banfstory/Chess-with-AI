using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MyChessGame
{
    class AI // Minimax with Alpha Beta Pruning Algorithm
    {
        int movesCount;
        int bestPath;
        int squareCount;
        private const int kingvalue = 10000;
        private int BoardState; // determine the current best board state which will be used to prune results from a path that may provide a worse result
        private int SourceY;
        private int SourceX;
        private int DestinationY;
        private int DestinationX;

        public AI(int count) 
        {
            movesCount = count;
        }

        // if it is currently at white's turn, it will try to look for the maximum boardstate value but if it is at black's turn, it will look for the minimum boardstate
        public int MiniMax(PictureBox[][] board, bool turn, int movesLimit, int currentBoardState, AIResult finalResult) 
        {
            if (movesCount == movesLimit) // this will be the base case which is an end point where this end value will be compared with other end values
                return currentBoardState;
            BoardState = turn ? int.MinValue : int.MaxValue; // boardstate looks at the materials represented in the current board (white looks for the maximum board state and black looks for the minimum board state)
            squareCount = turn ? int.MinValue : int.MaxValue; // squarecount looks at the number of spaces that can be moved represented in the current board (white looks for the maximum squarecount and black looks for the minimum squarecount - only compares when movecount is 0)
            bestPath = currentBoardState;
            for (int y = 0; y < board.Length; y++)
            {
                for (int x = 0; x < board[y].Length; x++)
                {
                    if (board[y][x] == null) continue;
                    ChessBoard.pieceName name = ChessBoard.pieceName.None;
                    // look for the piece that corresponds with its own selected piece (black must select a black piece and white must select a white piece)
                    if (turn && PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        name = PieceDetails.selectedWhitePiece(board[y][x].Name);
                    else if (!turn && !PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                        name = PieceDetails.selectedBlackPiece(board[y][x].Name);
                    else continue;
                    switch (name)
                    {
                        case ChessBoard.pieceName.Pawn:
                            if (PawnAI(board, y, x, turn, movesCount, movesLimit, currentBoardState, finalResult)) // if these return true than stop perform best path calculation and break out of this path
                                return turn ? int.MaxValue : int.MinValue;
                            break;
                        case ChessBoard.pieceName.Rook:
                            if (RookAI(board, y, x, turn, movesCount, movesLimit, currentBoardState, finalResult))
                                return turn ? int.MaxValue : int.MinValue;
                            break;
                        case ChessBoard.pieceName.Knight:
                            if (KnightAI(board, y, x, turn, movesCount, movesLimit, currentBoardState, finalResult))
                                return turn ? int.MaxValue : int.MinValue;
                            break;
                        case ChessBoard.pieceName.Bishop:
                            if (BishopAI(board, y, x, turn, movesCount, movesLimit, currentBoardState, finalResult))
                                return turn ? int.MaxValue : int.MinValue;
                            break;
                        case ChessBoard.pieceName.Queen:
                            if (QueenAI(board, y, x, turn, movesCount, movesLimit, currentBoardState, finalResult))
                                return turn ? int.MaxValue : int.MinValue;
                            break;
                        case ChessBoard.pieceName.King:
                            if (KingAI(board, y, x, turn, movesCount, movesLimit, currentBoardState, finalResult))
                                return turn ? int.MaxValue : int.MinValue;
                            break;
                    }
                }
            }
            if (movesCount == 0) // this will store the beginning point of the best path which will allow AI to make that specific move
            {
                finalResult.SourceY = SourceY;
                finalResult.SourceX = SourceX;
                finalResult.DestinationY = DestinationY;
                finalResult.DestinationX = DestinationX;
            }
            return BoardState;
        }

        private bool PawnAI(PictureBox[][] board, int y, int x, bool turn, int movesCount, int movesLimit, int currentBoardState, AIResult finalResult) 
        {
            if (turn) // white pawn move
            {
                if (y - 1 >= 0)
                {
                    if (board[y - 1][x] == null) // white pawn moving 1 square north to empty square
                    {
                        PictureBox[][] newBoard = new PictureBox[8][];
                        setNewBoard(board, newBoard, y, x, y - 1, x); // create new board object to be used as parameter to call minimax function
                        if (!IsValidMove(newBoard, turn))
                        {
                            AI newAI = new AI(movesCount + 1);
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult); // find best path
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, y - 1, x, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, y - 1, x, 0);
                        }
                    }
                    if (x - 1 >= 0 && board[y - 1][x - 1] != null && !PieceDetails.IsPieceBlackorWhite(board[y - 1][x - 1].Name)) // white pawn eat black piece at north-west
                    {
                        int value = PieceValue(PieceDetails.selectedBlackPiece(board[y - 1][x - 1].Name));
                        if (value == kingvalue) // if king is eatten prune this path completely
                            return true;
                        if (bestPath <= currentBoardState + value) // prune path that are smaller than the current board state
                        {
                            PictureBox[][] newBoard = new PictureBox[8][];
                            setNewBoard(board, newBoard, y, x, y - 1, x - 1);
                            if (!IsValidMove(newBoard, turn))
                            {
                                bestPath = currentBoardState + value;
                                AI newAI = new AI(movesCount + 1);
                                int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                                if (movesCount == 0)
                                {
                                    CapturedSquares capture = new CapturedSquares();
                                    setResult(turn, bestBoardState, y, x, y - 1, x - 1, capture.CaptureCount(newBoard));
                                }
                                else
                                    setResult(turn, bestBoardState, y, x, y - 1, x - 1, 0);
                            }
                        }
                    }
                    if (x + 1 < 8 && board[y - 1][x + 1] != null && !PieceDetails.IsPieceBlackorWhite(board[y - 1][x + 1].Name)) // white pawn eat black piece at north-east 
                    {
                        int value = PieceValue(PieceDetails.selectedBlackPiece(board[y - 1][x + 1].Name));
                        if (value == kingvalue)
                            return true;
                        if (bestPath <= currentBoardState + value)
                        {
                            PictureBox[][] newBoard = new PictureBox[8][];
                            setNewBoard(board, newBoard, y, x, y - 1, x + 1);
                            if (!IsValidMove(newBoard, turn))
                            {
                                bestPath = currentBoardState + value;
                                AI newAI = new AI(movesCount + 1);
                                int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                                if (movesCount == 0)
                                {
                                    CapturedSquares capture = new CapturedSquares();
                                    setResult(turn, bestBoardState, y, x, y - 1, x + 1, capture.CaptureCount(newBoard));
                                }
                                else
                                    setResult(turn, bestBoardState, y, x, y - 1, x + 1, 0);
                            }
                        }
                    }
                }
                if (y - 2 >= 0 && y == 6 && board[y - 1][x] == null && board[y - 2][x] == null) // white pawn moves 2 squares north to empty square 
                {
                    PictureBox[][] newBoard = new PictureBox[8][];
                    setNewBoard(board, newBoard, y, x, y - 2, x);
                    if (!IsValidMove(newBoard, turn))
                    {
                        AI newAI = new AI(movesCount + 1);
                        int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                        if (movesCount == 0)
                        {
                            CapturedSquares capture = new CapturedSquares();
                            setResult(turn, bestBoardState, y, x, y - 2, x, capture.CaptureCount(newBoard));
                        }
                        else
                            setResult(turn, bestBoardState, y, x, y - 2, x, 0);
                    }
                }
            }
            else // black pawn move
            {
                if (y + 1 < 8)
                {
                    if (board[y + 1][x] == null) // black pawn moving 1 square south to empty square
                    {
                        PictureBox[][] newBoard = new PictureBox[8][];
                        setNewBoard(board, newBoard, y, x, y + 1, x); // create new board object to be used as parameter to call minimax function
                        if (!IsValidMove(newBoard, turn))
                        {
                            AI newAI = new AI(movesCount + 1);
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, y + 1, x, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, y + 1, x, 0);
                        }          
                    }
                    if (x - 1 >= 0 && board[y + 1][x - 1] != null && PieceDetails.IsPieceBlackorWhite(board[y + 1][x - 1].Name)) // black pawn eat white piece at south-west
                    {
                        int value = PieceValue(PieceDetails.selectedWhitePiece(board[y + 1][x - 1].Name));
                        if (value == kingvalue) // if king is eatten prune this path completely
                            return true;
                        if (bestPath >= currentBoardState - value) // prune path that are larger than the current board state
                        {
                            PictureBox[][] newBoard = new PictureBox[8][];
                            setNewBoard(board, newBoard, y, x, y + 1, x - 1);
                            if (!IsValidMove(newBoard, turn))
                            {
                                bestPath = currentBoardState - value;
                                AI newAI = new AI(movesCount + 1);
                                int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                                if (movesCount == 0)
                                {
                                    CapturedSquares capture = new CapturedSquares();
                                    setResult(turn, bestBoardState, y, x, y + 1, x - 1, capture.CaptureCount(newBoard));
                                }
                                else
                                    setResult(turn, bestBoardState, y, x, y + 1, x - 1, 0);
                            }
                        }
                    }
                    if (x + 1 < 8 && board[y + 1][x + 1] != null && PieceDetails.IsPieceBlackorWhite(board[y + 1][x + 1].Name)) // black pawn eat white piece at south-east  
                    {
                        int value = PieceValue(PieceDetails.selectedWhitePiece(board[y + 1][x + 1].Name));
                        if (value == kingvalue)
                            return true;
                        if (bestPath >= currentBoardState - value)
                        {
                            PictureBox[][] newBoard = new PictureBox[8][];
                            setNewBoard(board, newBoard, y, x, y + 1, x + 1);
                            if (!IsValidMove(newBoard, turn))
                            {
                                bestPath = currentBoardState - value;
                                AI newAI = new AI(movesCount + 1);
                                int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                                if (movesCount == 0)
                                {
                                    CapturedSquares capture = new CapturedSquares();
                                    setResult(turn, bestBoardState, y, x, y + 1, x + 1, capture.CaptureCount(newBoard));
                                }
                                else
                                    setResult(turn, bestBoardState, y, x, y + 1, x + 1, 0);
                            }
                        }
                    }
                }
                if (y + 2 < 8 && y == 1 && board[y + 1][x] == null && board[y + 2][x] == null) // white pawn moves 2 squares south to empty square 
                {
                    PictureBox[][] newBoard = new PictureBox[8][];
                    setNewBoard(board, newBoard, y, x, y + 2, x);
                    if (!IsValidMove(newBoard, turn))
                    {
                        AI newAI = new AI(movesCount + 1);
                        int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                        if (movesCount == 0)
                        {
                            CapturedSquares capture = new CapturedSquares();
                            setResult(turn, bestBoardState, y, x, y + 2, x, capture.CaptureCount(newBoard));
                        }
                        else
                            setResult(turn, bestBoardState, y, x, y + 2, x, 0);
                    }
                }
            }
            return false;
        }

        private bool RookAI(PictureBox[][] board, int y, int x, bool turn, int movesCount, int movesLimit, int currentBoardState, AIResult finalResult)
        {
            bool[] pieceDirection = new bool[4]; // this array represents north, east, south, west and will reduce the processing time

            for (int i = 1; i < 8; i++) // represents the distance to be moved
            {
                // rook cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.RookDirection.Length; j++) // represents the array to define the direction of the move
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.RookDirection[j][0];
                    int X = x + i * PieceDetails.RookDirection[j][1];
                    if (Y >= 0 && Y < 8 && X >= 0 && X < 8)
                    {
                        PictureBox[][] newBoard = new PictureBox[8][];
                        if (board[Y][X] == null) // rook moving to empty square
                        {
                            setNewBoard(board, newBoard, y, x, Y, X); // create new board object to be used as parameter to call minimax function
                            if (IsValidMove(newBoard, turn))
                                continue;
                            AI newAI = new AI(movesCount + 1); 
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, Y, X, 0);
                        }
                        else // rook moving to square that is not empty
                        {
                            bool pieceColor = PieceDetails.IsPieceBlackorWhite(board[Y][X].Name);
                            int value = PieceValue(PieceDetails.selectedPiece(board[Y][X].Name));
                            if (turn && !pieceColor) // white rook eat black piece
                            {
                                if (value == kingvalue) // if king is eatten prune this path completely
                                    return true;
                                if (bestPath <= currentBoardState + value) // prune path that are smaller than the current board state
                                {
                                    setNewBoard(board, newBoard, y, x, Y, X);  // create new board object to be used as parameter to call minimax function
                                    if (IsValidMove(newBoard, turn))
                                    {
                                        pieceDirection[j] = true;
                                        continue;
                                    }
                                    bestPath = currentBoardState + value;
                                    AI newAI = new AI(movesCount + 1);
                                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                                    if (movesCount == 0)
                                    {
                                        CapturedSquares capture = new CapturedSquares();
                                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                                    }
                                    else
                                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                                }
                                pieceDirection[j] = true; 
                            }
                            else if (!turn && pieceColor) // black rook eat white piece
                            {
                                if (value == kingvalue)
                                    return true;
                                if (bestPath >= currentBoardState - value) // prune path that are larger than the current board state
                                {
                                    setNewBoard(board, newBoard, y, x, Y, X);
                                    if (IsValidMove(newBoard, turn))
                                    {
                                        pieceDirection[j] = true;
                                        continue;
                                    }
                                    bestPath = currentBoardState - value;
                                    AI newAI = new AI(movesCount + 1);
                                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                                    if (movesCount == 0)
                                    {
                                        CapturedSquares capture = new CapturedSquares();
                                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                                    }
                                    else
                                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                                }
                                pieceDirection[j] = true;
                            }
                            else
                                pieceDirection[j] = true; // if rook lands on the its same color piece
                        }
                    }
                    else
                        pieceDirection[j] = true; // if rook lands out of bounds
                }
            }
            return false;
        }

        private bool KnightAI(PictureBox[][] board, int y, int x, bool turn, int movesCount, int movesLimit, int currentBoardState, AIResult finalResult)
        {
            foreach (int[] dir in PieceDetails.KnightDirection) 
            {
                int Y = y + dir[0];
                int X = x + dir[1];
                if (Y < 0 || Y > 7 || X < 0 || X > 7) continue;
                PictureBox[][] newBoard = new PictureBox[8][];
                if (board[Y][X] == null) // knight moving to empty square
                {
                    setNewBoard(board, newBoard, y, x, Y, X); // create new board object to be used as parameter to call minimax function
                    if (IsValidMove(newBoard, turn))
                        continue;
                    AI newAI = new AI(movesCount + 1);
                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                    if (movesCount == 0)
                    {
                        CapturedSquares capture = new CapturedSquares();
                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                    }
                    else
                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                    continue;
                }
                int value = PieceValue(PieceDetails.selectedPiece(board[Y][X].Name));
                bool pieceColor = PieceDetails.IsPieceBlackorWhite(board[Y][X].Name);
                if (turn && !pieceColor) // white knight eatting black piece
                {
                    setNewBoard(board, newBoard, y, x, Y, X);
                    if (value == kingvalue)  // if king is eatten prune this path completely
                        return true;
                    if (bestPath <= currentBoardState + value) // prune path that are smaller than the current board state
                    {
                        if (IsValidMove(newBoard, turn))
                            continue;
                        bestPath = currentBoardState + value;
                        AI newAI = new AI(movesCount + 1);
                        int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                        if (movesCount == 0)
                        {
                            CapturedSquares capture = new CapturedSquares();
                            setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                        }
                        else
                            setResult(turn, bestBoardState, y, x, Y, X, 0);
                    }
                }
                else if (!turn && pieceColor) // black knight eatting white piece
                {
                    setNewBoard(board, newBoard, y, x, Y, X);
                    if (value == kingvalue)
                        return true;
                    if (bestPath >= currentBoardState - value) // prune path that are larger than the current board state
                    {
                        if (IsValidMove(newBoard, turn))
                            continue;
                        bestPath = currentBoardState - value;
                        AI newAI = new AI(movesCount + 1);
                        int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                        if (movesCount == 0)
                        {
                            CapturedSquares capture = new CapturedSquares();
                            setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                        }
                        else
                            setResult(turn, bestBoardState, y, x, Y, X, 0);
                    }
                }
            }
            return false;
        }

        private bool BishopAI(PictureBox[][] board, int y, int x, bool turn, int movesCount, int movesLimit, int currentBoardState, AIResult finalResult)
        {
            bool[] pieceDirection = new bool[4]; // this array represents northeast, southeast, southwest, northwest and will reduce the processing time

            for (int i = 1; i < 8; i++) // represents the distance to be moved
            {
                // bishop cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.BishopDirection.Length; j++) // represents the array to define the direction of the move
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.BishopDirection[j][0];
                    int X = x + i * PieceDetails.BishopDirection[j][1];
                    if (Y >= 0 && Y < 8 && X >= 0 && X < 8)
                    {
                        PictureBox[][] newBoard = new PictureBox[8][];
                        if (board[Y][X] == null) // bishop moving to empty square
                        {
                            setNewBoard(board, newBoard, y, x, Y, X); // create new board object to be used as parameter to call minimax function
                            if (IsValidMove(newBoard, turn))
                                continue;
                            AI newAI = new AI(movesCount + 1);
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, Y, X, 0);
                        }
                        else 
                        {
                            bool pieceColor = PieceDetails.IsPieceBlackorWhite(board[Y][X].Name);
                            int value = PieceValue(PieceDetails.selectedPiece(board[Y][X].Name));
                            if (turn && !pieceColor) // white bishop eatting black piece
                            {
                                setNewBoard(board, newBoard, y, x, Y, X);
                                if (value == kingvalue) // if king is eatten prune this path completely
                                    return true;
                                if (bestPath <= currentBoardState + value) // prune path that are smaller than the current board state
                                {
                                    if (IsValidMove(newBoard, turn))
                                    {
                                        pieceDirection[j] = true;
                                        continue;
                                    }
                                    bestPath = currentBoardState + value;
                                    AI newAI = new AI(movesCount + 1);
                                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                                    if (movesCount == 0)
                                    {
                                        CapturedSquares capture = new CapturedSquares();
                                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                                    }
                                    else
                                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                                }
                                pieceDirection[j] = true;
                            }
                            else if (!turn && pieceColor) // black bishop eatting white piece
                            {
                                setNewBoard(board, newBoard, y, x, Y, X);
                                if (value == kingvalue)
                                    return true;
                                if (bestPath >= currentBoardState - value) // prune path that are larger than the current board state
                                {
                                    if (IsValidMove(newBoard, turn))
                                    {
                                        pieceDirection[j] = true;
                                        continue;
                                    }
                                    bestPath = currentBoardState - value;
                                    AI newAI = new AI(movesCount + 1);
                                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                                    if (movesCount == 0)
                                    {
                                        CapturedSquares capture = new CapturedSquares();
                                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                                    }
                                    else
                                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                                }
                                pieceDirection[j] = true;
                            }
                            else
                                pieceDirection[j] = true; // if bishop lands on the its same color piece
                        }
                    }
                    else
                        pieceDirection[j] = true; // if bishop lands out of bounds
                }
            }
            return false;
        }

        private bool QueenAI(PictureBox[][] board, int y, int x, bool turn, int movesCount, int movesLimit, int currentBoardState, AIResult finalResult)
        {
            bool[] pieceDirection = new bool[8]; // this array represents north, east, south, west, northeast, southeast, southwest, northwest and will reduce the processing time

            for (int i = 1; i < 8; i++) // represents the distance to be moved
            {
                // queen cannot move anymore as it is either out of bound or there is another piece blocking it from its target
                if (PieceDetails.checkedAllDirections(pieceDirection))
                    break;
                for (int j = 0; j < PieceDetails.QueenDirection.Length; j++) // represents the array to define the direction of the move
                {
                    if (pieceDirection[j]) continue;
                    int Y = y + i * PieceDetails.QueenDirection[j][0];
                    int X = x + i * PieceDetails.QueenDirection[j][1];
                    if (Y >= 0 && Y < 8 && X >= 0 && X < 8)
                    {
                        PictureBox[][] newBoard = new PictureBox[8][];
                        if (board[Y][X] == null) // if queen moves to an empty square
                        {
                            setNewBoard(board, newBoard, y, x, Y, X); // create new board object to be used as parameter to call minimax function
                            if (IsValidMove(newBoard, turn))
                                continue;
                            AI newAI = new AI(movesCount + 1);
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, Y, X, 0);
                        }
                        else
                        {
                            bool pieceColor = PieceDetails.IsPieceBlackorWhite(board[Y][X].Name);
                            int value = PieceValue(PieceDetails.selectedPiece(board[Y][X].Name));
                            if (turn && !pieceColor) // if white queen eats a black piece
                            {
                                if (value == kingvalue) // if king is eatten prune this path completely
                                    return true;
                                if (bestPath <= currentBoardState + value) // prune path that are smaller than the current board state
                                {
                                    setNewBoard(board, newBoard, y, x, Y, X);
                                    if (IsValidMove(newBoard, turn))
                                    {
                                        pieceDirection[j] = true;
                                        continue;
                                    }
                                    bestPath = currentBoardState + value;
                                    AI newAI = new AI(movesCount + 1);
                                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                                    if (movesCount == 0)
                                    {
                                        CapturedSquares capture = new CapturedSquares();
                                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                                    }
                                    else
                                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                                }
                                pieceDirection[j] = true;
                            }
                            else if (!turn && pieceColor) // if black queen eats a white piece
                            {
                                if (value == kingvalue)
                                    return true;
                                if (bestPath >= currentBoardState - value) // prune path that are larger than the current board state
                                {
                                    setNewBoard(board, newBoard, y, x, Y, X);
                                    if (IsValidMove(newBoard, turn))
                                    {
                                        pieceDirection[j] = true;
                                        continue;
                                    }
                                    bestPath = currentBoardState - value;
                                    AI newAI = new AI(movesCount + 1);
                                    int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                                    if (movesCount == 0)
                                    {
                                        CapturedSquares capture = new CapturedSquares();
                                        setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                                    }
                                    else
                                        setResult(turn, bestBoardState, y, x, Y, X, 0);
                                }
                                pieceDirection[j] = true;
                            }
                            else
                                pieceDirection[j] = true; // if bishop lands on the its same color piece
                        }
                    }
                    else
                        pieceDirection[j] = true; // if bishop lands out of bounds
                }
            }
            return false;
        }

        private bool KingAI(PictureBox[][] board, int y, int x, bool turn, int movesCount, int movesLimit, int currentBoardState, AIResult finalResult)
        {
            // loop through all directions king can move by one square 
            for (int i = -1; i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++) 
                {
                    int Y = y + i;
                    int X = x + j;
                    if ((i == 0 && j == 0) || Y < 0 || Y > 7 || X < 0 || X > 7) continue;
                    PictureBox[][] newBoard = new PictureBox[8][];
                    if (board[Y][X] == null) // if king moves to empty square
                    {
                        setNewBoard(board, newBoard, y, x, Y, X);
                        if (IsValidMove(newBoard, turn))
                            continue;
                        AI newAI = new AI(movesCount + 1);
                        int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState, finalResult);
                        if (movesCount == 0)
                        {
                            CapturedSquares capture = new CapturedSquares();
                            setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                        }
                        else
                            setResult(turn, bestBoardState, y, x, Y, X, 0);
                        continue;
                    }
                    int value = PieceValue(PieceDetails.selectedPiece(board[Y][X].Name));
                    bool pieceColor = PieceDetails.IsPieceBlackorWhite(board[Y][X].Name);
                    if (turn && !pieceColor) // white king eats black piece
                    {
                        if (value == kingvalue) // if king is eatten prune this path completely
                            return true;
                        if (bestPath <= currentBoardState + value) // prune path that are smaller than the current board state
                        {
                            setNewBoard(board, newBoard, y, x, Y, X);
                            if (IsValidMove(newBoard, turn))
                                continue;
                            bestPath = currentBoardState + value;
                            AI newAI = new AI(movesCount + 1);
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState + value, finalResult);
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, Y, X, 0);
                        }
                    }
                    else if (!turn && pieceColor) // black king eats white piece
                    {
                        if (value == kingvalue)
                            return true;
                        if (bestPath >= currentBoardState - value) // prune path that are larger than the current board state
                        {
                            setNewBoard(board, newBoard, y, x, Y, X);
                            if (IsValidMove(newBoard, turn))
                                continue;
                            bestPath = currentBoardState - value;
                            AI newAI = new AI(movesCount + 1);
                            int bestBoardState = newAI.MiniMax(newBoard, !turn, movesLimit, currentBoardState - value, finalResult);
                            if (movesCount == 0)
                            {
                                CapturedSquares capture = new CapturedSquares();
                                setResult(turn, bestBoardState, y, x, Y, X, capture.CaptureCount(newBoard));
                            }
                            else
                                setResult(turn, bestBoardState, y, x, Y, X, 0);
                        }
                    }
                }
            }
            return false;
        }

        // this will be subtracted or added to the current board state depending on who's turn it is (if it is black's turn it will subtract and if it is white's turn it will add)
        public int PieceValue(ChessBoard.pieceName piece)
        {
            switch (piece)
            {
                case ChessBoard.pieceName.Pawn:
                    return 10;
                case ChessBoard.pieceName.Rook:
                    return 50;
                case ChessBoard.pieceName.Knight:
                    return 30;
                case ChessBoard.pieceName.Bishop:
                    return 30;
                case ChessBoard.pieceName.Queen:
                    return 90;
                default:
                    return kingvalue;
            }
        }

        // determine if move been made by the AI is illegal or not
        private bool IsValidMove(PictureBox[][] board, bool turn) 
        {
            if (movesCount == 0)
            {
                Check check = new Check();
                int[] kingCoord = PieceDetails.FindKing(board, turn);
                if (check.IsChecked(board, kingCoord[0], kingCoord[1], !turn)) // determine if this is an illegal move by check
                    return true;
            }
            return false;
        }
        
        // create a new board array to perform recursion using a different board object
        private void setNewBoard(PictureBox[][] board, PictureBox[][] newBoard, int sourceY, int sourceX, int destinationY, int destinationX) 
        {
            for (int i = 0; i < newBoard.Length; i++)
            {
                newBoard[i] = new PictureBox[8];
                board[i].CopyTo(newBoard[i], 0);
            }
            newBoard[destinationY][destinationX] = newBoard[sourceY][sourceX];
            newBoard[sourceY][sourceX] = null;
        }

        // set result for the current best boardstate
        private void setResult(bool turn, int bestBoardState, int sourceY, int sourceX, int destinationY, int destinationX, int squares) 
        {
            if ((turn && bestBoardState >= BoardState) || (!turn && bestBoardState <= BoardState))
            {
                // movesCount comparison will not be made if movesCount is not 0 as there is no point checking equal boardstates if it is not an actual move being made by the AI
                if (movesCount != 0 && bestBoardState == BoardState)
                    return;
                // This squareCount comparison will only be applied for movesCount of 0
                if (bestBoardState == BoardState && ((turn && squares <= squareCount) || (!turn && squares >= squareCount))) 
                    return;
                BoardState = bestBoardState;
                squareCount = squares;
                if (movesCount == 0) // only stores source and destination of move if movesCount is 0 as AI is actually make that move in that specific move
                {
                    SourceY = sourceY;
                    SourceX = sourceX;
                    DestinationY = destinationY;
                    DestinationX = destinationX;
                }
            }
        }
    }
}
