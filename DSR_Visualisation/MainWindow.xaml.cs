using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSR_Visualisation {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private DispatcherTimer timer;
        Ellipse ellipse = null;
        int loopCounter;
        Graph g;

        public MainWindow() {
            InitializeComponent();

            g = new Graph();
            g.AddNode(1, 10, 10);
            g.AddNode(2, 20, 10);
            g.AddNode(3, 30, 10);
            g.AddNode(4, 10, 20);
            g.AddNode(5, 20, 20);
            g.AddNode(6, 20, 30);
            g.AddNode(7, 30, 30);
            g.AddNode(8, 20, 40);

            g.AddEdge(12, 1, 2);
            g.AddEdge(23, 2, 3);
            g.AddEdge(25, 2, 5);
            g.AddEdge(45, 4, 5);
            g.AddEdge(56, 5, 6);
            g.AddEdge(67, 6, 7);
            g.AddEdge(68, 6, 8);
            //g.AddEdge(14, 1, 4);


            g.Set(1, 4);
            g.DSR();
            g.Draw(drawcanvas);
            q = g.queue;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); //Set the interval period here.
            timer.Tick += timer1_Tick;
        }

        double dx = 0, dy = 0;

        private void timer1_Tick(object sender, EventArgs e) {
            //Remove the previous ellipse from the paint canvas.
            drawcanvas.Children.Remove(ellipse);

            if (--loopCounter == 0)
                timer.Stop();
            
            
            //Add the ellipse to the canvas
            ellipse = CreateAnEllipse(12, 12);
            drawcanvas.Children.Add(ellipse);


            if ((g.FindNode(sec).x - g.FindNode(first).x) > 0)
                dx=dx+2;
            else if ((g.FindNode(sec).x - g.FindNode(first).x) < 0)
                dx=dx - 2;
            else
                dx = 0;

            if ((g.FindNode(sec).y - g.FindNode(first).y) > 0)
                dy = dy + 2;
            else if ((g.FindNode(sec).y - g.FindNode(first).y) < 0)
                dy = dy - 2;
            else
                dy = 0;

            Canvas.SetLeft(ellipse, g.FindNode(first).x * 13 + dx );
            Canvas.SetTop(ellipse,  g.FindNode(first).y * 13 + dy );

            Console.WriteLine(dx + " " + dy);
        }

        public Ellipse CreateAnEllipse(int height, int width) {
            SolidColorBrush fillBrush = new SolidColorBrush() { Color = Colors.Red };
            SolidColorBrush borderBrush = new SolidColorBrush() { Color = Colors.Black };

            return new Ellipse() {
                Height = height,
                Width = width,
                StrokeThickness = 1,
                Stroke = borderBrush,
                Fill = fillBrush
            };
        }

        private void drawcanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            
        }

        int first, sec;
        List<int> q;
        private void Button_Click(object sender, RoutedEventArgs e) {
            int counter = 0;
            foreach (var i in q) {
                counter++;
                if (counter == 1)
                    first = i;
                if (counter == 2) {
                    sec = i;
                    
                    q.RemoveAt(0);
                    q.RemoveAt(0);

                    if (i == g.tagetID) {
                        q.Clear();
                        q.AddRange(g.path);
                    }
                    break;
                }
            }

            Console.Write(first + " " + sec);

            loopCounter = 5 * 13;
            timer.Start();

            dx = 0; dy = 0;
        }


        public class Node {
            public int id;
            public int x, y;
            public int deep = -1;
            public List<Edge> edges = new List<Edge>();

            public Node(int id, int x, int y) {
                this.id = id;
                this.x = x;
                this.y = y;
            }

            public Node() {

            }

            public void AddEdge(int id, Node from, Node to) {
                Edge edge = new Edge(id, from, to);
                this.edges.Add(edge);
            }

            public void AddEdge(Edge edge) {
                this.edges.Add(edge);
            }
        }

        public class Edge {
            int id;
            public Node from, to;

            public Edge(int id, Node from, Node to) {
                this.id = id;
                this.from = from;
                this.to = to;
            }
        }

        public class Graph {
            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();

            public int fromID, toID, tagetID;
            public List<int> queue = new List<int>();
            public List<int> path = new List<int>();
            
            int maxdeep;

            public Graph() {

            }

            public void Set(int from, int to) {
                this.fromID = from;
                this.toID = to;
                this.tagetID = toID;
            }

            public void DSR() {
                Node node = FindNode(fromID);
                DSR_Core(node);
                FindPath();

                foreach(var i in queue) {
                    Console.Write(i + " ");
                }
                Console.WriteLine();
                foreach (var i in path) {
                    Console.Write(i + " ");
                }
            }

            public void FindPath() {
                int d = 0;
                Node first = new Node(), second = new Node();
                while (fromID != toID) {
                    foreach (var i in queue) {
                        if (d % 2 == 0) {
                            first = FindNode(i);
                        } else {
                            second = FindNode(i);
                        }

                        if (first.id == toID && first.deep > second.deep) {
                            path.Add(first.id);
                            path.Add(second.id);
                            toID = second.id;
                        }
                        d++;
                    }
                }
            }

            public void DSR_Core(Node node) {
                int deep = 0;
                node.deep = deep;
                while (!AllMarkered()) {
                    foreach(Node n in nodes) {
                        if (n.deep == deep) {
                            List<int> q = new List<int>();
                            List<int> r = new List<int>();
                            foreach (Edge e in n.edges) {
                                if (e.to.deep == -1) {
                                    e.to.deep = deep + 1;
                                    q.Add(n.id);
                                    q.Add(e.to.id);
                                    r.Add(e.to.id);
                                    r.Add(n.id);
                                }
                            }
                            queue.AddRange(q);
                            queue.AddRange(r);
                        }
                    }
                    deep++;
                    maxdeep = deep;
                }
            }

            public bool AllMarkered() {
                foreach(Node n in nodes) {
                    if (n.deep == -1) {
                        return false;
                    }
                }
                return true;
            }

            public void AddNode(int id, int x, int y) {
                Node node = new Node(id, x, y);
                this.nodes.Add(node);
            }

            public Node FindNode(int id) {
                foreach (Node n in nodes) {
                    if (n.id == id)
                        return n;
                }
                return null;
            }

            public void AddEdge(int id, int from, int to) {
                Edge edge = new Edge(id,
                                     this.FindNode(from),
                                     this.FindNode(to));
                edges.Add(edge);
                this.FindNode(from).AddEdge(edge);

                Edge edge2 = new Edge(id,
                                     this.FindNode(to),
                                     this.FindNode(from));
                edges.Add(edge2);
                this.FindNode(to).AddEdge(edge2);
            }

            public void Draw(Canvas canvas) {
                int mult = 13;
                foreach (Edge n in edges) {
                    int shift = 5;

                    Line line = new Line();

                    line.Stroke = new SolidColorBrush(Colors.Red);
                    line.StrokeThickness = 2;
                    line.X1 = n.from.x * mult + shift;
                    line.Y1 = n.from.y * mult + shift;
                    line.X2 = n.to.x * mult + shift;
                    line.Y2 = n.to.y * mult + shift;

                    canvas.Children.Add(line);


                }
                foreach (Node n in nodes) {
                    Ellipse el = new Ellipse();
                    el.Width = 10;
                    el.Height = 10;
                    el.SetValue(Canvas.LeftProperty, (Double)n.x * mult);
                    el.SetValue(Canvas.TopProperty, (Double)n.y * mult);
                    if (n.id == fromID)
                        el.Fill = Brushes.Green;
                    else if (n.id == tagetID)
                        el.Fill = Brushes.Blue;
                    else
                        el.Fill = Brushes.Orange;

                    canvas.Children.Add(el);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = n.id.ToString();
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    Canvas.SetLeft(textBlock, n.x * mult - 8);
                    Canvas.SetTop(textBlock, n.y * mult - 12);

                    canvas.Children.Add(textBlock);

                    TextBlock textBlock2 = new TextBlock();
                    textBlock2.Text = n.deep.ToString();
                    textBlock2.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    Canvas.SetLeft(textBlock2, n.x * mult - 8);
                    Canvas.SetTop(textBlock2, n.y * mult + 10);

                    canvas.Children.Add(textBlock2);

                    //foreach(Edge e in n.edges) {
                    //     Console.Write(e.from.id + " " + e.to.id + ":");

                    //}Console.WriteLine();
                }

                
            }
        }

        
    }
}
