using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCFList;

namespace CVFList__ElementsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VCFElement teelement = new VCFElement(textBox1.Text);
            textBox2.Text = teelement.Name;
            textBox3.Text = teelement.Value;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            VCFPhoto tePhoto = new VCFPhoto(textBox4.Text);

            pictureBox1.Image = tePhoto.Img;
            if (pictureBox1.Image != null)
            {
                button7.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = openFileDialog1.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = openFileDialog1.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            VCFFile teFile1, teFile2;
            teFile1 = new VCFFile(textBox5.Text);
            teFile2 = new VCFFile(textBox6.Text);
            teFile1.Merge(teFile2);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            VCFFile teFile1, teFile2;
            teFile1 = new VCFFile(textBox5.Text);
            teFile2 = new VCFFile(textBox6.Text);
            teFile1.Merge(teFile2);
            teFile1.SaveToFile("tot.vcf");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("blabla.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            if (od.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = Image.FromFile(od.FileName);
            }
            od.Dispose();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //pictureBox2.Image = (Image)VCFPhoto.ResizeImageProportionaly(pictureBox2.Image, -1,  96/*, -1*/);
            pictureBox2.Image = (Image)VCFPhoto.ResizeToMax96(pictureBox2.Image);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            VCFPhoto tePhoto = new VCFPhoto(pictureBox2.Image);
            textBox4.Text = tePhoto.CodedText;
        }

    }
}
