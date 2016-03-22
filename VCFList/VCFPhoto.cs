using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace VCFList
{
    public class VCFPhoto
    {
        public Image Img 
        {
            get { return this._Img; }
        }

        public string CodedText
        {
            get { return this._UncodedString; }
        }

        private string _UncodedString;
        private Image _Img;

        public VCFPhoto() 
        {
        }

        public VCFPhoto(string photoString)
        {
            this._UncodedString = photoString.TrimEnd(new char[] { '\r', '\n' });
            this._Img = Base64ToImage(this._UncodedString);
        }

        public VCFPhoto(Image image)
        {
            this._Img = image;
            this._UncodedString = ImageToBase64(image, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            //StreamWriter sw = new StreamWriter("_$temp.$$$");
            //foreach (byte a in imageBytes)
                //sw.BaseStream.WriteByte(a);
            //sw.Close();
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static Bitmap ResizeToMax96(Image image)
        {
            if (image.Width < image.Height)
            {
                return ResizeImageProportionaly(image, -1, 96);
            }
            else
            {
                return ResizeImageProportionaly(image, 96, -1);
            }
        }

        public static Bitmap ResizeImageProportionaly(Image image, int width = -1, int height = -1)
        {
            double raport = 0;

            if ((width < 0) && (height < 0))
            {
                return null;
            }

            if (width < 0)
            {
                raport = image.Width * height / image.Height;
                width = (int)Math.Round(raport);

            }
            else if (height < 0)
            {
                raport = image.Height * width / image.Width;
                height = (int)Math.Round(raport);
            }
            else
            {
                return null;
            }

            return ResizeImage(image, width, height);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}
