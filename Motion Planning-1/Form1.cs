using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Motion_Planning_1
{
    public partial class Form1 : Form
    {
        ReadObstacleFile readobstacle = new ReadObstacleFile();
        ReadRobotFile readrobot = new ReadRobotFile();
        MouseEvent mouse = new MouseEvent();
        MyBitmap bmp;
        LinkedList<PointF[]> drawPoly = new LinkedList<PointF[]>();
        //LinkedList<Polygon[]> drawPoly = new LinkedList<Polygon[]>();

        bool canMoveO = false, canRotateO = false, canMoveR = false, canRotateR = false, canMoveR_G = false, canRotateR_G = false;
        bool findSuccess = false, collision = false, outOfBorder;
        int moveObstacleNum = 0, moveRobotNum = 0;
        float x1, x2, y1, y2, a1, a2, b1, b2;

        private void button4_Click(object sender, EventArgs e)
        {
            if (findSuccess == true)
            {
                Graphics g = this.pictureBox1.CreateGraphics();
                SolidBrush blueBrush = new SolidBrush(Color.Blue);
                while (drawPoly.Count != 0)
                {
                    Console.WriteLine(drawPoly.Count + " " + drawPoly.First.Value[0]);
                    pictureBox1.Refresh();
                    DrawObstaclePointF();
                    DrawRobotPointF();
                    //DrawRobotPath(drawPoly.First.Value, 1);
                    g.FillPolygon(blueBrush, drawPoly.First.Value);
                    drawPoly.RemoveFirst();
                }
            }
            else
            {
                label1.Text = "Found Fail!\n\nCannot Move!";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            readobstacle.Clear();
            readrobot.Clear();
            pictureBox1.Refresh();
            bmp.Clear();
            drawPoly.Clear();
            label1.Text = "Data Clean!";
            label4.Text = "...";
            label5.Text = "...";
        }
        private void DrawRobotPath(Polygon[] poly, int mode)
        {
            Graphics g = this.pictureBox1.CreateGraphics();
            if (mode == 0)
            {
                Pen blackPen = new Pen(Color.Black, 1);
                for(int i = 0; i < poly.Length; i++)
                {
                    PointF[] temp = new PointF[poly[i].GetNumOfVertices()];
                    for (int j = 0; j < poly[i].GetNumOfVertices(); j++)
                    {
                        temp[j] = poly[i].GetInitPoints()[j];
                    }
                    Console.WriteLine("Draw!! " + temp[0]);
                    drawPoly.AddLast(temp);
                    g.DrawPolygon(blackPen, temp);
                }
            }
            else if (mode == 1)
            {
                SolidBrush blueBrush = new SolidBrush(Color.Blue);
                for (int i = 0; i < poly.Length; i++)
                {
                    PointF[] temp = new PointF[poly[i].GetNumOfVertices()];
                    for (int j = 0; j < poly[i].GetNumOfVertices(); j++)
                    {
                        temp[j] = poly[i].GetInitPoints()[i];
                    }
                    g.FillPolygon(blueBrush, temp);
                }
            }

        }
        private void SetObstacleField()
        {
            for (int i = 0; i < readobstacle.GetNumOfObstacle(); i++)
            {
                Obstacle obstacle = readobstacle.GetObstacle(i);
                for (int j = 0; j < obstacle.GetNumOfPoly(); j++)
                {
                    bmp.SetObstaclePixel(obstacle.GetPolygons()[j]);
                }
            }
            //label1.Text = "Set Potential Field";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (collision == true || outOfBorder == true)
            {
                label1.Text = "Some Collisions occurs.\n\nCannot find the path.";
            }
            else
            {
                label1.Text = "Wait For a while...";
                drawPoly.Clear();
                Obstacle[] obstacles = new Obstacle[readobstacle.GetNumOfObstacle()];
                for (int i = 0; i < readobstacle.GetNumOfObstacle(); i++)
                {
                    obstacles[i] = readobstacle.GetObstacle(i);
                }
                for (int i = 0; i < readrobot.GetNumOfRobots(); i++)
                {
                    Robot robot = readrobot.GetRobot(i);
                    bmp = new MyBitmap(128, 128,/*robot.GetControlPoint().Length*/1);
                    SetObstacleField();
                    bmp.SetRobotPixel(robot);
                    bmp.GetTotalPixel();
                    LinkedList<Point> path = bmp.FindPath(robot, obstacles);
                    /*LinkedList<Point>[] path = new LinkedList<Point>[robot.GetControlPoint().Length];
                    for(int j = 0; j < path.Length; j++)
                    {
                        path[j] = new LinkedList<Point>();
                        bmp.FindPath(robot, obstacles, j);
                    }*/
                    if (path == null)
                    {
                        Console.WriteLine("Found Fail !");
                        label1.Text = "Found Fail !";
                    }
                    else
                    {
                        label1.Text = "Found Success !";
                        findSuccess = true;
                        PointF moveCenter = robot.GetInitCenterPoint();
                        Point[] controlP = new Point[robot.GetControlPoint().Length];
                        for(int c = 0; c < robot.GetControlPoint().Length; c++)
                        {
                            controlP[i] = new Point();
                            controlP[i].X = (int)robot.GetControlPoint()[i].X;
                            controlP[i].Y = (int)robot.GetControlPoint()[i].Y;
                        }
                        while (path.Count != 0)
                        {
                            Point point = path.First.Value;
                            RobotPath robotpaths = new RobotPath(robot.GetNumOfPoly(), robot.GetPolygons(), moveCenter, controlP);
                            Polygon[] poly = (Polygon[])robotpaths.GetTransfer(point).Clone();
                            moveCenter = robotpaths.GetPlanCenter();
                            DrawRobotPath(poly, 0);
                            //drawPoly.AddLast(shallowCopy(poly,moveCenter));
                            path.RemoveFirst();
                        }
                    }
                }
                bmp.Clear();
            }
        }
        private Polygon[] shallowCopy(Polygon[] p1,PointF moveCenter)
        {
            Polygon[] temp = new Polygon[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                temp[i] = new Polygon(p1[i].GetNumOfVertices());
                for (int j = 0; j < p1[i].GetNumOfVertices(); j++)
                {
                    temp[i].AddVertices(p1[i].GetInitPointsOnPlan()[j].X, p1[i].GetInitPointsOnPlan()[j].Y, j);
                }
                temp[i].SetInitConfiguration(0, 0, 0, moveCenter);
            }
            return temp; 
        }
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            if (readrobot.IORead(textBox1.Text) == true)
            {
                label4.Text = "Read RobotFile Done!";
                DrawRobotPointF();
            }
            else label4.Text = "RobotFile Doesn't Exist!";
            if (readobstacle.IORead(textBox2.Text) == true)
            {
                label5.Text = "Read ObstacleFile Done!";
                DrawObstaclePointF();
            }
            else label5.Text = "ObstacleFile Doesn't Exist!";
        }

        public Form1()
        {
            InitializeComponent();
        }
        public void DrawRobotPointF()
        {
            // Create Brush.
            Graphics g = this.pictureBox1.CreateGraphics();
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            SolidBrush greenBrush = new SolidBrush(Color.Green);
            for (int i = 0; i < readrobot.GetNumOfRobots(); i++)
            {
                Robot robot = readrobot.GetRobot(i);
                for(int j = 0; j < robot.GetNumOfPoly(); j++)
                {
                    PointF[] initPoints = robot.GetInitPoints(j);
                    PointF[] goalPoints = robot.GetGoalPoints(j);
                    g.FillPolygon(blueBrush, initPoints);
                    g.FillPolygon(greenBrush, goalPoints);
                }
            }
        }
        public void DrawObstaclePointF()
        {
            // Create Brush.
            Graphics g = this.pictureBox1.CreateGraphics();
            SolidBrush blackBrush = new SolidBrush(Color.Black);

            // Create points that define polygon.
            for (int i = 0; i < readobstacle.GetNumOfObstacle(); i++)
            {
                Obstacle obstacle = readobstacle.GetObstacle(i);
                for (int j = 0; j < obstacle.GetNumOfPoly(); j++)
                {
                    PointF[] curvePoints = obstacle.GetPoints(j);
                    // Draw polygon curve to screen.
                    g.FillPolygon(blackBrush, curvePoints);
                }
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            label1.Text = "";
            mouse.SetMousePoint(e.X, e.Y);
            x1 = mouse.GetMousePoint().X;
            y1 = mouse.GetMousePoint().Y;
            Console.WriteLine(x1 + "," + y1);
            a1 = x1; b1 = y1;

            for (int i = 0; i < readobstacle.GetNumOfObstacle(); i++)
            {
                Obstacle obstacle = readobstacle.GetObstacle(i);
                for(int j = 0; j < obstacle.GetNumOfPoly(); j++)
                {
                    PointF[] curvePoints = obstacle.GetPoints(j);
                    if (mouse.IsPointInPolygon(curvePoints, mouse.GetMousePoint()) && e.Button == MouseButtons.Left)
                    {
                        Console.WriteLine("MouseLeft " + i);
                        moveObstacleNum = i;
                        canMoveO = true;
                    }
                    else if (mouse.IsPointInPolygon(curvePoints, mouse.GetMousePoint()) && e.Button == MouseButtons.Right)
                    {
                        Console.WriteLine("MouseRight " + i);
                        moveObstacleNum = i;
                        canRotateO = true;
                    }
                }

            }
            for (int i = 0; i < readrobot.GetNumOfRobots(); i++)
            {
                Robot robot = readrobot.GetRobot(i);
                for(int j = 0; j < robot.GetNumOfPoly(); j++)
                {

                    PointF[] initPoints = robot.GetInitPoints(j);
                    PointF[] goalPoints = robot.GetGoalPoints(j);
                    if (mouse.IsPointInPolygon(initPoints, mouse.GetMousePoint()) && e.Button == MouseButtons.Left)
                    {
                        moveRobotNum = i;
                        canMoveR = true;
                    }
                    else if (mouse.IsPointInPolygon(initPoints, mouse.GetMousePoint()) && e.Button == MouseButtons.Right)
                    {
                        moveRobotNum = i;
                        canRotateR = true;
                    }
                    else if (mouse.IsPointInPolygon(goalPoints, mouse.GetMousePoint()) && e.Button == MouseButtons.Left)
                    {
                        moveRobotNum = i;
                        canMoveR_G = true;
                    }
                    else if (mouse.IsPointInPolygon(goalPoints, mouse.GetMousePoint()) && e.Button == MouseButtons.Right)
                    {
                        moveRobotNum = i;
                        canRotateR_G = true;
                    }
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (canMoveO == true)
            {
                x2 = e.X; y2 = e.Y;
                Obstacle obstacle = readobstacle.GetObstacle(moveObstacleNum);
                obstacle.MoveObstacle(x2 - x1, y2 - y1);
            }
            else if (canRotateO == true)
            {
                x2 = e.X; y2 = e.Y;
                //Console.WriteLine("Rotate:" + (e.X - mouse.GetMousePoint().X) + " " + (e.Y - mouse.GetMousePoint().Y));
                Obstacle obstacle = readobstacle.GetObstacle(moveObstacleNum);
                obstacle.RotateObstacle(x2 - x1);
            }
            else if (canMoveR == true)
            {
                x2 = e.X; y2 = e.Y;
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.MoveRobot(x2 - x1, y2 - y1, 0);
            }
            else if (canRotateR == true)
            {
                x2 = e.X; y2 = e.Y;
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.RotateRobot(x2 - x1, 0);

            }
            else if (canMoveR_G == true)
            {
                x2 = e.X; y2 = e.Y;
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.MoveRobot(x2 - x1, y2 - y1, 1);
            }
            else if (canRotateR_G == true)
            {
                x2 = e.X; y2 = e.Y;
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.RotateRobot(x2 - x1, 1);
            }
            if (canMoveR == true || canMoveO == true || canRotateO == true || canRotateR == true || canMoveR_G == true || canRotateR_G == true)
            {
                x1 = x2; y1 = y2;
                pictureBox1.Refresh();
                DrawObstaclePointF();
                DrawRobotPointF();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            a2 = e.X;
            b2 = e.Y;
            outOfBorder = false;
            collision = false;
            //Refresh pixel on planner
            
            if (canMoveO == true)
            {
                Obstacle obstacle = readobstacle.GetObstacle(moveObstacleNum);
                obstacle.MoveObstacleOnPlan((a2 - a1) / 5, (b2 - b1) / 5);
            }
            else if (canRotateO == true)
            {
                Obstacle obstacle = readobstacle.GetObstacle(moveObstacleNum);
                obstacle.RotateObstacleOnPlan(a2 - a1);
            }
            else if (canMoveR == true)
            {
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.MoveRobotOnPlan((a2 - a1) / 5, (b2 - b1) / 5, 0);
            }
            else if (canMoveR_G == true)
            {
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.MoveRobotOnPlan((a2 - a1) / 5, (b2 - b1) / 5, 1);
            }
            else if (canRotateR == true)
            {
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.RotateRobotOnPlan(a2 - a1, 0);
            }
            else if (canRotateR_G == true)
            {
                Robot robot = readrobot.GetRobot(moveRobotNum);
                robot.RotateRobotOnPlan(a2 - a1, 1);
            }
            
            //Collision Detection
            if (canMoveR == true || canRotateR == true)
            {
                Robot robot = readrobot.GetRobot(moveRobotNum);
                Obstacle[] obstacles = new Obstacle[readobstacle.GetNumOfObstacle()];
                for (int i = 0; i < readobstacle.GetNumOfObstacle(); i++)
                {
                    obstacles[i] = readobstacle.GetObstacle(i);
                }
                collision = robot.IsCollision(obstacles, 0);
                for (int i = 0; i < robot.GetNumOfPoly(); i++)
                {
                    for (int j = 0; j < robot.GetPolygons()[i].GetNumOfVertices(); j++)
                    {
                        if (robot.GetPolygons()[i].GetInitPointsOnPlan()[j].X > 128) outOfBorder = true;
                        if (robot.GetPolygons()[i].GetInitPointsOnPlan()[j].Y > 128) outOfBorder = true;
                    }
                }
            }
            else if (canMoveR_G == true || canRotateR_G == true)
            {
                Robot robot = readrobot.GetRobot(moveRobotNum);
                Obstacle[] obstacles = new Obstacle[readobstacle.GetNumOfObstacle()];
                for (int i = 0; i < readobstacle.GetNumOfObstacle(); i++)
                {
                    obstacles[i] = readobstacle.GetObstacle(i);
                }
                collision = robot.IsCollision(obstacles, 1);
                for (int i = 0; i < robot.GetNumOfPoly(); i++)
                {
                    for (int j = 0; j < robot.GetPolygons()[i].GetNumOfVertices(); j++)
                    {
                        if (robot.GetPolygons()[i].GetGoalPointsOnPlan()[j].X > 128) outOfBorder = true;
                        if (robot.GetPolygons()[i].GetGoalPointsOnPlan()[j].Y > 128) outOfBorder = true;
                    }
                }
            }
            else if (canMoveO == true || canRotateO == true)
            {
                collision = false;
                Robot[] robots = new Robot[readrobot.GetNumOfRobots()];
                Obstacle[] obstacles = new Obstacle[1];
                obstacles[0] = readobstacle.GetObstacle(moveObstacleNum);
                for (int i = 0; i < readrobot.GetNumOfRobots(); i++)
                {
                    robots[i] = readrobot.GetRobot(i);
                    collision = robots[i].IsCollision(obstacles, 0) || robots[i].IsCollision(obstacles, 1);
                    if (collision == true) break;
                }
                //Console.WriteLine(collision);
            }
            if (collision == true) label1.Text = "Collision !!";
            else if (outOfBorder == true) label1.Text = "Out Of Border !!";
            else label1.Text = "Movement Accept !";

            canMoveO = false;
            canRotateO = false;
            canMoveR = false;
            canRotateR = false;
            canMoveR_G = false;
            canRotateR_G = false;
        }
    }
}
