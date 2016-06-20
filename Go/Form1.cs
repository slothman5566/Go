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
            groupBox1.Controls.Add(newBoard);
            
        }

        private void 這樣就可以假裝你有朋友ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoard.AI = true;
            newBoard.ReSet();
        }

        private void 幻想有朋友跟你玩ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newBoard.AI = false;
            newBoard.ReSet();
        }

        private void 重新開始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }
    }
}
