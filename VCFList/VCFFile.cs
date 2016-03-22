using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCFList
{
    public class VCFFile
    {
        //--------------PULBIC VARIABLES
        public List<string> ColumnsList
        {
            get { return this._ColsList; }
        }
        
        public int ColumnsCount
        {
            get { return this._ColsList.Count; }
        }

        public List<VCFCard> CardList
        {
            get { return this._CardList; }
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

        public event EventHandler Changed;

        //--------------PRIVATE VARIABLES
        private static string _CardBeg = "BEGIN:VCARD";
        private static string _CardEnd = "END:VCARD";
        private static string _ENL = "\n";
        private string _FileName;
        private List<VCFCard> _CardList;
        private List<string> _ColsList;


        //--------------PUBLIC METHODS
        public VCFFile()
        {
            this._FileName = "";
            this._CardList = new List<VCFCard>();
            this._ColsList = new List<string>();
        }

        public VCFFile(string fileName)
        {
            if (this._CardList == null)
                this._CardList = new List<VCFCard>();
            if (this._ColsList == null)
                this._ColsList = new List<string>();
            this._FileName = fileName;
            this.ReadVCFFile();
        }

        public void Merge(VCFFile pVCFFile)
        {
            this._CardList.AddRange(pVCFFile.CardList);
        }

        public void Merge(string fileName)
        {
            VCFFile teFile = new VCFFile(fileName);
            Merge(teFile);
        }

        public void SaveToFile(string fileName)
        { 
            StreamWriter file = new StreamWriter(fileName);
            foreach(VCFCard teCard in this._CardList)
            {
                file.Write(VCFFile._CardBeg + VCFFile._ENL);
                /*
                foreach (VCFElement teElem in teCard.ElementsList)
                {
                    file.Write(teElem.Key + ":" + teElem.Info + VCFFile._ENL);
                }
                 */
                foreach (string teString in teCard._StrElement)
                {
                    file.Write(teString);
                }
                file.Write(VCFFile._CardEnd + VCFFile._ENL);
            }
            file.Close();
        }
        //--------------PROTECTED METHODS
        protected void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
            this.ReadVCFFile();
        }

        //--------------PRIVATE METHODS
        private int ReadVCFFile()
        {
            RawReadVCFFile();
            GetColsList();

            return 1;
        }

        private int RawReadVCFFile()
        {
            string fileLine, teStr = "";
            Boolean firstLine = true;
            VCFCard teCard = null;

            if (this._FileName != null)
            {
                if (File.Exists(this._FileName))
                {
                    StreamReader file = new StreamReader(this._FileName);

                    while ((fileLine = file.ReadLine()) != null)
                    {
                        fileLine += "\n";
                        if (fileLine.Contains(VCFFile._CardBeg))
                        {
                            teCard = new VCFCard();
                            firstLine = true;
                            continue;
                        }
                        if (fileLine.Contains(VCFFile._CardEnd))
                        {
                            teCard.AddStrElement(teStr);
                            teStr = "";
                            this._CardList.Add(teCard);
                            continue;
                        }
                        if (fileLine.Contains(":"))
                        {
                            if (!(firstLine))
                            {
                                teCard.AddStrElement(teStr);
                                teStr = "";
                            }
                            firstLine = false;
                        }
                        teStr += fileLine;
                    }
                }
            }

            return 1;
        }

        private int GetColsList()
        {
            List<string> telist = new List<string>();

            foreach(VCFCard elem in this._CardList)
            {
                telist = elem.KeysList;
                foreach (string testring in telist)
                {
                    if (this._ColsList.Find(x => x == testring) == null)
                    {
                        this._ColsList.Add(testring);
                    }
                }
            }

            return 1;
        }
    }
}
