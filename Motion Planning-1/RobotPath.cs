using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Motion_Planning_1
{
    class RobotPath
    {
        private PointF plan_c, canvas_c;
        private Polygon[] polygons;
        private Point[] control_p;
        private int numOfPoly;

        public RobotPath(int num, Polygon[] p, PointF centerP, Point[] controlP)
        {
            this.numOfPoly = num;
            this.polygons = new Polygon[numOfPoly];
            for (int i = 0; i < numOfPoly; i++)
            {
                polygons[i] = new Polygon(p[i].GetNumOfVertices());
                for(int j = 0; j < p[i].GetNumOfVertices(); j++)
                {
                    polygons[i].AddVertices(p[i].GetInitPointsOnPlan()[j].X,p[i].GetInitPointsOnPlan()[j].Y,j);
                }
                polygons[i].SetInitConfiguration(0, 0, 0, centerP);
            }
            plan_c.X = centerP.X;
            plan_c.Y = centerP.Y;
            control_p = new Point[controlP.Length];
            for(int i = 0; i < controlP.Length; i++)
            {
                control_p[i].X = controlP[i].X;
                control_p[i].Y = controlP[i].Y;
            }
        }
        public Polygon[] GetTransfer(Point pathC)
        {
            Console.WriteLine("Move: " + plan_c + "-->" + pathC);
            return TransferVertices(plan_c.X - pathC.X, plan_c.Y - pathC.Y);
        }
        public Polygon[] TransferVertices(float x, float y)
        {
            for (int i = 0; i < numOfPoly; i++)
            {
                Console.WriteLine("Before Transfer: " + polygons[i].GetInitPointsOnPlan()[0]);
                polygons[i].MovePolygonOnPlan(-x, y, 0);
                polygons[i].MovePolygon(-x * (650 / 128), y * (650 / 128), 0);
                Console.WriteLine("After Transfer: " + polygons[i].GetInitPointsOnPlan()[0]);
            }
            return polygons;
        }
        public PointF GetPlanCenter()
        {
            return plan_c;
        }
        public int GetNumOfPoly()
        {
            return numOfPoly;
        }
        public Polygon[] GetPlanPoly()
        {
            return polygons;
        }
        private void canvasPointCenter()
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
            canvas_c.X = totalX / totalVertices;
            canvas_c.Y = totalY / totalVertices;
        }
        private PointF planPointCenter(Polygon[] poly)
        {
            float totalX = 0, totalY = 0;
            int totalVertices = 0;
            PointF tmp = new PointF();
            for (int i = 0; i < numOfPoly; i++)
            {
                totalVertices += poly[i].GetNumOfVertices();
                for (int j = 0; j < poly[i].GetNumOfVertices(); j++)
                {
                    totalX += poly[i].GetInitPointsOnPlan()[j].X;
                    totalY += poly[i].GetInitPointsOnPlan()[j].Y;
                }
            }
            tmp.X = totalX / totalVertices;
            tmp.Y = totalY / totalVertices;
            //Console.WriteLine("center Point On Plan: " + plan_c);
            return tmp;
        }
        public bool IsCollision(Obstacle[] obstacles)
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
                                if (isLineIntersectAnObstacle(polygons[p].GetInitPointsOnPlan()[j], polygons[p].GetInitPointsOnPlan()[k], obstacles[i]) == true)
                                {
                                    Console.WriteLine("====================== obstacle: " + i );
                                    return true;
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
            for (int p = 0; p < o.GetNumOfPoly(); p++)
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
        private bool isLineIntersectLine(PointF p1, PointF q1, PointF p2, PointF q2)
        {
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;
            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;
            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;
            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;
            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;
            return false; // Doesn't fall in any of the above cases 
        }
        private int orientation(PointF p, PointF q, PointF r)
        {

            float val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0;  // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }
        private bool onSegment(PointF p, PointF q, PointF r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }
        public bool MoveWithCenter(Point c, Obstacle[] o)
        {
            Polygon[] temps = new Polygon[numOfPoly];
            PointF center = new PointF();
            for(int i = 0; i < numOfPoly; i++)
            {
                temps[i] = polygons[i];
            }
            center = planPointCenter(temps);
            float move_x = c.X - center.X;
            float move_y = c.Y - center.Y;

            for (int i = 0; i < numOfPoly; i++)
            {
                for(int j = 0; j < polygons[i].GetNumOfVertices(); j++)
                {
                    temps[i].GetInitPointsOnPlan()[j].X += move_x;
                    temps[i].GetInitPointsOnPlan()[j].Y += move_y;
                }
            }
            Console.WriteLine("Move: " + move_x + "," + move_y + " => " + center);

            bool collision = false;
            Console.WriteLine("robot is at : " + center);
            if (IsCollision(o) == true)
            {
                collision = true;
            }
            return !collision;
        }
    }
}
