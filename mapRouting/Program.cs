using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace mapRouting
{
    /*This class represent the nodes on the graph*/
    class point
    {
        public double x;
        public double y;
        public point()
        { }
        public point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

    }

    /*this will represent the quary 
    quary is the problem that we want to solve*/

    class quary
    {
        public double xs;
        public double ys;
        public double xd;
        public double yd;
        public double R;
        public quary()
        { }
        public quary(double xs, double ys, double xd, double yd, double R)
        {
            this.xs = xs;
            this.ys = ys;
            this.xd = xd;
            this.yd = yd;
            this.R = R;
        }
    }

    /*this class "Edge" represent roads that connect intersections "nodes" of the graph*/
    // we * time with 60 as we want to convert it form hours to minutes
    //ds in KM and speed in KM per hour 

    class edge
    {
        public int from;
        public int to;
        public double distance;
        public double speed;
        public double time;
        public edge()
        { }
        public edge(int from, int to, double dis, double s)
        {
            this.from = from;
            this.to = to;
            this.distance = dis;
            this.speed = s;
            this.time = (dis / s) * 60;
        }
        
    }
    
    /*class pair represents the intersection number and its optimal time
     (used in dijkstra algorithms with the priority queue). */

    class pair
    {
        public int Id_node;
        public double time;
        public pair() { }
        public pair(int id , double t)
        {
            this.Id_node = id;
            this.time = t;
        }
    }

    /*This class "program" contain all the functions and the main function that will solve the problem*/

    class Program
    {
        
        public static Dictionary<int, point> points; // key is the ID of the point
        public static Dictionary<int, List<edge>> map; // key is the source of the edge
        public static List<int> source_intersections;// store the intersections connected to source
        public static List<int> destination_intersections;// store the intersections connected to destination
        public const int oo = 1000000; //id for the starting node
        public const int n_oo = -1000000; //id for the end node
        public static long inner_st; //to get the execution time.
        /*This function read the map Text file witch contain the nodes and the edges*/
        //takes the file name as a parameter

        public static void read_map(string file_name)   // Overall Complexity is O(|E|).
        {
            file_name += ".txt";
            points=new Dictionary<int, point>();
            map = new Dictionary<int, List<edge>>();
            FileStream F = new FileStream(file_name, FileMode.OpenOrCreate);
            StreamReader R = new StreamReader(F);

            int n_points = int.Parse(R.ReadLine());
            for (int i=0; i<n_points;i++)        // O(|V|).
            {
                string line = R.ReadLine();
                string[] l = line.Split(' ');
                point p =new point();
                p.x = double.Parse(l[1]);
                p.y = double.Parse(l[2]);
                points.Add(int.Parse(l[0]),p);
            }

            int n_edges = int.Parse(R.ReadLine());
            for (int i=0; i<n_edges;i++)             // O(|E|).
            {
                string line = R.ReadLine();
                string[] l = line.Split(' ');
                
                int from = int.Parse(l[0]);
                int to = int.Parse(l[1]);
                double distance = double.Parse(l[2]);
                double speed = double.Parse(l[3]);

                /*here we do this as we want to add the edge twice 
                so we cheak that if it was there just insert not add another new one and forget the old one */

                if (map.ContainsKey(from))
                {
                    List<edge> temp = map[from];
                    edge temp_e = new edge(from, to, distance, speed);
                    temp.Add(temp_e);
                    map[from] = temp;
                }
                else
                {
                    List<edge> temp = new List<edge>();
                    edge temp_e = new edge(from, to, distance, speed);
                    temp.Add(temp_e);
                    map.Add(from, temp);

                } 

                if (map.ContainsKey(to))
                {
                    List<edge> temp1 = map[to];
                    edge temp_p = new edge(to, from, distance, speed);
                    temp1.Add(temp_p);
                    map[to] = temp1;
                }
                else
                {
                    List<edge> temp1 = new List<edge>();
                    edge temp_k = new edge(to, from, distance, speed);
                    temp1.Add(temp_k);
                    map.Add(to, temp1);
                }

            }
            R.Close();
        }

        // this fuction read the quary "main problem" and take file name as a parameter
        //return a list of quaries that we should solve

        public static List<quary> read_queries(string file_name)     // O(Q) Q is the # of Queries.
        {
            file_name += ".txt";
            List<quary> queries = new List<quary>();
            FileStream F = new FileStream(file_name, FileMode.OpenOrCreate);
            StreamReader R = new StreamReader(F);
            int n_queries = int.Parse(R.ReadLine());
            for (int i=0;i<n_queries;i++)                    // O(Q) Q is the # of Queries.
            {
                string line = R.ReadLine();
                string[] l = line.Split(' ');
                quary q = new quary(double.Parse(l[0]), double.Parse(l[1]), double.Parse(l[2]), double.Parse(l[3]), double.Parse(l[4]));
                queries.Add(q);
            }

            R.Close();
            return queries;
        }

        // Frist function that calc the source and distination edges that will be inserted into the graph to solve
        // insert the edge twice sorce to dis and the opposite
        public static void start_checking(quary q)  //Overall Complexity is O(|V|)
        {
            double x1 = new double();
            double y1 = new double();
            double x2 = new double();
            double y2 = new double();
            double r1 = new double();
            double x3 = new double();
            double y3 = new double();
            double x4 = new double();
            double y4 = new double();
            double r2 = new double();

            for (int i = 0; i < points.Count; i++)  //O(|V|)
            {
                x1 = Math.Abs(q.xs - points[i].x);
                y1 = Math.Abs(q.ys - points[i].y);
                x2 = x1*x1;
                y2 = y1*y1;
                r1 = Math.Sqrt(x2 + y2);
                x3 = Math.Abs(q.xd - points[i].x);
                y3 = Math.Abs(q.yd - points[i].y);
                x4 = x3*x3;
                y4 = y3*y3;
                r2 = Math.Sqrt(x4 + y4);
                if ((q.R/1000) >= r1)
                {
                    source_intersections.Add(i);
                    edge e1 = new edge(oo, i, r1, 5);
                    edge e2 = new edge(i, oo, r1, 5);
       
                    map[i].Add(e2);
                    if (map.ContainsKey(oo))
                    {
                        List<edge> temp1 = map[oo];
                        temp1.Add(e1);
                        map[oo] = temp1;
                    }
                    else
                    {
                        List<edge> temp1 = new List<edge>();
                        temp1.Add(e1);
                        map.Add(oo, temp1);
                    }
                }

                if ((q.R/1000) >= r2)
                {
                    destination_intersections.Add(i);
                    edge e1 = new edge(n_oo, i, r2, 5);
                    edge e2 = new edge(i, n_oo, r2, 5);

                    map[i].Add(e2);
                    if (map.ContainsKey(n_oo))
                    {
                        List<edge> temp1 = map[n_oo];
                        temp1.Add(e1);
                        map[n_oo] = temp1;
                    }
                    else
                    {
                        List<edge> temp1 = new List<edge>();
                        temp1.Add(e1);
                        map.Add(n_oo, temp1);
                    }
                }
            }
        }


        //delete the added nodes from the original map.
        static void reset() //Overall Complexity is O(|V|)
        {
            for (int i=0;i<source_intersections.Count;i++)  //O(|V|)
            {
                map[source_intersections[i]].RemoveAt(map[source_intersections[i]].Count - 1); //O(1)
            }   
            for (int i=0; i<destination_intersections.Count; i++)  //O(|V|)
            {
                map[destination_intersections[i]].RemoveAt(map[destination_intersections[i]].Count - 1); //O(1)
            }
            map.Remove(oo); //O(1)
            map.Remove(n_oo); //O(1)
        }


        static void dijkstra (StreamWriter writer) // Over All Complexity is O(E log|V|).
        {
            //Initialize the needed data.
            Dictionary<int, double> op_time = new Dictionary<int, double>();
            Dictionary<int, double> op_dis = new Dictionary<int, double>();
            Dictionary<int, int> path = new Dictionary<int, int>();
            List<int> shortest_path = new List<int>();


            op_time.Add(oo, 0);
            op_time.Add(n_oo, oo);
            
            op_dis.Add(oo, 0);
            op_dis.Add(n_oo, oo);
           
            path.Add(oo, -1);
            path.Add(n_oo, -1);

            for (int i = 0; i < points.Count; i++)        //O(|V|)
            {
                op_time.Add(i, oo);
                op_dis.Add(i, oo);
                path.Add(i, -1);
            }


            PriorityQueue<pair> pq = new PriorityQueue<pair>(false);
            
            pair start = new pair(oo, 0);
          
            pq.Enqueue(start);//O(log N) N is the Count of the priority queue.

            while (pq.Count != 0)  //O(E log|V|).
            {
                pair u = pq.Dequeue();

                if (op_time[u.Id_node] < u.time)
                    continue;

                for (int i = 0; i < map[u.Id_node].Count; i++) //O(# of neighbours).
                {                 
                    if (op_time[map[u.Id_node][i].to] > map[u.Id_node][i].time + op_time[u.Id_node])
                    {
                        path[map[u.Id_node][i].to] = u.Id_node;
                        op_time[map[u.Id_node][i].to] = map[u.Id_node][i].time + op_time[u.Id_node];
                        op_dis[map[u.Id_node][i].to] = map[u.Id_node][i].distance + op_dis[u.Id_node];
                        pair f = new pair(map[u.Id_node][i].to, op_time[map[u.Id_node][i].to]);
                        pq.Enqueue(f);   //O(log |v|).
                    }
                }
            }

            int k = n_oo;
            int cou = 0;

            while (path[k] != -1) //O(|V|)
            {
                if (path[k] == oo)
                    cou = k;
                shortest_path.Add(path[k]);
                k = path[k];
            }
            shortest_path.Reverse();   //O(|V|).
            double dis_sor = 0;
            double dis_dis = 0;
            for (int i = 0; i < map[oo].Count; i++)  //O(|V|).
            {
                if (map[oo][i].to == cou)
                {
                    dis_sor = map[oo][i].distance;
                    break;
                }
            }
            for (int i = 0; i < map[n_oo].Count; i++) //O(|V|).
            {
                if (map[n_oo][i].to == path[n_oo])
                {
                    dis_dis = map[n_oo][i].distance;
                    break;
                }
            }

            for (int i = 1; i < shortest_path.Count; i++) //O(|V|).
            {
                writer.Write(shortest_path[i].ToString());
                if (i != shortest_path.Count - 1)
                {
                    writer.Write(" ");
                }
            }

            writer.WriteLine();
            writer.WriteLine(Math.Round(op_time[n_oo], 2).ToString("0.00") + " mins");
            writer.WriteLine(Math.Round(op_dis[n_oo], 2).ToString("0.00") + " km");
            writer.WriteLine(Math.Round((dis_sor + dis_dis), 2).ToString("0.00") + " km");
            writer.WriteLine(Math.Round((op_dis[n_oo] - (dis_sor + dis_dis)), 2).ToString("0.00") + " km");
            
        }

        static void Main(string[] args)
        {
            map = new Dictionary<int, List<edge>>();

            Console.Write("Enter Map Name : ");
            string map_name = Console.ReadLine();
            Console.Write("Enter Query Name : ");
            string q = Console.ReadLine();


            Stopwatch outer_st = new Stopwatch();
            outer_st.Start();
            read_map(map_name);
            List<quary> queries = new List<quary>();
            string output_name = map_name + "_output.txt";
            FileStream file = new FileStream(output_name,FileMode.Create);
            StreamWriter write = new StreamWriter(file);
            
            queries = read_queries(q);
          
            for (int i=0;i<queries.Count;i++)   //overall complexity is O( Q*|E|*log|V| ).
            {   
                source_intersections = new List<int>();
                destination_intersections = new List<int>();

                Stopwatch timer = new Stopwatch();
                timer.Start();

                start_checking(queries[i]); //O(Q).

                dijkstra(write);//O(E * log|V|).

                reset();//O(V).

                timer.Stop();
                inner_st += timer.ElapsedMilliseconds;
                write.WriteLine();
            }

            outer_st.Stop();

            write.WriteLine();
            write.WriteLine(inner_st.ToString() + " ms");
            write.WriteLine();

            write.WriteLine();
            write.WriteLine(outer_st.ElapsedMilliseconds.ToString() + " ms");
            write.Close();
            file.Close();
        }
    }
}