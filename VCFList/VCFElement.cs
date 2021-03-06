﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Threading.Tasks;

namespace VCFList
{
    #region VCFElement Class
    public class VCFElement
    {
        #region public_param
        public string Name
        { 
            get { return this._Name; }
            set
            {
                ProcessName(value);
            }
        }
        public string Value
        { 
            get { return this._Value; }
            set
            {
                ProcessValue(value);
            }
        }
        public string Parameters { get { return this._Parameters; } }
        #endregion

        #region private_param
        private string _Name;
        private string _Value;
        private string _Parameters;
        private enum charset_types { def, utf_8 };
        private charset_types _charset;
        private enum encoding_types { def, qp }
        private encoding_types _encoding;
        #endregion

        #region contructors
        public VCFElement()
        {
            this._Name = "";
            this._Value = "";
            this._Parameters = "";
            this._charset = charset_types.def;
            this._encoding = encoding_types.def;
        }

        public VCFElement(string value)
        {
            UppdateInfo(value);
        }
        #endregion

        #region Interacting_public_methods
        public void AddInfo(string value)
        {
            if (this._Name == "")
            {
                UppdateInfo(value);
            }
            else
            {
                throw new Exception("You cannot add info to an already completed object!");
            }
        }

        public void UppdateInfo(string value)
        {
            if (CheckForRightness(value))
            {
                this._Name = "";
                this._Value = "";
                this._Parameters = "";
                this._charset = charset_types.def;
                this._encoding = encoding_types.def;

                value = value.Replace("\n", "");
                ProcessString(value);
            }
            else
            {
                throw new Exception("There is an no name element! This cannot be added");
            }
        }

        public override string ToString()
        {
            string resultStr;


            resultStr = this._Name + DoParamAndValueString();

            return resultStr;
        }
        #endregion

        #region Private_methods
        private string DoParamAndValueString()
        {
            if (this._Name.ToUpper() == "PHOTO")
            {
                return
                    (((this.Parameters != "") && (this.Parameters != null)) ?
                            ";" + this._Parameters + ":" + this._Value
                        : ":" + this._Value);
                            
            }
            return 
                (((this.Parameters != "") && (this.Parameters != null)) ?
                    ";" + this._Parameters + ":" + "=" + BitConverter.ToString(Encoding.UTF8.GetBytes(this._Value)).Replace('-', '=')
                : ":" + this._Value);
        }

        private Boolean CheckForRightness(string ElementString)
        {
            if (GetName(ElementString) == "")
            {
                return false;
            }
            return true;
        }

        private void ProcessString(string value)
        {
            string teStr1, teStr2;

            if (value.IndexOf(":") >= 0)
            {
                teStr1 = value.Split(':')[0];
                teStr2 = value.Split(':')[1];

                this._Name = CheckKeyForParameters(teStr1);
                this._Value = DoEncodingProcess(teStr2);
            }
            else
            {
                this._Name = CheckKeyForParameters(value);
                this._Value = "";
            }
        }

        private string CheckKeyForParameters(string value)
        {
            string[] stringList;
            string[] paramStringList;
            string teStr;

            stringList = value.Split(';');
            if (stringList.Length > 1)
            {
                paramStringList = new string[stringList.Length - 1];
                Array.Copy(stringList, 1, paramStringList, 0, stringList.Length - 1);
                this._Parameters = string.Join(";", paramStringList);

                if (stringList[0].ToUpper().Contains("PHOTO"))
                {
                    Array.Copy(stringList, 1, paramStringList, 0, stringList.Length - 1);
                    this._Parameters = string.Join(";", paramStringList);
                    return stringList[0];
                }

                teStr = Array.Find(stringList, s => s.ToUpper().Contains("CHARSET"));
                if (teStr != null)
                {
                    if (teStr.ToUpper().Contains("UTF-8"))
                    {
                        this._charset = charset_types.utf_8;
                    }
                    stringList = stringList.Where(val => !(val.ToUpper().Contains("CHARSET"))).ToArray();
                }

                teStr = Array.Find(stringList, s => s.ToUpper().Contains("ENCODING"));
                if (teStr != null)
                {
                    if (teStr.ToUpper().Contains("QUOTED-PRINTABLE"))
                    {
                        this._encoding = encoding_types.qp;
                    }
                    stringList = stringList.Where(val => !(val.ToUpper().Contains("ENCODING"))).ToArray();
                }
            }
            return string.Join(";", stringList);
        }

