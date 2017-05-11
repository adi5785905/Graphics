using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq.JObject;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace graphics2
{
    public partial class Form1 : Form
    {
        public PictureJson picture;
        public PictureJson background;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void createPictureJson(PictureJson json)
        {
            JObject jsonObject = (JObject)JToken.FromObject(json);
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

        public PictureJson parseJson()
        {
            
        }

        public void printPicture()
        {

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
    }
}
