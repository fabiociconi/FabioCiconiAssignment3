using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FabioCiconiAssignment3.Models
{
    public class Commentaries
    {
        public int IdComm { get; set; }
        public string Desc { get; set; }
        public string User { get; set; }
        public string IdVideo { get; set; }
        public DateTime DateIncluded { get; set; }

        public IConfigurationRoot Configuration;
        public void InsertComment()
        {            
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            string con = $"{Configuration["ConnectionStrings:ConnectionContext"]}";

            using (var cn = new SqlConnection(con))
            {

                string _sql = @"INSERT into  [dbo].[commentaries]([Description],[IdUser],[IdVideo],[DateIncluded])
VALUES (@b,@c,@d,@e);";

                var cmd = new SqlCommand(_sql, cn);
                //cmd.Parameters.AddWithValue("@a", "300" + IdVideo + DateTime.UtcNow );
                cmd.Parameters.AddWithValue("@b", Desc);
                cmd.Parameters.AddWithValue("@c", User);
                cmd.Parameters.AddWithValue("@d", IdVideo);
                cmd.Parameters.AddWithValue("@e", DateTime.Today);

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
        public List<Commentaries> ShowCommentariesVideo()
        {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            string con = $"{Configuration["ConnectionStrings:ConnectionContext"]}";

            using (var cn = new SqlConnection(con))
            {
                string _sql = @"SELECT [IdComm],[Description],[IdUser],[IdVideo],[DateIncluded]FROM [ApiMovies].[dbo].[commentaries] where [IdVideo] =@a ";

                var cmd = new SqlCommand(_sql, cn);

                cn.Open();
                cmd.Parameters
                       .Add(new SqlParameter("@a", SqlDbType.NVarChar))
                       .Value = IdVideo;

                List<Commentaries> cmm = new List<Commentaries>();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        cmm.Add(new Commentaries()
                        {
                            IdComm          = (int)rd["IdComm"],
                            User            = (string)rd["IdUser"],
                            IdVideo         = (string)rd["IdVideo"],
                            Desc            = (string)rd["Description"],
                            DateIncluded    = (DateTime)rd["DateIncluded"]
                        });
                    }
                }
                return cmm;
            }
        }
    }
}
