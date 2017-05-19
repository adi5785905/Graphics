///Graphics 2
///Gabriel ___, Adi Gonen and Liron Sharabi
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
        public PictureJson backgroundPicture;
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
        public double baziaFactor = 0.00001;
        bool pressed = false;
        point centerPoint;
        double maxX = double.MinValue;
        double maxY = double.MinValue;
        double minX = double.MaxValue;
        double minY = double.MaxValue;
        public point absCenter;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            //set window size
            this.Size = new Size(1100, 650);
            absCenter = new point(panel1.Width / 2, panel1.Height / 2);
            base.OnLoad(e);
            drawFrame();
            OpenFile();
            OpenBackGroundFile();
            //set defult centerPoint
            if (picture.Lines != null)
            {
                centerPoint = new point(picture.Lines[0].first.x, picture.Lines[0].first.y);
            }
            else if (picture.Circles != null)
            {
                centerPoint = new point(picture.Circles[0].center);
            }
            else if (picture.Curves != null)
            {
                centerPoint = new point(picture.Curves[0].first.x, picture.Curves[0].first.y);
            }
            else if (picture.Poligon != null)
            {
                centerPoint = new point(picture.Poligon[0].center);
            }
            radioButton1.Text = "Move";
            radioButton2.Text = "Rotate";
            radioButton3.Text = "Scale out";
            radioButton10.Text = "Scale in";
            radioButton5.Text = "Shear X";
            radioButton9.Text = "Shear Y";
            radioButton6.Text = "Choose Center";
            radioButton7.Text = "Mirror X";
            radioButton8.Text = "Mirror Y";
            draw();
            comboBox1.Text = "Choose color";
            comboBox1.Items.AddRange(new object[] {"Black",
                        "Green",
                        "Red",
                        "Blue",
                        "Yellow"});
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
            printPicture(backgroundPicture);
        }

        public void OpenFile()
        {

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("JsonFile.json"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();

                    JToken root = JObject.Parse(line);
                    JToken pictureJson = root["pictureJson"];
                    picture = JsonConvert.DeserializeObject<PictureJson>(pictureJson.ToString()); //creating PictureJson obj and setting it in the var
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The picture file could not be read:");
                Console.WriteLine(e.Message);
            }
        }


        public void OpenBackGroundFile()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("backgroundJsonFile.json"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();

                    JToken root = JObject.Parse(line);
                    JToken pictureJson = root["pictureJson"];
                    backgroundPicture = JsonConvert.DeserializeObject<PictureJson>(pictureJson.ToString()); //creating PictureJson obj and setting it in the var
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The picture file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public JObject createPictureJson(PictureJson json)
        {
            JObject jsonObject = (JObject)JToken.FromObject(json);
            return jsonObject;
        }

        public void saveJsonFile(JObject json, string fileName)
        {
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
            PictureJson userObject = serializer.Deserialize<PictureJson>(root.CreateReader());
            return userObject;
        }
        
        public void printPicture(PictureJson pic)
        {
            //call each array of shaps in pic and draw by type
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
            //Add calculateX to each x and calculateY to each y to each point in the picture shaps
            centerPoint.x += calculateX;
            centerPoint.y += calculateY;
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
            //move the entire picture from point (x1,y1) to point (x2,y2)
            double calculateX = x2 - x1;
            double calculateY = y2 - y1;
            picture = addValueToPictureJson(picture,calculateX, calculateY);
            draw();
        }

        public PictureJson moveToZero(PictureJson tempPicture)
        {
            //move the entire picture to point (0,0)
            double calculateX = 0 - centerPoint.x;
            double calculateY = 0 - centerPoint.y;
            tempPicture = addValueToPictureJson(tempPicture, calculateX, calculateY);

            return tempPicture;
        }

        public void rotate(double x, double y, double choosenX, double choosenY,double angle)
        {
            //rotate picture "angle" angles around choosen point (choosenX,choosenY)
            PictureJson tranform = picture;
            //get the angle in radians
            double angleInRad = angle / 180.0 * Math.PI;
            //pre calculate the cos and sin
            double cos = Math.Cos(angleInRad);
            double sin = Math.Sin(angleInRad);
            centerPoint.x = (((centerPoint.x - choosenX) * cos) - ((centerPoint.y - choosenY) * sin) + choosenX);
            centerPoint.y = (((centerPoint.y - choosenY) * cos) + ((centerPoint.x - choosenX) * sin) + choosenY);
            if (tranform.Lines != null)
                for (int i = 0; i < tranform.Lines.Length; ++i)
                {
                    tranform.Lines[i].first.x = (((tranform.Lines[i].first.x - choosenX) * cos) - ((tranform.Lines[i].first.y - choosenY) * sin)+ choosenX);
                    tranform.Lines[i].first.y = (((tranform.Lines[i].first.y - choosenY) * cos) + ((tranform.Lines[i].first.x - choosenX) * sin)+ choosenY);
                    tranform.Lines[i].second.x = (((tranform.Lines[i].second.x - choosenX) * cos) - ((tranform.Lines[i].second.y - choosenY) * sin)+ choosenX);
                    tranform.Lines[i].second.y = (((tranform.Lines[i].second.y - choosenY) * cos) + ((tranform.Lines[i].second.x - choosenX) * sin)+ choosenY);
                }
            if (tranform.Circles != null)
                for (int i = 0; i < tranform.Circles.Length; ++i)
                {
                    tranform.Circles[i].center.x = (((tranform.Circles[i].center.x - choosenX) * cos) - ((tranform.Circles[i].center.y - choosenY) * sin) + choosenX);
                    tranform.Circles[i].center.y = (((tranform.Circles[i].center.y - choosenY) * cos) + ((tranform.Circles[i].center.x - choosenX) * sin) + choosenY);
                }
            if (tranform.Curves != null)
                for (int i = 0; i < tranform.Curves.Length; ++i)
                {
                    tranform.Curves[i].first.x = (((tranform.Curves[i].first.x - choosenX) * cos) - ((tranform.Curves[i].first.y - choosenY) * sin) + choosenX);
                    tranform.Curves[i].first.y = (((tranform.Curves[i].first.y - choosenY) * cos) + ((tranform.Curves[i].first.x - choosenX) * sin) + choosenY);
                    tranform.Curves[i].second.x = (((tranform.Curves[i].second.x - choosenX) * cos) - ((tranform.Curves[i].second.y - choosenY) * sin) + choosenX);
                    tranform.Curves[i].second.y = (((tranform.Curves[i].second.y - choosenY) * cos) + ((tranform.Curves[i].second.x - choosenX) * sin) + choosenY);
                    tranform.Curves[i].thired.x = (((tranform.Curves[i].thired.x - choosenX) * cos) - ((tranform.Curves[i].thired.y - choosenY) * sin) + choosenX);
                    tranform.Curves[i].thired.y = (((tranform.Curves[i].thired.y - choosenY) * cos) + ((tranform.Curves[i].thired.x - choosenX) * sin) + choosenY);
                    tranform.Curves[i].fourth.x = (((tranform.Curves[i].fourth.x - choosenX) * cos) - ((tranform.Curves[i].fourth.y - choosenY) * sin) + choosenX);
                    tranform.Curves[i].fourth.y = (((tranform.Curves[i].fourth.y - choosenY) * cos) + ((tranform.Curves[i].fourth.x - choosenX) * sin) + choosenY);
                }
            if (tranform.Poligon != null)
                for (int i = 0; i < tranform.Poligon.Length; ++i)
                {
                    tranform.Poligon[i].center.x = (((tranform.Poligon[i].center.x - choosenX) * cos) - ((tranform.Poligon[i].center.y - choosenY) * sin) + choosenX);
                    tranform.Poligon[i].center.y = (((tranform.Poligon[i].center.y - choosenY) * cos) + ((tranform.Poligon[i].center.x - choosenX) * sin) + choosenY);
                    tranform.Poligon[i].radius.x = (((tranform.Poligon[i].radius.x - choosenX) * cos) - ((tranform.Poligon[i].radius.y - choosenY) * sin) + choosenX);
                    tranform.Poligon[i].radius.y = (((tranform.Poligon[i].radius.y - choosenY) * cos) + ((tranform.Poligon[i].radius.x - choosenX) * sin) + choosenY);
                }
            draw();
        }

        public void mirroringX()
        {
            //mirror the picture around X by switching each value with it's negetive one
            point refrence = centerPoint;
            centerPoint.x = 0-centerPoint.x;
            centerPoint.y = 0-centerPoint.y;
            if (picture.Lines != null)
                for (int i = 0; i < picture.Lines.Length; ++i)
                {
                    picture.Lines[i].first.x = 0-picture.Lines[i].first.x;
                    picture.Lines[i].first.y = 0-picture.Lines[i].first.y;
                    picture.Lines[i].second.x = 0-picture.Lines[i].second.x;
                    picture.Lines[i].second.y = 0-picture.Lines[i].second.y;
                }
            if (picture.Circles != null)
                for (int i = 0; i < picture.Circles.Length; ++i)
                {
                    picture.Circles[i].center.x = 0-picture.Circles[i].center.x;
                    picture.Circles[i].center.y = 0-picture.Circles[i].center.y;
                }
            if (picture.Curves != null)
                for (int i = 0; i < picture.Curves.Length; ++i)
                {
                    picture.Curves[i].first.x = 0-picture.Curves[i].first.x;
                    picture.Curves[i].first.y = 0-picture.Curves[i].first.y;
                    picture.Curves[i].second.x = 0-picture.Curves[i].second.x;
                    picture.Curves[i].second.y = 0-picture.Curves[i].second.y;
                    picture.Curves[i].thired.x = 0-picture.Curves[i].thired.x;
                    picture.Curves[i].thired.y = 0-picture.Curves[i].thired.y;
                    picture.Curves[i].fourth.x = 0-picture.Curves[i].fourth.x;
                    picture.Curves[i].fourth.y = 0-picture.Curves[i].fourth.y;
                }
            if (picture.Poligon != null)
                for (int i = 0; i < picture.Poligon.Length; ++i)
                {
                    picture.Poligon[i].center.x = 0- picture.Poligon[i].center.x;
                    picture.Poligon[i].center.y = 0- picture.Poligon[i].center.y;
                    picture.Poligon[i].radius.x = 0- picture.Poligon[i].radius.x;
                    picture.Poligon[i].radius.y = 0- picture.Poligon[i].radius.y;
                }
            move(centerPoint.x, centerPoint.y, refrence.x, refrence.y);
            draw();
        }

        public void mirroringY()
        {
            //mirror the picture around Y by switching each x value with it's negetive one
            point refrence = centerPoint;
            centerPoint.x = 0 - centerPoint.x;
            if (picture.Lines != null)
                for (int i = 0; i < picture.Lines.Length; ++i)
                {
                    picture.Lines[i].first.x = 0 - picture.Lines[i].first.x;
                    picture.Lines[i].second.x = 0 - picture.Lines[i].second.x;
                }
            if (picture.Circles != null)
                for (int i = 0; i < picture.Circles.Length; ++i)
                {
                    picture.Circles[i].center.x = 0 - picture.Circles[i].center.x;
                }
            if (picture.Curves != null)
                for (int i = 0; i < picture.Curves.Length; ++i)
                {
                    picture.Curves[i].first.x = 0 - picture.Curves[i].first.x;
                    picture.Curves[i].second.x = 0 - picture.Curves[i].second.x;
                    picture.Curves[i].thired.x = 0 - picture.Curves[i].thired.x;
                    picture.Curves[i].fourth.x = 0 - picture.Curves[i].fourth.x;
                }
            if (picture.Poligon != null)
                for (int i = 0; i < picture.Poligon.Length; ++i)
                {
                    picture.Poligon[i].center.x = 0 - picture.Poligon[i].center.x;
                    picture.Poligon[i].radius.x = 0 - picture.Poligon[i].radius.x;
                }
            move(centerPoint.x, centerPoint.y, refrence.x, refrence.y);
            draw();
        }

        public void shearY(double disY)
        {
            //shear along the Y using y'=y+x*distanceY;
            disY = disY / 1000;
            point refrence = new point(centerPoint.x, centerPoint.y);
            moveToZero(picture);
            centerPoint.x = centerPoint.x + centerPoint.y * disY;
            if (picture.Lines != null)
                for (int i = 0; i < picture.Lines.Length; ++i)
                {
                    picture.Lines[i].first.y = picture.Lines[i].first.y + picture.Lines[i].first.x * disY;
                    picture.Lines[i].second.y = picture.Lines[i].second.y + picture.Lines[i].second.x * disY;
                }
            if (picture.Circles != null)
                for (int i = 0; i < picture.Circles.Length; ++i)
                {
                    picture.Circles[i].center.y = picture.Circles[i].center.y + picture.Circles[i].center.x * disY;
                }
            if (picture.Curves != null)
                for (int i = 0; i < picture.Curves.Length; ++i)
                {
                    picture.Curves[i].first.y = picture.Curves[i].first.y + picture.Curves[i].first.x * disY;
                    picture.Curves[i].second.y = picture.Curves[i].second.y + picture.Curves[i].second.x * disY;
                    picture.Curves[i].thired.y = picture.Curves[i].thired.y + picture.Curves[i].thired.x * disY;
                    picture.Curves[i].fourth.y = picture.Curves[i].fourth.y + picture.Curves[i].fourth.x * disY;
                }
            if (picture.Poligon != null)
                for (int i = 0; i < picture.Poligon.Length; ++i)
                {
                    picture.Poligon[i].center.y = picture.Poligon[i].center.y + picture.Poligon[i].center.x * disY;
                    picture.Poligon[i].radius.y = picture.Poligon[i].radius.y + picture.Poligon[i].radius.x * disY;
                }
            move(centerPoint.x, centerPoint.y, refrence.x, refrence.y);
            draw();
        }

        public void shearX(double disX)
        {
            //shear along the X using x'=x+y*distanceX;
            disX = disX / 1000;
            point refrence = new point(centerPoint.x,centerPoint.y);
            moveToZero(picture);
            centerPoint.x = centerPoint.x + centerPoint.y * disX;
            // centerPoint.y = 0 - centerPoint.y;
            if (picture.Lines != null)
                for (int i = 0; i < picture.Lines.Length; ++i)
                {
                    picture.Lines[i].first.x = picture.Lines[i].first.x + picture.Lines[i].first.y*disX;
                    // picture.Lines[i].first.y = 0 - picture.Lines[i].first.y;
                    picture.Lines[i].second.x = picture.Lines[i].second.x + picture.Lines[i].second.y * disX;
                    // picture.Lines[i].second.y = 0 - picture.Lines[i].second.y;
                    
                }
            if (picture.Circles != null)
                for (int i = 0; i < picture.Circles.Length; ++i)
                {
                    picture.Circles[i].center.x = picture.Circles[i].center.x + picture.Circles[i].center.y*disX;
                    //picture.Circles[i].center.y = 0 - picture.Circles[i].center.y;
                }
            if (picture.Curves != null)
                for (int i = 0; i < picture.Curves.Length; ++i)
                {
                    picture.Curves[i].first.x = picture.Curves[i].first.x+ picture.Curves[i].first.y*disX;
                    // picture.Curves[i].first.y = 0 - picture.Curves[i].first.y;
                    picture.Curves[i].second.x = picture.Curves[i].second.x + picture.Curves[i].second.y*disX;
                    // picture.Curves[i].second.y = 0 - picture.Curves[i].second.y;
                    picture.Curves[i].thired.x = picture.Curves[i].thired.x + picture.Curves[i].thired.y*disX;
                    // picture.Curves[i].thired.y = 0 - picture.Curves[i].thired.y;
                    picture.Curves[i].fourth.x = picture.Curves[i].fourth.x + picture.Curves[i].fourth.y*disX;
                    // picture.Curves[i].fourth.y = 0 - picture.Curves[i].fourth.y;
                }
            if (picture.Poligon != null)
                for (int i = 0; i < picture.Poligon.Length; ++i)
                {
                    picture.Poligon[i].center.x = picture.Poligon[i].center.x + picture.Poligon[i].center.y*disX;
                    // picture.Poligon[i].center.y = 0 - picture.Poligon[i].center.y;
                    picture.Poligon[i].radius.x = picture.Poligon[i].radius.x + picture.Poligon[i].radius.y*disX;
                    // picture.Poligon[i].radius.y = 0 - picture.Poligon[i].radius.y;
                }
            move(centerPoint.x, centerPoint.y, refrence.x, refrence.y);
            draw();
        }

        public void scale(int newX,int newY)
        {
            PictureJson tranform = new PictureJson(picture);
            point refrence = centerPoint;
            double scaleRatio = 1.0;

            if (action == 3){
                scaleRatio = 1.2;
            }else if (action == 10){
                scaleRatio = 0.8;
            }

            picture = moveToZero(picture);

            if (picture.Lines != null)
            {
                for (int i = 0; i < picture.Lines.Length; ++i)
                {
                    picture.Lines[i].first.x = picture.Lines[i].first.x * scaleRatio;
                    picture.Lines[i].second.x = picture.Lines[i].second.x * scaleRatio;
                    picture.Lines[i].first.y = picture.Lines[i].first.y * scaleRatio;
                    picture.Lines[i].second.y = picture.Lines[i].second.y * scaleRatio;
                }
            }
            if (picture.Circles != null)
                for (int i = 0; i < picture.Circles.Length; ++i)
                {
                   picture.Circles[i].radius = picture.Circles[i].radius * scaleRatio;
                   picture.Circles[i].center.x = picture.Circles[i].center.x * scaleRatio;
                   picture.Circles[i].center.y = picture.Circles[i].center.y * scaleRatio;
                }
            if (picture.Curves != null)
                for (int i = 0; i < picture.Curves.Length; ++i)
                {
                    //need to check - had no curves
                    picture.Curves[i].first.x = picture.Curves[i].first.x * scaleRatio;
                    picture.Curves[i].first.y = picture.Curves[i].first.y * scaleRatio;
                    picture.Curves[i].second.x = picture.Curves[i].second.x * scaleRatio;
                    picture.Curves[i].second.y = picture.Curves[i].second.y * scaleRatio;
                    picture.Curves[i].thired.x = picture.Curves[i].thired.x * scaleRatio;
                    picture.Curves[i].thired.y = picture.Curves[i].thired.y * scaleRatio;
                    picture.Curves[i].fourth.x = picture.Curves[i].fourth.x * scaleRatio;
                    picture.Curves[i].fourth.y = picture.Curves[i].fourth.y * scaleRatio;
                }
            if (picture.Poligon != null)
                for (int i = 0; i < picture.Poligon.Length; ++i)
                {
                    picture.Poligon[i].center.x = picture.Poligon[i].center.x * scaleRatio;
                    picture.Poligon[i].center.y = picture.Poligon[i].center.y * scaleRatio;
                    picture.Poligon[i].radius.x = picture.Poligon[i].radius.x * scaleRatio;
                    picture.Poligon[i].radius.y = picture.Poligon[i].radius.y * scaleRatio;
                }

            move(centerPoint.x, centerPoint.y, refrence.x, refrence.y);
            draw();
        }

        public void Normalize()
        {
            //if the picture is bigger the the screen then center it and scale it to fit the screen
            point tempCenter = new point(centerPoint.x, centerPoint.y);
            //get max and min
            getMaxAndMin();
            centerPoint = new point((minX + ((maxX - Math.Abs(minX)) / 2)), minY + ((maxY - Math.Abs(minY)) / 2));
            point hold = new point(centerPoint.x,centerPoint.y);
            move(centerPoint.x, centerPoint.y, absCenter.x, absCenter.y);
            if ((maxX - Math.Abs(minX)) > panel1.Width || (maxY - Math.Abs(minY)) > (panel1.Height))
            {
                //scale down until not bigger then screen
                while ((maxX - Math.Abs(minX)) > panel1.Width-1 || (maxY - Math.Abs(minY)) > panel1.Height-1)
                {
                    moveToZero(picture);
                    double scaleRatio = 0.8 * Math.Min(panel1.Width / maxX, panel1.Height / maxY);
                    if (picture.Lines != null)
                    {
                        for (int i = 0; i < picture.Lines.Length; ++i)
                        {
                            picture.Lines[i].first.x = picture.Lines[i].first.x * scaleRatio;
                            picture.Lines[i].second.x = picture.Lines[i].second.x * scaleRatio;
                            picture.Lines[i].first.y = picture.Lines[i].first.y * scaleRatio;
                            picture.Lines[i].second.y = picture.Lines[i].second.y * scaleRatio;
                        }
                    }
                    if (picture.Circles != null)
                        for (int i = 0; i < picture.Circles.Length; ++i)
                        {
                            picture.Circles[i].radius = picture.Circles[i].radius * scaleRatio;
                            picture.Circles[i].center.x = picture.Circles[i].center.x * scaleRatio;
                            picture.Circles[i].center.y = picture.Circles[i].center.y * scaleRatio;
                        }
                    if (picture.Curves != null)
                        for (int i = 0; i < picture.Curves.Length; ++i)
                        {
                            picture.Curves[i].first.x = picture.Curves[i].first.x * scaleRatio;
                            picture.Curves[i].first.y = picture.Curves[i].first.y * scaleRatio;
                            picture.Curves[i].second.x = picture.Curves[i].second.x * scaleRatio;
                            picture.Curves[i].second.y = picture.Curves[i].second.y * scaleRatio;
                            picture.Curves[i].thired.x = picture.Curves[i].thired.x * scaleRatio;
                            picture.Curves[i].thired.y = picture.Curves[i].thired.y * scaleRatio;
                            picture.Curves[i].fourth.x = picture.Curves[i].fourth.x * scaleRatio;
                            picture.Curves[i].fourth.y = picture.Curves[i].fourth.y * scaleRatio;
                        }
                    if (picture.Poligon != null)
                        for (int i = 0; i < picture.Poligon.Length; ++i)
                        {
                            picture.Poligon[i].center.x = picture.Poligon[i].center.x * scaleRatio;
                            picture.Poligon[i].center.y = picture.Poligon[i].center.y * scaleRatio;
                            picture.Poligon[i].radius.x = picture.Poligon[i].radius.x * scaleRatio;
                            picture.Poligon[i].radius.y = picture.Poligon[i].radius.y * scaleRatio;
                        }
                    move(centerPoint.x, centerPoint.y, absCenter.x, absCenter.y);
                    getMaxAndMin();
                }
                move(centerPoint.x, centerPoint.y, absCenter.x, absCenter.y);         
                centerPoint = new point(tempCenter.x, tempCenter.y);
            }
        }

        private void getMaxAndMin()
        {
            //get the maximum and minimum values of x and y in the picture
            maxX = double.MinValue;
            maxY = double.MinValue;
            minX = double.MaxValue;
            minY = double.MaxValue;
            if (picture.Lines != null)
                foreach (line l in picture.Lines)
                {
                    if (l.first.x > maxX) maxX = l.first.x;
                    if (l.second.x > maxX) maxX = l.second.x;
                    if (l.first.y > maxY) maxY = l.first.y;
                    if (l.second.y > maxY) maxY = l.second.y;
                    if (l.first.x < minX) minX = l.first.x;
                    if (l.second.x < minX) minX = l.second.x;
                    if (l.first.y < minY) minY = l.first.y;
                    if (l.second.y < minY) minY = l.second.y;
                }
            if (picture.Circles != null)
                foreach (circle c in picture.Circles)
                {
                    if (c.center.x + c.radius > maxX) maxX = c.center.x + c.radius;
                    if (c.center.y + c.radius > maxY) maxY = c.center.y + c.radius;
                    if (c.center.x - c.radius < minX) minX = c.center.x - c.radius;
                    if (c.center.y - c.radius < minY) minY = c.center.y - c.radius;
                }
            if (picture.Curves != null)
                foreach (curve c in picture.Curves)
                {
                    if (c.first.x > maxX) maxX = c.first.x;
                    if (c.second.x > maxX) maxX = c.second.x;
                    if (c.thired.x > maxX) maxX = c.thired.x;
                    if (c.fourth.x > maxX) maxX = c.fourth.x;
                    if (c.first.y > maxY) maxY = c.first.y;
                    if (c.second.y > maxY) maxY = c.second.y;
                    if (c.thired.y > maxX) maxY = c.thired.y;
                    if (c.fourth.y > maxX) maxY = c.fourth.y;
                    if (c.first.x < minX) minX = c.first.x;
                    if (c.second.x < minX) minX = c.second.x;
                    if (c.thired.x < minX) minX = c.thired.x;
                    if (c.fourth.x < minX) minX = c.fourth.x;
                    if (c.first.y < minY) minY = c.first.y;
                    if (c.second.y < minY) minY = c.second.y;
                    if (c.thired.y < minY) minY = c.thired.y;
                    if (c.fourth.y < minY) minY = c.fourth.y;
                }
            if (picture.Poligon != null)
                foreach (poligon p in picture.Poligon)
                {
                    if (p.center.x + getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y) > maxX) maxX = p.center.x + getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y);
                    if (p.center.y + getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y) > maxY) maxY = p.center.y + getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y);
                    if (p.center.x - getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y) < minX) minX = p.center.x - getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y);
                    if (p.center.y - getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y) < minY) minY = p.center.y - getRadius(p.center.x, p.center.y, p.radius.x, p.radius.y);
                }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseDown_1(object sender, MouseEventArgs e)
        {
            //Event handling
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
                pressed = true;
               
                //scale in
                if (action == 3)  { scale(e.Location.X, e.Location.Y);}
               
                //scale out
                if (action == 10) { scale(e.Location.X, e.Location.Y); }
            }
        }

        private void panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            //Event handling
            if(pressed)
            {
                if (action == 1)
                {
                    move(centerPoint.x, centerPoint.y, e.Location.X, e.Location.Y);
                }
                if (action == 2)
                {
                    rotate(e.Location.X, e.Location.Y, centerPoint.x, centerPoint.y, Math.Atan2(e.Location.Y - centerPoint.y, e.Location.X - centerPoint.x));
                }
                if (action == 5)
                    shearX(e.Location.X - MouseDownLocation.X);
                if (action == 9)
                    shearY(e.Location.Y - MouseDownLocation.Y);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //Event handling
            pressed = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (action == 1) 
                 move(centerPoint.x, centerPoint.y, e.Location.X, e.Location.Y);
                if (action == 2) { }
                if (action == 6)
                    centerPoint = new point(e.Location.X, e.Location.Y);
                if (action == 7)
                    mirroringX();
                if (action == 8)
                    mirroringY();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Click or drag picture to move it";
            if (radioButton1.AutoCheck)
            {
                action = 1;
                textBox1.Text = instructions;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Drage to Rotate, move slowly";
            if (radioButton2.AutoCheck)
            {
                action = 2;
                textBox1.Text = instructions;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Click to enlarge";
            if (radioButton3.AutoCheck)
            {
                action = 3;
                textBox1.Text = instructions;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.AutoCheck)
            {
                action = 4;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Drag to shear X";
            if (radioButton5.AutoCheck)
            {
                action = 5;
                textBox1.Text = instructions;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Click to choose center";
            if (radioButton6.AutoCheck)
            {
                action = 6;
                textBox1.Text = instructions;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Click to miror X";
            if (radioButton7.AutoCheck)
            {
                action = 7;
                textBox1.Text = instructions;
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Click to miror Y";
            if (radioButton8.AutoCheck)
            {
                action = 8;
                textBox1.Text = instructions;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            string instructions = "Drag to shear y";
            if (radioButton9.AutoCheck)
            {
                action = 9;
                textBox1.Text = instructions;
            }
        }

        private void radioButton10_CheckedChanged_1(object sender, EventArgs e)
        {
            string instructions = "Click to shrink";
            if (radioButton10.AutoCheck)
            {
                action = 10;
                textBox1.Text = instructions;
            }
        }

        public double getRadius(double x1, double y1, double x2, double y2)
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
            double xc = p.center.x;
            double yc = p.center.y;
            double px = p.radius.x;
            double py = p.radius.y;
            int n = p.polis;

            double lastx, lasty;
            double r = getRadius(xc, yc, px, py);
            double a = Math.Atan2(py - yc, px - xc);

            for (int i = 1; i <= n; i++)
            {
                lastx = px; lasty = py;
                a = a + PI * 2 / n;
                px = Math.Round(xc + r * Math.Cos(a));
                py = Math.Round(yc + r * Math.Sin(a));
                drawLine(new point(lastx, lasty), new point( px, py));
            }
        }

        public void DrawBazia(curve Curve)
        {
            double px;
            double py;
            double px2;
            double py2;
            double t; // t is 1.0/the number of wanted line, calculated in the mouseClick function
            point[] bazia = new point[4];
            bazia[0] = new point(Curve.first.x,Curve.first.y);
            bazia[1] = new point(Curve.second.x, Curve.second.y);
            bazia[2] = new point(Curve.thired.x, Curve.thired.y);
            bazia[3] = new point(Curve.fourth.x, Curve.fourth.y);
            
            for (t = 0.0; t <= 1.0; t+= 0.001)
            {
                px = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].x + 3 * t * (1 - t) * (1 - t) * bazia[1].x + 3 * t * t * (1 - t) * bazia[2].x + t * t * t * bazia[3].x);
                py = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].y + 3 * t * (1 - t) * (1 - t) * bazia[1].y + 3 * t * t * (1 - t) * bazia[2].y + t * t * t * bazia[3].y);
                t += baziaFactor; //incress t to get the second point
                px2 = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].x + 3 * t * (1 - t) * (1 - t) * bazia[1].x + 3 * t * t * (1 - t) * bazia[2].x + t * t * t * bazia[3].x);
                py2 = (int)((1 - t) * (1 - t) * (1 - t) * bazia[0].y + 3 * t * (1 - t) * (1 - t) * bazia[1].y + 3 * t * t * (1 - t) * bazia[2].y + t * t * t * bazia[3].y);

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

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
           // normalize();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            color = Color.FromName(comboBox1.SelectedItem.ToString());
            if (color == Color.Black)
            {
                aBrush = (Brush)Brushes.Black;
            }
            else if (color == Color.Blue)
            {
                aBrush = (Brush)Brushes.Blue;
            }
            else if (color == Color.Red)
            {
                aBrush = (Brush)Brushes.Red;
            }
            else if (color == Color.Yellow)
            {
                aBrush = (Brush)Brushes.Yellow;
            }
            else if (color == Color.Green)
            {
                aBrush = (Brush)Brushes.Green;
            }
            draw();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                comboBox1.Text = "Choose color";
            }
            else
            {
                comboBox1.Text = comboBox1.SelectedText;
            }
        }

        private void button3_MouseClick_1(object sender, MouseEventArgs e)
        {
            Normalize();
        }

        //reload button
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFile();
            draw();
        }


        //save picture to json file
        private void button2_Click(object sender, EventArgs e)
        {
            JObject jsonObject = createPictureJson(picture);
            saveJsonFile(jsonObject, "JsonFile");
        }
    }
}


/*
 *        public JObject createPictureJson(PictureJson json)
        {
            JObject jsonObject = (JObject)JToken.FromObject(json);
            return jsonObject;
        }

        public void saveJsonFile(JObject json, string fileName)
*/