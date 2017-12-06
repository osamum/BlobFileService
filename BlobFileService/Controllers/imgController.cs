using System.IO;
using System.Collections.Generic;
using BlobFileService.Models;
using BlobFileService.Helper;
using System.Web.Http;
using System.Web;
using ImageResizer;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace BlobFileService.Controllers
{
    public class imgController : ApiController
    {
        const string THUMBNAIL_CONTAINER_HEADER = "thumbs";

        [HttpPost()]
        public List<ImageInfo> UploadFiles(string containerName)
        {
            string thumbnailContainerName = HttpContext.Current.Request.QueryString["thumb"];
            if (thumbnailContainerName == "") {thumbnailContainerName = THUMBNAIL_CONTAINER_HEADER + containerName; }
            string sWidth = HttpContext.Current.Request.QueryString["w"];
            string sHeight = HttpContext.Current.Request.QueryString["h"];
            int thumbsWidth = (sWidth == "") ? 150 : int.Parse(sWidth);
            int thumbsHeight = (sHeight == "") ? 150 : int.Parse(sHeight);

            List<UpFile> upFiles = new List<UpFile>();
            List<UpFile> thumbFiles = new List<UpFile>();
            HttpFileCollection postedFiles = HttpContext.Current.Request.Files;
            int postedFilesCount = postedFiles.Count;
            for (int i = 0; i < postedFilesCount; i++)
            {
                HttpPostedFile postedFile = postedFiles[i];
                if (postedFile.ContentType.Contains("image")) {
                    var instructions = new Instructions
                    {
                        Width = thumbsWidth,
                        Height = thumbsHeight,
                        Mode = FitMode.Crop,
                        Scale = ScaleMode.Both
                    };

                    string fileName = FileService.getFileName(postedFile.FileName);
                    if (fileName != "")
                    {
                        int fileLen = postedFile.ContentLength;
                        byte[] input = new byte[fileLen];
                        Stream fileStream = postedFile.InputStream;
                        fileStream.Read(input, 0, fileLen);
                        fileStream.Position = 0;
                        UpFile upFile = new UpFile(fileName, fileStream, fileLen);
                        upFiles.Add(upFile);
                        MemoryStream thumbStream = new MemoryStream();
                        ImageBuilder.Current.Build(new ImageJob(postedFile, thumbStream, instructions));
                        thumbStream.Position = 0;
                        UpFile thumbFile = new UpFile(fileName, thumbStream, (int)thumbStream.Length);
                        thumbFiles.Add(thumbFile);
                    }
                }
            }
            return FileService.Upload(upFiles,thumbFiles, containerName.ToLower(), thumbnailContainerName);
        }

        
        //Return image file of specified size and format.
        public IHttpActionResult Get(string containerName)
        {
            string blobName = HttpContext.Current.Request.QueryString["name"];
            string sWidth = HttpContext.Current.Request.QueryString["w"];
            string sHeight = HttpContext.Current.Request.QueryString["h"];
            string sformat = HttpContext.Current.Request.QueryString["f"];
            string contentDsp = HttpContext.Current.Request.QueryString["cdsp"];
            int outPrm = 0;
            int.TryParse(sHeight, out outPrm);
            int imgHeight = outPrm;
            int.TryParse(sWidth, out outPrm);
            int imgWidth = outPrm;
            if (sformat == null || sformat == "") { sformat = "jpg"; }

            ChangeImageInfo chgImageInfo = new ChangeImageInfo(sformat.ToLower(), imgHeight, imgWidth);
            CloudBlobContainer container = Helper.FileService.GetContainerInstance(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            var memoryStream = new MemoryStream();
            blockBlob.DownloadToStream(memoryStream);
            byte[] buff = ImageService.ChangeImageFormat(memoryStream, chgImageInfo);
            if (contentDsp == "at")
            {
                string dlFileName = FileService.getFileNameWithoutEtn(blobName) + sformat;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + dlFileName + "\"");
            }
            HttpContext.Current.Response.ContentType = "image/" + sformat;
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.BinaryWrite(buff);
            HttpContext.Current.Response.End();
         
            return Ok(); 
        }
        

        // DELETE: api/Blob/5
        //Delete image file and thumbnail file from specified container.
        public IHttpActionResult Delete(string containerName)
        {
            string blobName = HttpContext.Current.Request.QueryString["name"];
            if (blobName == "") {
                return NotFound();
            }
            string thumbnailContainerName = HttpContext.Current.Request.QueryString["thumb"];
            if (thumbnailContainerName == "") { thumbnailContainerName = THUMBNAIL_CONTAINER_HEADER + containerName; }
            try
            {
                FileService.Delete(blobName, containerName);
                FileService.Delete(blobName, thumbnailContainerName);
                return Ok();
            }
            catch (Exception ex)
            {
                return new System.Web.Http.Results.ExceptionResult(ex, this);
            }
        }
    }
}