        private string DoEncodingProcess(string value)
        {
            string[] stringList, stringList2;
            string teRes = "", teStr = "";

            switch (this._encoding)
            {
                case encoding_types.qp:
                    stringList = value.Split(';');
                    foreach (string teString1 in stringList)
                    {
                        stringList2 = teString1.Split('=');
                        teStr = "";
                        foreach (string teString2 in stringList2)
                        {
                            teStr += teString2;
                        }
                        teRes += DoCharsetProcess(teStr) + ";";
                    }
                    teRes = teRes.Substring(0, teRes.Length - 1);
                    break;
                default:
                    teRes = value;
                    break;
            }

            return teRes;
        }

        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private string ByteArrayToBinHex(byte[] bytes)
        { 
            return bytes.Select(b => b.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
        }

        private string DoCharsetProcess(string value)
        {
            byte[] teBytesList = null;
            string teRes = "";

            switch (this._charset)
            {
                case charset_types.utf_8:
                    teBytesList = StringToByteArray(value);
                    teRes = Encoding.UTF8.GetString(teBytesList);
                    break;
            }

            return teRes;
        }

        private Boolean IsTheStringAscii(string value)
        {
            string tstring = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
            return (string.Compare(tstring, value) == 0);
        }

        private void ProcessName(string value)
        {
            if (!(IsTheStringAscii(value)))
            {
                throw new Exception("You name contains forbidden chars!");
            }
            else
            {
                if (VCFTypes.IsInVCFFields(value))
                {
                    this._Name = value;
                }
                else
                {
                    throw new Exception("You name is not in the names list!");
                }
            }
        }

        private void ProcessValue(string value)
        {
            if (!(IsTheStringAscii(value)))
            {
                this._charset = charset_types.utf_8;
                this._encoding = encoding_types.qp;

            }
            else
            {
                this._charset = charset_types.def;
                this._encoding = encoding_types.def;
            }

            this._Value = value;
        }
        #endregion

        #region Public_Static_functions
        public static string GetName(string ElementString)
        {
            if (ElementString.IndexOf(':') >= 0)
                return ElementString.Split(':')[0];
            else
                return "";
        }

        public static string GetValue(string ElementString)
        {
            if (ElementString.IndexOf(':') >= 0)
                return ElementString.Split(':')[0];
            else
                return "";
        }
        #endregion
    }
    #endregion

