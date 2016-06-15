using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Go
{
    public partial class Form1 : Form
    {
        private Board newBoard;
        public Form1()
        {
            InitializeComponent();
            AddBoard
                ();
        }

        private void AddBoard()

        {


            newBoard = new Board(15);
            newBoard.Location = new Point(0, 0);
            newBoard.Size = newBoard.BoardRealSize;
            newBoard.ChessMovingDel = MoveChess;
            groupBox1.Controls.Add(newBoard);
            
        }

        private void MoveChess(int row, int col)
        {
            newBoard.MoveChess(row, col);
        }


    }
}
