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
    public partial class AISpecifications : Form
    {
        ChessBoard Main;
        NewGame GameMode;

        public AISpecifications(ChessBoard main, NewGame gamemode)
        {
            InitializeComponent();
            normal.Checked = true;
            white.Checked = true;
            Main = main;
            GameMode = gamemode;
        }

        private void confirm_Click(object sender, EventArgs e)
        {
            Main.type.Text = "Player vs AI";
            Main.PvE = true;
            foreach (RadioButton radio in level.Controls) 
            {
                if (radio.Checked)
                {
                    if (radio.Name == "easy")
                        Main.AIComplexity = 3;
                    else if (radio.Name == "normal")
                        Main.AIComplexity = 4;
                    else
                        Main.AIComplexity = 5;
                    break;
                }
            }
            foreach (RadioButton radio in color.Controls) 
            {
                if (radio.Checked) 
                {
                    if (radio.Name == "white")
                    {
                        Main.AIColor = false;
                        Main.type.Text = "(White) " + Main.type.Text;
                    }
                    else
                    {
                        Main.AIColor = true;
                        Main.type.Text = "(Black) " + Main.type.Text;
                    }
                    break;
                }
            }
            this.Close();
            GameMode.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
