using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_Planning_1
{
    class MyBitmap
    {
        private Bitmap mybitmap;
        private int width, height, NumOfControlP;
        private Point[] goalP, initP;
        private LinkedList<Point> list = new LinkedList<Point>();
        public MyBitmap(int x, int y, int ControlPNo)
        {
            mybitmap = new Bitmap(x, y);
            goalP = new Point[ControlPNo];
            initP = new Point[ControlPNo];
            NumOfControlP = ControlPNo;
            width = x; height = y;
            for (int k = 0; k < ControlPNo; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        mybitmap.SetPixel(i, j, Color.White);
                    }
                }
            }
        }
        private Point getP(int x, int y)
        {
            Point point = new Point(x, y);
            return point;
        }
        private void mySetPixel(int x, int y, int p, int ControlPNo)
        {
            Color mycolor = Color.FromArgb(p);
            mybitmap.SetPixel(x, y, mycolor);
        }
        public int MyGetPixel(int x, int y)
        {
            int tmp = mybitmap.GetPixel(x, y).ToArgb();
            return tmp;
        }
        public int[,] GetTotalPixel()
        {
            int[,] totalpixel = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    totalpixel[i, j] = mybitmap.GetPixel(i, j).ToArgb();
                    if (totalpixel[i, j] == 255) Console.Write("255 ");
                    else if (totalpixel[i, j] == 0) Console.Write("000 ");
                    else if (totalpixel[i, j] == 999) Console.Write("999 ");
                    else if (totalpixel[i, j] == 555) Console.Write("555 ");
                    else Console.Write("    ");
                    /*
                    if (totalpixel[i, j] == -1) Console.Write("    ");
                    else
                    {
                        if (totalpixel[i, j] < 10) Console.Write("00" + totalpixel[i, j] + " ");
                        else if (totalpixel[i, j] < 100) Console.Write("0" + totalpixel[i, j] + " ");
                        else Console.Write(totalpixel[i, j] + " ");
                    }*/
                }
                Console.WriteLine();
            }
            return totalpixel;
        }
        public void SetRobotPixel(Robot r)
        {
            /*PointF[] goalPointF = new PointF[NumOfControlP];
            PointF[] initPointF = new PointF[NumOfControlP];
            for (int i = 0; i < NumOfControlP; i++)
            {
                goalPointF[i] = r.GetGoalPoints()[i];
                goalP[i].X = (int)goalPointF[i].X; goalP[i].Y = (int)goalPointF[i].Y;
                initPointF[i] = r.GetControlPoint()[i];
                initP[i].X = (int)initPointF[i].X; initP[i].Y = (int)initPointF[i].Y;
            }*/
            PointF goalPointF = r.GetGoalCenterPoint();
            goalP[0].X = (int)goalPointF.X; goalP[0].Y = (int)goalPointF.Y;
            Console.WriteLine(goalP[0]);
            PointF initPointF = r.GetInitCenterPoint();
            initP[0].X = (int)initPointF.X; initP[0].Y = (int)initPointF.Y;
            Console.WriteLine(initP[0]);

            for (int i = 0; i < NumOfControlP; i++)
            {
                //mySetPixel(initP.X, initP.Y, 999);
                mySetPixel(goalP[i].X, goalP[i].Y, 0, i);
                SetPotentialField(goalP[i].X, goalP[i].Y, i);
            }
            
        }
        public void SetObstaclePixel(Polygon p)
        {
            PointF[] points = p.GetInitPointsOnPlan();

            int nodes, pixelX, pixelY, i, j;
            int[] nodeX = new int[10];
            for (pixelY = 0; pixelY < 128; pixelY++)
            {
                nodes = 0; j = points.Length - 1;
                for (i = 0; i < points.Length; i++)
                {
                    if (points[i].Y < (float)pixelY && points[j].Y >= (float)pixelY
                        || points[j].Y < (float)pixelY && points[i].Y >= (float)pixelY)
                    {
                        nodeX[nodes++] = (int)(points[i].X + (pixelY - points[i].Y) / (points[j].Y - points[i].Y) * (points[j].X - points[i].X));
                    }
                    j = i;
                }
                i = 0;
                while (i < nodes - 1)
                {
                    if (nodeX[i] > nodeX[i + 1])
                    {
                        int swap = nodeX[i]; nodeX[i] = nodeX[i + 1]; nodeX[i + 1] = swap;
                        if (i > 0) i--;
                    }
                    else
                    {
                        i++;
                    }
                }
                for (i = 0; i < nodes; i += 2)
                {
                    if (nodeX[i] >= 128) break;
                    if (nodeX[i + 1] > 0)
                    {
                        if (nodeX[i] < 0) nodeX[i] = 0;
                        if (nodeX[i + 1] > 128) nodeX[i + 1] = 128;
                        for (pixelX = nodeX[i]; pixelX < nodeX[i + 1]; pixelX++)
                        {
                            for(int k = 0; k < NumOfControlP; k++)
                            {
                                mySetPixel(pixelX, pixelY, 255, k);
                            }
                        }
                    }
                }
            }
        }
        public void SetPotentialField(int a, int b, int ControlPNo)
        {
            list.AddLast(getP(a, b));

            for (int k = 0; k < list.Count; k++)
            {
                LinkedList<Point> tmp = list;
                while (tmp.First != null)
                {
                    int x = list.First.Value.X;
                    int y = list.First.Value.Y;
                    for (int i = x - 1; i <= x + 1 && i >= 0 && i < 128; i++)
                    {
                        for (int j = y - 1; j <= y + 1 && j >= 0 && j < 128; j++)
                        {
                            if (mybitmap.GetPixel(i, j).ToArgb() == -1)
                            {
                                mySetPixel(i, j, mybitmap.GetPixel(x, y).ToArgb() + 1, ControlPNo);
                                list.AddLast(getP(i, j));
                            }
                        }
                    }
                    tmp.RemoveFirst();
                }
            }
        }
        public LinkedList<Point> FindPath(Robot r, Obstacle[] o)
        {
            Console.WriteLine("Goal: " + goalP[0].X + "," + goalP[0].Y);
            return BestFirstSearch(initP[0].X, initP[0].Y, r, o);

        }
        private LinkedList<Point> BestFirstSearch(int a, int b, Robot r, Obstacle[] o)
        {
            int[,] visited = new int[width, height];

            Console.WriteLine("Start Point: " + a + "," + b + " = " + MyGetPixel(a, b));

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (MyGetPixel(i, j) == 255)
                    {
                        visited[i, j] = 1;
                    }
                }
            }
            LinkedList<Point> open = new LinkedList<Point>();
            RobotPath robotpath = new RobotPath(r.GetNumOfPoly(), r.GetPolygons(),r.GetInitCenterPoint(),initP);

            open = new LinkedList<Point>();
            open.AddLast(getP(a, b));
            visited[a, b] = 1;
            bool success = false;

            while (open.Count != 0 && !success)
            {
                List<Point> neighbors = new List<Point>();

                Point target = open.Last();
                if (target.X > 1 && visited[target.X - 1, target.Y] == 0) neighbors.Add(getP(target.X - 1, target.Y));
                if (target.X < 127 && visited[target.X + 1, target.Y] == 0) neighbors.Add(getP(target.X + 1, target.Y));
                if (target.Y > 1 && visited[target.X, target.Y - 1] == 0) neighbors.Add(getP(target.X, target.Y - 1));
                if (target.Y < 127 && visited[target.X, target.Y + 1] == 0) neighbors.Add(getP(target.X, target.Y + 1));

                if (neighbors.Count == 0)
                {
                    open.RemoveLast();
                }
                else
                {
                    int min = findMin(neighbors);
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        if (MyGetPixel(neighbors[i].X, neighbors[i].Y) == min && visited[neighbors[i].X, neighbors[i].Y] == 0 && robotpath.MoveWithCenter(neighbors[i], o) == true)
                        {
                            Console.WriteLine("Min: " + neighbors[i].X + "," + neighbors[i].Y + " value = " + min);
                            Console.WriteLine(robotpath.MoveWithCenter(neighbors[i], o));
                            //install x’ in T with a pointer toward x
                            open.AddLast(neighbors[i]);
                            visited[neighbors[i].X, neighbors[i].Y] = 1;
                            if (neighbors[i].X == goalP[0].X && neighbors[i].Y == goalP[0].Y)
                            {
                                success = true;
                            }
                            break;
                        }
                        else if (i == (neighbors.Count - 1))
                        {
                            Console.WriteLine("i = " + i);
                            open.RemoveLast();
                            Console.WriteLine("open.Last: " + open.Last.Value.X + "," + open.Last.Value.Y);
                            break;
                        }
                    }
                    neighbors.Clear();
                }
            }
            if (success)
            {
                //return the constructed path by tracing the pointers in T from Xgoal back to Xinit
                Console.WriteLine(open.Count);
                return open;
            }
            else return null;
        }
        private int findMin(List<Point> tmp)
        {
            int[] value = new int[tmp.Count];
            Console.WriteLine("neighbors: ");
            for (int i = 0; i < tmp.Count; i++)
            {
                value[i] = MyGetPixel(tmp[i].X, tmp[i].Y);
                Console.Write(value[i] + " ");
            }
            Console.WriteLine();
            return value.Min();
        }
        
        public void Clear()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for(int k = 0; k < NumOfControlP; k++)
                    {
                        mySetPixel(i, j, -1, k);
                    }
                }
            }
        }
    }
}
