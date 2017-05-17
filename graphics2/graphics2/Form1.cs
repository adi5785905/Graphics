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
        point centerPoint;
        double maxX;
        double maxY;
        double minX;
        double minY;
        int[,] arr;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Size = new Size(1100, 650);
            base.OnLoad(e);
            drawFrame();
            //createTemp();
            OpenFile();
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
        }

        public void OpenFile()
        {
            /*
            try
            {
                string st = File.ReadAllText("JsonFile");
                Console.WriteLine(st);
            }
            catch (Exception e)
            {
                Console.WriteLine("The picture file could not be read:");
                Console.WriteLine(e.Message);
            }
            
            Console.Read();
            */

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("JsonFile.json"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    //Console.Write(line);

                    JToken root = JObject.Parse(line);
                    JToken pictureJson = root["pictureJson"];
                    //PictureJson deserializedUser = JsonConvert.DeserializeObject<PictureJson>(pictureJson.ToString());
                    picture = JsonConvert.DeserializeObject<PictureJson>(pictureJson.ToString()); //creating PictureJson obj and setting it in the var
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The picture file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        //doesnt work
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
            picture = new PictureJson(4, 2, 1, 0, 1);
            picture.Lines[0] = new line(new point(50, 50), new point(70, 50));
            picture.Lines[1] = new line(new point(80, 90), new point(30, 70));
            picture.Circles[0] = new circle(new graphics2.centerPoint(100, 100), 10);
            picture.Poligon[0] = new poligon(new graphics2.centerPoint(150, 150), new point(170, 150), 4);
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
        public PictureJson setValueToPic(PictureJson pic, double calculateX, double calculateY)
        {
            centerPoint.x = calculateX;
            centerPoint.y = calculateY;
            if (pic.Lines != null)
                for (int i = 0; i < pic.Lines.Length; ++i)
                {

                    pic.Lines[i].first.x = calculateX;
                    pic.Lines[i].first.y = calculateY;
                    pic.Lines[i].second.x = calculateX;
                    pic.Lines[i].second.y = calculateY;
                }
            if (pic.Circles != null)
                for (int i = 0; i < pic.Circles.Length; ++i)
                {
                    pic.Circles[i].center.x = calculateX;
                    pic.Circles[i].center.y = calculateY;
                }
            if (pic.Curves != null)
                for (int i = 0; i < pic.Curves.Length; ++i)
                {
                    pic.Curves[i].first.x = calculateX;
                    pic.Curves[i].first.y = calculateY;
                    pic.Curves[i].second.x = calculateX;
                    pic.Curves[i].second.y = calculateY;
                    pic.Curves[i].thired.x = calculateX;
                    pic.Curves[i].thired.y = calculateY;
                    pic.Curves[i].fourth.x = calculateX;
                    pic.Curves[i].fourth.y = calculateY;
                }
            if (pic.Poligon != null)
                for (int i = 0; i < pic.Poligon.Length; ++i)
                {
                    pic.Poligon[i].center.x = calculateX;
                    pic.Poligon[i].center.y = calculateY;
                    pic.Poligon[i].radius.x = calculateX;
                    pic.Poligon[i].radius.y = calculateY;
                }
            return pic;
    }
        //public void normalize()
        //{
        //    point refrence = new point(centerPoint.x,centerPoint.y);
        //    maxX = Double.MinValue;
        //    maxY = Double.MinValue;
        //    minX = Double.MaxValue;
        //    minY = Double.MaxValue;
        //    point middle;
        //    double yValue;
        //    double hight = panel1.Top - panel1.Bottom;
        //    double width = panel1.Right - panel1.Left;
        //    arr = new int[4, 4];//lines, circle,curve,poligon// minX,maxX,miny,maxY
        //    for (int i = 0; i < 4; i++)
        //    {
        //        arr[0, i] = -1;
        //        arr[1, i] = -1;
        //        arr[2, i] = -1;
        //        arr[3, i] = -1;
        //    }
        //    //find maxX maxY minX minY
        //    getMaxAndMin();
        //    // find one that out of bounds
        //    if ((maxX > panel1.Height) || (minX < 0) || (maxY > panel1.Width) || (minY < 0))
        //    {
        //        // take the minX and set it at the left
        //        if (arr[1, 0] > -1)
        //        {
        //            centerPoint.x = picture.Circles[arr[1, 0]].center.x - picture.Circles[arr[1, 0]].radius;
        //            centerPoint.y = picture.Circles[arr[1, 0]].center.y - picture.Circles[arr[1, 0]].radius;

        //        }
        //        else if (arr[3, 0] > -1)
        //        {
        //            centerPoint.x = picture.Poligon[arr[3, 0]].center.x - getRadius(picture.Poligon[arr[3, 0]].center.x, picture.Poligon[arr[3, 0]].center.y, picture.Poligon[arr[3, 0]].radius.x, picture.Poligon[arr[3, 0]].radius.y);
        //            centerPoint.y = picture.Poligon[arr[3, 0]].center.y - getRadius(picture.Poligon[arr[3, 0]].center.x, picture.Poligon[arr[3, 0]].center.y, picture.Poligon[arr[3, 0]].radius.x, picture.Poligon[arr[3, 0]].radius.y);
        //        }
        //        else
        //        {
        //            if (arr[0, 0] > -1)
        //            {
        //                if (picture.Lines[arr[0, 0]].first.x == minX)
        //                {
        //                    centerPoint.x = picture.Lines[arr[0, 0]].first.x;
        //                    centerPoint.y = picture.Lines[arr[0, 0]].first.y;
        //                }
        //                else
        //                {
        //                    centerPoint.x = picture.Lines[arr[0, 0]].second.x;
        //                    centerPoint.y = picture.Lines[arr[0, 0]].second.y;
        //                }
        //            }
        //            else if (arr[2, 0] > -1)
        //            {
        //                if (picture.Curves[arr[2, 0]].first.x == minX)
        //                {
        //                    centerPoint.x = picture.Curves[arr[2, 0]].first.x;
        //                    centerPoint.y = picture.Curves[arr[2, 0]].first.y;
        //                }
        //                else if (picture.Curves[arr[2, 0]].second.x == minX)
        //                {
        //                    centerPoint.x = picture.Curves[arr[2, 0]].second.x;
        //                    centerPoint.y = picture.Curves[arr[2, 0]].second.y;
        //                }
        //                else if (picture.Curves[arr[2, 0]].thired.x == minX)
        //                {
        //                    centerPoint.x = picture.Curves[arr[2, 0]].thired.x;
        //                    centerPoint.y = picture.Curves[arr[2, 0]].thired.y;
        //                }
        //                else if (picture.Curves[arr[2, 0]].fourth.x == minX)
        //                {
        //                    centerPoint.x = picture.Curves[arr[2, 0]].fourth.x;
        //                    centerPoint.y = picture.Curves[arr[2, 0]].fourth.y;
        //                }
        //            }
        //        }
        //        middle = new point((maxX - minX)/2+minX, (maxY - minY)/2+minY);
        //        yValue = ((centerPoint.y - minY) / (maxY - minY)) * (hight);
        //        double addedX = 0 - centerPoint.x;
        //        double addedY = yValue - centerPoint.y;
        //        refrence.x += addedX;
        //        refrence.y += addedY;
        //        addValueToPictureJson(picture, addedX, addedY);
        //        addedX = 0;
        //        addedY = middle.y - (hight / 2);
        //        refrence.x += addedX;
        //        refrence.y += addedY;
        //        addValueToPictureJson(picture, addedX, addedY);
        //        centerPoint.x = refrence.x;
        //        centerPoint.y = refrence.y;
                
                
        //        if (minY < 0)
        //        {
        //            refrence.y += 0 - minY;
        //            addValueToPictureJson(picture, 0, 0 - minY);
                    
        //        }
        //        if (minX < 0)
        //        {
        //            refrence.x += 0 - minX;
        //            addValueToPictureJson(picture, 0 - minX, 0);
                    
        //        }
        //        if((maxY-minY < panel1.Height) && (maxX-minX < panel1.Width))
        //        {
        //            refrence.x += centerPoint.x-(maxX - minX) / 2 + minX;
        //            refrence.y += centerPoint.y-(maxY - minY) / 2 + minY;
        //            move(centerPoint.x, centerPoint.y, (maxX - minX) / 2 + minX, (maxY - minY) / 2 + minY);
        //        }
        //        //make the rest smaller until all points are inbound
        //        while ((maxY > panel1.Height) ||  (maxX > panel1.Width) )
        //        { 
        //        //PictureJson tranform = new PictureJson(picture);
                
        //        double scaleRatio = 0.8;
        //            picture = moveToZero(picture);
        //            centerPoint.x *= scaleRatio;
        //            centerPoint.y *= scaleRatio;
        //            refrence.x *= scaleRatio;
        //            refrence.y *= scaleRatio;
        //        if (picture.Lines != null)
        //        {
        //            for (int i = 0; i < picture.Lines.Length; ++i)
        //            {
        //                    picture.Lines[i].first.x = picture.Lines[i].first.x * scaleRatio;
        //                    picture.Lines[i].second.x = picture.Lines[i].second.x * scaleRatio;
        //                    picture.Lines[i].first.y = picture.Lines[i].first.y * scaleRatio;
        //                    picture.Lines[i].second.y = picture.Lines[i].second.y * scaleRatio;
        //            }
        //        }
        //        if (picture.Circles != null)
        //            for (int i = 0; i < picture.Circles.Length; ++i)
        //            {
        //                picture.Circles[i].radius = picture.Circles[i].radius * scaleRatio;
        //                picture.Circles[i].center.x = picture.Circles[i].center.x * scaleRatio;
        //                picture.Circles[i].center.y = picture.Circles[i].center.y * scaleRatio;
        //            }
        //        if (picture.Curves != null)
        //            for (int i = 0; i < picture.Curves.Length; ++i)
        //            {
        //                //need to check - had no curves
        //                picture.Curves[i].first.x = picture.Curves[i].first.x * scaleRatio;
        //                picture.Curves[i].first.y = picture.Curves[i].first.y * scaleRatio;
        //                picture.Curves[i].second.x = picture.Curves[i].second.x * scaleRatio;
        //                picture.Curves[i].second.y = picture.Curves[i].second.y * scaleRatio;
        //                picture.Curves[i].thired.x = picture.Curves[i].thired.x * scaleRatio;
        //                picture.Curves[i].thired.y = picture.Curves[i].thired.y * scaleRatio;
        //                picture.Curves[i].fourth.x = picture.Curves[i].fourth.x * scaleRatio;
        //                picture.Curves[i].fourth.y = picture.Curves[i].fourth.y * scaleRatio;
        //            }
        //        if (picture.Poligon != null)
        //            for (int i = 0; i < picture.Poligon.Length; ++i)
        //            {
        //                picture.Poligon[i].center.x = picture.Poligon[i].center.x * scaleRatio;
        //                picture.Poligon[i].center.y = picture.Poligon[i].center.y * scaleRatio;
        //                picture.Poligon[i].radius.x = picture.Poligon[i].radius.x * scaleRatio;
        //                picture.Poligon[i].radius.y = picture.Poligon[i].radius.y * scaleRatio;
        //            }
                    
        //            if (arr[1, 0] > -1)
        //            {
        //                centerPoint.x = picture.Circles[arr[1, 0]].center.x - picture.Circles[arr[1, 0]].radius;
        //                centerPoint.y = picture.Circles[arr[1, 0]].center.y - picture.Circles[arr[1, 0]].radius;

        //            }
        //            else if (arr[3, 0] > -1)
        //            {
        //                centerPoint.x = picture.Poligon[arr[3, 0]].center.x - getRadius(picture.Poligon[arr[3, 0]].center.x, picture.Poligon[arr[3, 0]].center.y, picture.Poligon[arr[3, 0]].radius.x, picture.Poligon[arr[3, 0]].radius.y);
        //                centerPoint.y = picture.Poligon[arr[3, 0]].center.y - getRadius(picture.Poligon[arr[3, 0]].center.x, picture.Poligon[arr[3, 0]].center.y, picture.Poligon[arr[3, 0]].radius.x, picture.Poligon[arr[3, 0]].radius.y);
        //            }
        //            else
        //            {
        //                if (arr[0, 0] > -1)
        //                {
        //                    if (picture.Lines[arr[0, 0]].first.x < picture.Lines[arr[0, 0]].second.x)
        //                    {
        //                        centerPoint.x = picture.Lines[arr[0, 0]].first.x;
        //                        centerPoint.y = picture.Lines[arr[0, 0]].first.y;
        //                    }
        //                    else
        //                    {
        //                        centerPoint.x = picture.Lines[arr[0, 0]].second.x;
        //                        centerPoint.y = picture.Lines[arr[0, 0]].second.y;
        //                    }
        //                }
        //                else if (arr[2, 0] > -1)
        //                {
        //                    if ((picture.Curves[arr[2, 1]].first.x < picture.Curves[arr[2, 1]].second.x) && (picture.Curves[arr[2, 1]].first.x < picture.Curves[arr[2, 1]].thired.x) && (picture.Curves[arr[2, 1]].first.x < picture.Curves[arr[2, 1]].fourth.x))
        //                    {
        //                        centerPoint.x = picture.Curves[arr[2, 1]].first.x;
        //                        centerPoint.y = picture.Curves[arr[2, 1]].first.y;
        //                    }
        //                    else if ((picture.Curves[arr[2, 1]].second.x < picture.Curves[arr[2, 1]].first.x) && (picture.Curves[arr[2, 1]].second.x < picture.Curves[arr[2, 1]].thired.x) && (picture.Curves[arr[2, 1]].second.x < picture.Curves[arr[2, 1]].fourth.x))
        //                    {
        //                        centerPoint.x = picture.Curves[arr[2, 1]].second.x;
        //                        centerPoint.y = picture.Curves[arr[2, 1]].second.y;
        //                    }
        //                    else if ((picture.Curves[arr[2, 1]].thired.x < picture.Curves[arr[2, 1]].first.x) && (picture.Curves[arr[2, 1]].thired.x < picture.Curves[arr[2, 1]].second.x) && (picture.Curves[arr[2, 1]].thired.x < picture.Curves[arr[2, 1]].fourth.x))
        //                    {
        //                        centerPoint.x = picture.Curves[arr[2, 1]].thired.x;
        //                        centerPoint.y = picture.Curves[arr[2, 1]].thired.y;
        //                    }
        //                    else if ((picture.Curves[arr[2, 1]].fourth.x < picture.Curves[arr[2, 1]].first.x) && (picture.Curves[arr[2, 1]].fourth.x < picture.Curves[arr[2, 1]].second.x) && (picture.Curves[arr[2, 1]].fourth.x < picture.Curves[arr[2, 1]].thired.x))
        //                    {
        //                        centerPoint.x = picture.Curves[arr[2, 1]].fourth.x;
        //                        centerPoint.y = picture.Curves[arr[2, 1]].fourth.y;
        //                    }
        //                }
        //            }
        //            //middle = new point(maxx - minx, maxy - miny);
        //            double yvalue = ((centerPoint.y - minY) / 2 + minY);
        //            double addedx = 0 - centerPoint.x;
        //            double addedy = yvalue - centerPoint.y;
        //            refrence.x += addedx;
        //            refrence.y += addedy;
        //            addValueToPictureJson(picture, addedx, addedy);
                    
        //           // move(centerPoint.x, centerPoint.y,0,0);
        //            if (minY < 0)
        //            {
        //                refrence.y += 0 - minY;
        //                addValueToPictureJson(picture, 0, 0 - minY);
                       
        //            }
        //            if (minX < 0)
        //            {
        //                refrence.x += 0 - minX;
        //                addValueToPictureJson(picture, 0 - minX, 0);
                        
        //            }
        //        } 
        //        }
            
        //    centerPoint = refrence;
        //    centerPoint.x = minX;
        //    centerPoint.y = minY;
        //    draw();
        //}

        public void normalize()
        {
            arr = new int[4, 4];//lines, circle,curve,poligon// minX,maxX,miny,maxY
            for (int i = 0; i < 4; i++)
            {
                arr[0, i] = -1;
                arr[1, i] = -1;
                arr[2, i] = -1;
                arr[3, i] = -1;
            }
            getMaxAndMin();
            if(maxX - minX > panel1.Width || maxY - minY > panel1.Height)
            {
                if (maxX - minX > maxY - minY)
                {
                    PictureJson tranform = new PictureJson(picture);
                    point refrence = centerPoint;
                    while (maxX-minX>panel1.Width)
                    {                      
                        double scaleRatio = 0.9;

                        tranform = moveToZero(tranform);

                        if (tranform.Lines != null)
                        {
                            for (int i = 0; i < tranform.Lines.Length; ++i)
                            {
                                tranform.Lines[i].first.x = tranform.Lines[i].first.x * scaleRatio;
                                tranform.Lines[i].second.x = tranform.Lines[i].second.x * scaleRatio;
                                tranform.Lines[i].first.y = tranform.Lines[i].first.y * scaleRatio;
                                tranform.Lines[i].second.y = tranform.Lines[i].second.y * scaleRatio;
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

                        move(centerPoint.x, centerPoint.y, (maxX - minX) / 2,(maxY - minY) / 2);
                    }
                }
                else
                {
                    PictureJson tranform = new PictureJson(picture);
                    point refrence = centerPoint;
                    while (maxY - minY > panel1.Height)
                    {
                        double scaleRatio = 0.9;

                        tranform = moveToZero(tranform);

                        if (tranform.Lines != null)
                        {
                            for (int i = 0; i < tranform.Lines.Length; ++i)
                            {
                                tranform.Lines[i].first.x = tranform.Lines[i].first.x * scaleRatio;
                                tranform.Lines[i].second.x = tranform.Lines[i].second.x * scaleRatio;
                                tranform.Lines[i].first.y = tranform.Lines[i].first.y * scaleRatio;
                                tranform.Lines[i].second.y = tranform.Lines[i].second.y * scaleRatio;
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

                        move(centerPoint.x, centerPoint.y, (maxX - minX) / 2, (maxY - minY) / 2);
                    }
                }
            }
            else
            {
                centerPoint.x = (maxX - minX) / 2;
                centerPoint.y = (maxY - minY) / 2;
                draw();
            }
            centerPoint.x = (maxX - minX) / 2;
            centerPoint.y = (maxY - minY) / 2;
            draw();

        }
        public void getMaxAndMin()
        {
            if (picture.Lines != null)
                for (int i = 0; i < picture.Lines.Length; ++i)
                {
                    isMaxorMin(picture.Lines[i].first.x, picture.Lines[i].first.y, i, 0);
                    isMaxorMin(picture.Lines[i].second.x, picture.Lines[i].second.y, i, 0);
                }
            if (picture.Circles != null)
                for (int i = 0; i < picture.Circles.Length; ++i)
                {
                    isMaxorMinWithRadius(picture.Circles[i].center.x, picture.Circles[i].center.y, picture.Circles[i].radius, i, 1);
                }
            if (picture.Curves != null)
                for (int i = 0; i < picture.Curves.Length; ++i)
                {
                    isMaxorMin(picture.Curves[i].first.x, picture.Curves[i].first.y, i, 2);
                    isMaxorMin(picture.Curves[i].second.x, picture.Curves[i].second.y, i, 2);
                    isMaxorMin(picture.Curves[i].thired.x, picture.Curves[i].second.y, i, 2);
                    isMaxorMin(picture.Curves[i].fourth.x, picture.Curves[i].second.y, i, 2);
                }
            if (picture.Poligon != null)
                for (int i = 0; i < picture.Poligon.Length; ++i)
                {
                    isMaxorMinWithRadius(picture.Poligon[i].center.x, picture.Poligon[i].center.y, getRadius(picture.Poligon[i].center.x, picture.Poligon[i].center.y, picture.Poligon[i].radius.x, picture.Poligon[i].radius.y), i, 3);
                }
        }
        private void isMaxorMin(double xValue, double yValue, int index , int arrLine)
        {
            if (xValue < minX)
            {
                minX = xValue;
                arr[arrLine, 0] = index;
            }
            if (xValue > maxX)
            {
                maxX = xValue;
                arr[arrLine, 1] = index;
            }
            if (yValue < minY)
            {
                minY = yValue;
                arr[arrLine, 2] = index;
            }
            if (yValue > maxY)
            {
                maxY = yValue;
                arr[arrLine, 3] = index;
            }
        }

        private void isMaxorMinWithRadius(double xValue, double yValue, double rad, int index, int arrLine)
        {
            if (xValue-rad < minX)
            {
                minX = xValue - rad;
                arr[arrLine, 0] = index;
            }
            if (xValue + rad > maxX)
            {
                maxX = xValue;
                arr[arrLine, 1] = index;
            }
            if (yValue - rad < minY)
            {
                minY = yValue - rad;
                arr[arrLine, 2] = index;
            }
            if (yValue + rad > maxY)
            {
                maxY = yValue;
                arr[arrLine, 3] = index;
            }
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
            double calculateX = 0 - centerPoint.x;
            double calculateY = 0 - centerPoint.y;
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

        public void rotate(double x, double y, double choosenX, double choosenY,double angle)
        {
            PictureJson tranform = picture;
           // choosenX = centerPoint.x;
           // choosenY = centerPoint.y;
            //tranform = moveToZero(tranform);
            //double angleInDegrees = Math.Atan2(x-choosenX, y-choosenY);
            double angleInRad = angle / 180.0 * Math.PI;
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
            // picture = moveBack(tranform);
            //move(picture.Lines[0].first.x, picture.Lines[0].first.y, refrence.x, refrence.y);
            draw();
        }

        public void mirroringX()
        {
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

            tranform = moveToZero(tranform);

            if (tranform.Lines != null)
            {
                for (int i = 0; i < tranform.Lines.Length; ++i)
                {
                    tranform.Lines[i].first.x = tranform.Lines[i].first.x * scaleRatio;
                    tranform.Lines[i].second.x = tranform.Lines[i].second.x * scaleRatio;
                    tranform.Lines[i].first.y = tranform.Lines[i].first.y * scaleRatio;
                    tranform.Lines[i].second.y = tranform.Lines[i].second.y * scaleRatio;
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseDown_1(object sender, MouseEventArgs e)
        {
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
            if(pressed)
            {
                if (action == 1)
                    move(centerPoint.x, centerPoint.y, e.Location.X, e.Location.Y);
                if (action == 2)
                {
                    rotate(e.Location.X, e.Location.Y, centerPoint.x, centerPoint.y, Math.Atan2(e.Location.Y - centerPoint.y, e.Location.X - centerPoint.x));
                }
                if (action == 3) { }
                if (action == 5)
                    shearX(e.Location.X - MouseDownLocation.X);
                if (action == 9)
                    shearY(e.Location.Y - MouseDownLocation.Y);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (action == 1) 
                 move(centerPoint.x, centerPoint.y, e.Location.X, e.Location.Y);
                if (action == 2)
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

        //Distance
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

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            normalize();
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
    }
}
