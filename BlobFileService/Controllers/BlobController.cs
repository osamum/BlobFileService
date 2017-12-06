using System.Collections.Generic;
using BlobFileService.Models;
using BlobFileService.Helper;
using System.Web.Http;
using System.Web;
using System;

namespace BlobFileService.Controllers
{
    public class BlobController : ApiController
    {
        [HttpPost()]
        public List<BlobInfo> UploadFiles(string containerName)
        {
            List<UpFile> upFiles = new List<UpFile>();
            HttpFileCollection postedFiles = HttpContext.Current.Request.Files;
            int postedFilesCount = postedFiles.Count;
            for (int i = 0; i < postedFilesCount; i++)
            {
                HttpPostedFile postedFile = postedFiles[i];
                string fileName = FileService.getFileName(postedFile.FileName);
                if (fileName != "")
                {
                    int fileLen = postedFile.ContentLength;
                    byte[] input = new byte[fileLen];
                    System.IO.Stream fileStream = postedFile.InputStream;
                    fileStream.Read(input, 0, fileLen);
                    fileStream.Position = 0;
                    UpFile upFile = new UpFile(fileName, fileStream, fileLen);
                    upFiles.Add(upFile);
                }
            }
            return FileService.Upload(upFiles, containerName.ToLower());
        }

        // GET: api/Blob/5
        public List<FileBlob> Get(string containerName)
        {
            return Helper.FileService.EnumBlobs(containerName.ToLower());
        }

        // DELETE: api/Blob/5
        public IHttpActionResult Delete(string containerName)
        {
            string blobName = HttpContext.Current.Request.QueryString["name"];
            try
            {
                Helper.FileService.Delete(blobName, containerName.ToLower());
                return Ok();
            }
            catch (Exception ex) {
                return new System.Web.Http.Results.ExceptionResult(ex, this);
            }
        }
    }
}
