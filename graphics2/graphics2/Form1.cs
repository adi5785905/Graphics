using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace graphics2
{
    public partial class Form1 : Form
    {
        public PictureJson picture;
        public PictureJson background;
        Brush aBrush = (Brush)Brushes.Black;
        Graphics g;
        public double baziaFactor = 0.0001;

        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        public JObject createPictureJson(PictureJson json)
        {
            JObject jsonObject = (JObject)JToken.FromObject(json);
            return jsonObject;
        }

        public void saveJsonFile(JObject json, string fileName)
        {
            //string jsonFile = JsonConvert.SerializeObject(json);

            //write string to file
            //System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + fileName, jsonFile);
            using (StreamWriter file = File.CreateText((Environment.SpecialFolder.Desktop) + @"\" + fileName + ".json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                json.WriteTo(writer);

            }
        }

        private void createBackPictureJson()
        {

        }

        public PictureJson parseJson(string jsonString)
        {
            var root = JObject.Parse(jsonString);
            var serializer = new JsonSerializer();
            PictureJson userObject = serializer.Deserialize<PictureJson>(root["user"].CreateReader());
            return userObject;
        }

        public void printPicture()
        {
            foreach(line l in picture.Lines)
            {
                drawLine((int)l.first.x, (int)l.first.y, (int)l.second.x, (int)l.second.y);
            }
            foreach(circle c in picture.Circles)
            {
                MyCircle((int)c.center.x, (int)c.center.y, (int)c.radius);
            }
            foreach(curve c in picture.Curves)
            {
                DrawBazia(c);
            }
            foreach (poligon p in picture.Poligon)
            {
                myPoli(p);
            }
        }

        public void move(float x1 , float y1, float x2, float y2)
        {
            float calculateX =x2 - x1;
            float calculateY = y2 - y1;
            for(int i = 0; i < picture.Points.Length; ++i)
            {
                picture.Points[i].x += calculateX;
                picture.Points[i].y += calculateY;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }






        public double getRadius(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public double getDeltaX(double x1, double x2, double y1, double y2, double range)
        {
            double delta = x2 - x1;
            return delta / range;
        }

        public double getDeltaY(double x1, double x2, double y1, double y2, double range)
        {
            double delta = y2 - y1;
            return delta / range;
        }

        public void drawLine(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                g.FillRectangle(aBrush, x1, y1, 1, 1);
                return;
            }
            g.FillRectangle(aBrush, x1, y1, 1, 1);
            double newX = x2;
            double newY = y2;

            g.FillRectangle(aBrush, (int)newX, (int)newY, 1, 1);
            double x = x1;
            double y = y1;
            int i = 0;
            double steps;
            if (Math.Abs(newX - x) > Math.Abs(newY - y))
            {
                steps = Math.Abs(newX - x);
            }
            else steps = Math.Abs(newY - y);
            double deltax = getDeltaX(x1, x2, y1, y2, steps);
            double deltay = getDeltaY(x1, x2, y1, y2, steps);
            for (i = 0; i <= steps; ++i)
            {
                x = x + deltax;
                y = y + deltay;

                g.FillRectangle(aBrush, (int)x, (int)y, 1, 1);
            }
        }

        public void MyCircle(int xc, int yc, int r)
        {
            //calculate 1/8 of the circle
            int px = 0;
            int py = r;
            int p = 3 - 2 * r;

            try
            {
                while (py >= px)
                {
                    g.FillRectangle(aBrush, xc - px, yc - py, 1, 1);
                    g.FillRectangle(aBrush, xc - py, yc - px, 1, 1);
                    g.FillRectangle(aBrush, xc + py, yc - px, 1, 1);
                    g.FillRectangle(aBrush, xc + px, yc - py, 1, 1);
                    g.FillRectangle(aBrush, xc - px, yc + py, 1, 1);
                    g.FillRectangle(aBrush, xc - py, yc + px, 1, 1);
                    g.FillRectangle(aBrush, xc + py, yc + px, 1, 1);
                    g.FillRectangle(aBrush, xc + px, yc + py, 1, 1);

                    if (p < 0) p += 4 * px++ + 6;
                    else p += 4 * (px++ - py--) + 10;
                }
            }
            catch
            {
                clear();
            }
        }

        public void myPoli(poligon p)
        {
            int xc = (int)p.center.x;
            int yc = (int)p.center.y;
            int px = (int)p.radius.x;
            int py = (int)p.radius.y;
            int n = p.polis;
            double PI = 3.14159265;
            int lastx, lasty;
            double r = getRadius(xc, yc, px, py);
            double a = Math.Atan2(py - yc, px - xc);

            for (int i = 1; i <= n; i++)
            {
                lastx = px; lasty = py;
                a = a + PI * 2 / n;
                px = (int)Math.Round(xc + r * Math.Cos(a));
                py = (int)Math.Round(yc + r * Math.Sin(a));
                drawLine(lastx, lasty, px, py);
            }
        }

        public void DrawBazia(curve Curve)
        {
            int px;
            int py;
            int px2;
            int py2;
            double t; // t is 1.0/the number of wanted line, calculated in the mouseClick function
            Point[] bazia = new Point[4];
            bazia[0] = new Point((int)Curve.first.x,(int)Curve.first.y);
            bazia[1] = new Point((int)Curve.second.x, (int)Curve.second.y);
            bazia[2] = new Point((int)Curve.thired.x, (int)Curve.thired.y);
            bazia[3] = new Point((int)Curve.fourth.x, (int)Curve.fourth.y);
            //clean the bazia choise
            //baziaButton = false;
            //baziaPara = 0;
            //textBox5.Clear();
            //textBox4.Clear();
            //textBox3.Clear();
            //textBox2.Clear();

            for (t = 0.0; t <= 1.0; t += 0.001)
            {
                px = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].X + 3 * t * (1 - t) * (1 - t) * bazia[1].X + 3 * t * t * (1 - t) * bazia[2].X + t * t * t * bazia[3].X);
                py = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].Y + 3 * t * (1 - t) * (1 - t) * bazia[1].Y + 3 * t * t * (1 - t) * bazia[2].Y + t * t * t * bazia[3].Y);
                t += baziaFactor; //incress t to get the second point
                px2 = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].X + 3 * t * (1 - t) * (1 - t) * bazia[1].X + 3 * t * t * (1 - t) * bazia[2].X + t * t * t * bazia[3].X);
                py2 = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].Y + 3 * t * (1 - t) * (1 - t) * bazia[1].Y + 3 * t * t * (1 - t) * bazia[2].Y + t * t * t * bazia[3].Y);

                drawLine(px, py, px2, py2);
            }
            this.Refresh();
        }

        public void clear()
        {
            panel1.BackgroundImage.Dispose();
            //draw();
            this.Refresh();
        }
    }
}
