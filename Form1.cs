using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace Tìm_đường_đi_ngắn_nhất_bằng_Dijkstra
{
    public partial class Form1 : Form
    {
        private Graph graph;
        public Form1()
        {
            InitializeComponent();
            graph = new Graph();
            InitLocations();
            InitEdges();
            pictureBox1.Paint += PictureBoxMap_Paint;
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        public class Location
        {
            public string Name;
            public double X1, Y1, X2, Y2;
            public Location(string name, double x1, double y1, double x2, double y2)
            {
                Name = name;
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
            }
        }

        public class Edge
        {
            public string Start {  get; set; }
            public string End {  get; set; }
          
            public Edge(string start, string end)
            {
               Start = start;
                End = end;
            }
        }

        public class Node
        {
            public object Data;
            public Node Next;
            public Node Prev;
            public Node(object data)
            {
                Data = data;
                Next = null;
                Prev = null;
            }
        }

        public class LinkedList
        {
            public Node Head;
            public int Count;
            public LinkedList() { Head = null; Count = 0; }
            public void AddToFront(string data)
            {
                Node newNode = new Node(data);
                if (Head == null)
                {
                    Head = newNode;
                }
                else
                {
                    newNode.Next = Head;
                    Head = newNode;
                }
            }

            public void Add(Object data)
            {
                Node newNode = new Node(data);
                if (Head == null)
                    Head = newNode;
                else
                {
                    Node current = Head;
                    while (current.Next != null)
                        current = current.Next;
                    current.Next = newNode;
                }
                Count++;
            }
        }

        public class DoubleLinkedList
        {
            public Node Head;
            public Node Tail;
            public int Count;

            public DoubleLinkedList()
            {
                Head = null;
                Tail = null;
                Count = 0;
            }

            public void Add(Object data)
            {
                Node newNode = new Node(data);
                if (Head == null)
                {
                    Head = Tail = newNode;
                }
                else
                {
                    Tail.Next = newNode;
                    newNode.Prev = Tail;
                    Tail = newNode;
                }
                Count++;
            }
        }

        public class Vertex
        {
            public string Name;
            public double X1, Y1, X2, Y2;
            public bool Visited;
            public Vertex(Location location)
            {
                Name = location.Name;
                X1 = location.X1;
                Y1 = location.Y1;
                X2 = location.X2;
                Y2 = location.Y2;
                Visited = false;
            }
        }
        public class HeapNode
        {
            public int Index;
            public double Distance;

            public HeapNode(int index, double distance)
            {
                Index = index;
                Distance = distance;
            }
        }
        public class Graph
        {
            public DoubleLinkedList Vertices;
            public LinkedList Edges;
            private double[,] adjMatrix;

            public Graph()
            {
                Vertices = new DoubleLinkedList();
                Edges = new LinkedList();
                adjMatrix = new double[50, 50];
                for (int i = 0; i < 50; i++)
                    for (int j = 0; j < 50; j++)
                        adjMatrix[i, j] = double.MaxValue;
            }

            public void AddVertex(Location loc)
            {
                Vertices.Add(new Vertex(loc));
            }

            public void RemoveVertex(string name)
            {
                Vertices.Head = RemoveNode(Vertices.Head, name);
            }

            private Node RemoveNode(Node head, string name)
            {
                if (head == null) return null;
                if (head.Data is Vertex vertex && vertex.Name == name) return head.Next;
                head.Next = RemoveNode(head.Next, name);
                return head;
            }

            public void AddEdge(string start,string  end)
            {
                Vertex v1 = FindVertex(start);
                Vertex v2 = FindVertex(end);
                if (v1 == null || v2 == null) return;
                double distance = Euclid(v1, v2);
                Edges.Add(new Edge(start,end));
                int i = GetVertexIndex(start);
                int j = GetVertexIndex(end);
                adjMatrix[i, j] = distance;
                adjMatrix[j, i] = distance;
            }

            public Vertex FindVertex(string name)
            {
                Node current = Vertices.Head;
                while (current != null)
                {
                    if (current.Data is Vertex vertex && vertex.Name == name)
                        return vertex; 
                    current = current.Next;
                }
                return null;
            }
            public string GetVertexName(int index)
            {
                Node current = Vertices.Head;
                int i = 0;
                while (current != null)
                {
                    if (i == index && current.Data is Vertex vertex)
                        return vertex.Name;
                    current = current.Next;
                    i++;
                }
                return null; 
            }
            public int GetVertexIndex(string name)
            {
                Node current = Vertices.Head;
                int index = 0;
                while (current != null)
                {
                    if (current.Data is Vertex vertex && vertex.Name == name)
                        return index;
                    current = current.Next;
                    index++;
                }
                return -1;
            }

            private double Euclid(Vertex v1, Vertex v2)
            {
                double dx = (v2.X1 - v1.X1) * 109.57;
                double dy = (v2.Y1 - v1.Y1) * 111.32;
                return Sqrt(dx * dx + dy * dy);
            }

            private double Sqrt(double number)
            {
                if (number == 0) return 0;
                double guess = number;
                double epsilon = 0.00001;

                while (true)
                {
                    double newGuess = (guess + number / guess) / 2;
                    if (Abs(newGuess, guess) < epsilon)
                        return newGuess;
                    guess = newGuess;
                }
            }

            private double Abs(double a, double b)
            {
                return (a > b) ? a - b : b - a;
            }
            public LinkedList Dijkstra(string startName, string endName)
            {
                int n = 50;
                double[] distances = new double[n];
                bool[] visited = new bool[n];
                int[] previous = new int[n];

                for (int i = 0; i < n; i++)
                {
                    distances[i] = double.MaxValue;
                    previous[i] = -1;
                }

                int startIndex = GetVertexIndex(startName);
                int endIndex = GetVertexIndex(endName);
                if (startIndex == -1 || endIndex == -1) return new LinkedList();

                distances[startIndex] = 0;

                BinaryMinHeap heap = new BinaryMinHeap(n); 
                heap.Add(startIndex, 0);

                while (!heap.IsEmpty())
                {
                    HeapNode minNode = heap.ExtractMin(); 
                    int u = minNode.Index;

                    if (visited[u]) continue;
                    visited[u] = true;

                    for (int v = 0; v < n; v++)
                    {
                        if (!visited[v] && adjMatrix[u, v] != double.MaxValue)
                        {
                            double newDist = distances[u] + adjMatrix[u, v];
                            if (newDist < distances[v])
                            {
                                distances[v] = newDist;
                                previous[v] = u;
                                heap.Add(v, newDist);
                            }
                        }
                    }
                }

                return BuildPath(previous, endIndex);
            }

            public class BinaryMinHeap
            {
                private HeapNode[] heap;
                private int size;

                public BinaryMinHeap(int capacity)
                {
                    heap = new HeapNode[capacity];
                    size = 0;
                }

                public void Add(int index, double distance)
                {
                    HeapNode node = new HeapNode(index, distance);
                    heap[size] = node;
                    HeapifyUp(size);
                    size++;
                }

                public HeapNode ExtractMin()
                {
                    if (size == 0) return null;
                    HeapNode min = heap[0];
                    heap[0] = heap[size - 1];
                    size--;
                    HeapifyDown(0);
                    return min;
                }

                public void Update(int index, double newDistance)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (heap[i].Index == index)
                        {
                            if (newDistance < heap[i].Distance)
                            {
                                heap[i].Distance = newDistance;
                                HeapifyUp(i);
                            }
                            break;
                        }
                    }
                }

                public bool IsEmpty()
                {
                    return size == 0;
                }

                private void HeapifyUp(int i)
                {
                    while (i > 0)
                    {
                        int parent = (i - 1) / 2;
                        if (heap[i].Distance < heap[parent].Distance)
                        {
                            Swap(i, parent);
                            i = parent;
                        }
                        else break;
                    }
                }

                private void HeapifyDown(int i)
                {
                    while (true)
                    {
                        int left = 2 * i + 1;
                        int right = 2 * i + 2;
                        int smallest = i;

                        if (left < size && heap[left].Distance < heap[smallest].Distance)
                            smallest = left;
                        if (right < size && heap[right].Distance < heap[smallest].Distance)
                            smallest = right;

                        if (smallest != i)
                        {
                            Swap(i, smallest);
                            i = smallest;
                        }
                        else break;
                    }
                }

                private void Swap(int i, int j)
                {
                    HeapNode temp = heap[i];
                    heap[i] = heap[j];
                    heap[j] = temp;
                }
            }


            private LinkedList BuildPath(int[] previous, int endIndex)
            {
                LinkedList path = new LinkedList();
                int current = endIndex;
                while (current != -1)
                {
                    Vertex vertex = FindVertexByIndex(current);
                    if (vertex != null)
                        path.Add(vertex);
                    current = previous[current];
                }
                return path;
            }

            public Vertex FindVertexByIndex(int index)
            {
                Node current = Vertices.Head;
                int i = 0;
                while (current != null)
                {
                    if (i == index) return (Vertex)current.Data;
                    current = current.Next;
                    i++;
                }
                return null;
            }
            public double CalculatePathDistance(LinkedList path)
            {
                double totalDistance = 0;
                Node current = path.Head;

                while (current != null && current.Next != null)
                {
                    if (current.Data is Vertex v1 && current.Next.Data is Vertex v2)
                    {
                        totalDistance += Euclid(v1, v2);
                    }
                    current = current.Next;
                }

                return totalDistance;
            }
        }
        private void InitLocations()
        {
            DoubleLinkedList locations = new DoubleLinkedList();
            locations.Add(new Vertex(new Location("Đầm Sen", 10.768544, 106.636138, 84, 131)));
            locations.Add(new Vertex(new Location("Hồ Con Rùa", 10.782631, 106.695896, 255, 135)));
            locations.Add(new Vertex(new Location("Nhà Hát TP.HCM", 10.776601, 106.703061, 477, 211)));
            locations.Add(new Vertex(new Location("Chợ Bến Thành", 10.772548, 106.697988, 292, 228)));
            locations.Add(new Vertex(new Location("Dinh Độc Lập", 10.777099, 106.695302, 264, 183)));
            locations.Add(new Vertex(new Location("Bưu Điện TP.HCM", 10.779912, 106.699969, 329, 162)));
            locations.Add(new Vertex(new Location("Bến Nhà Rồng", 10.768254, 106.706832, 392, 255)));
            locations.Add(new Vertex(new Location("Thảo Cầm Viên", 10.787476, 106.705075, 456, 184)));
            locations.Add(new Vertex(new Location("Landmark 81", 10.795132, 106.722085, 564, 203)));
            locations.Add(new Vertex(new Location("Nhà thờ Đức Bà", 10.779911, 106.699029, 361, 199)));
            locations.Add(new Vertex(new Location("Phố đi bộ Nguyễn Huệ", 10.774252, 106.703596, 408, 226)));
            locations.Add(new Vertex(new Location("Bảo tàng Chứng tích Chiến tranh", 10.779647, 106.692102, 330, 127)));
            locations.Add(new Vertex(new Location("Công viên Tao Đàn", 10.774695, 106.692485, 195, 204)));
            locations.Add(new Vertex(new Location("Sân bay Tân Sơn Nhất", 10.817732, 106.658842, 324, 97)));
            locations.Add(new Vertex(new Location("Bến xe miền Tây", 10.741126, 106.619142, 70, 188)));
            locations.Add(new Vertex(new Location("Bến xe chợ Lớn", 10.751391, 106.651627, 142, 160)));
            locations.Add(new Vertex(new Location("Bến xe miền Đông", 10.814796, 106.711835, 492, 250)));

            Node current = locations.Head;
            while (current != null)
            {
                if (current.Data is Vertex vertex)
                {
                    graph.AddVertex(new Location(vertex.Name, vertex.X1, vertex.Y1, vertex.X2, vertex.Y2));
                    comboBox1.Items.Add(vertex.Name);
                    comboBox2.Items.Add(vertex.Name);
                }
                current = current.Next;
            }
            pictureBox1.Invalidate();
        }
        private void InitEdges()
        {
           LinkedList<Edge> edges = new LinkedList<Edge>();
            graph.AddEdge("Đầm Sen", "Bến xe miền Tây");
            graph.AddEdge("Đầm Sen", "Bến xe chợ Lớn");
            graph.AddEdge("Đầm Sen", "Hồ Con Rùa");
            graph.AddEdge("Bến xe miền Tây", "Bến xe chợ Lớn");
            graph.AddEdge("Bến xe chợ Lớn", "Công viên Tao Đàn");
            graph.AddEdge("Phố đi bộ Nguyễn Huệ", "Bến Nhà Rồng");
            graph.AddEdge("Bến xe chợ Lớn", "Dinh Độc Lập");
            graph.AddEdge("Công viên Tao Đàn", "Dinh Độc Lập");
            graph.AddEdge("Công viên Tao Đàn", "Chợ Bến Thành");
            graph.AddEdge("Dinh Độc Lập", "Bảo tàng Chứng tích Chiến tranh");
            graph.AddEdge("Dinh Độc Lập", "Hồ Con Rùa");
            graph.AddEdge("Công viên Tao Đàn", "Bưu Điện TP.HCM");
            graph.AddEdge("Chợ Bến Thành", "Nhà thờ Đức Bà");
            graph.AddEdge("Chợ Bến Thành", "Phố đi bộ Nguyễn Huệ");
            graph.AddEdge("Chợ Bến Thành", "Dinh Độc Lập");
            graph.AddEdge("Hồ Con Rùa", "Bảo tàng Chứng tích Chiến tranh");
            graph.AddEdge("Hồ Con Rùa", "Bưu Điện TP.HCM");
            graph.AddEdge("Sân bay Tân Sơn Nhất", "Bảo tàng Chứng tích Chiến tranh");
            graph.AddEdge("Nhà thờ Đức Bà", "Bưu Điện TP.HCM");
            graph.AddEdge("Nhà thờ Đức Bà", "Thảo Cầm Viên");
            graph.AddEdge("Nhà thờ Đức Bà", "Phố đi bộ Nguyễn Huệ");
            graph.AddEdge("Phố đi bộ Nguyễn Huệ", "Nhà Hát TP.HCM");
            graph.AddEdge("Phố đi bộ Nguyễn Huệ", "Bến xe miền Đông");
            graph.AddEdge("Nhà Hát TP.HCM", "Landmark 81");
            graph.AddEdge("Nhà Hát TP.HCM", "Thảo Cầm Viên");
            graph.AddEdge("Bến xe miền Đông", "Landmark 81");
            graph.AddEdge("Thảo Cầm Viên", "Landmark 81");
            graph.AddEdge("Thảo Cầm Viên", "Bảo tàng Chứng tích Chiến tranh");
            graph.AddEdge("Bưu Điện TP.HCM", "Bảo tàng Chứng tích Chiến tranh");
            var current = edges.First;
            while (current != null)
            {
                int start = graph.GetVertexIndex(current.Value.Start);
                int end = graph.GetVertexIndex(current.Value.End);
                if (start != -1 && end != -1)
                {
                    graph.AddEdge(start.ToString(), end.ToString());
                }
                current = current.Next;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            

        }
        private void PictureBoxMap_Paint(object sender, PaintEventArgs e)
        {
           
            if (graph == null || graph.Edges == null || graph.Vertices == null) return;
            Graphics g = e.Graphics;

           
            Node currentEdge = graph.Edges.Head;
            while (currentEdge != null)
            {
                if (currentEdge.Data is Edge edge)
                {
                    var v1 = graph.FindVertex(edge.Start);
                    var v2 = graph.FindVertex(edge.End);
                    if (v1 != null && v2 != null)
                    {
                        g.DrawLine(Pens.Black, (float)v1.X2, (float)v1.Y2, (float)v2.X2, (float)v2.Y2);
                    }
                }
                currentEdge = currentEdge.Next;
            }

            int start = comboBox1.SelectedIndex;
            int end = comboBox2.SelectedIndex;

            
            Node currentVertex = graph.Vertices.Head;
            while (currentVertex != null)
            {
                if (currentVertex.Data is Vertex vertex)
                {
                    Brush brush = Brushes.Red;
                    int vertexIndex = graph.GetVertexIndex(vertex.Name);
                    if (vertexIndex == start)
                    {
                        brush = Brushes.Blue;
                    }
                    else if (vertexIndex == end)
                    {
                        brush = Brushes.DarkRed;
                    }
                    g.FillEllipse(brush, (float)vertex.X2 - 5, (float)vertex.Y2 - 5, 10, 10);
                    g.DrawString(vertex.Name, DefaultFont, Brushes.Black, (float)vertex.X2 + 5, (float)vertex.Y2 + 5);
                }
                currentVertex = currentVertex.Next;
            }

            if (start == -1 || end == -1) return;
            LinkedList path = graph.Dijkstra(graph.GetVertexName(start), graph.GetVertexName(end));
            if (path == null || path.Head == null) return;

            Node currentPathNode = path.Head;
            while (currentPathNode != null && currentPathNode.Next != null)
            {
                if (currentPathNode.Data is Vertex currentVertexPath && currentPathNode.Next.Data is Vertex nextVertexPath)
                {
                    int pathIndex = graph.GetVertexIndex(currentVertexPath.Name);
                    int nextPathIndex = graph.GetVertexIndex(nextVertexPath.Name);
                    var current = graph.FindVertexByIndex(pathIndex);
                    var next = graph.FindVertexByIndex(nextPathIndex);
                    if (current != null && next != null)
                    {
                        g.DrawLine(new Pen(Color.Red, 2), (float)current.X2, (float)current.Y2, (float)next.X2, (float)next.Y2);
                    }
                }
                currentPathNode = currentPathNode.Next;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int start = comboBox1.SelectedIndex;
            int end = comboBox2.SelectedIndex;

            if (start == -1 || end == -1 || start == end)
            {
                MessageBox.Show("Vui lòng chọn điểm đi và điểm đến hợp lệ!");
                return;
            }

            LinkedList path = graph.Dijkstra(graph.GetVertexName(start), graph.GetVertexName(end));
            if (path == null || path.Head == null)
            {
                textBox1.Text = "Không tìm thấy đường đi!";
                return;
            }

           
            LinkedList reversedPath = new LinkedList();

          
            Node current = path.Head;
            while (current != null)
            {
                if (current.Data is Vertex vertex)
                {
                   
                    reversedPath.AddToFront(vertex.Name);
                }
                current = current.Next;
            }

        
            string result = "Đường đi: ";
            Node reversedCurrent = reversedPath.Head;
            while (reversedCurrent != null)
            {
                if (reversedCurrent.Data is string vertexName)
                {
                    result += vertexName;
                    if (reversedCurrent.Next != null) result += " -> ";
                }
                reversedCurrent = reversedCurrent.Next;
            }

            double totalDistance = graph.CalculatePathDistance(path);
            result += $" (Khoảng cách: ~{totalDistance:F2} km)";

            textBox1.Text = result;
            pictureBox1.Invalidate();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable table = new DataTable();
            table.Columns.Add("FROM", typeof(string));
            table.Columns.Add("TO", typeof(string));
            table.Columns.Add("KM", typeof(double));

            table.Rows.Add("Đầm Sen", "Bến xe miền Tây", 3.55);
            table.Rows.Add("Đầm Sen", "Bến xe chợ Lớn", 2.55);
            table.Rows.Add("Đầm Sen", "Hồ Con Rùa", 6.83);
            table.Rows.Add("Bến xe miền Tây", "Bến xe chợ Lớn", 3.79);
            table.Rows.Add("Bến xe chợ Lớn", "Công viên Tao Đàn", 5.22);
            table.Rows.Add("Phố đi bộ Nguyễn Huệ", "Bến Nhà Rồng", 0.75);
            table.Rows.Add("Bến xe chợ Lớn", "Dinh Độc Lập", 5.62);
            table.Rows.Add("Công viên Tao Đàn", "Dinh Độc Lập", 0.41);
            table.Rows.Add("Công viên Tao Đàn", "Chợ Bến Thành", 0.66);
            table.Rows.Add("Dinh Độc Lập", "Bảo tàng Chứng tích Chiến tranh",0.45);
            table.Rows.Add("Dinh Độc Lập", "Hồ Con Rùa", 0.61);
            table.Rows.Add("Công viên Tao Đàn", "Bưu Điện TP.HCM", 1.01);
            table.Rows.Add("Chợ Bến Thành", "Nhà thờ Đức Bà", 0.82);
            table.Rows.Add("Chợ Bến Thành", "Phố đi bộ Nguyễn Huệ", 0.65);
            table.Rows.Add("Chợ Bến Thành", "Dinh Độc Lập", 0.58);
            table.Rows.Add("Hồ Con Rùa", "Bảo tàng Chứng tích Chiến tranh", 0.53);
            table.Rows.Add("Hồ Con Rùa", "Bưu Điện TP.HCM", 0.54);
            table.Rows.Add("Sân bay Tân Sơn Nhất", "Bảo tàng Chứng tích Chiến tranh", 5.58);
            table.Rows.Add("Nhà thờ Đức Bà", "Bưu Điện TP.HCM", 0.1);
            table.Rows.Add("Nhà thờ Đức Bà", "Thảo Cầm Viên", 1.07);
            table.Rows.Add("Nhà thờ Đức Bà", "Phố đi bộ Nguyễn Huệ", 0.8);
            table.Rows.Add("Phố đi bộ Nguyễn Huệ", "Nhà Hát TP.HCM", 0.26);
            table.Rows.Add("Phố đi bộ Nguyễn Huệ", "Bến xe miền Đông", 4.54);
            table.Rows.Add("Nhà Hát TP.HCM", "Landmark 81", 2.93);
            table.Rows.Add("Nhà Hát TP.HCM", "Thảo Cầm Viên", 1.21);
            table.Rows.Add("Bến xe miền Đông", "Landmark 81", 2.44);
            table.Rows.Add("Thảo Cầm Viên", "Landmark 81", 2.07);
            table.Rows.Add("Thảo Cầm Viên", "Bảo tàng Chứng tích Chiến tranh", 1.68);
            table.Rows.Add("Bưu Điện TP.HCM", "Bảo tàng Chứng tích Chiến tranh", 0.88);

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoGenerateColumns = true;

            dataGridView1.DataSource = table;
        }
    }
}
