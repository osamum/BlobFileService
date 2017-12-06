using BlobFileService.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BlobFileService.Helper
{
    public static class ImageService
    {
        //Change image format and size.
        static public byte[] ChangeImageFormat(Stream imgStrm, ChangeImageInfo chgImageInfo)
        {
            Stream changedStrm;
            byte[] pixelByteArray;
            bool noSizeFlg = (chgImageInfo.width == 0 && chgImageInfo.height == 0);
            Bitmap bmp;

            if (noSizeFlg)
            {
                //When the image size and format to be converted is not designated.
                bmp = new Bitmap(Image.FromStream(imgStrm));
            }
            else
            {
                //When the image size and format to be converted is designated.
                bmp = new Bitmap(Image.FromStream(imgStrm), chgImageInfo.width, chgImageInfo.height);
            }

            changedStrm = new MemoryStream();
            bmp.Save(changedStrm, selectImageFormat(chgImageInfo.format));
            changedStrm.Position = 0;
            long strmLength = changedStrm.Length;
            pixelByteArray = new byte[strmLength];
            changedStrm.Read(pixelByteArray, 0, (int)strmLength);
            return pixelByteArray;
        }

        //Setting image format from content of query string. 
        static public ImageFormat selectImageFormat(string imgformat)
        {
            switch (imgformat)
            {
                case "jpg": return ImageFormat.Jpeg;
                case "gif": return ImageFormat.Gif;
                case "bmp": return ImageFormat.Bmp;
                case "png": return ImageFormat.Png;
                case "tiff": return ImageFormat.Tiff;
                case "icon": return ImageFormat.Icon;
                default: return ImageFormat.Jpeg;
            }
        }
    }
}