using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using grafikaPZ2.Model;

namespace grafikaPZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public struct Point
    {
        public double x;
        public double y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public partial class MainWindow : Window
    {
        enum LineType{
            Verical,
            Horizontal,
        }


        public const double minLongitude = 19.727275;
        public const double maxLongitude = 19.950944;

        public const double minLatitude = 45.189725;
        public const double maxLatitude= 45.328735;


        private static  Dictionary<long, Model.Point> ExistingPoints = new Dictionary<long, Model.Point>();

        private static Dictionary<Point, LineType> horizontalLineOnPoint = new Dictionary<Point, LineType>();
        private static Dictionary<Point, LineType> verticalLineOnPoint = new Dictionary<Point, LineType>();

        private static List<Model.Point> tackePreseka = new List<Model.Point>(); 

        public XmlEntities xmlEntities { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.xmlEntities = ParseXml();
          
            //List<SwitchEntity> SortedList = xmlEntities.Switches.OrderBy(o => o.Latitude).ToList();
            //double x = SortedList[0].Latitude;
            //List<SubstationEntity> SortedList2 = xmlEntities.Substations.OrderBy(o => o.Latitude).ToList();
            //double x2 = SortedList2[0].Latitude;

            //List<NodeEntity> SortedList3 = xmlEntities.Nodes.OrderBy(o => o.Latitude).ToList();
            //double x3 = SortedList3[0].Latitude;

        }

        private void Load_Grid(object sender, System.Windows.RoutedEventArgs e)
        {
            List<Ellipse> elipses = new List<Ellipse>();

            elipses = DrawSubstations(xmlEntities.Substations);
            foreach (var item in elipses)
            {
                mapCanvas.Children.Add(item);
            }
            elipses = DrawNodes(xmlEntities.Nodes);
            foreach (var item in elipses)
            {
                mapCanvas.Children.Add(item);
            }
            elipses = DrawSwitch(xmlEntities.Switches);
            foreach (var item in elipses)
            {
                mapCanvas.Children.Add(item);
            }
            DrawLines(xmlEntities.Lines, mapCanvas);

            foreach(var point in tackePreseka)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Height = 4;
                rectangle.Width = 4;
                rectangle.Fill = new SolidColorBrush(Colors.Black);
                rectangle.SetValue(Canvas.LeftProperty, point.X + 3);
                rectangle.SetValue(Canvas.TopProperty, point.Y + 3);
                mapCanvas.Children.Add(rectangle);
            }

        }



        public static List<Ellipse> DrawSubstations(List<SubstationEntity> substations)
        {
            List<Ellipse> subs = new List<Ellipse>();
            foreach (var item in substations)
            {
                if (ExistingPoints.ContainsKey(item.Id))
                {
                    continue;
                }

                Ellipse elipse = createNewEllipse(Colors.DarkGreen);
             
                var point= CreatePoint(item.Longitude,item.Latitude) ;

                ExistingPoints.Add(item.Id, point);

                elipse.SetValue(Canvas.LeftProperty, point.X);
                elipse.SetValue(Canvas.TopProperty, point.Y );

                elipse.ToolTip = "\tSubstation\nID: " + item.Id.ToString()+ "\nName:"+ item.Name;
                subs.Add(elipse);
            }

            return subs;
        }

        public static List<Ellipse> DrawNodes(List<NodeEntity> nodes)
        {
            List<Ellipse> subs = new List<Ellipse>();
            foreach (var item in nodes)
            {
                if (ExistingPoints.ContainsKey(item.Id))
                {
                    continue;
                }
                Ellipse elipse = createNewEllipse(Colors.Blue);

                var point = CreatePoint(item.Longitude, item.Latitude);

                ExistingPoints.Add(item.Id, point);

                elipse.SetValue(Canvas.LeftProperty, point.X);
                elipse.SetValue(Canvas.TopProperty, point.Y);

                elipse.ToolTip = "\tNode\nID: " + item.Id.ToString()+ "\nName: " + item.Name;
                subs.Add(elipse);
            }

            return subs;
        }

        public static List<Ellipse> DrawSwitch(List<SwitchEntity> switches)
        {
            List<Ellipse> subs = new List<Ellipse>();
            foreach (var item in switches)
            {
                if (ExistingPoints.ContainsKey(item.Id))
                {
                    continue;
                }
                Ellipse elipse = createNewEllipse(Colors.Red);

                var point = CreatePoint(item.Longitude, item.Latitude);

                ExistingPoints.Add(item.Id, point);

                elipse.SetValue(Canvas.LeftProperty, point.X);
                elipse.SetValue(Canvas.TopProperty, point.Y);

                elipse.ToolTip = "\tSwitch \nID: " + item.Id.ToString() + "\nName: " + item.Name + "\nStatus: " + item.Status;
                subs.Add(elipse);
            }

            return subs;
        }

        public static void DrawLines(List<Model.LineEntity> lines,Canvas canvas)
        {
            foreach (var item in lines)
            {
                if (!ExistingPoints.ContainsKey(item.FirstEnd) || !ExistingPoints.ContainsKey(item.SecondEnd))
                {
                    continue;
                }

                List<Model.Point> path = GetPointsForLine(ExistingPoints[item.FirstEnd],ExistingPoints[item.SecondEnd],canvas);

            }
        }
        public static List<Model.Point> GetPointsForLine(Model.Point startNode,Model.Point EndNode, Canvas canvas)
        {
            
            List<Model.Point> points = new List<Model.Point>();
            Model.Point currPoint = new Model.Point();
            Model.Point prevPoint = new Model.Point();
            prevPoint.X = currPoint.X = startNode.X;
            prevPoint.Y = currPoint.Y = startNode.Y;

            int step = (currPoint.X > EndNode.X) ? -10 : 10;
            while (currPoint.X != EndNode.X)
            {
                currPoint.X += step;
                if (!horizontalLineOnPoint.ContainsKey(new Point(currPoint.X, currPoint.Y)))
                {
                    horizontalLineOnPoint.Add(new Point(currPoint.X, currPoint.Y), LineType.Horizontal);
                    Line l1 = new Line();
                    l1.Stroke = Brushes.DeepSkyBlue;
                    l1.X1 = prevPoint.X + 5;
                    l1.Y1 = prevPoint.Y + 5;

                    l1.X2 = currPoint.X + 5;
                    l1.Y2 = currPoint.Y + 5;
                    l1.StrokeThickness = 2;

                    canvas.Children.Add(l1);
                }


                if (verticalLineOnPoint.ContainsKey(new Point(currPoint.X, currPoint.Y)))
                {
                    tackePreseka.Add(currPoint);

                }

                prevPoint.X = currPoint.X; 
                
            }

            step = (currPoint.Y > EndNode.Y) ? -10 : 10;
            while (currPoint.Y != EndNode.Y)
            {
                currPoint.Y+= step;
                if (!verticalLineOnPoint.ContainsKey(new Point(currPoint.X, currPoint.Y)))
                {
                    verticalLineOnPoint.Add(new Point(currPoint.X, currPoint.Y), LineType.Verical);
                    Line l1 = new Line();
                    l1.Stroke = Brushes.DeepSkyBlue;
                    l1.X1 = prevPoint.X + 5;
                    l1.Y1 = prevPoint.Y + 5;

                    l1.X2 = currPoint.X + 5;
                    l1.Y2 = currPoint.Y + 5;
                    l1.StrokeThickness = 2;

                    canvas.Children.Add(l1);
                }
                if(horizontalLineOnPoint.ContainsKey(new Point(currPoint.X, currPoint.Y))) {
                    tackePreseka.Add(currPoint);

                }
                prevPoint.Y = currPoint.Y;

            }

            return points;
        }

        private static Ellipse createNewEllipse(Color color)
        {
            Ellipse elipse = new Ellipse();
            elipse.Height = 9;
            elipse.Width = 9;
            elipse.Fill = new SolidColorBrush(color);
            
            return elipse;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Shape clickedShape = e.OriginalSource as Shape;

            if (clickedShape != null && clickedShape.GetType().Name.ToString()=="Ellipse")
            {
                DoubleAnimation widthAnimation = new DoubleAnimation
                {
                    From = 9,
                    To = 90,
                    Duration = TimeSpan.FromSeconds(1.5)
                };

                DoubleAnimation heightAnimation = new DoubleAnimation
                {
                    From = 9,
                    To = 90,
                    Duration = TimeSpan.FromSeconds(1.5)
                };
                mapCanvas.Children.Remove(clickedShape);
                mapCanvas.Children.Add(clickedShape);

                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Ellipse.WidthProperty));
                Storyboard.SetTarget(widthAnimation, clickedShape);

                Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Ellipse.HeightProperty));
                Storyboard.SetTarget(heightAnimation, clickedShape);

                Storyboard s = new Storyboard();
                s.Children.Add(widthAnimation);
                s.Children.Add(heightAnimation);

                s.Completed += (t,r) => StoryboardCompleted(clickedShape);
                s.Begin();

            }

        }
        private void StoryboardCompleted(Shape e)
        {
            DoubleAnimation myDoubleAnimation2 = new DoubleAnimation();
            myDoubleAnimation2.From =90;
            myDoubleAnimation2.To = 9;
            myDoubleAnimation2.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            e.BeginAnimation(Ellipse.WidthProperty, myDoubleAnimation2);
            e.BeginAnimation(Ellipse.HeightProperty, myDoubleAnimation2);
        }



        private static Model.Point CreatePoint(double longitude, double latitude)
        {
            double ValueoOfOneLongitude=(maxLongitude - minLongitude) / 2200; //pravimo 2200delova (Longituda) jer nam je canvas 2200x2200 
            double ValueoOfOneLatitude = (maxLatitude - minLatitude) / 2200;  //isto kao gore
    
            double x =Math.Round((longitude - minLongitude) / ValueoOfOneLongitude); // koliko longituda stane u rastojanje izmedju trenutne i minimalne longitude
            double y =Math.Round(( maxLatitude - latitude ) / ValueoOfOneLatitude);
       
            x = x - x % 10;
            y = y - y % 10;

            int cout = 0;
            
                while(true)
                {
                    if (ExistingPoints.Any(z => z.Value.X == x && z.Value.Y == y))
                    {
                        if ( cout == 0)
                        {
                            x += 10;
                            cout++;
                            continue;
                        }
                        else if (cout == 1)
                        {
                            x -= 20;
                            cout++;
                            continue;
                        }
                        else if ( cout == 2)
                        {
                            x += 10; //vraxamo x na pocetnu vrednost
                            y += 10;
                            cout++;
                            continue;
                        }
                        else if ( cout == 3)
                        {
                            y -= 20;
                            cout++;
                            continue;
                        }
                        else if ( cout == 4)
                        {
                            x += 10;
                            cout++;
                            continue;
                        }
                        else if ( cout == 5)
                        {
                            x -= 20;
                            cout++;
                            continue;
                        }
                        else if ( cout == 6)
                        {
                            y += 20;
                            cout++;
                            continue;
                        }
                        else if ( cout == 6)
                        {
                            x += 20;
                            cout++;
                            continue;
                        }
                        else
                        {
                             cout = 0;
                            continue;
                        }
                    } 
                    else
                    {
                        break;
                    }
                }

            return new Model.Point
            {
                X = x,
                Y = y,
            };
        }








        public static XmlEntities ParseXml()
        {
            var filename = "Geographic.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = System.IO.Path.Combine(currentDirectory, filename);

            StringBuilder result = new StringBuilder();

            //Load xml
            XDocument xdoc = XDocument.Load(filename);

            //Run query
            var substations = xdoc.Descendants("SubstationEntity")
                     .Select(sub => new SubstationEntity
                     {
                         Id = (long)sub.Element("Id"),
                         Name = (string)sub.Element("Name"),
                         X = (double)sub.Element("X"),
                         Y = (double)sub.Element("Y"),
                     }).ToList();

            double longit = 0;
            double latid = 0;

            foreach (var item in substations)
            {
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                item.Latitude= latid;
                item.Longitude = longit;
            }

            var nodes = xdoc.Descendants("NodeEntity")
                     .Select(node => new NodeEntity
                     {
                         Id = (long)node.Element("Id"),
                         Name = (string)node.Element("Name"),
                         X = (double)node.Element("X"),
                         Y = (double)node.Element("Y"),
                     }).ToList();

            foreach (var item in nodes)
            {
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                item.Latitude = latid;
                item.Longitude = longit;
            }

            var switches = xdoc.Descendants("SwitchEntity")
                     .Select(sw => new SwitchEntity
                     {
                         Id = (long)sw.Element("Id"),
                         Name = (string)sw.Element("Name"),
                         Status = (string)sw.Element("Status"),
                         X = (double)sw.Element("X"),
                         Y = (double)sw.Element("Y"),
                     }).ToList();

            foreach (var item in switches)
            {
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                item.Latitude = latid;
                item.Longitude = longit;
            }

            var lines = xdoc.Descendants("LineEntity")
                     .Select(line => new LineEntity
                     {
                         Id = (long)line.Element("Id"),
                         Name = (string)line.Element("Name"),
                         ConductorMaterial = (string)line.Element("ConductorMaterial"),
                         IsUnderground = (bool)line.Element("IsUnderground"),
                         R = (float)line.Element("R"),
                         FirstEnd = (long)line.Element("FirstEnd"),
                         SecondEnd = (long)line.Element("SecondEnd"),
                         LineType = (string)line.Element("LineType"),
                         ThermalConstantHeat = (long)line.Element("ThermalConstantHeat"),
                         Vertices = line.Element("Vertices").Descendants("Point").Select(p => new Model.Point
                         {
                             X = (double)p.Element("X"),
                             Y = (double)p.Element("Y"),
                         }).ToList()
                     }).ToList();

            foreach (var line in lines)
            {
                foreach (var point in line.Vertices)
                {
                    ToLatLon(point.X, point.Y, 34, out latid, out longit);
                    point.Latitude = latid;
                    point.Longitude = longit;
                }
            }

            return new XmlEntities { Substations = substations, Switches = switches, Nodes = nodes, Lines = lines };
        }


        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

    }

}
