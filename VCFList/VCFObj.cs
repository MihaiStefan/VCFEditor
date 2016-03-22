using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VCFList
{
    public partial class VCFObj : UserControl
    {
        public VCFObj()
        {
            InitializeComponent();
        }

        public VCFObj(VCFCard _VCFCard)
        {
            foreach (VCFElement elem in _VCFCard.ElementsList)
            {
                _CreateElement(elem.Name, elem.Value);
            }
        }

        public void ShowCard(VCFCard _VCFCard)
        {
            //foreach(Control cntrlElem in )
            this._ClearControls();
            foreach (VCFElement elem in _VCFCard.ElementsList)
            {
                _CreateElement(elem.Name, elem.Value);
            }
        }

        private void _CreateElement(string key, string info)
        {
            Label locLabelKey;
            TextBox locTxtBxInf;
            PictureBox locPicBxInf;


            locTxtBxInf = new TextBox();
            locLabelKey = new Label();
            locPicBxInf = new PictureBox();

            if (this.Controls.Count > 0)
            {
                locLabelKey.Top = this.Controls[this.Controls.Count - 1].Top + this.Controls[this.Controls.Count - 1].Height + 5;
                locLabelKey.Left = 10;

            }
            else
            {
                locLabelKey.Top = 5;
                locLabelKey.Left = 10;
            }
            if (key.ToUpper().Contains("PHOTO"))
            {
                locPicBxInf.Top = locLabelKey.Top;
                locPicBxInf.Left = locLabelKey.Left + locLabelKey.Width + 10;
                //locPicBxInf.Text = info;
                //locPicBxInf.ModifiedChanged += this.textBox1_ModifiedChanged;
                locPicBxInf.Image = new VCFPhoto(info).Img;
                this.Controls.Add(locPicBxInf);
                this.Height = 1000; // locPicBxInf.Top + locPicBxInf.Height + 5;
            }
            else
            {
                locTxtBxInf.Top = locLabelKey.Top;
                locTxtBxInf.Left = locLabelKey.Left + locLabelKey.Width + 10;
                locTxtBxInf.Text = info;
                locTxtBxInf.ModifiedChanged += this.textBox1_ModifiedChanged;
                this.Controls.Add(locTxtBxInf);
            }

            locLabelKey.Text = key;
            this.Controls.Add(locLabelKey);
        }

        private void _ClearControls()
        {
            this.Controls.Clear();
            foreach (Control cntrl in this.Controls.OfType<Label>())
            {
                this.Controls.Remove(cntrl);
            }
            foreach (Control cntrl in this.Controls.OfType<TextBox>())
            {
                this.Controls.Remove(cntrl);
            }

        }

        private void textBox1_ModifiedChanged(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                MessageBox.Show("balalala");
            }
        }
    }
}
