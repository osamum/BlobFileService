using BlobFileService.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Collections.Generic;


namespace BlobFileService.Helper
{
    static public class FileService
    {
        //Upload files to Azure blob.
        static public List<BlobInfo> Upload(List<UpFile> upFiles, string containerName)
        {
            CloudBlobContainer container = GetContainerInstance(containerName);
            //If not exist specified container, create container and allow anonymous access.
            if (container.CreateIfNotExists()){
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            
            List<BlobInfo> BlobInfos = new List<BlobInfo>();
            foreach (UpFile upFile in upFiles) {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(upFile.name);
                blockBlob.UploadFromStream(upFile.contentStream, upFile.lenght);
                BlobInfo blobInfo = new BlobInfo(upFile.name, blockBlob.Uri.ToString());
                BlobInfos.Add(blobInfo);
            }
            return BlobInfos;
        }

        //Upload files and thumbnail files to Azure Blob.
        static public List<ImageInfo> Upload(List<UpFile> upFiles, List<UpFile> upThumbFiles, string containerName, string thumbContainerName)
        {
            CloudBlobContainer container = GetContainerInstance(containerName);
            CloudBlobContainer thumbContainer = GetContainerInstance(thumbContainerName);
            //If not exist specified container, create container and allow anonymous access.
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            if (thumbContainer.CreateIfNotExists())
            {
                thumbContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            int length = upFiles.Count;
            List<ImageInfo> imageInfos = new List<ImageInfo>();

            for (int i = 0; i < length; i++)
            {
                UpFile upFile = upFiles[i];
                UpFile upThumbFile = upThumbFiles[i];
                CloudBlockBlob fileBlob = container.GetBlockBlobReference(upFile.name);
                CloudBlockBlob thumbBlob = thumbContainer.GetBlockBlobReference(upThumbFile.name);

                fileBlob.UploadFromStream(upFile.contentStream, upFile.lenght);
                thumbBlob.UploadFromStream(upThumbFile.contentStream, upThumbFile.lenght);

                ImageInfo blobInfo = new ImageInfo(upFile.name, fileBlob.Uri.ToString(), thumbBlob.Uri.ToString());
                imageInfos.Add(blobInfo);
            }
            return imageInfos;
        }

        //Return Blob list in specifyed container.
        static public List<FileBlob> EnumBlobs(string containerName) {
            CloudBlobContainer container = GetContainerInstance(containerName);
            List<FileBlob> files = new List<FileBlob>();
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                FileBlob fileBlob = new FileBlob();
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    fileBlob.length = (int)blob.Properties.Length;
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    fileBlob.length = (int)pageBlob.Properties.Length;
                }
               
                string url = item.Uri.ToString();
                fileBlob.name = Helper.FileService.getFileName(url,'/');
                fileBlob.url = url;
                fileBlob.type = getFileName(item.GetType().ToString(),'.');
                files.Add(fileBlob);
            }
            return files;
        }

        //Delete Blob in specifyed container.
        static public bool Delete(string blobName, string containerName) {
            CloudBlobContainer container = GetContainerInstance(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.Delete();
            return true;
        }
        
        //Retrun instance for connect to Azure container.
        static public CloudBlobContainer GetContainerInstance(string containerName) {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        //Return filename of not contain directory name.
        static public string getFileName(string fullName, char splitChar= '\\')
        {
            string[] dirNames = fullName.Split(splitChar);
            if (dirNames.Length > 0)
            {
                return dirNames[dirNames.Length - 1];
            }
            else
            {
                return fullName;
            }
        }

        //Return filename of not contain extension name.
        static public string getFileNameWithoutEtn(string fileName) {
            string[] parts = fileName.Split('.');
            if (parts.Length >= 2)
            {
                return fileName.Substring(0, fileName.Length -  parts[parts.Length -1].Length);
            }
            else {
                return fileName;
            }
        }
    }


}