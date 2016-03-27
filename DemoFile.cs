using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AdjustmentPoints
{
    public partial class Form1 : Form
    {
        ArrayList listOfAdjacencyPoints = new ArrayList();
        ArrayList listOfObstacles = new ArrayList();
        IShape draggingShape = null;
        int padding = 7;                  /* Отступ от границы рисования точек
                                           * в зависимости от радиуса */

        // Комментарий для теста в master.

        PointF offset = new PointF(0, 0);

        PointF initPosOnPanning;          /* Начальные позиция перетаскивания. */
        PointF initOffOnPanning;          /* Начальное смещение перед перетаскиванием */        
        bool isPanning = false;           /* Переменная, указывающая возможность
                                           * перетаскивания. */

        bool isAllowedToMoveShape = true; /* Указывает возможность перемещения
                                           * элементов холста */

        bool isFound = false;             /* Найдена ли геометрическая форма
                                           * при нажатии левой кнопки мыши? */

        float zoom = 1;                   /* Масштабный множитель */

        int mouseX;
        int mouseY;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            if (listOfObstacles.Count == 0 && 
                listOfAdjacencyPoints.Count == 0)
            {
                return;
            }
            
            Matrix matrix = e.Graphics.Transform;
            matrix.Scale(zoom, zoom);
            matrix.Translate(offset.X, offset.Y);
            e.Graphics.Transform = matrix;


            foreach (var item in listOfObstacles)
            {
                ((IShape)item).Draw(e.Graphics);
            }

            foreach (var item in listOfAdjacencyPoints)
            {
                ((IShape)item).Draw(e.Graphics);
            }
            Invalidate();            
        }

        private void highToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Random random = new Random();
            int limitLeft = (int)(padding * zoom);
            int limitRight = (int)(this.ClientSize.Width / zoom) - 
                (int)(padding * zoom);

            int limitTop = this.menuStrip1.Height + 
                (int)(padding * zoom);
            int limitBottom = (int)(this.ClientSize.Height / zoom) - 
                (int)(padding * zoom);

            int x = random.Next(limitLeft, limitRight);
            int y = random.Next(limitTop, limitBottom);
            Color color = Color.FromArgb(6, 89, 165);
            
            listOfAdjacencyPoints.Add(new AdjustmentPoint(x, y, color));
            Invalidate();
        }

        private void lowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int limitLeft = (int)(padding * zoom);
            int limitRight = (int)(this.ClientSize.Width / zoom) -
                (int)(padding * zoom);

            int limitTop = this.menuStrip1.Height +
                (int)(padding * zoom);
            int limitBottom = (int)(this.ClientSize.Height / zoom) -
                (int)(padding * zoom);

            int x = random.Next(limitLeft, limitRight);
            int y = random.Next(limitTop, limitBottom);
            Color color = Color.FromArgb(162, 217, 247);
            
            listOfAdjacencyPoints.Add(new AdjustmentPoint(x, y, color));
            Invalidate();
        }

        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int limitLeft = (int)(padding * zoom);
            int limitRight = (int)(this.ClientSize.Width / zoom) -
                (int)(padding * zoom);

            int limitTop = this.menuStrip1.Height +
                (int)(padding * zoom);
            int limitBottom = (int)(this.ClientSize.Height / zoom) -
                (int)(padding * zoom);

            int x = random.Next(limitLeft, limitRight);
            int y = random.Next(limitTop, limitBottom);
            Color color = Color.FromArgb(167, 158, 205);
            
            listOfAdjacencyPoints.Add(new AdjustmentPoint(x, y, color));
            Invalidate();
        }

        private void addObstacleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Здесь границы заданы неправильно!
            Random random = new Random();
            int limitLeft = (int)(padding * zoom);
            int limitRight = (int)(this.ClientSize.Width / zoom) -
                (int)(padding * zoom);

            int limitTop = this.menuStrip1.Height +
                (int)(padding * zoom);
            int limitBottom = (int)(this.ClientSize.Height / zoom) -
                (int)(padding * zoom);

            int x = random.Next(limitLeft, limitRight);
            int y = random.Next(limitTop, limitBottom);
            Color color = Color.FromArgb(200, 255, 54, 33);
            listOfObstacles.Add(new Obstacle(x, y, 100f, 50f, color));
            Invalidate();
        } 

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //foreach (IShape item in list)
            //{
            //    if (item.InRange(e))
            //    {
            //        if (e.Button == MouseButtons.Left)
            //        {
            //            draggingShape = item;
            //            break;
            //        }
            //        else if (e.Button == MouseButtons.Right)
            //        {
            //            list.Remove(item);
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        draggingShape = null;
            //    }
            //}

            mouseX = e.X;
            mouseY = e.Y;
            
            initPosOnPanning = e.Location;
            initOffOnPanning = offset;
            isPanning = true;
            draggingShape = null;

            for (int i = listOfAdjacencyPoints.Count - 1; i >= 0; i--)
            {
                if (((IShape)listOfAdjacencyPoints[i]).InRange(e, zoom, offset) &&
                    isAllowedToMoveShape)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        draggingShape = (IShape)listOfAdjacencyPoints[i];
                        listOfAdjacencyPoints.RemoveAt(i);
                        listOfAdjacencyPoints.Add(draggingShape);
                        return;
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        listOfAdjacencyPoints.Remove((IShape)listOfAdjacencyPoints[i]);
                        return;
                    }
                }
            }

            for (int i = listOfObstacles.Count - 1; i >= 0; i--)
            {
                if (((IShape)listOfObstacles[i]).InRange(e, zoom, offset) &&
                    isAllowedToMoveShape)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        draggingShape = (IShape)listOfObstacles[i];
                        listOfObstacles.RemoveAt(i);
                        listOfObstacles.Add(draggingShape);
                        return;
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        listOfObstacles.Remove((IShape)listOfObstacles[i]);
                        return;
                    }
                }
            }

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            String formTitle = String.Format("({0}, {1})", e.X, e.Y);
            this.Text = formTitle;

            if (isPanning && !isAllowedToMoveShape)
            {
                offset = new PointF(
                    initOffOnPanning.X + (e.X - initPosOnPanning.X) / zoom,
                    initOffOnPanning.Y + (e.Y - initPosOnPanning.Y) / zoom
                    );
                Invalidate();
            }

            if (draggingShape != null)
            {
                int dx = e.X - mouseX;
                int dy = e.Y - mouseY;

                if (draggingShape is AdjustmentPoint)
                {
                    if (((AdjustmentPoint)draggingShape).IsCollisionAvoided(listOfObstacles, dx, dy))
                    {
                        draggingShape.MoveShape(e, zoom, offset, dx, dy);
                        mouseX = e.X;
                        mouseY = e.Y;
                    }
                    else
                        draggingShape = null;
                }
                else if (draggingShape is Obstacle)
                {
                    if (((Obstacle)draggingShape).IsCollisionAvoided(listOfAdjacencyPoints))
                    {
                        draggingShape.MoveShape(e, zoom, offset, dx, dy);
                    }
                }                
            }

            //if (draggingShape != null)
            //{
            //    draggingShape.MoveShape(e, zoom, offset);
            //}
            
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            draggingShape = null;

            if (e.Button == MouseButtons.Left)
            {
                isPanning = false;
                isFound = false;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            zoom += (float)(e.Delta / 120) / 2.0f;
            if (zoom < 1)
                zoom = 1;

            Invalidate();
        }

        private void panToolButton_CheckedChanged(object sender, EventArgs e)
        {
            if (panToolButton.Checked)
            {
                isAllowedToMoveShape = false;
            }
            else
            {
                isAllowedToMoveShape = true;
            }
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Invalidate();
        }              
    }
}
