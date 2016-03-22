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

namespace VCFList_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            vcfVisLstObj1.Viewer = vcfObj1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //vcfObj1.FileName = "c:\\Temp\\__\\send_contacts.vcf";
            vcfVisLstObj1.FileName = "c:\\Temp\\__\\1.vcf";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //vcfObj1.AddFile("c:\\Temp\\__\\contacts.vcf");
            vcfVisLstObj1.AddFile("c:\\Temp\\__\\2.vcf");
            this.ActiveControl = vcfVisLstObj1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            vcfVisLstObj1.SaveToFile();
        }

        /*private void vcfObj1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            vcfObj2.ShowCard(vcfObj1.);
        }*/
    }
}
