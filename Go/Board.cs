using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Go
{
   
    public partial class Board : UserControl
    {

        protected int boardSize;
        protected const int chessSize = 40;
        private const int boardLeftTopX = 30;
        private const int boardLeftTopY = 30;

        private PictureBox[,] chessBoxes;

        private Size realSize;
        public Size BoardRealSize
        {
            get
            {
                return realSize;
            }
        }


        private Point lastMovePoint = new Point(-1, -1);

        private bool isTurnToBlack;

        public bool IsStartFromBlack
        {
            set
            {
                isTurnToBlack = value;
            }
        }


        public delegate void ChessMovesEvent(int row, int col);
        public ChessMovesEvent ChessMovingDel { set; get; }


        public Board(int boardSize)
        {
            InitializeComponent();

            this.boardSize = boardSize;
            chessBoxes = new PictureBox[boardSize, boardSize];

            InitBoard();
            MouseMove += Board_MouseMove;
            MouseLeave += Board_MouseLeave;
            MouseClick += Board_MouseClick;


            int showSize = (boardSize - 1) * chessSize;
            int realSize = boardLeftTopX * 2 + showSize;
            this.realSize = new Size(realSize, realSize);
            
            isTurnToBlack = true;
        }

        private void InitBoard()
        {
            PaintBoard();
            InitChesses();
        }

 
        private void PaintBoard()
        {
            Paint += Board_Paint;
        }

 
        private void InitChesses()
        {
 
            int startX = boardLeftTopX - (chessSize / 2);
            int startY = boardLeftTopY - (chessSize / 2);

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    PictureBox chess = new PictureBox();
                    chess.Location = new Point(x * chessSize + startX, y * chessSize + startY);
                    chess.Size = new Size(chessSize, chessSize);
                    chess.BackColor = Color.Transparent;
                    chess.SizeMode = PictureBoxSizeMode.CenterImage;
                    chess.Visible = false;

                    Controls.Add(chess);
                    chessBoxes[x, y] = chess;
                }
            }
        }


        private void Board_Paint(object sender, PaintEventArgs e)
        {
            Graphics context = e.Graphics;
            Pen linePen = new Pen(Color.Black, 1);
            SolidBrush blackBrush = new SolidBrush(Color.Black);

            int hX = boardLeftTopX;
            int hY = boardLeftTopY;
            int lineLen = (boardSize - 1) * chessSize;

 
            int redPointIndex = 3;

            for (int i = 0; i < boardSize; i++)
            {
 
                int vX = hY;
                int vY = hX;

                context.DrawLine(linePen, hX, hY, hX + lineLen, hY);
                context.DrawLine(linePen, vX, vY, vX, vY + lineLen);

                if (i == redPointIndex)
                {
                    int width = 8;
                    
                    context.FillEllipse(blackBrush,  vY+ lineLen -  (3 * chessSize) - (width / 2), hY - (width / 2), width, width);
                    context.FillEllipse(blackBrush,  vY+lineLen -  (7 * chessSize) - (width / 2), hY - (width / 2), width, width);
                    context.FillEllipse(blackBrush,  vY+lineLen - ( 11* chessSize) - (width / 2), hY - (width / 2), width, width);

                    redPointIndex += 4;
                }

                hY += chessSize;
            }
        }

    
        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            int marginH = boardLeftTopX - (chessSize / 2);   
            int marginV = boardLeftTopY - (chessSize / 2);   
            int xIndex = (e.X - marginH) / chessSize;       
            int yIndex = (e.Y - marginV) / chessSize;       


            if (xIndex < 0 || yIndex < 0 || (xIndex + 1) > boardSize || (yIndex + 1) > boardSize)
            {
                RemoveLastPreview();
                return;
            }

            PictureBox chess = chessBoxes[xIndex, yIndex];

            if (lastMovePoint.X != xIndex || lastMovePoint.Y != yIndex)
            {
                RemoveLastPreview();
            }

            if (chess.Visible == false)
            {
                DisplayPreviewBox(chess);
            }
            else
            {
                return;
            }
            lastMovePoint.X = xIndex;
            lastMovePoint.Y = yIndex;
        }


        private void Board_MouseLeave(object sender, EventArgs e)
        {
            RemoveLastPreview();
        }


        private void RemoveLastPreview()
        {
            if (lastMovePoint.X == -1)
            {
                return;
            }

            RemovePreviewBox(chessBoxes[lastMovePoint.X, lastMovePoint.Y]);
            lastMovePoint.X = -1;
            lastMovePoint.Y = -1;
        }

       
        private void DisplayPreviewBox(PictureBox chess)
        {
            Point chessLocation = GetCenter(chess);
            PaintPreviewBox(chessLocation, Color.Red);
        }
        //remove RedBox
        private void RemovePreviewBox(PictureBox chess)
        {
            Point picBoxLocation = GetCenter(chess);
            PaintPreviewBox(picBoxLocation, BackColor);
        }

        private static Point GetCenter(PictureBox chess)
        {
            Point chesslocation = chess.Location;
            Point chessCenter = new Point(chesslocation.X + (chessSize / 2), chesslocation.Y + (chessSize / 2));
            return chessCenter;
        }

        //paint Red Box
        private void PaintPreviewBox(int x, int y, Color color)
        {
            Graphics context = CreateGraphics();
            Pen pen = new Pen(color);
            const int lineWidth = 1; 
            pen.Width = lineWidth;


            Point leftTop = new Point(x - chessSize / 2 + lineWidth, y - chessSize / 2 + lineWidth);
            Point rightTop = new Point(x + chessSize / 2 - lineWidth, y - chessSize / 2 + lineWidth);
            Point leftBottom = new Point(x - chessSize / 2 + lineWidth, y + chessSize / 2 - lineWidth);
            Point rightBottom = new Point(x + chessSize / 2 - lineWidth, y + chessSize / 2 - lineWidth);


            const int lineLen = 10;


            context.DrawLine(pen, leftTop, new Point(leftTop.X + lineLen, leftTop.Y));
            context.DrawLine(pen, leftTop, new Point(leftTop.X, leftTop.Y + lineLen));
   
            context.DrawLine(pen, leftBottom, new Point(leftBottom.X + lineLen, leftBottom.Y));
            context.DrawLine(pen, leftBottom, new Point(leftBottom.X, leftBottom.Y - lineLen));
  
            context.DrawLine(pen, rightTop, new Point(rightTop.X - lineLen, rightTop.Y));
            context.DrawLine(pen, rightTop, new Point(rightTop.X, rightTop.Y + lineLen));
       
            context.DrawLine(pen, rightBottom, new Point(rightBottom.X - lineLen, rightBottom.Y));
            context.DrawLine(pen, rightBottom, new Point(rightBottom.X, rightBottom.Y - lineLen));
        }
        private void PaintPreviewBox(Point point, Color color)
        {
            PaintPreviewBox(point.X, point.Y, color);
        }

        private void Board_MouseClick(object sender, MouseEventArgs e)
        {
            if (lastMovePoint.X >= boardSize || lastMovePoint.X < 0 || lastMovePoint.Y >= boardSize || lastMovePoint.X < 0)
                return;
            PictureBox chess = chessBoxes[lastMovePoint.X, lastMovePoint.Y];
            if (chess.Visible == true) return;


            ChessMovingDel(lastMovePoint.X, lastMovePoint.Y);

        }


        public void MoveChess(int row, int col)
        {
        

            PictureBox chess = chessBoxes[row, col];
         
            Image chessImage = isTurnToBlack == true ? global::Go.Properties.Resources.blackstone : global::Go.Properties.Resources.whitestone;

            RemoveLastPreview();
            chess.Visible = true;
            chess.BackgroundImage = chessImage;


            isTurnToBlack = !isTurnToBlack;
        }

    }
}
