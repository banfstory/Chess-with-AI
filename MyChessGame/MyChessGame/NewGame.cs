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
    public partial class NewGame : Form
    {
        ChessBoard Main;
        public NewGame(ChessBoard main)
        {
            InitializeComponent();
            Main = main;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PvP_Click(object sender, EventArgs e)
        {
            Main.ResetGame();
            Main.type.Text = "Player vs Player";
            Main.PvP = true;
            this.Close();
        }

        private void PvE_Click(object sender, EventArgs e)
        {
            Main.ResetGame();
            AISpecifications aiSpecs = new AISpecifications(Main, this);
            DialogResult show = aiSpecs.ShowDialog();
        }
    }
}
