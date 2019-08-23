using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Motion_Planning_1
{
    class Obstacle
    {
        private Polygon[] polygons;
        private float[] configurations = new float[3];
        private int obstacleNum, numOfPoly;
        private PointF centerPoint, centerOnPlan;

        public Obstacle(int p, int n)
        {
            obstacleNum = n;
            numOfPoly = p;
            polygons = new Polygon[p];
            Console.WriteLine("Obstacle " + obstacleNum + " has " + numOfPoly + " polygons.");
        }
        public void AddNumOfPolyVertice(int n, int p)
        {
            polygons[n] = new Polygon(p);
        }
        public int GetNumOfPoly()
        {
            return numOfPoly;
        }
        public int GetObstacleNum()
        {
            return obstacleNum;
        }
        public void AddPoly(float x, float y, int n, int p)
        {
            polygons[n].AddVertices(x, y, p);
        }

        public PointF[] GetPointsOnPlan(int n)
        {
            return polygons[n].GetInitPointsOnPlan();
        }
        public PointF[] GetPoints(int n)
        {
            return polygons[n].GetInitPoints();
        }
        public void AddConfiguration(float x, float y, float d)
        {
            configurations[0] = x;
            configurations[1] = y;
            configurations[2] = d;
            Console.Write("Points: ");
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].SetInitConfiguration(x, y, d, centerOnPlan);
            }
            SetCenterPointOnPlan();
            Console.WriteLine(centerOnPlan);
        }
        public Polygon[] GetPolygons()
        {
            return polygons;
        }
        public void MoveObstacle(float x, float y)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].MovePolygon(x, y, 0);
            }
            SetCenterPoint();
        }
        public void MoveObstacleOnPlan(float x, float y)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].MovePolygonOnPlan(x, y, 0);
            }
            SetCenterPointOnPlan();
        }
        public void RotateObstacle(float x)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].RotatePolygon(-x, centerPoint, 0);
            }
            SetCenterPoint();
        }
        public void RotateObstacleOnPlan(float x)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i].RotatePolygonOnPlan(x, centerOnPlan, 0);
            }
            SetCenterPointOnPlan();
        }
        public void SetCenterPoint()
        {
            float totalX = 0, totalY = 0;
            int totalVertices = 0;
            for (int i = 0; i < numOfPoly; i++)
            {
                totalVertices += polygons[i].GetNumOfVertices();
                for (int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                {
                    totalX += polygons[i].GetInitPoints()[j].X;
                    totalY += polygons[i].GetInitPoints()[j].Y;
                }
            }
            centerPoint.X = totalX / totalVertices;
            centerPoint.Y = totalY / totalVertices;
        }
        public void SetCenterPointOnPlan()
        {
            float totalX = 0, totalY = 0;
            int totalVertices = 0;
            for (int i = 0; i < numOfPoly; i++)
            {
                totalVertices += polygons[i].GetNumOfVertices();
                for (int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                {
                    totalX += polygons[i].GetInitPointsOnPlan()[j].X;
                    totalY += polygons[i].GetInitPointsOnPlan()[j].Y;
                }
            }
            centerOnPlan.X = totalX / totalVertices;
            centerOnPlan.Y = totalY / totalVertices;
        }
    }
}
