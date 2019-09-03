using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;

namespace UploadFileTest1
{
    public class AzureStorageMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        private readonly CloudBlobContainer _blobContainer;

       

        public AzureStorageMultipartFormDataStreamProvider(CloudBlobContainer blobContainer) : base("azure")
        {
            _blobContainer = blobContainer;
           

        }

        

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (headers == null) throw new ArgumentNullException(nameof(headers));
         

           
            string fileName = headers.ContentDisposition.FileName;

            //retrieve reference to a blob
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(fileName);

            if (headers.ContentType != null)
            {
                // Set appropriate content type for the uploaded file
                blob.Properties.ContentType = headers.ContentType.MediaType;        
            }


            this.FileData.Add(new MultipartFileData(headers, blob.Name));
 
            

            //write to blob
            return blob.OpenWrite();


        }

       
       

        

    }
}