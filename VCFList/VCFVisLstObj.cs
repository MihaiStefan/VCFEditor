using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using System.Threading;

namespace VCFList
{
    public class VCFVisLstObj:DataGridView
    {
        //--------------PULBIC VARIABLES
        public List<string> ColumnList
        {
            get { return this._GetColumnList(); }
        }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                OnChanged();
            }
        }

        public VCFObj Viewer = null;

        /*
        public List<VCFFile> MergedFiles
        {
            get { return this._OtherFiles; }
        }
        */

        //--------------PRIVATE VARIABLES
        private string _FileName;
        private VCFFile _VCFFile;
        //private List<VCFFile> _OtherFiles;

        //--------------PUBLIC METHODS
        public event EventHandler Changed;

        public VCFVisLstObj()
        {
            this._VCFFile = new VCFFile();
            //this._OtherFiles = new List<VCFFile>();
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToOrderColumns = true;
            this.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Click += new System.EventHandler(this.VCFVisLstObj_Click);
        }

        public void AddFile(string FileName)
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                this.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
            }
            VCFFile teFile = new VCFFile(FileName);
            this._VCFFile.Merge(teFile);
            RefreshDataFromFile(teFile);
        }

        private void VCFVisLstObj_Click(object sender, EventArgs e)
        {
            if (this.CurrentCell != null)
            {
                if (this.CurrentCell.RowIndex >= 0)
                {
                    if (this.Viewer != null)
                    {
                        Viewer.ShowCard(this._VCFFile.CardList[this.CurrentCell.RowIndex]);
                    }
                }
            }
        }

        public void SaveToFile()
        {
            this._VCFFile.SaveToFile("blabla.vcf");
        }
        //--------------PROTECTED METHODS
        protected void OnChanged()
        {
            //Thread th;
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
            this._VCFFile.FileName = this._FileName;
            this.ColumnCount = 0;
            this.RowCount = 0;
            //this._OtherFiles.Clear();
            this._RefreshDataFromMainFile();
            /*
            th = new Thread(this._RefreshDataFromMainFile);
            th.Start();
             */ 
        }

        protected void RefreshDataFromFile(VCFFile CardsFile)
        {
            List<string> tecolslist;
            string[] rowString;
            bool isThereData = false;
            int i;

            tecolslist = CardsFile.ColumnsList;

            for(i = 0; i < tecolslist.Count; i++)
            {
                if (_GetColumnPosAfterName(tecolslist[i]) == -1)
                {
                    this.ColumnCount += 1;
                    this.Columns[ColumnCount - 1].Name = tecolslist[i];
                }
            }

            foreach(VCFCard teCard in CardsFile.CardList)
            {
                rowString = new string[this.ColumnCount];
                isThereData = false;

                foreach (VCFElement teElement in teCard.ElementsList)
                {
                    int teint = _GetColumnPosAfterName(teElement.Name); 
                    rowString[teint] = teElement.Value;
                    isThereData = true;

                }
                if (isThereData)
                {
                    this.Rows.Add(rowString);
                    foreach (int col in _GetPhotoColumnList())
                    {
                        for (i = 0; i < this.RowCount; i++)
                        {
                            if ((this.Rows[i].Cells[col].Value != null) &&
                                (Object.ReferenceEquals(this.Rows[i].Cells[col].Value.GetType(), "".GetType())))
                            {
                                DataGridViewImageCell IC = new DataGridViewImageCell();
                                VCFPhoto tePhoto = new VCFPhoto(this.Rows[i].Cells[col].Value.ToString());
                                IC.Value = (Image)tePhoto.Img;
                                this.Rows[i].Cells[col] = IC;
                                this.Rows[i].Height = tePhoto.Img.Height;
                            }
                        }
                    }
                }
            }
            this._NumerotateTable();
        }

        //--------------PRIVATE METHODS
        private void _RefreshDataFromMainFile()
        {
            RefreshDataFromFile(this._VCFFile);
        }

        private int _GetColumnPosAfterName(string ColName)
        { 
            int teVal = -1;
            for (int i = 0; i < this.ColumnCount; i++)
            {
                if (ColName == this.Columns[i].Name)
                {
                    teVal = i;
                    break;
                }
            }
            return teVal;
        }

        private int[] _GetPhotoColumnList()
        {
            List<int> teArray = new List<int>();
            for (int i = 0; i < this.ColumnCount; i++)
            {
                if (this.Columns[i].Name.ToUpper().Contains("PHOTO"))
                {
                    teArray.Add(i);
                }
            }
            return teArray.ToArray();
        }

        private List<string> _GetColumnList()
        {
            List<string> teList = new List<string>();

            for (int i = 0; i < this.ColumnCount; i++)
            {
                teList.Add(this.Columns[i].Name);
            }
                return null;
        }
        private void _NumerotateTable()
        {
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            for (int i = 0; i < this.RowCount; i++)
            {
                this.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }
    }
}
