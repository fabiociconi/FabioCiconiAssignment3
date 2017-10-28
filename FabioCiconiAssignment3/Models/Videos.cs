using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FabioCiconiAssignment3.Models
{
    public class Videos
    {
        public string IdVideo { get; set; }
        [Required(ErrorMessage = "Please insert a Title")]
        public string Title { get; set; }
        public string LocalVideo { get; set; }
        public string GoogleUrl { get; set; }
        public string DestinationVideo { get; set; }
        public List<Commentaries> ListComm { get; set; }

        // Your Google Cloud Platform project ID.
        private string projectID = "erudite-concord-181214";
        // The name for the new bucket.
        private string bucketName = "erudite-concord-181214-test-bucket";

        public IConfigurationRoot Configuration;
        public Videos()
        {
            ListComm = new List<Commentaries>();
        }
        public void DownloadVideoFromGoogleCloud()
        {
            StorageClient sc = StorageClient.Create();       
            using (var stream = File.OpenWrite(DestinationVideo))
            {
                sc.DownloadObject(bucketName, IdVideo, stream);
            }
        }
        public void UploadVideos()
        {
            //*   TO DO --- check user
           
            // Instantiates a client.
            StorageClient sc = StorageClient.Create();

            try
            {
                // Creates the new bucket.
                sc.CreateBucket(projectID, bucketName);
                Console.WriteLine($"Bucket {bucketName} created.");
            }
            catch (Google.GoogleApiException e)

            when (e.Error.Code == 409)
            {

                // The bucket already exists.  That's fine.
                Console.WriteLine(e.Error.Message);
            }
            finally
            {
                string idMovie = "100" + Title;
                FileStream fileStream = new FileStream(LocalVideo,FileMode.Open,FileAccess.Read);
                var obj1 = sc.UploadObject(bucketName, idMovie, "video/mp4", fileStream);        
                GoogleUrl = "https://storage.cloud.google.com/" + bucketName + "/" + idMovie +".mp4";    
                ControlRelatBuckectSql(idMovie);
            }
        }
        public void ControlRelatBuckectSql(string idMovie)
        {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            string con = $"{Configuration["ConnectionStrings:ConnectionContext"]}";

            using (var cn = new SqlConnection(con))
            {
                string _sql = @"INSERT into  [dbo].[videoSqlControl]([IdVideo],[Title],[LocalVideo],[GoogleUrl],[User])VALUES (@a,@b,@c,@d,@e);";

                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters.AddWithValue("@a", idMovie);
                cmd.Parameters.AddWithValue("@b", Title);
                cmd.Parameters.AddWithValue("@c", LocalVideo);
                cmd.Parameters.AddWithValue("@d", GoogleUrl);
                cmd.Parameters.AddWithValue("@e", "Fabio Ciconi");
                try
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message.ToString());
                }
            }
        }
        public List<Videos> ListVideos()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            string con = $"{Configuration["ConnectionStrings:ConnectionContext"]}";
            using (var cn = new SqlConnection(con))
            {
                string _sql = @"SELECT [IdVideo],[Title],[LocalVideo],[GoogleUrl],[User] FROM  [ApiMovies].[dbo].[videoSqlControl]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();           
                StorageClient sc = StorageClient.Create();
                List<Videos> vd = new List<Videos>();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        vd.Add(new Videos()
                        {
                            IdVideo     = rd.GetString(0),
                            Title       = rd.GetString(1),
                            LocalVideo  = rd.GetString(2),
                            GoogleUrl   = rd.GetString(3)
                        });
                    }
                }
                foreach (Videos item in vd)               
                {
                    Commentaries comments = new Commentaries
                    {
                        IdVideo = item.IdVideo
                    };
                    item.ListComm = comments.ShowCommentariesVideo();                    
                }
                return vd;
            }
        }
    }
}
