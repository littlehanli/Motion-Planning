using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Motion_Planning_1
{
    class MouseEvent
    {
        public MouseEvent()
        {
        }
        private PointF mousePoint;
        public void SetMousePoint(int x, int y)
        {
            mousePoint.X = x;
            mousePoint.Y = y;
        }
        public PointF GetMousePoint()
        {
            return mousePoint;
        }
        public bool IsPointInPolygon(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}