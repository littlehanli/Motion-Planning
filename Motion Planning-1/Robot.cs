using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Motion_Planning_1
{
    class Robot
    {
        private PointF[] controlPoints,goalPoints;
        private Polygon[] polygons;
        private float[] initConfigurations = new float[3];
        private float[] goalConfigurations = new float[3];
        private int robotNum, numOfPoly;
        private PointF initCenter, goalCenter, i_center, g_center;

        public Robot(int p,int n)
        {
            robotNum = n;
            numOfPoly = p;
            polygons = new Polygon[p];
            Console.WriteLine("Robot " + robotNum + " has " + numOfPoly + " polygons.");
        }
        public void AddNumOfPolyVertice(int n, int p)
        {
            polygons[n] = new Polygon(p);
        }
        public int GetNumOfPoly()
        {
            return numOfPoly;
        }
        public void AddPoly(float x, float y, int n, int p)
        {
            polygons[n].AddVertices(x, y, p);
        }
        public Polygon[] GetPolygons()
        {
            return polygons;
        }
        public PointF[] GetInitPointsOnPlan(int polyNum)
        {
            return polygons[polyNum].GetInitPointsOnPlan();
        }
        public PointF[] GetGoalPointsOnPlan(int polyNum)
        {
            return polygons[polyNum].GetGoalPointsOnPlan();
        }
        public PointF[] GetInitPoints(int polyNum)
        {
            return polygons[polyNum].GetInitPoints();
        }
        public PointF[] GetGoalPoints(int polyNum)
        {
            return polygons[polyNum].GetGoalPoints();
        }
        public void SetNumControlPoints(int n)
        {
            controlPoints = new PointF[n];
            goalPoints = new PointF[n];
        }
        public void AddControlP(float x,float y,int n)
        {
            controlPoints[n].X = x;
            controlPoints[n].Y = y;
            goalPoints[n].X = x;
            goalPoints[n].Y = y;

            float temp = controlPoints[n].X * (float)Math.Cos((double)initConfigurations[2] * Math.PI / 180.0) - controlPoints[n].Y * (float)Math.Sin((double)initConfigurations[2] * Math.PI / 180.0);
            controlPoints[n].Y = controlPoints[n].X  * (float)Math.Sin((double)initConfigurations[2] * Math.PI / 180.0) + controlPoints[n].Y * (float)Math.Cos((double)initConfigurations[2] * Math.PI / 180.0);
            controlPoints[n].X = temp;

            float temp1 = goalPoints[n].X * (float)Math.Cos((double)goalConfigurations[2] * Math.PI / 180.0) - goalPoints[n].Y * (float)Math.Sin((double)goalConfigurations[2] * Math.PI / 180.0);
            controlPoints[n].Y = goalPoints[n].X * (float)Math.Sin((double)goalConfigurations[2] * Math.PI / 180.0) + goalPoints[n].Y * (float)Math.Cos((double)goalConfigurations[2] * Math.PI / 180.0);
            controlPoints[n].X = temp1;

            controlPoints[n].X += initConfigurations[0];
            controlPoints[n].Y += initConfigurations[1];

            goalPoints[n].X += goalConfigurations[0];
            goalPoints[n].Y += goalConfigurations[1];

            Console.WriteLine(n + " " + controlPoints[n].X + " " + controlPoints[n].Y);
        }
        public PointF[] GetControlPoint()
        {
            return controlPoints;
        }
        public PointF[] GetGoalPoints()
        {
            return goalPoints;
        }
        public void AddInitConfiguration(float x, float y, float d)
        {
            initConfigurations[0] = x;
            initConfigurations[1] = y;
            initConfigurations[2] = d;
            Console.Write("InitPoints: ");
            for(int i = 0; i < numOfPoly; i++)
            {
                polygons[i].SetInitConfiguration(x, y, d, i_center);
            }
            SetCenterPointOnPlan(0);
            SetCenterPoint(0);
            Console.WriteLine(i_center);
        }
        public void AddGoalConfiguration(float x,float y, float d)
        {
            goalConfigurations[0] = x;
            goalConfigurations[1] = y;
            goalConfigurations[2] = d;
            Console.Write("GoalPoints: ");
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].SetGoalConfiguration(x, y, d, g_center);
            }
            SetCenterPointOnPlan(1);
            SetCenterPoint(1);
            Console.WriteLine(g_center);
        }
        
        public void MoveRobot(float x, float y, int IorG)
        {
            for(int i = 0; i < numOfPoly; i++)
            {
                polygons[i].MovePolygon(x, y, IorG);
            }
            SetCenterPoint(IorG);
        }
        public void MoveRobotOnPlan(float x,float y, int IorG)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].MovePolygonOnPlan(x, y, IorG);
            }
            for(int i = 0; i < controlPoints.Length; i++)
            {
                if(IorG == 0)
                {
                    controlPoints[i].X += x;
                    controlPoints[i].Y -= y;
                }
                else if (IorG == 1)
                {
                    goalPoints[i].X += x;
                    goalPoints[i].Y -= y;
                }
            }
            SetCenterPointOnPlan(IorG);
        }
        public void RotateRobot(float x,int IorG)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                if (IorG == 0)
                {
                    polygons[i].RotatePolygon(x, initCenter, IorG);
                }
                else if (IorG == 1)
                {
                    polygons[i].RotatePolygon(x, goalCenter, IorG);
                }
            }
            SetCenterPoint(IorG);
        }
        public void RotateRobotOnPlan(float x, int IorG)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                if (IorG == 0)
                {
                    polygons[i].RotatePolygonOnPlan(-x, i_center, IorG);
                    for (int j = 0; j < controlPoints.Length; j++)
                    {
                        float temp = i_center.X + ((controlPoints[i].X - i_center.X) * (float)Math.Cos((double)x * Math.PI / 180.0)) - (controlPoints[i].Y - i_center.Y) * (float)Math.Sin((double)x * Math.PI / 180.0);
                        controlPoints[i].Y = i_center.Y + ((controlPoints[i].X - i_center.X) * (float)Math.Sin((double)x * Math.PI / 180.0)) + (controlPoints[i].Y - i_center.Y) * (float)Math.Cos((double)x * Math.PI / 180.0);
                        controlPoints[i].X = temp;
                    }
                }
                else if (IorG == 1)
                {
                    polygons[i].RotatePolygonOnPlan(-x, g_center, IorG);
                    for (int j = 0; j < goalPoints.Length; j++)
                    {
                        float temp = g_center.X + ((goalPoints[i].X - g_center.X) * (float)Math.Cos((double)x * Math.PI / 180.0)) - (goalPoints[i].Y - g_center.Y) * (float)Math.Sin((double)x * Math.PI / 180.0);
                        goalPoints[i].Y = g_center.Y + ((goalPoints[i].X - g_center.X) * (float)Math.Sin((double)x * Math.PI / 180.0)) + (goalPoints[i].Y - g_center.Y) * (float)Math.Cos((double)x * Math.PI / 180.0);
                        goalPoints[i].X = temp;
                    }
                }
            }
            SetCenterPointOnPlan(IorG);
        }
        public void SetCenterPoint(int IorG)
        {
            float totalX = 0, totalY = 0;
            int totalVertices = 0;
            if (IorG == 0)
            {
                for (int i = 0; i < numOfPoly; i++)
                {
                    totalVertices += polygons[i].GetNumOfVertices();
                    for (int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                    {
                        totalX += polygons[i].GetInitPoints()[j].X;
                        totalY += polygons[i].GetInitPoints()[j].Y;
                    }
                }
                initCenter.X = totalX / totalVertices;
                initCenter.Y = totalY / totalVertices;
            }
            else if (IorG == 1)
            {
                for (int i = 0; i < numOfPoly; i++)
                {
                    totalVertices += polygons[i].GetNumOfVertices();
                    for (int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                    {
                        totalX += polygons[i].GetGoalPoints()[j].X;
                        totalY += polygons[i].GetGoalPoints()[j].Y;
                    }
                }
                goalCenter.X = totalX / totalVertices;
                goalCenter.Y = totalY / totalVertices;
            }
        }
        public void SetCenterPointOnPlan(int IorG)
        {
            float totalX = 0, totalY = 0;
            int totalVertices = 0;
            if (IorG == 0)
            {
                for (int i = 0; i < numOfPoly; i++)
                {
                    totalVertices += polygons[i].GetNumOfVertices();
                    for (int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                    {
                        totalX += polygons[i].GetInitPointsOnPlan()[j].X;
                        totalY += polygons[i].GetInitPointsOnPlan()[j].Y;
                    }
                }
                i_center.X = totalX / totalVertices;
                i_center.Y = totalY / totalVertices;
            }
            else if (IorG == 1)
            {
                for (int i = 0; i < numOfPoly; i++)
                {
                    totalVertices += polygons[i].GetNumOfVertices();
                    for (int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                    {
                        totalX += polygons[i].GetGoalPointsOnPlan()[j].X;
                        totalY += polygons[i].GetGoalPointsOnPlan()[j].Y;
                    }
                }
                g_center.X = totalX / totalVertices;
                g_center.Y = totalY / totalVertices;
            }
        }
        public PointF GetInitCenterPoint()
        {
            return i_center;
        }
        public PointF GetGoalCenterPoint()
        {
            return g_center;
        }
        public bool IsCollision(Obstacle[] obstacles, int checkObject)
        {
            if (obstacles != null)
            {
                for (int p = 0; p < numOfPoly; p++)
                {
                    for (int i = 0; i < obstacles.Length; i++)
                    {
                        for (int j = 0; j < polygons[p].GetNumOfVertices(); j++)
                        {
                            for (int k = j + 1; k < polygons[p].GetNumOfVertices(); k++)
                            {
                                if (checkObject == 0)
                                {
                                    if (isLineIntersectAnObstacle(polygons[p].GetInitPointsOnPlan()[j], polygons[p].GetInitPointsOnPlan()[k], obstacles[i]) == true)
                                    {
                                        Console.WriteLine("Collision Obstacle: " + i);
                                        return true;
                                    }
                                }
                                else if (checkObject == 1)
                                {
                                    if (isLineIntersectAnObstacle(polygons[p].GetGoalPointsOnPlan()[j], polygons[p].GetGoalPointsOnPlan()[k], obstacles[i]) == true) { return true; }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            else return false;
        }
        private bool isLineIntersectAnObstacle(PointF a, PointF b, Obstacle o)
        {
            for(int p = 0; p < o.GetNumOfPoly(); p++)
            {
                for (int i = 0; i < o.GetPolygons()[p].GetInitPointsOnPlan().Length; i++)
                {
                    for (int j = i + 1; j < o.GetPolygons()[p].GetInitPointsOnPlan().Length; j++)
                    {
                        PointF temp1 = new PointF(o.GetPolygons()[p].GetInitPointsOnPlan()[i].X, o.GetPolygons()[p].GetInitPointsOnPlan()[i].Y);
                        PointF temp2 = new PointF(o.GetPolygons()[p].GetInitPointsOnPlan()[j].X, o.GetPolygons()[p].GetInitPointsOnPlan()[j].Y);
                        if (isLineIntersectLine(a, b, temp1, temp2) == true)
                        {
                            Console.WriteLine("a: " + a + " b: " + b + " temp1: " + temp1 + "  temp2: " + temp2);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool isLineIntersectLine(PointF a, PointF b, PointF c, PointF d)
        {
            //Console.WriteLine("Detect Point " + a + " , " + b + " and " + c + " , " + d);
            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            // Detect coincident lines (has a problem, read below)
            if (denominator == 0) { return numerator1 == 0 && numerator2 == 0; }

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;
            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

    }
}
