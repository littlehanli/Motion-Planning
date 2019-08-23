using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Motion_Planning_1
{
    class ReadRobotFile
    {
        private static string FILE_NAME = "";
        private Robot[] robots;
        private int numOfRobot;

        public ReadRobotFile()
        {
        }
        public bool IORead(string fileName)
        {
            FILE_NAME = fileName;
            if (!File.Exists(FILE_NAME))
            {
                Console.WriteLine("{0} does not exist!", FILE_NAME);
                Console.ReadLine();
                return false;
            }
            using (StreamReader sr = File.OpenText(FILE_NAME))
            {
                Console.WriteLine("RobotFile Start!");
                string input;
                sr.ReadLine(); // #number of robot
                numOfRobot = Convert.ToInt32(sr.ReadLine());
                robots = new Robot[numOfRobot];
                for (int i = 0; i < numOfRobot; i++)
                {
                    sr.ReadLine(); // # robot0
                    sr.ReadLine(); // # number of polygons

                    int numOfPolygon = Convert.ToInt32(sr.ReadLine());
                    int numOfVertices = 0;
                    robots[i] = new Robot(numOfPolygon, i);
                    PointF[,] temps = new PointF[50, 50];
                    for (int j = 0; j < numOfPolygon; j++)
                    {
                        sr.ReadLine(); // # polygon #0
                        sr.ReadLine(); // # numver of vertices

                        numOfVertices = Convert.ToInt32(sr.ReadLine());
                        robots[i].AddNumOfPolyVertice(j, numOfVertices);
                        sr.ReadLine(); // # vertices

                        for (int v = 0; v < numOfVertices; v++)
                        {
                            input = sr.ReadLine();
                            string[] words = input.Split(' ');
                            temps[j, v].X = float.Parse(words[0]);
                            temps[j, v].Y = float.Parse(words[1]);
                        }
                    }
                    for (int j = 0; j < numOfPolygon; j++)
                    {
                        for (int k = 0; k < numOfVertices; k++)
                        {
                            robots[i].AddPoly(temps[j,k].X, temps[j,k].Y, j, k);
                        }
                    }

                    sr.ReadLine(); // # initial configuration
                    input = sr.ReadLine();
                    string[] initialConfiguration = input.Split(' ');
                    robots[i].AddInitConfiguration(float.Parse(initialConfiguration[0]), float.Parse(initialConfiguration[1]), float.Parse(initialConfiguration[2]));

                    sr.ReadLine(); // # goal configuration
                    input = sr.ReadLine();
                    string[] goalConfiguration = input.Split(' ');
                    robots[i].AddGoalConfiguration(float.Parse(goalConfiguration[0]), float.Parse(goalConfiguration[1]), float.Parse(goalConfiguration[2]));

                    sr.ReadLine(); // # number of control points
                    int numOfControl = Convert.ToInt32(sr.ReadLine());
                    robots[i].SetNumControlPoints(numOfControl);
                    for (int c = 0; c < numOfControl; c++)
                    {
                        sr.ReadLine(); // # control point c
                        input = sr.ReadLine();
                        string[] controlP = input.Split(' ');
                        robots[i].AddControlP(float.Parse(controlP[0]), float.Parse(controlP[1]), c);
                    }
                }
                Console.WriteLine("The end of the stream");
                sr.Close();
                return true;
            }
        }
        public int GetNumOfRobots()
        {
            return numOfRobot;
        }
        public Robot GetRobot(int n)
        {
            return robots[n];
        }
        public void Clear()
        {
            for (int i = 0; i < numOfRobot; i++)
            {
                robots[i] = null;
            }
            numOfRobot = 0;
        }
    }
}
