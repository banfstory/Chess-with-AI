using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyChessGame
{
    public class History // a doubly linked list that allows player to undo or redo move based these variables
    {
        public bool Turn; 
        public int SourceY;
        public int SourceX;
        public int DestinationY;
        public int DestinationX;
        public PictureBox Source;
        public PictureBox Destination;
        public ChessBoard.gameState State;
        public History Prev;
        public History Next;

        public History() { }

        public History(bool turn, int sourceY, int sourceX, int destinationY, int destinationX, PictureBox source, PictureBox destination, ChessBoard.gameState state)
        {
            Turn = turn;
            SourceY = sourceY;
            SourceX = sourceX;
            DestinationY = destinationY;
            DestinationX = destinationX;
            Source = source;
            Destination = destination;
            State = state;
        }
    }
}
