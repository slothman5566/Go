using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Go
{

    //http://programmermagazine.github.io/201407/htm/focus2.html cal score reference
    public partial class Board : UserControl
    {
        protected int boardSize;
        protected const int chessSize = 40;
        private const int boardLeftTopX = 30;
        private const int boardLeftTopY = 30;
        private int [,] chessField;
        private PictureBox [,] chessBoxes;
        private  List<int> z9 = new List<int>();//{ 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private  List<int> i9 = new List<int>();//{ -4, -3, -2, -1, 0, 1, 2, 3, 4 };
        private  List<int> d9 = new List<int>();//{ 4, 3, 2, 1, 0, -1, -2, -3, -4 };
        private  List<int> z5 = new List<int>();//{ 0, 0, 0, 0, 0 };
        private List<int> i2 = new List<int>();
        private List<int> d2 = new List<int>();
        private int[] attackScores = { 0, 3, 10, 30, 100, 500 };
        private int[] guardScores = { 0, 2, 9, 25, 90, 400 };
        private Size realSize;
        public bool AI=false;
        public  struct Best
        {
            public int x,y,max;            
        }
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
        public Board()
        {
          
        } 
        public Board(int boardSize)
        {
            InitializeComponent();
            z9.AddRange(new int [] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            i9.AddRange(new int [] { -4, -3, -2, -1, 0, 1, 2, 3, 4 });
            d9.AddRange(new int [] { 4, 3, 2, 1, 0, -1, -2, -3, -4 });
            z5.AddRange(new int [] { 0, 0, 0, 0, 0 });
            i2.AddRange(new int [] { -2, -1, 0, 1, 2 });
            d2.AddRange(new int [] { 2, 1, 0, -1, -2 });
            this.boardSize = boardSize;
            chessBoxes = new PictureBox[boardSize, boardSize];
            chessField = new int[boardSize, boardSize];
            InitBoard();
            MouseMove += Board_MouseMove;
            MouseLeave += Board_MouseLeave;
            MouseClick += Board_MouseClick;


            var showSize = (boardSize - 1) * chessSize;
            var realSize = boardLeftTopX * 2 + showSize;
            this.realSize = new Size(realSize, realSize);

            isTurnToBlack = true;
        }

        public bool patternCheck(int turn, int r, int c, List<int> dr, List<int> dc)//判斷是否有連線
        {
            for (var i = 0; i != dr.Count; i++)
            {
                var tr = (r + dr[i]);
                var tc = (c + dc[i]);
                if (tr < 0 || tr >= boardSize || tc < 0 || tc >= boardSize)
                {
                    return false;
                }
                var v = chessField[tr, tc];
                if (v != turn)
                {
                    return false;
                }
            }
            return true;
        }
    
     
        private int getScore(int r, int c, int turn, int mode)
        {
            var score = 0;
            var mScores = (mode == 0) ? attackScores : guardScores;
            chessField[r, c] = turn;
            for (var start = 0; start <= 4; start++)
            {
                for (var len = 5; len >= 1; len--)
                {
                    List<int> zero = new List<int>();
                    for (int i = start; i < start + len; i++)
                    {
                        zero.Add(z9[i]);
                    }
                    List<int> dec=new List<int>();
                    for (int i = start; i < start + len; i++)
                       {
                        dec.Add(d9 [i]);
                    }
                    List<int> inc = new List<int>();
                    for (int i = start; i < start + len; i++)
                    {
                        inc.Add(i9[i]);
                    }
                  
                    if (patternCheck(turn, r, c, zero, inc))
                    {
                        score += mScores[len];
                    }
                    if (patternCheck(turn, r, c, inc, zero))
                    {
                        score += mScores[len];
                    }
                    if (patternCheck(turn, r, c, inc, inc))
                    {
                        score += mScores[len];
                    }
                    if (patternCheck(turn, r, c, inc, dec))
                    {
                        score += mScores[len];
                    }
                }
            }
            chessField[r, c] = 0;
            return score;
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
            var startX = boardLeftTopX - (chessSize / 2);
            var startY = boardLeftTopY - (chessSize / 2);

            for (var x = 0; x < boardSize; x++)
            {
                for (var y = 0; y < boardSize; y++)
                {
                    var chess = new PictureBox();
                    chess.Location = new Point(x * chessSize + startX, y * chessSize + startY);
                    chess.Size = new Size(chessSize, chessSize);
                    chess.BackColor = Color.Transparent;
                    chess.SizeMode = PictureBoxSizeMode.CenterImage;
                    chess.Visible = false;
                    chessField [x, y] = 0;
                    Controls.Add(chess);
                    chessBoxes [x, y] = chess;
                }
            }
        }


        private void Board_Paint(object sender, PaintEventArgs e)
        {
            var context = e.Graphics;
            var linePen = new Pen(Color.Black, 1);
            var blackBrush = new SolidBrush(Color.Black);

            var hX = boardLeftTopX;
            var hY = boardLeftTopY;
            var lineLen = (boardSize - 1) * chessSize;


            var redPointIndex = 3;

            for (var i = 0; i < boardSize; i++)
            {
                var vX = hY;
                var vY = hX;

                context.DrawLine(linePen, hX, hY, hX + lineLen, hY);
                context.DrawLine(linePen, vX, vY, vX, vY + lineLen);

                if (i == redPointIndex)
                {
                    var width = 8;

                    context.FillEllipse(blackBrush,  vY + lineLen -  (3 * chessSize) - (width / 2), hY - (width / 2), width, width);
                    context.FillEllipse(blackBrush,  vY + lineLen -  (7 * chessSize) - (width / 2), hY - (width / 2), width, width);
                    context.FillEllipse(blackBrush,  vY + lineLen - ( 11 * chessSize) - (width / 2), hY - (width / 2), width, width);

                    redPointIndex += 4;
                }

                hY += chessSize;
            }
        }


        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            var marginH = boardLeftTopX - (chessSize / 2);
            var marginV = boardLeftTopY - (chessSize / 2);
            var xIndex = (e.X - marginH) / chessSize;
            var yIndex = (e.Y - marginV) / chessSize;


            if (xIndex < 0 || yIndex < 0 || (xIndex + 1) > boardSize || (yIndex + 1) > boardSize)
            {
                RemoveLastPreview();
                return;
            }

            var chess = chessBoxes [xIndex, yIndex];

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

            RemovePreviewBox(chessBoxes [lastMovePoint.X, lastMovePoint.Y]);
            lastMovePoint.X = -1;
            lastMovePoint.Y = -1;
        }


        private void DisplayPreviewBox(PictureBox chess)
        {
            var chessLocation = GetCenter(chess);
            PaintPreviewBox(chessLocation, Color.Red);
        }

        private void RemovePreviewBox(PictureBox chess)
        {
            var picBoxLocation = GetCenter(chess);
            PaintPreviewBox(picBoxLocation, BackColor);
        }

        private static Point GetCenter(PictureBox chess)
        {
            var chesslocation = chess.Location;
            var chessCenter = new Point(chesslocation.X + (chessSize / 2), chesslocation.Y + (chessSize / 2));
            return chessCenter;
        }


        private void PaintPreviewBox(int x, int y, Color color)
        {
            var context = CreateGraphics();
            var pen = new Pen(color);
            const int lineWidth = 1;
            pen.Width = lineWidth;


            var leftTop = new Point(x - chessSize / 2 + lineWidth, y - chessSize / 2 + lineWidth);
            var rightTop = new Point(x + chessSize / 2 - lineWidth, y - chessSize / 2 + lineWidth);
            var leftBottom = new Point(x - chessSize / 2 + lineWidth, y + chessSize / 2 - lineWidth);
            var rightBottom = new Point(x + chessSize / 2 - lineWidth, y + chessSize / 2 - lineWidth);


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

        public  void ReSet()
        {
            for (var x = 0; x != boardSize; x++)
            {
                for (var y = 0; y != boardSize; y++ )
                {
                    chessBoxes [x, y].Visible = false;
                    chessBoxes [x, y].BackgroundImage = null;
                    chessField [x, y] = 0;
                }
            }
            isTurnToBlack = true;
        }

        private void Bangjudge()
        {
            for (var x = 0; x != boardSize - 2; x++)
            {
                for (var y = 0; y != boardSize - 2; y++)
                {
                    bool win = false;
                    if (chessField [x, y] != 0)
                    {
                        if (patternCheck(chessField [x, y], x, y, z5, i2)) // 垂直 | ;
                            win = true;      
                        
                        if (patternCheck(chessField[x, y], x, y, i2, z5)) // 水平 - ;
                            win = true;
                        if (patternCheck(chessField[x, y], x, y, i2, i2)) // 下斜 \ ;
                            win = true;
                        if (patternCheck(chessField[x, y], x, y, i2, d2)) // 上斜 / ;
                            win = true;
                        if (win)
                            {
                                if (chessField[x, y] == 1)
                                {
                                    MessageBox.Show
                                    ("Black is Good");
                                }
                                else
                                {
                                    if (chessField[x, y] == 2)
                                    {
                                        MessageBox.Show
                                        ("White is Good");
                                    }
                                }
                            if (AI && chessField [x, y] == 2)
                            {
                                MessageBox.Show
                                       ("輸給電腦丟臉!!");
                                       
                            }
                            
                                ReSet();
                            }
                    }
                   
                }
            }
        }

     
        private void ComputerTurn()
        {
            Best best = new Best();
            best.x = best.y = 0;
            best.max = -1;
            for (int x = 0; x != boardSize; x++ )
                for (int y = 0; y != boardSize; y++ )
                {
                    if (chessField [x, y] == 0)
                    {
                        int attackScore = getScore(x, y, 2, 0);
                        int guardScore = getScore(x, y, 1, 1);
                        int score = attackScore + guardScore;
                        if (score > best.max)
                        {
                            best.x = x;
                            best.y = y;
                            best.max = score;
                        }
                    }
                }
            MoveChess(best.x, best.y);
        }
        private void Board_MouseClick(object sender, MouseEventArgs e)
        {
            if (lastMovePoint.X >= boardSize || lastMovePoint.X < 0 || lastMovePoint.Y >= boardSize || lastMovePoint.X < 0)
            {
                return;
            }
            var chess = chessBoxes [lastMovePoint.X, lastMovePoint.Y];
            if (chess.Visible == true)
            {
                return;
            }
            lastMovePoint = new Point(lastMovePoint.X, lastMovePoint.Y);
            MoveChess(lastMovePoint.X, lastMovePoint.Y);
            if(AI)
            ComputerTurn();
            Bangjudge();
        }


        public void MoveChess(int row, int col)
        {
            Image chessImage = isTurnToBlack == true ? global::Go.Properties.Resources.blackstone : global::Go.Properties.Resources.whitestone;
            chessField [row, col] = isTurnToBlack == true ? 1 : 2;
            RemoveLastPreview();
            chessBoxes [row, col].Visible = true;
            chessBoxes [row, col].BackgroundImage = chessImage;

            isTurnToBlack = !isTurnToBlack;
        }
    }
}
