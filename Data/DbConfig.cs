using System;

namespace BopeChat.Data
{
    internal class DbConfig
    {
        // MySQL database settings
        private const string Server = "localhost";
        private const string Port = "3306";
        private const string DatabaseName = "bopechat_db";
        private const string Username = "root";
        private const string Password = "";

        /// <summary>
        /// Returns the MySQL connection string.
        /// </summary>
        public static string GetConnectionString()
        {
            return $"Server={Server};Port={Port};Database={DatabaseName};Uid={Username};Pwd={Password};";
        }
    }
}