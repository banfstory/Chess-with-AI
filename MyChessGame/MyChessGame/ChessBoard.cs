using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyChessGame
{
    public partial class ChessBoard : Form
    {
        private History History = new History();
        private gameState currentState = gameState.Normal;
        private bool turn = true; // determine whether it is white's or black's turn : true for white and false for black
        public enum pieceName { Rook, Knight, Bishop, Queen, King, Pawn, None}; 
        public enum gameState { Normal, Stalemate, Checkmate, Check };
        private PictureBox[][] board = new PictureBox[8][]; // overview of all pieces on the board currently
        private PictureBox selectedpiece; // piece that is currently selected to move
        private bool alreadySelected = false; // determine if a piece is already selected
        private List<PictureBox> PossiblePieceToTake = new List<PictureBox>(); // highlight all possible pieces that can be taken by a selected piece
        public bool PvE = false; // player vs AI
        public bool PvP = false; //player vs player
        public bool AIColor = false; // determine the AI color
        public int AIComplexity = 2;

        public ChessBoard()
        {
            InitializeComponent();
            ResetGame();
        }

        private void AIMove() // determine how the AI will move and swap back to players turn after AI makes its move
        {
            if (PvE && (currentState == gameState.Normal || currentState == gameState.Check) && ((turn && AIColor) || (!turn && !AIColor))) 
            {
                int currentBoardState = 0;
                AIResult aiResult = new AIResult();
                AI ai = new AI(0);
                for (int y = 0; y < board.Length; y++)
                {
                    for (int x = 0; x < board[y].Length; x++)
                    {
                        if (board[y][x] == null) continue;
                        if (PieceDetails.IsPieceBlackorWhite(board[y][x].Name))
                            currentBoardState += ai.PieceValue(PieceDetails.selectedPiece(board[y][x].Name));
                        else
                            currentBoardState -= ai.PieceValue(PieceDetails.selectedPiece(board[y][x].Name));
                    }
                }
                ai.MiniMax(board, turn, AIComplexity, currentBoardState, aiResult);
                selectedpiece = board[aiResult.SourceY][aiResult.SourceX];
                if (!IsValidMove(PieceDetails.selectedPiece(board[aiResult.SourceY][aiResult.SourceX].Name), aiResult.DestinationY, aiResult.DestinationX))
                {
                    MessageBox.Show("AI ERROR");
                    undo();
                    return;
                }
                completeTurn();
            }
        }

        private void clickWhite(object sender, EventArgs e)
        {
            if (currentState != gameState.Checkmate && currentState != gameState.Stalemate && (PvP || PvE))
            {
                if (alreadySelected && !turn) // when black piece eats white black piece
                {
                    if (MovePiece((PictureBox)sender))
                    {
                        panel.Refresh();
                        selectedpiece.BackColor = Color.Transparent;
                        foreach (PictureBox p in PossiblePieceToTake)
                            p.BackColor = Color.Transparent;
                        PossiblePieceToTake = new List<PictureBox>();
                        completeTurn();
                        AIMove();
                    }
                    else
                        MessageBox.Show("Invalid Move, please try again.");
                }
                else if (turn && selectedpiece != (PictureBox)sender) // selecting white piece
                {
                    resetHighlightedPieces();
                    selectedpiece = (PictureBox)sender;
                    selectedpiece.BackColor = Color.FromArgb(220, 13, 86, 212);
                    alreadySelected = true;
                    pieceName sourcePieceType = PieceDetails.selectedWhitePiece(selectedpiece.Name);
                    int Y = PieceDetails.FindCoordinate(selectedpiece.Location.Y);
                    int X = PieceDetails.FindCoordinate(selectedpiece.Location.X);
                    DisplayPossibleMoves dpm = new DisplayPossibleMoves(panel.CreateGraphics(), Color.FromArgb(125, 48, 118, 240));
                    dpm.DisplayMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                }
            }
        }

        private void clickBlack(object sender, EventArgs e)
        {
            if (currentState != gameState.Checkmate && currentState != gameState.Stalemate && (PvP || PvE))
            {
                if (alreadySelected && turn) // when white piece eats black piece
                {
                    if (MovePiece((PictureBox)sender))
                    {
                        panel.Refresh();
                        selectedpiece.BackColor = Color.Transparent;
                        foreach (PictureBox p in PossiblePieceToTake)
                            p.BackColor = Color.Transparent;
                        PossiblePieceToTake = new List<PictureBox>();
                        completeTurn();
                        AIMove();
                    }
                    else
                        MessageBox.Show("Invalid Move, please try again.");
                }
                else if (!turn && selectedpiece != (PictureBox)sender) // selecting black piece
                {
                    resetHighlightedPieces();
                    selectedpiece = (PictureBox)sender;
                    selectedpiece.BackColor = Color.FromArgb(220, 13, 86, 212);
                    alreadySelected = true;
                    pieceName sourcePieceType = PieceDetails.selectedBlackPiece(selectedpiece.Name);
                    int Y = PieceDetails.FindCoordinate(selectedpiece.Location.Y);
                    int X = PieceDetails.FindCoordinate(selectedpiece.Location.X);
                    DisplayPossibleMoves dpm = new DisplayPossibleMoves(panel.CreateGraphics(), Color.FromArgb(125, 48, 118, 240));
                    dpm.DisplayMoves(Y, X, board, PossiblePieceToTake, turn, sourcePieceType);
                }
            }
        }

        private void board_Click(object sender, EventArgs e) // when selected piece move to empty square
        {
            if (currentState != gameState.Checkmate && currentState != gameState.Stalemate && (PvP || PvE))
            {
                if (alreadySelected)
                {
                    Point position = Cursor.Position;
                    position = panel.PointToClient(position);
                    int destinationY = PieceDetails.FindCoordinate(position.Y);
                    int destinationX = PieceDetails.FindCoordinate(position.X);
                    if (MovePiece(destinationY, destinationX))
                    {
                        resetHighlightedPieces(); 
                        completeTurn();
                        AIMove();
                    }
                    else
                        MessageBox.Show("Invalid Move, please try again.");
                }
            }
        }

        private bool MovePiece(PictureBox destination) // moving a piece to eat another piece
        {
            pieceName sourcePieceType = turn ? PieceDetails.selectedWhitePiece(selectedpiece.Name) : PieceDetails.selectedBlackPiece(selectedpiece.Name);
            int destinationY = PieceDetails.FindCoordinate(destination.Location.Y);
            int destinationX = PieceDetails.FindCoordinate(destination.Location.X);
            return IsValidMove(sourcePieceType, destinationY, destinationX);
        }


        private bool MovePiece(int destinationY, int destinationX) // moving a piece to an empty square
        {
            pieceName sourcePieceType = turn ? PieceDetails.selectedWhitePiece(selectedpiece.Name) : PieceDetails.selectedBlackPiece(selectedpiece.Name);
            return IsValidMove(sourcePieceType, destinationY, destinationX);
        }

        private bool IsValidMove(pieceName sourcePieceType, int destinationY, int destinationX) // determine if the piece is able to move to its target square
        {
            int sourceY = PieceDetails.FindCoordinate(selectedpiece.Location.Y);
            int sourceX = PieceDetails.FindCoordinate(selectedpiece.Location.X);
            switch (sourcePieceType)
            {
                case pieceName.Pawn:
                    Pawn pawn = new Pawn(board, sourceY, sourceX, destinationY, destinationX, History, turn);
                    return pawn.Move(board);
                case pieceName.Rook:
                    Rook rook = new Rook(board, sourceY, sourceX, destinationY, destinationX, History, turn);
                    return rook.Move(board);
                case pieceName.Knight:
                    Knight knight = new Knight(board, sourceY, sourceX, destinationY, destinationX, History, turn);
                    return knight.Move(board);
                case pieceName.Bishop:
                    Bishop bishop = new Bishop(board, sourceY, sourceX, destinationY, destinationX, History, turn);
                    return bishop.Move(board);
                case pieceName.Queen:
                    Queen queen = new Queen(board, sourceY, sourceX, destinationY, destinationX, History, turn);
                    return queen.Move(board);
                default:
                    King king = new King(board, sourceY, sourceX, destinationY, destinationX, History, turn);
                    return king.Move(board);
            }
        }

        private void completeTurn()
        {
            selectedpiece = null;
            alreadySelected = false;
            History = History.Next;
            currentState = History.State;
            displayGameState();
            turn = !turn;
        }

        private void displayGameState() // display state of game over completing a move such as Check,Checkmate,Stalemate
        {
            if (currentState == gameState.Check)
            {
                if (turn)
                    Status.Text = "Black king checked";
                else
                    Status.Text = "White king checked";
            }
            else if (currentState == gameState.Checkmate)
            {
                if (turn)
                    Status.Text = "Checkmate! White wins!";
                else
                    Status.Text = "Checkmate! Black wins!";
            }
            else if (currentState == gameState.Stalemate)
            {
                if (turn)
                    Status.Text = "Stalemate by white";
                else
                    Status.Text = "Stalemate by black!";
            }
            else
                Status.Text = "";
        }

        private void resetHighlightedPieces() // reset all highlighted squares for selected piece and all other squares with a piece highlighted to transparent
        {
            panel.Refresh();
            if (selectedpiece != null)
                selectedpiece.BackColor = Color.Transparent;
            foreach (PictureBox p in PossiblePieceToTake)
                p.BackColor = Color.Transparent;
            PossiblePieceToTake = new List<PictureBox>();
        }

        public void ResetGame() // reset chess game
        {
            resetHighlightedPieces();
            History = new History();
            currentState = gameState.Normal;
            turn = true;
            selectedpiece = null;
            alreadySelected = false;
            PossiblePieceToTake = new List<PictureBox>();
            PvE = false;
            PvP = false;
            AIColor = false;
            AIComplexity = 2;
            board = new PictureBox[8][];
            for (int i = 0; i < board.Length; i++)
                board[i] = new PictureBox[8];
            board[0][0] = br1; board[0][1] = bkn1; board[0][2] = bb1; board[0][3] = bq; board[0][4] = bk; board[0][5] = bb2; board[0][6] = bkn2; board[0][7] = br2;
            board[1][0] = bp1; board[1][1] = bp2; board[1][2] = bp3; board[1][3] = bp4; board[1][4] = bp5; board[1][5] = bp6; board[1][6] = bp7; board[1][7] = bp8;
            board[7][0] = wr1; board[7][1] = wkn1; board[7][2] = wb1; board[7][3] = wq; board[7][4] = wk; board[7][5] = wb2; board[7][6] = wkn2; board[7][7] = wr2;
            board[6][0] = wp1; board[6][1] = wp2; board[6][2] = wp3; board[6][3] = wp4; board[6][4] = wp5; board[6][5] = wp6; board[6][6] = wp7; board[6][7] = wp8;
            for (int y = 0; y < board.Length; y++)
            {
                if ((y >= 0 && y <= 1) || y >= 6)
                {
                    for (int x = 0; x < board.Length; x++)
                    {
                        board[y][x].Visible = true;
                        PieceDetails.movePiece(y, x, board[y][x]);
                    }
                }
            }
        }

        private void TestStaleMate() // Move white queen to E3 to perform stalemate (Used for testing purposes)
        {
            PvP = true;
            br1.Visible = false; bkn1.Visible = false; bb1.Visible = false; bq.Visible = false; bk.Visible = false; bb2.Visible = false; bkn2.Visible = false; br2.Visible = false;
            bp1.Visible = false; bp2.Visible = false; bp3.Visible = false; bp4.Visible = false; bp5.Visible = false; bp6.Visible = false; bp7.Visible = false; bp8.Visible = false;
            wr1.Visible = false; wkn1.Visible = false; wb1.Visible = false; wq.Visible = false; wk.Visible = false; wb2.Visible = false; wkn2.Visible = false; wr2.Visible = false;
            wp1.Visible = false; wp2.Visible = false; wp3.Visible = false; wp4.Visible = false; wp5.Visible = false; wp6.Visible = false; wp7.Visible = false; wp8.Visible = false;

            board = new PictureBox[8][];
            for (int i = 0; i < board.Length; i++)
                board[i] = new PictureBox[8];
            board[0][2] = wq; board[0][5] = bb1; board[0][6] = bkn1; board[0][7] = br1; board[1][4] = bp1; board[1][6] = bp2; board[1][7] = bq; board[2][5] = bp3;
            board[2][6] = bk; board[2][7] = br2; board[3][7] = bp4; board[7][0] = wk; board[6][0] = wp1; board[6][1] = wp2; board[7][2] = wr1; board[4][7] = wp3;
            for (int y = 0; y < board.Length; y++)
            {
                for (int x = 0; x < board[y].Length; x++)
                {
                    if (board[y][x] != null)
                    {
                        board[y][x].Visible = true;
                        PieceDetails.movePiece(y, x, board[y][x]);
                    }
                }
            }
        }

        private void TestAICheckmate() // Move white queen to E3 to perform stalemate (Used for testing purposes)
        {
            ResetGame();
            PvE = true;
            for (int i = 0; i < board.Length; i++) 
            {
                for (int j = 0; j < board[i].Length; j++) 
                {
                    if (board[i][j] == null)
                        continue;
                    board[i][j].Visible = false;
                }
            }

            board = new PictureBox[8][];
            for (int i = 0; i < board.Length; i++)
                board[i] = new PictureBox[8];
            board[0][0] = br1; board[0][2] = bb1; board[0][4] = bq; board[0][6] = br2; board[1][0] = bp1; board[1][1] = bp2; board[1][2] = bp3; board[1][3] = bp4; board[1][7] = bp5;
            board[2][4] = bp6; board[2][6] = bk;
            board[7][5] = wq; board[3][4] = wp1; board[3][6] = wb1; board[5][2] = wp2; board[5][7] = wp3; board[6][0] = wp4; board[6][6] = wp5; board[7][0] = wr1; board[7][4] = wk;
            board[7][7] = wr2;
            for (int y = 0; y < board.Length; y++)
            {
                for (int x = 0; x < board[y].Length; x++)
                {
                    if (board[y][x] != null)
                    {
                        board[y][x].Visible = true;
                        PieceDetails.movePiece(y, x, board[y][x]);
                    }
                }
            }
        }

        private void TestAICheckmate2() // Move white queen to E3 to perform stalemate (Used for testing purposes)
        {
            ResetGame();
            PvE = true;
            AIComplexity = 4;
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == null)
                        continue;
                    board[i][j].Visible = false;
                }
            }

            board = new PictureBox[8][];
            for (int i = 0; i < board.Length; i++)
                board[i] = new PictureBox[8];
            board[1][6] = bk;
            board[2][4] = wq; board[1][5] = wp1; board[4][7] = wb1; board[6][6] = wk; 
            for (int y = 0; y < board.Length; y++)
            {
                for (int x = 0; x < board[y].Length; x++)
                {
                    if (board[y][x] != null)
                    {
                        board[y][x].Visible = true;
                        PieceDetails.movePiece(y, x, board[y][x]);
                    }
                }
            }
        }

        private void undo() 
        {
            if (History.Prev != null) // the current game state should be represented two turns before
                currentState = History.Prev.State;
            else
                currentState = gameState.Normal;
            turn = History.Turn;
            board[History.SourceY][History.SourceX] = History.Source;
            PieceDetails.movePiece(History.SourceY, History.SourceX, History.Source);
            board[History.DestinationY][History.DestinationX] = History.Destination;
            if (History.Destination != null) // if destination was null than the picturebox object does not need to be modified
            {
                PieceDetails.movePiece(History.DestinationY, History.DestinationX, History.Destination);
                History.Destination.Visible = true;
            }
            History = History.Prev; // go to previous History object
        }

        private void redo()
        {
            History = History.Next;
            currentState = History.State;
            turn = !History.Turn;
            board[History.DestinationY][History.DestinationX] = History.Source;
            PieceDetails.movePiece(History.DestinationY, History.DestinationX, History.Source);
            board[History.SourceY][History.SourceX] = null;
            if (History.Destination != null)
                History.Destination.Visible = false;
        }

        private void UndoRedoReset() 
        {
            resetHighlightedPieces();
            selectedpiece = null;
            alreadySelected = false;
            displayGameState();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) // undo feature
        {
            if (History.Prev != null) // The previous move is based on the current History object
            {
                panel.Refresh();
                if (currentState != gameState.Checkmate && currentState != gameState.Stalemate)
                {
                    undo();
                    if (PvE)
                    {
                        if (History.Prev != null)
                            undo();
                        if (turn && AIColor)
                            AIMove();
                    }
                }
                else 
                    undo();
                UndoRedoReset();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) // redo feature
        {
            if (History.Next != null) // The next History object is the next of the current History object
            {
                panel.Refresh();
                redo();
                if (PvE)
                {
                    if (History.Next != null)
                        redo();
                    else
                        turn = !turn;
                }
                UndoRedoReset();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e) // start a new game with player vs player or player vs ai 
        {
            NewGame newgame = new NewGame(this);
            DialogResult show = newgame.ShowDialog();
            if (PvE && AIColor)
                AIMove();
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e) // game rules
        {
            Rules rules = new Rules();
            DialogResult show = rules.ShowDialog();
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e) // details of game
        {
            About about = new About();
            DialogResult show = about.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) // exit application
        {
            this.Close();
        }

        private void ChessBoard_KeyDown(object sender, KeyEventArgs e)
        {
            // shortcut keys
            if (e.Control && e.KeyCode.ToString() == "Z") // undo
            {
                undoToolStripMenuItem_Click(sender, e);
            }
            else if (e.Control && e.KeyCode.ToString() == "R") // redo
            {
                redoToolStripMenuItem_Click(sender, e);
            }
            else if (e.Control && e.KeyCode.ToString() == "N") // new game
            {
                newGameToolStripMenuItem_Click(sender, e); 
            }
            else if (e.KeyCode == Keys.Escape) // exit game
            {
                this.Close();
            }
        }
    }
}