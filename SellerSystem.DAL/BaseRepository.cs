using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SellerSystem.DAL
{
    public class BaseRepository : IDisposable
    {
        protected IDbConnection con;
        private static IConfigurationRoot _configuration { get; set; }
        public BaseRepository()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = builder.Build();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            con = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
