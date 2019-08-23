using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Motion_Planning_1
{
    class Polygon
    {
        private PointF[] vertices, i_vertices, g_vertices, initVertices, goalVertices;
        private int numOfVertices;
        public Polygon(int num)
        {
            this.numOfVertices = num;
            vertices = new PointF[num];
            i_vertices = new PointF[num];
            g_vertices = new PointF[num];
            initVertices = new PointF[num];
            goalVertices = new PointF[num];
        }
        public void AddVertices(float x, float y, int p)
        {
            vertices[p].X = x;
            vertices[p].Y = y;
            //Console.WriteLine("point: " + p + " = " + vertices[p]);
        }
        
        public void SetInitConfiguration(float x,float y, float d, PointF center)
        {
            for (int i = 0; i < numOfVertices; i++)
            {
                float temp = center.X + ((vertices[i].X - center.X) * (float)Math.Cos((double)d * Math.PI / 180.0)) - (vertices[i].Y - center.Y) * (float)Math.Sin((double)d * Math.PI / 180.0);
                i_vertices[i].Y = center.Y + ((vertices[i].X - center.X) * (float)Math.Sin((double)d * Math.PI / 180.0)) + (vertices[i].Y - center.Y) * (float)Math.Cos((double)d * Math.PI / 180.0);
                i_vertices[i].X = temp;

                i_vertices[i].X += x;
                i_vertices[i].Y += y; 
            }
            for (int i = 0; i < numOfVertices; i++)
            {
                initVertices[i].X = i_vertices[i].X * (650 / 128);
                initVertices[i].Y = 650 - i_vertices[i].Y * (650 / 128);
            }
        }
        public void SetGoalConfiguration(float x, float y, float d, PointF center)
        {
            for (int i = 0; i < numOfVertices; i++)
            {
                float temp = center.X + ((vertices[i].X - center.X) * (float)Math.Cos((double)d * Math.PI / 180.0)) - (vertices[i].Y - center.Y) * (float)Math.Sin((double)d * Math.PI / 180.0);
                g_vertices[i].Y = center.Y + ((vertices[i].X - center.X) * (float)Math.Sin((double)d * Math.PI / 180.0)) + (vertices[i].Y - center.Y) * (float)Math.Cos((double)d * Math.PI / 180.0);
                g_vertices[i].X = temp;

                g_vertices[i].X += x;
                g_vertices[i].Y += y;
            }
            for (int i = 0; i < numOfVertices; i++)
            {
                goalVertices[i].X = g_vertices[i].X * (650 / 128);
                goalVertices[i].Y = 650 - g_vertices[i].Y * (650 / 128);
            }
        }
        public int GetNumOfVertices()
        {
            return numOfVertices;
        }
        public PointF[] GetInitPointsOnPlan()
        {
            return i_vertices;
        }
        public PointF[] GetGoalPointsOnPlan()
        {
            return g_vertices;
        }
        public PointF[] GetInitPoints()
        {
            return initVertices;
        }
        public PointF[] GetGoalPoints()
        {
            return goalVertices;
        }
        
        public void MovePolygon(float x, float y,int IorG)
        {
            if (IorG == 0)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    initVertices[i].X += x;
                    initVertices[i].Y += y;
                }
            }
            else if (IorG == 1)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    goalVertices[i].X += x;
                    goalVertices[i].Y += y;
                }
            }
        }
        public void MovePolygonOnPlan(float x, float y, int IorG)
        {
            if (IorG == 0)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    i_vertices[i].X += x;
                    i_vertices[i].Y -= y;
                }
            }
            else if (IorG == 1)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    g_vertices[i].X += x;
                    g_vertices[i].Y -= y;
                }
            }
        }
        
        public void RotatePolygon(float x, PointF center, int IorG)
        {
            if (IorG == 0)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    float temp = center.X + ((initVertices[i].X - center.X) * (float)Math.Cos((double)x * Math.PI / 180.0)) - (initVertices[i].Y - center.Y) * (float)Math.Sin((double)x * Math.PI / 180.0);
                    initVertices[i].Y = center.Y + ((initVertices[i].X - center.X) * (float)Math.Sin((double)x * Math.PI / 180.0)) + (initVertices[i].Y - center.Y) * (float)Math.Cos((double)x * Math.PI / 180.0);
                    initVertices[i].X = temp;
                }
            }
            else if(IorG == 1)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    float temp = center.X + ((goalVertices[i].X - center.X) * (float)Math.Cos((double)x * Math.PI / 180.0)) - (goalVertices[i].Y - center.Y) * (float)Math.Sin((double)x * Math.PI / 180.0);
                    goalVertices[i].Y = center.Y + ((goalVertices[i].X - center.X) * (float)Math.Sin((double)x * Math.PI / 180.0)) + (goalVertices[i].Y - center.Y) * (float)Math.Cos((double)x * Math.PI / 180.0);
                    goalVertices[i].X = temp;
                }
            }
            
        }
        public void RotatePolygonOnPlan(float x, PointF center, int IorG)
        {
            if (IorG == 0)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    float temp = center.X + ((i_vertices[i].X - center.X) * (float)Math.Cos((double)x * Math.PI / 180.0)) - (i_vertices[i].Y - center.Y) * (float)Math.Sin((double)x * Math.PI / 180.0);
                    i_vertices[i].Y = center.Y + ((i_vertices[i].X - center.X) * (float)Math.Sin((double)x * Math.PI / 180.0)) + (i_vertices[i].Y - center.Y) * (float)Math.Cos((double)x * Math.PI / 180.0);
                    i_vertices[i].X = temp;
                }
            }
            else if (IorG == 1)
            {
                for (int i = 0; i < numOfVertices; i++)
                {
                    float temp = center.X + ((g_vertices[i].X - center.X) * (float)Math.Cos((double)x * Math.PI / 180.0)) - (g_vertices[i].Y - center.Y) * (float)Math.Sin((double)x * Math.PI / 180.0);
                    g_vertices[i].Y = center.Y + ((g_vertices[i].X - center.X) * (float)Math.Sin((double)x * Math.PI / 180.0)) + (g_vertices[i].Y - center.Y) * (float)Math.Cos((double)x * Math.PI / 180.0);
                    g_vertices[i].X = temp;
                }
            }
        }
        public Polygon ShallowCopy()
        {
            return (Polygon)this.MemberwiseClone();
        }
    }
}
