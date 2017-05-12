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
        int action = 0; // 1= move
        double PI = 3.14159265;
        public double baziaFactor = 0.0001;

        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
        }

        public void draw()
        {
            clear();
           // printPicture(background);
            printPicture(picture);
        }

        public void OpenFile()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("JsonFile.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    picture = parseJson(line,"PictureJson");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The picture file could not be read:");
                Console.WriteLine(e.Message);
            }

            //try
            //{   // Open the text file using a stream reader.
            //    using (StreamReader sr = new StreamReader("BackgroundFile.txt"))
            //    {
            //        // Read the stream to a string, and write the string to the console.
            //        String line = sr.ReadToEnd();
            //        picture = parseJson(line,"PictureJson");
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("The background file could not be read:");
            //    Console.WriteLine(e.Message);
            //}
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
            using (StreamWriter file = File.CreateText(@"\" + fileName + ".json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                json.WriteTo(writer);

            }
        }

        public PictureJson parseJson(string jsonString, string name)
        {
            var root = JObject.Parse(jsonString);
            var serializer = new JsonSerializer();
            PictureJson userObject = serializer.Deserialize<PictureJson>(root[name].CreateReader());
            return userObject;
        }
        
        public void printPicture(PictureJson pic)
        {
            foreach(line l in pic.Lines)
            {
                drawLine((int)l.first.x, (int)l.first.y, (int)l.second.x, (int)l.second.y);
            }
            foreach(circle c in pic.Circles)
            {
                MyCircle((int)c.center.x, (int)c.center.y, (int)c.radius);
            }
            foreach(curve c in pic.Curves)
            {
                DrawBazia(c);
            }
            foreach (poligon p in pic.Poligon)
            {
                myPoli(p);
            }
        }

        public void move(double x1 , double y1, double x2, double y2)
        {
            double calculateX = x2 - x1;
            double calculateY = y2 - y1;
            for(int i = 0; i < picture.Points.Length; ++i)
            {
                picture.Points[i].x += calculateX;
                picture.Points[i].y += calculateY;
            }
            draw();
        }

        public PictureJson moveToZero(PictureJson tempPicture)
        {
            double calculateX = 0 - tempPicture.Points[0].x;
            double calculateY = 0 - tempPicture.Points[0].y;
            for (int i = 0; i < tempPicture.Points.Length; ++i)
            {
                tempPicture.Points[i].x += calculateX;
                tempPicture.Points[i].y += calculateY;
            }
            return tempPicture;
        }

        public PictureJson moveBack(PictureJson tempPicture)
        {
            double calculateX = picture.Points[0].x - tempPicture.Points[0].x;
            double calculateY = picture.Points[0].y - tempPicture.Points[0].y;
            for (int i = 0; i < tempPicture.Points.Length; ++i)
            {
                tempPicture.Points[i].x += calculateX;
                tempPicture.Points[i].y += calculateY;
            }
            return tempPicture;
        }

        public void rotate(int x, int y, int choosenX, int choosenY)
        {
            PictureJson tranform = picture;
            tranform = moveToZero(tranform);
            double angleInDegrees = Math.Atan2(y, x) * 180 / PI;
            for(int i = 0; i < tranform.Points.Length; ++i)
            {
                tranform.Points[i].x = (tranform.Points[i].x * Math.Cos(angleInDegrees)) - (tranform.Points[i].y * Math.Sin(angleInDegrees));
                tranform.Points[i].y = (tranform.Points[i].y * Math.Cos(angleInDegrees)) + (tranform.Points[i].x * Math.Sin(angleInDegrees));
            }
            picture = moveBack(tranform);
            draw();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e)
        {
            if(action == 1)
                 move(picture.Points[0].x, picture.Points[0].y, e.X, e.Y);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.AutoCheck) action = 1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.AutoCheck) action = 2;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.AutoCheck) action = 3;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.AutoCheck) action = 4;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.AutoCheck) action = 5;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.AutoCheck) action = 6;
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

            for (t = 0.0; t <= 1.0; t += baziaFactor)
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

        private void groupBox1_Enter(object sender, EventArgs e)
        {
           
        }
    }
}
