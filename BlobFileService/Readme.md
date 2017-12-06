# BlobFileService
This service saves the file POSTed by HTTP/HTTPS in the Azure Blob and returns the URL can be accessed anonymously in json.  
When get an image file, you can specify the size of the image and the format of the image arbitrarily.  
  
This service feature is the follwoing:
* **File upload**
  - Auto create the  Blob container
  - Auto create the thumbnail picture file (When upload image files)
* **Enumerate Blobs in the container**
* **Delete Blob**
  - You can delete thumbnail file in same time when use img parameter
* **Get image file**
  - You can specify the size of the image and the format of the image arbitrarily when get image file
   
## Description
This service provid the following feature with API:
  
### File uplod  
You can upload multiple files using this service's REST API.  
  
**[Request]**  
URL : _%your domain name%_/api/**blob**/_%container name%_  
Method : POST
  
For example if you upload to container name is "**mycontainer**" then you have to use the following url.
  
_%your domain name%_/api/blob/**mycontainer**
  
**CAUTION**: Container name used only small-case alphabet and numeric.  
  
Please use file control (&lt;iput type="file"&gt;) in form or Formdata in JavaScript to post files.
Multiple files can be specified in one post.  
  
**[Response]**  
Response JSON format is the following:

[{  
&nbsp;&nbsp;"name":"_File name_",  
&nbsp;&nbsp;"url":"_Blob url_"  
&nbsp;}, {  
&nbsp;&nbsp;_%Next blob information%_  
}]
  
  
  
### Image file upload  
Auto create the thumbnail picture file when upload image file and use **img** parameter.

**[Request]**    
URL : _%your domain name%_/api/**img**/_%Container name%_  
Method : POST
  
You can use following the query strings:  

| Query string| Type|Description|
| :-:|:-:|:-|
| thumb|string|Thumbnail image container name(*)|
| w |Numeric|Thumbnail image width|
| h |Numeric|Thumbnail image height|   

(*)If you omit it, the following folders are automatically generated.  
**thumbs**_%your container name%_  
  
For example if you upload to container name is "**mycontainer**",thumbnail container name is "**thumbs**",thumbnail imaze size is **200**x**200**px then you have to use the following url.
  
_%your domain name%_/api/img/**mycontainer**?thumb=**thumbs**&w=**200**&h=**200**  
  
**[Response]**  
Response JSON format is the following:  

[{  
&nbsp;&nbsp;"name":"_File name_",  
&nbsp;&nbsp;"url":"_Blob url_",  
&nbsp;&nbsp;"thumbnailUrl":"_Thumbnail blob url_",    
&nbsp;}, {  
&nbsp;&nbsp;_%Next blob information%_  
}]
  
  
### Enumlate Blob in container  
You can get a blob list of any container.  
  
**[Request]**  
URL : _%your domain name%_/api/**blob**/_%container name%_  
Method : GET  
  
For example, to get a list of blobs from a container named "**mycontainer**", use the following URL:  
  
_%your domain name%_/api/blob/**mycontainer**
  
  
### Flexible Change image format and size
You can get any image format and size from one image blob.  
  
**[Request]**   
URL : _%your domain name%_/api/**img**/_%container name%_  
Method : GET  
 
| Query string| Type|Description|
| :-:|:-:|:-|
| name|String|Blob name|
| w |Numeric|Thumbnail image width|
| h |Numeric|Thumbnail image height
| f |String|Image format<br>(jpg/gif/bmp/png/tiff/icon)|
| cdsp |String|Value of Content-Dispotsion in HTTP headers.<br>("at"=attachment,""=inline)|          

For example, to display **cat.png** in **mycontainer** as a jpeg image of 200 x 200 px size, specify the following URL. 
  
 _%your domain name%_/api/img/**mycontainer**?name=**cat.png**&f=**jpg**&h=**200**&w=**200**

### Delete Blob 
You can delete any blob.
    
**[Request]**   
_%your domain name%_/api/**blob**/_%Container name%_?name=_%Blob name%_  
  
Method : DELETE  
  
For example, to delete **cat.png** in **mycontainer** then specify the following the URL.  
_%your domain name%_/api/blob/**mycontainer**/?name=**cat.png** 
  
**[Response]**  
HTTP staus.


### Delete Blob and thumbnail Blob  
You can delete any Blob and thumbnail Blob in same time.
  
**[Request]**   
_%your domain name%_/api/**img**/_%Container name%_?name=_%Blob name%_ & _thumb=%Thumbnail Blob name%_
  
Method : DELETE  
  
**[Response]**  
HTTP staus.
  
## Requirement  
To open and deploy this project you need the following items :   
* [Microsoft Azure account](https://azure.microsoft.com/)
* [Visual Studio 2017 with latest update](https://www.visualstudio.com/)  

## Usage  
These service are can be used by HTML tag and JavaScript.  
This project does including test page.  
Please access the following the URL:  
  
_%your domain name%_/index.html  
  
The sample code below is included in the project.

**[HTML tag]**      
When using from HTML, specify the URL of this API in the action attribute of the form tag.  
For example, when uploading 3 files to a "mycontainer", the form tag looks like this:
  
`<form id="filesForm" action="api/blob/mycontainer" method="post" enctype="multipart/form-data">`  
`<input type="file" name="postedFile1" required />`  
`<br>`  
`<input type="file" name="postedFile2" />`  
`<br>`  
`<input type="file" name="postedFile3" />`  
`<br>`   
`<input type="submit" value="Post" />`  
  
**[JavaScript]**  
When using JavaScript is the following:  
`uploadButton.addEventListener('click', function () {`  
`　　var formData = new FormData(document.getElementOfId('filesForm'));`  
`　　var xhr = new XMLHttpRequest();`  
`　　xhr.onreadystatechange = function() {`  
`　　　　　　if (xhr.readyState == 4 && xhr.status == 200) {`  
`　　　　　　　　createList(xhr.responseText);`  
`　　　　　　}`  
`　　　　};`  
`　　xhr.open("POST", "api/blob/mycontainer", true);`  
`　　xhr.send(formData);`  
`});`

## How to open and execute project
  
### Preparation
* [Create Azure storage account](https://docs.microsoft.com/azure/storage/common/storage-create-storage-account)
* Get Azure storage account access key

### Open project
1. Open **BlobFileService.sln** file using Visual Studio 2017.
2. Open Web.config from Solution Explorer in Visual Studio 2017.
3. Add the following the schema to &lt;configuration&gt; node in Web.config.  
  `<add key="StorageConnectionString" value="Your Azure storage account access key" />;`
4. Click menu [BUILD] - [Rebuild solution]  

You can now run the project on Visual Studio with the above settings.
  
## Deploy to Microsoft Azure 
About how to ASP.NET app deploy to Azure please see **Publish to Azure** section in the following the document:  
  
[Create an ASP.NET Framework web app in Azure](https://docs.microsoft.com/azure/app-service/app-service-web-get-started-dotnet-framework)






