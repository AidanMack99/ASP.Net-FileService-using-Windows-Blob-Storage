using Microsoft.AspNetCore.Http;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UploadFileTest1.Models;

namespace UploadFileTest1.Controllers
{

    [RoutePrefix("api/upload")]
    public class UploadController : ApiController
    {
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> UploadFile()
        {


            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new AzureStorageMultipartFormDataStreamProvider(BlobHelper.GetWebApiContainer());

            
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                

            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }

            
            string originalFileName = provider.Contents[0].Headers.ContentDisposition.FileName;


            //Capture File details and store in SQL Database "BlobFiles"
            SqlCommand cmd;
            SqlConnection con;
            
            CloudBlockBlob blob = BlobHelper.GetWebApiContainer().GetBlockBlobReference(originalFileName);
            con = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = BlobFiles; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
            con.Open();
            blob.FetchAttributes();
            string extension = Path.GetExtension(blob.Uri.AbsolutePath);
            cmd = new SqlCommand("INSERT INTO Files (Name,Size,ContentType,Extension,Location,Timestamp,FilePath) VALUES (@Name,@Size,@ContentType,@Extension,@Location,@Timestamp,@FilePath)", con);
            cmd.Parameters.AddWithValue("@Name", blob.Name);
            cmd.Parameters.AddWithValue("@Size", ((blob.Properties.Length) / 1000) + "kb");
            cmd.Parameters.AddWithValue("@ContentType", blob.Properties.ContentType);
            cmd.Parameters.AddWithValue("@Extension", extension.Substring(0, extension.Length - 3));
            cmd.Parameters.AddWithValue("@Location", blob.Uri.AbsoluteUri);
            cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString());
            cmd.Parameters.AddWithValue("@FilePath", blob.Uri.AbsolutePath);
            cmd.ExecuteNonQuery();

            if (string.IsNullOrEmpty(originalFileName))
            {
                return BadRequest("An error has occured while uploading your file. Please try again.");
            }
            
            return Ok($"File: {originalFileName} has successfully uploaded");
        }
  
    }


    







    

        


    }






