using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FabioCiconiAssignment3.Models
{
    public class LoginViewModel
    {

        [Key]
        [Required]
        public string User { get; set; }


        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        public IConfigurationRoot Configuration;

        public bool IsValid()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            string con = $"{Configuration["ConnectionStrings:ConnectionContext"]}";

            using (var cn = new SqlConnection(con))
            {
                string _sql = @"SELECT [User] FROM [dbo].[Users] WHERE [User] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, cn);

                cmd.Parameters
                        .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                        .Value = User;
                cmd.Parameters
                       .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                       .Value = Password;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }
    }
}
