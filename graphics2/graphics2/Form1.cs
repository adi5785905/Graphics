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
using System.Drawing.Drawing2D;

namespace graphics2
{
    public partial class Form1 : Form
    {
        public PictureJson picture;
        public PictureJson background;
        Brush aBrush = (Brush)Brushes.Black;
        Graphics g;
        Bitmap myBitmap;
        Color color = Color.Black;
        int frameSizex;
        int framSizey;
        private Point MouseDownLocation;
        Brush frameBrush = (Brush)Brushes.Black;
        int action = 1; // 1= move
        double PI = 3.14159265;
        public double baziaFactor = 0.0001;
        bool pressed = false;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            drawFrame();
            createTemp();
            

            draw();
            //comboBox1.Items.AddRange(new object[] {"Black",
                        //"Green",
                        //"Red",
                        //"Blue",
                        //"Yellow"});
        }

        private void drawFrame()
        {
            //setup and draw frame
            myBitmap = new Bitmap(panel1.Width, panel1.Height);
            frameSizex = panel1.Width - 1;
            framSizey = panel1.Height - 1;
            panel1.BackgroundImage = (Image)myBitmap;
            panel1.BackgroundImageLayout = ImageLayout.None;

            g = Graphics.FromImage((Image)myBitmap);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int i = 1;
            int j = 1;
            g.FillRectangle(frameBrush, i, j, 1, 1);
            for (i = 1; i < frameSizex; ++i)
            {
                g.FillRectangle(frameBrush, i, j, 1, 1);
            }
            for (j = framSizey, i = 1; i < frameSizex; ++i)
            {
                g.FillRectangle(frameBrush, i, j, 1, 1);
            }
            for (i = 1, j = 1; j < framSizey; ++j)
            {
                g.FillRectangle(frameBrush, i, j, 1, 1);
            }
            for (i = frameSizex, j = 1; j < framSizey; ++j)
            {
                g.FillRectangle(frameBrush, i, j, 1, 1);
            }
        }
        public void draw()
        {
            clear();
            // printPicture(background);
            drawFrame();
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

        public void createTemp()
        {
            picture = new PictureJson(4, 2, 0, 0, 0);
            //picture.Points[0] = new point(50, 50);
            //picture.Points[1] = new point(70, 50);
            //picture.Points[2] = new point(80, 90);
            //picture.Points[3] = new point(30, 70);
            picture.Lines[0] = new line(new point(50, 50), new point(70, 50));
            picture.Lines[1] = new line(new point(80, 90), new point(30, 70));
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
            if (pic.Lines != null)
                foreach(line l in pic.Lines)
            {
                drawLine(l.first,l.second);
            }
            if (pic.Circles != null)
                foreach (circle c in pic.Circles)
            {
                MyCircle((int)c.center.x, (int)c.center.y, (int)c.radius);
            }
            if (pic.Curves != null)
                foreach (curve c in pic.Curves)
            {
                DrawBazia(c);
            }
            if (pic.Poligon != null)
                foreach (poligon p in pic.Poligon)
            {
                myPoli(p);
            }
        }

        public PictureJson addValueToPictureJson(PictureJson pic,double calculateX, double calculateY)
        {
            if (pic.Lines != null)
                for (int i = 0; i < pic.Lines.Length; ++i)
                {

                    pic.Lines[i].first.x += calculateX;
                    pic.Lines[i].first.y += calculateY;
                    pic.Lines[i].second.x += calculateX;
                    pic.Lines[i].second.y += calculateY;
                }
            if (pic.Circles != null)
                for (int i = 0; i < pic.Circles.Length; ++i)
                {
                    pic.Circles[i].center.x += calculateX;
                    pic.Circles[i].center.y += calculateY;
                }
            if (pic.Curves != null)
                for (int i = 0; i < pic.Curves.Length; ++i)
                {
                    pic.Curves[i].first.x += calculateX;
                    pic.Curves[i].first.y += calculateY;
                    pic.Curves[i].second.x += calculateX;
                    pic.Curves[i].second.y += calculateY;
                    pic.Curves[i].thired.x += calculateX;
                    pic.Curves[i].thired.y += calculateY;
                    pic.Curves[i].fourth.x += calculateX;
                    pic.Curves[i].fourth.y += calculateY;
                }
            if (pic.Poligon != null)
                for (int i = 0; i < pic.Poligon.Length; ++i)
                {
                    pic.Poligon[i].center.x += calculateX;
                    pic.Poligon[i].center.y += calculateY;
                    pic.Poligon[i].radius.x += calculateX;
                    pic.Poligon[i].radius.y += calculateY;
                }
            return pic;
        }

        public void move(double x1 , double y1, double x2, double y2)
        {
            double calculateX = x2 - x1;
            double calculateY = y2 - y1;
            picture = addValueToPictureJson(picture,calculateX, calculateY);
            draw();
        }

        public PictureJson moveToZero(PictureJson tempPicture)
        {
            double calculateX = 0 - tempPicture.Lines[0].first.x;
            double calculateY = 0 - tempPicture.Lines[0].first.y;
            //for (int i = 0; i < tempPicture.Points.Length; ++i)
            //{
            //    tempPicture.Points[i].x += calculateX;
            //    tempPicture.Points[i].y += calculateY;
            //}
            tempPicture = addValueToPictureJson(tempPicture, calculateX, calculateY);

            return tempPicture;
        }

        public PictureJson moveBack(PictureJson tempPicture)
        {
            double calculateX = picture.Lines[0].first.x - tempPicture.Lines[0].first.x;
            double calculateY = picture.Lines[0].first.y - tempPicture.Lines[0].first.y;
            //for (int i = 0; i < tempPicture.Points.Length; ++i)
            //{
            //    tempPicture.Points[i].x += calculateX;
            //    tempPicture.Points[i].y += calculateY;
            //}
            tempPicture = addValueToPictureJson(tempPicture, calculateX, calculateY);
            return tempPicture;
        }

        public void rotate(int x, int y, int choosenX, int choosenY)
        {
            PictureJson tranform = picture;
            point refrence = picture.Lines[0].first;
            tranform = moveToZero(tranform);
            double angleInDegrees = Math.Atan2(y, x) * 180 / PI;

            //for (int i = 0; i < tranform.Points.Length; ++i)
            //{
            //    tranform.Points[i].x += (tranform.Points[i].x * Math.Cos(angleInDegrees)) - (tranform.Points[i].y * Math.Sin(angleInDegrees));
            //    tranform.Points[i].y += (tranform.Points[i].y * Math.Cos(angleInDegrees)) + (tranform.Points[i].x * Math.Sin(angleInDegrees));
            //}
            if (tranform.Lines != null)
                for (int i = 0; i < tranform.Lines.Length; ++i)
                {
                    tranform.Lines[i].first.x += (tranform.Lines[i].first.x * Math.Cos(angleInDegrees)) - (tranform.Lines[i].first.y * Math.Sin(angleInDegrees));
                    tranform.Lines[i].first.y += (tranform.Lines[i].first.y * Math.Cos(angleInDegrees)) + (tranform.Lines[i].first.x * Math.Sin(angleInDegrees));
                    tranform.Lines[i].second.x += (tranform.Lines[i].second.x * Math.Cos(angleInDegrees)) - (tranform.Lines[i].second.y * Math.Sin(angleInDegrees));
                    tranform.Lines[i].second.y += (tranform.Lines[i].second.y * Math.Cos(angleInDegrees)) + (tranform.Lines[i].second.x * Math.Sin(angleInDegrees));
                }
            if (tranform.Circles != null)
                for (int i = 0; i < tranform.Circles.Length; ++i)
                {
                    tranform.Circles[i].center.x += (tranform.Circles[i].center.x * Math.Cos(angleInDegrees)) - (tranform.Circles[i].center.y * Math.Sin(angleInDegrees));
                    tranform.Circles[i].center.y += (tranform.Circles[i].center.y * Math.Cos(angleInDegrees)) + (tranform.Circles[i].center.x * Math.Sin(angleInDegrees));
                }
            if (tranform.Curves != null)
                for (int i = 0; i < tranform.Curves.Length; ++i)
                {
                    tranform.Curves[i].first.x += (tranform.Curves[i].first.x * Math.Cos(angleInDegrees)) - (tranform.Curves[i].first.y * Math.Sin(angleInDegrees));
                    tranform.Curves[i].first.y += (tranform.Curves[i].first.y * Math.Cos(angleInDegrees)) + (tranform.Curves[i].first.x * Math.Sin(angleInDegrees));
                    tranform.Curves[i].second.x += (tranform.Curves[i].second.x * Math.Cos(angleInDegrees)) - (tranform.Curves[i].second.y * Math.Sin(angleInDegrees));
                    tranform.Curves[i].second.y += (tranform.Curves[i].second.y * Math.Cos(angleInDegrees)) + (tranform.Curves[i].second.x * Math.Sin(angleInDegrees));
                    tranform.Curves[i].thired.x += (tranform.Curves[i].thired.x * Math.Cos(angleInDegrees)) - (tranform.Curves[i].thired.y * Math.Sin(angleInDegrees));
                    tranform.Curves[i].thired.y += (tranform.Curves[i].thired.y * Math.Cos(angleInDegrees)) + (tranform.Curves[i].thired.x * Math.Sin(angleInDegrees));
                    tranform.Curves[i].fourth.x += (tranform.Curves[i].fourth.x * Math.Cos(angleInDegrees)) - (tranform.Curves[i].fourth.y * Math.Sin(angleInDegrees));
                    tranform.Curves[i].fourth.y += (tranform.Curves[i].fourth.y * Math.Cos(angleInDegrees)) + (tranform.Curves[i].fourth.x * Math.Sin(angleInDegrees));
                }
            if (tranform.Poligon != null)
                for (int i = 0; i < tranform.Poligon.Length; ++i)
                {
                    tranform.Poligon[i].center.x += (tranform.Poligon[i].center.x * Math.Cos(angleInDegrees)) - (tranform.Poligon[i].center.y * Math.Sin(angleInDegrees));
                    tranform.Poligon[i].center.y += (tranform.Poligon[i].center.y * Math.Cos(angleInDegrees)) + (tranform.Poligon[i].center.x * Math.Sin(angleInDegrees));
                    tranform.Poligon[i].radius.x += (tranform.Poligon[i].radius.x * Math.Cos(angleInDegrees)) - (tranform.Poligon[i].radius.y * Math.Sin(angleInDegrees));
                    tranform.Poligon[i].radius.y += (tranform.Poligon[i].radius.y * Math.Cos(angleInDegrees)) + (tranform.Poligon[i].radius.x * Math.Sin(angleInDegrees));
                }
            // picture = moveBack(tranform);
            move(picture.Lines[0].first.x, picture.Lines[0].first.y, refrence.x, refrence.y);
            draw();
        }



        public void scale(int newX,int newY)
        {
            PictureJson tranform = new PictureJson(picture);


            ///////////////////scale all shapes





            //to scale a line, we scale the second point by the scale factore
            if (tranform.Lines != null)
            {
                for (int i = 0; i < tranform.Lines.Length; ++i)
                {
                    //calculate scaling factor of x
                    double xScaleFactor = newX / tranform.Lines[i].second.x;
                        //(newX - tranform.Lines[i].first.x)  / (tranform.Lines[i].second.x - tranform.Lines[i].first.x);

                    //calculate scaling factor of y
                    double yScaleFactor = newY / tranform.Lines[i].second.y;

                    tranform = moveToZero(tranform);

                    tranform.Lines[i].second.x = tranform.Lines[i].second.x * xScaleFactor;
                    tranform.Lines[i].second.y = tranform.Lines[i].second.y * yScaleFactor;
                }
            }



            //to scale a circle we need to scale the radius


            picture = moveBack(tranform);
            draw();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void button1_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e)
        //{
        //    if(action == 1)
        //         move(picture.Points[0].x, picture.Points[0].y, e.X, e.Y);
        //}

        private void panel1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                pressed = true;
            }
        }

        private void panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if(pressed)
            {
                if (action == 1)
                    move(picture.Lines[0].first.x, picture.Lines[0].first.y, e.Location.X, e.Location.Y);
                if (action == 2)
                    rotate(e.Location.X, e.Location.Y, 0, 0);
                if (action == 3) { }
                   // scale(e.Location.X, e.Location.Y);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (action == 1) { }
                // move(picture.Lines[0].first.x, picture.Lines[0].first.y, MouseDownLocation.X, MouseDownLocation.Y);
                if (action == 2) { }
                    //rotate(e.Location.X, e.Location.Y, 0, 0);
                if (action == 3)
                    scale(e.Location.X, e.Location.Y);
            }
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

        public void drawLine(point one, point two)
        {
            double x1,y1,  x2, y2;
            x1 = one.x;
            y1 = one.y;
            x2 = two.x;
            y2 = two.y;
            if (x1 == x2 && y1 == y2)
            {
                g.FillRectangle(aBrush, (int)x1, (int)y1, 1, 1);
                return;
            }
            g.FillRectangle(aBrush, (int)x1, (int)y1, 1, 1);
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
                drawLine(new point(lastx, lasty), new point( px, py));
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

                drawLine(new point(px, py),new point(px2, py2));
            }
            this.Refresh();
        }

        public void clear()
        {
            panel1.BackgroundImage.Dispose();
            drawFrame();
            this.Refresh();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