    #region VCFTypes_enums Class
    public class VCFTypes 
    {
        enum Types { V2_1, V3_0, V4_0 };
        enum Fields {         // 2.1|3.0|4.0| Description -> Example
            //none,
            ADR,            //  S | S | S | A structured representation of the physical delivery address for the vCard object.  ADR;TYPE=home:;;123 Main St.;Springfield;IL;12345;USA
            AGENT,          //  S | S |   | Information about another person who will act on behalf of the vCard object. Typically this would be an area administrator, assistant, or secretary for the individual. Can be either a URL or an embedded vCard.   AGENT:http://mi6.gov.uk/007
            ANNIVERSARY,    //    |   | S | Defines the person's anniversary.   ANNIVERSARY:19901021
            BDAY,           //  S | S | S | Date of birth of the individual associated with the vCard.  BDAY:19700310
            BEGIN,          //  R | R | R | All vCards must start with this property.   BEGIN:VCARD
            CALADRURI,      //    |   | S | A URL to use for sending a scheduling request to the person's calendar. CALADRURI:http://example.com/calendar/jdoe
            CALURI,         //    |   | S | A URL to the person's calendar. CALURI:http://example.com/calendar/jdoe
            CATEGORIES,     //  S | S | S | A list of "tags" that can be used to describe the object represented by this vCard. CATEGORIES:swimmer,biker
            CLASS,          //    | S |   | Describes the sensitivity of the information in the vCard.  CLASS:public
            CLIENTPIDMAP,   //    |   | S | Used for synchronizing different revisions of the same vCard.   CLIENTPIDMAP:1;urn:uuid:3df403f4-5924-4bb7-b077-3c711d9eb34b
            EMAIL,          //  S | S | S | The address for electronic mail communication with the vCard object.    EMAIL:johndoe@hotmail.com
            END,            //  R | R | R | All vCards must end with this property. END:VCARD
            FBURL,          //    |   | S | Defines a URL that shows when the person is "free" or "busy" on their calendar. FBURL:http://example.com/fb/jdoe
            FN,             //  S | R | R | The formatted name string associated with the vCard object. FN:Dr. John Doe
            GENDER,         //    |   | S | Defines the person's gender.    GENDER:F
            GEO,            //  S | S | S | Specifies a latitude and longitude. 2.1, 3.0: GEO:39.95;-75.1667 4.0: GEO:geo:39.95,-75.1667
            IMPP,           //    |SP*| S | Defines an instant messenger handle.
                            //  X | X | X | * This property was introduced in a separate RFC when the latest vCard version was 3.0. Therefore, 3.0 vCards may use this property, even though it's not part of the 3.0 specifications.   IMPP:aim:johndoe@aol.com
            KEY,            //  S | S | S | The public encryption key associated with the vCard object. It may point to an external URL, may be plain text, or may be embedded in the vCard as a Base64 encoded block of text.  2.1: KEY;PGP:http://example.com/key.pgp
                            //  X | X | X | 2.1: KEY;PGP;ENCODING=BASE64:[base64-data]
                            //  X | X | X | 3.0: KEY;TYPE=PGP:http://example.com/key.pgp
                            //  X | X | X | 3.0: KEY;TYPE=PGP;ENCODING=B:[base64-data]
                            //  X | X | X | 4.0: KEY;MEDIATYPE=application/pgp-keys:http://example.com/key.pgp
                            //  X | X | X | 4.0: KEY:data:application/pgp-keys;base64,[base64-data]
            KIND,           //    |   | S | Defines the type of entity that this vCard represents: 'application', 'individual, 'group', 'location' or 'organization'; 'x-*' values may be used for experimental purposes.[5][6]	KIND:individual
            LABEL,          //  S | S |NS*| Represents the actual text that should be put on the mailing label when delivering a physical package to the person/object associated with the vCard (related to the ADR property).
                            //  X | X | X | * Not supported in version 4.0. Instead, this information is stored in the LABEL parameter of the ADR property.	LABEL;TYPE=HOME:123 Main St.\nSpringfield, IL 12345\nUSA
            LANG,           //    |   | S | Defines a language that the person speaks.  LANG:fr-CA
            LOGO,           //  S | S | S | An image or graphic of the logo of the organization that is associated with the individual to which the vCard belongs. It may point to an external URL or may be embedded in the vCard as a Base64 encoded block of text.   2.1: LOGO;PNG:http://example.com/logo.png
                            //  X | X | X | 2.1: LOGO;PNG;ENCODING=BASE64:[base64-data]
                            //  X | X | X | 3.0: LOGO;TYPE=PNG:http://example.com/logo.png
                            //  X | X | X | 3.0: PHOTO;TYPE=PNG;ENCODING=B:[base64-data]
                            //  X | X | X | 4.0: LOGO;MEDIATYPE=image/png:http://example.com/logo.png
                            //  X | X | X | 4.0: PHOTO:data:image/png;base64,[base64-data]
            MAILER,         //  S | S |   | Type of email program used. MAILER:Thunderbird
            MEMBER,         //    |   | S | Defines a member that is part of the group that this vCard represents. Acceptable values include:
                            //  X | X | X | a "mailto:" URL containing an email address
                            //  X | X | X | a UID which references the member's own vCard
                            //  X | X | X | The KIND property must be set to "group" in order to use this property.	MEMBER:urn:uuid:03a0e51f-d1aa-4385-8a53-e29025acd8af
            N,              //  R | R | S | A structured representation of the name of the person, place or thing associated with the vCard object. N:Doe;John;;Dr;
            NAME,           //    | S |   | Provides a textual representation of the SOURCE property.	
            NICKNAME,       //    | S | S | One or more descriptive/familiar names for the object represented by this vCard.    NICKNAME:Jon,Johnny
            NOTE,           //  S | S | S | Specifies supplemental information or a comment that is associated with the vCard.  NOTE:I am proficient in Tiger-Crane Style,\nand I am more than proficient in the exquisite art of the Samurai sword.
            ORG,            //  S | S | S | The name and optionally the unit(s) of the organization associated with the vCard object. This property is based on the X.520 Organization Name attribute and the X.520 Organization Unit attribute.    ORG:Google;GMail Team;Spam Detection Squad
            PHOTO,          //  S | S | S | An image or photograph of the individual associated with the vCard. It may point to an external URL or may be embedded in the vCard as a Base64 encoded block of text.  2.1: PHOTO;JPEG:http://example.com/photo.jpg
                            //  X | X | X | 2.1: PHOTO;JPEG;ENCODING=BASE64:[base64-data]
                            //  X | X | X | 3.0: PHOTO;TYPE=JPEG:http://example.com/photo.jpg
                            //  X | X | X | 3.0: PHOTO;TYPE=JPEG;ENCODING=B:[base64-data]
                            //  X | X | X | 4.0: PHOTO;MEDIATYPE=image/jpeg:http://example.com/photo.jpg
                            //  X | X | X | 4.0: PHOTO:data:image/jpeg;base64,[base64-data]
            PRODID,         //    | S | S | The identifier for the product that created the vCard object.   PRODID:-//ONLINE DIRECTORY//NONSGML Version 1//EN
            PROFILE,        //  S | S |   | States that the vCard is a vCard.   PROFILE:VCARD
            RELATED,        //    |   | S | Another entity that the person is related to. Acceptable values include:
                            //  X | X | X | a "mailto:" URL containing an email address
                            //  X | X | X | a UID which references the person's own vCard
                            //  X | X | X | RELATED;TYPE=friend:urn:uuid:03a0e51f-d1aa-4385-8a53-e29025acd8af
            REV,            //  S | S | S | A timestamp for the last time the vCard was updated.    REV:20121201T134211Z
            ROLE,           //  S | S | S | The role, occupation, or business category of the vCard object within an organization.  ROLE:Executive
            SORT_STRING,    //  S | S |NS*| Defines a string that should be used when an application sorts this vCard in some way.
                            //  X | X | X | * Not supported in version 4.0. Instead, this information is stored in the SORT-AS parameter of the N and/or ORG properties.	SORT-STRING:Doe
            SOUND,          //  S | S | S | By default, if this property is not grouped with other properties it specifies the pronunciation of the FN property of the vCard object. It may point to an external URL or may be embedded in the vCard as a Base64 encoded block of text.	2.1: SOUND;OGG:http://example.com/sound.ogg
                            //  X | X | X | 2.1: SOUND;OGG;ENCODING=BASE64:[base64-data]
                            //  X | X | X | 3.0: SOUND;TYPE=OGG:http://example.com/sound.ogg
                            //  X | X | X | 3.0: SOUND;TYPE=OGG;ENCODING=B:[base64-data]
                            //  X | X | X | 4.0: SOUND;MEDIATYPE=audio/ogg:http://example.com/sound.ogg
                            //  X | X | X | 4.0: SOUND:data:audio/ogg;base64,[base64-data]
            SOURCE,         //  S | S | S | A URL that can be used to get the latest version of this vCard. SOURCE:http://johndoe.com/vcard.vcf
            TEL,            //  S | S | S | The canonical number string for a telephone number for telephony communication with the vCard object.   TEL;TYPE=cell:(123) 555-5832
            TITLE,          //  S | S | S | Specifies the job title, functional position or function of the individual associated with the vCard object within an organization. TITLE:V.P. Research and Development
            TZ,             //  S | S | S | The time zone of the vCard object.  2.1, 3.0: TZ:-0500
                            //  X | X | X | 4.0: TZ:America/New_York
            UID,            //  S | S | S | Specifies a value that represents a persistent, globally unique identifier associated with the object.  UID:urn:uuid:da418720-3754-4631-a169-db89a02b831b
            URL,            //  S | S | S | A URL pointing to a website that represents the person in some way. URL:http://www.johndoe.com
            VERSION,        //  R | R | R | The version of the vCard specification. In versions 3.0 and 4.0, this must come right after the BEGIN property. VERSION:3.0
            XML             //    |   | S | Any XML data that is attached to the vCard. This is used if the vCard was encoded in XML (xCard standard) and the XML document contained elements which are not part of the xCard standard.	XML:<b>Not an xCard XML element</b>
        }

        public static Boolean IsInVCFFields(string value)
        {
            Fields teField;
            try
            {
                teField = (Fields)Enum.Parse(typeof(Fields), value.ToUpper());
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
    #endregion
}
