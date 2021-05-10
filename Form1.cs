using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;

namespace Metadata_IO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.AllowDrop = true;
        }


        OpenFileDialog openImage;
        String pathImage = "";
       

        Dictionary<string, int> property = new Dictionary<string, int> // Data should be decoded according to EXIF specification
        {                                                              // 
            {"Width", 256},                                            // long value
            {"Height", 257},                                           // long value
            {"Date", 306},                                             // ASCII value
            {"Focal length", 37386},                                   // Rational value
            {"Altitude", 6},                                           // Rational value
            {"Longitude", 4},                                          // Rational value
            {"Latitude", 2}                                            // Rational value
        };
        Dictionary<string, byte[]> values = new Dictionary<string, byte[]>();
        private void button1_Click(object sender, EventArgs e)
        {
            openImage = new OpenFileDialog();
            //openImage.Filter = " ";
            if (openImage.ShowDialog() == DialogResult.OK)
            {
                pathImage = openImage.FileName;
                ExifToText(pathImage);
                values.Clear();
              //  var value = System.Text.ASCIIEncoding.ASCII.GetString(bv);
            }
        }
        void ExifToText(string path)
        {
            Image img = new Bitmap(path);
            pictureBox1.Image = img;
            PropertyItem[] propertiesValues = img.PropertyItems;
            var propertiesID = img.PropertyIdList;
            string output = " ";
            foreach(var id in property)
            {
                for(int i = 0; i < propertiesID.Length; i++)
                {
                    if(id.Value == propertiesID[i])
                    {
                        values.Add(id.Key , propertiesValues[i].Value);
                        if(id.Key == "Width" || id.Key == "Height")
                        {
                            long value = Convert.ToInt64(propertiesValues[i].Value[0] + propertiesValues[i].Value[1] * 255 +
                                propertiesValues[i].Value[2] * Math.Pow(255, 2) + propertiesValues[i].Value[2] * Math.Pow(255, 2));
                            textBox1.Text += String.Format(id.Key + ": {0} "+ Environment.NewLine, value);
                        }
                        else if( id.Key == "Focal length" || id.Key == "Alttitude" || id.Key == "Longitude" || id.Key == "Latitude")
                        {
                            float numerator = Convert.ToSingle(propertiesValues[i].Value[0] + propertiesValues[i].Value[1] * 255 +
                                propertiesValues[i].Value[2] * Math.Pow(255, 2) + propertiesValues[i].Value[2] * Math.Pow(255, 2)),
                                denominator = Convert.ToInt64(propertiesValues[i].Value[4] + propertiesValues[i].Value[5] * 255 +
                                propertiesValues[i].Value[6] * Math.Pow(255, 2) + propertiesValues[i].Value[7] * Math.Pow(255, 2));
                            float rationalvalue = numerator / denominator;
                            textBox1.Text += String.Format(id.Key + ": {0} " + Environment.NewLine, rationalvalue);
                        }
                        else if (id.Key == "Date")
                        {
                            textBox1.Text += String.Format(id.Key + ": {0} " + Environment.NewLine, ASCIIEncoding.ASCII.GetString(propertiesValues[i].Value));

                        }
                    }
                }
            }

        }

        private void pictureBox1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                label1.Text = " Drop photo";
            }
          
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
            ExifToText(data[0].ToString());
            label1.Visible = false;
            label1.Enabled = false;
        }

        private void pictureBox1_DragLeave(object sender, EventArgs e)
        {
            label1.Text = "Drag photo into region";
            label1.TextAlign = ContentAlignment.MiddleCenter;
        }
    }
}
