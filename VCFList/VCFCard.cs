using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCFList
{
    public class VCFCard
    {
        //KeyList is the list of the keys that are in this Card
        public List<string> KeysList
        {
            get { return GetElementsKeyList(); }
        }

        //The public list of elements that are in this Card
        public List<VCFElement> ElementsList
        {
            get { return this._ElementsList; }
        }

        //The list of strings that form this Card
        public List<string> StrElement;

        //The private list of elements that are in this Card
        private List<VCFElement> _ElementsList;
        //Total number of elements in this Card
        private int NbrOfElements;

        public VCFCard()
        {
            this.StrElement = new List<string>();
            this._ElementsList = new List<VCFElement>();
            this.NbrOfElements = 0;
        }

        //Add an element to the Card object
        public void AddStrElement(string value)
        {
            VCFElement teelement = new VCFElement();

            this.StrElement.Add(value);
            teelement.AddInfo(value);
            this._ElementsList.Add(teelement);
            this.NbrOfElements++;
        }

        private List<string> GetElementsKeyList()
        {
            List<string> locList = new List<string>();

            foreach (VCFElement elem in this._ElementsList)
            {
                locList.Add(elem.Name);
            }

            return locList;
        }
    }
}
