using System.IO;

namespace BlobFileService.Models
{
    //This class store upload file information.
    public class UpFile
    {
        public UpFile(string _name, Stream _contentStream, int _length)
        {
            this.name = _name;
            this.contentStream = _contentStream;
            this.lenght = _length;
        }
        public string name { get; set; }
        public Stream contentStream { get; set; }
        public int lenght { get; set; }
    }

    //This class store return file infomation from function.
    public class BlobInfo {
        public BlobInfo(string _name, string _url) {
            this.name = _name;
            this.url = _url;
        }
        public string name { get; set; }
        public string url { get; set; }
    }

    //This class store return image file infomation from function.
    public class ImageInfo : BlobInfo {
        public ImageInfo(string _name, string _url, string _thumbnailUrl) : base(_name, _url){
            this.name = _name;
            this.url = _url;
            this.thumbnailUrl = _thumbnailUrl;
        }
        public string thumbnailUrl { get; set; }
    }

    //This class store blob information for Web API response what is item of file list.
    public class FileBlob
    {
        public string name { get; set; }
        public string url { get; set; }
        public int length { get; set; }
        public string type { get; set; }
    }

    //This class store information of the image format to be change.
    public class ChangeImageInfo {
        public ChangeImageInfo(string _format, int _height, int _width) {
            this.format = _format;
            this.height = _height;
            this.width = _width;
        }
        public string format { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}