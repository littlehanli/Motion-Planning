using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Motion_Planning_1
{
    class ReadObstacleFile
    {
        private static string FILE_NAME = "";
        private Obstacle[] obstacles;
        private int numOfObstacle;
        public ReadObstacleFile()
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
                Console.WriteLine("ObstacleFile Start!");
                string input;
                sr.ReadLine(); // #number of obstacle

                numOfObstacle = Convert.ToInt32(sr.ReadLine());
                obstacles = new Obstacle[numOfObstacle];
                for (int i = 0; i < numOfObstacle; i++)
                {
                    sr.ReadLine(); // # obstacle0
                    sr.ReadLine(); // # number of polygons

                    int numOfPolygon = Convert.ToInt32(sr.ReadLine());
                    int numOfVertices = 0;
                    obstacles[i] = new Obstacle(numOfPolygon, i);
                    PointF[,] temps = new PointF[50,50];
                    for (int j = 0; j < numOfPolygon; j++)
                    {
                        sr.ReadLine(); // # polygon #0
                        sr.ReadLine(); // #numver of vertices

                        numOfVertices = Convert.ToInt32(sr.ReadLine());
                        obstacles[i].AddNumOfPolyVertice(j, numOfVertices);
                        sr.ReadLine(); // # vertices

                        for (int v = 0; v < numOfVertices ; v++)
                        {
                            input = sr.ReadLine();
                            string[] words = input.Split(' ');
                            temps[j,v].X = float.Parse(words[0]);
                            temps[j,v].Y = float.Parse(words[1]);
                        }
                    }
                    for (int j = 0; j < numOfPolygon; j++)
                    {
                        for (int k = 0; k < numOfVertices; k++)
                        {
                            obstacles[i].AddPoly(temps[j,k].X, temps[j,k].Y, j, k);
                        }
                    }

                    sr.ReadLine(); // # configuration
                    input = sr.ReadLine();
                    string[] configuration = input.Split(' ');
                    obstacles[i].AddConfiguration(float.Parse(configuration[0]), float.Parse(configuration[1]), float.Parse(configuration[2]));
                }
                Console.WriteLine("The end of the stream");
                sr.Close();
                return true;
            }
        }
        public Obstacle GetObstacle(int n)
        {
            return obstacles[n];
        }
        public int GetNumOfObstacle()
        {
            return numOfObstacle;
        }
        public void Clear()
        {
            for (int i = 0; i < numOfObstacle; i++)
            {
                obstacles[i] = null;
            }
            numOfObstacle = 0;
        }
    }
}
