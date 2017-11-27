using System;
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

namespace DSR_Visualisation {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            Graph g = new Graph();
            g.AddNode(1, 10, 10);
            g.AddNode(2, 20, 10);
            g.AddNode(3, 30, 10);
            g.AddNode(4, 10, 20);
            g.AddNode(5, 20, 20);
            g.AddNode(6, 20, 30);

            g.AddEdge(12, 1, 2);
            g.AddEdge(23, 2, 3);
            g.AddEdge(25, 2, 5);
            g.AddEdge(45, 4, 5);
            g.AddEdge(56, 5, 6);

            g.Draw(drawcanvas);
        }

        public class Node {
            public int id;
            public int x, y;
            List<Edge> edges;

            public Node(int id, int x, int y) {
                this.id = id;
                this.x = x;
                this.y = y;
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

            public Graph() {

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
            }

            public void Draw(Canvas canvas) {
                int mult = 9;
                foreach (Node n in nodes) {
                    Ellipse el = new Ellipse();
                    el.Width = 10;
                    el.Height = 10;
                    el.SetValue(Canvas.LeftProperty, (Double)n.x * mult);
                    el.SetValue(Canvas.TopProperty, (Double)n.y * mult);
                    el.Fill = Brushes.Red;

                    canvas.Children.Add(el);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = n.id.ToString();
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    Canvas.SetLeft(textBlock, n.x * mult - 8);
                    Canvas.SetTop(textBlock, n.y * mult - 12);

                    canvas.Children.Add(textBlock);
                }

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
            }
        }

    }
}
