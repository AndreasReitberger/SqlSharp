using AndreasReitberger.Shared.Core.Utilities;
using Microsoft.Data.SqlClient;

namespace AndreasReitberger.SQL
{
    public partial class SqlClientSharp
    {
        public class SqlClientConnectionBuilder
        {
            #region Instance
            readonly SqlClientSharp _client = new();
            #endregion

            #region Methods

            public SqlClientSharp Build()
            {
                return _client;
            }

            public SqlClientConnectionBuilder WithConnectionString(string connectionString)
            {
                _client.ConnectionString = connectionString;
                _client.IsInitialized = !string.IsNullOrEmpty(_client.ConnectionString);
                return this;
            }

            public SqlClientConnectionBuilder WithCredentials(string user, string password)
            {
                _client.UserId = user;
                _client.UserPassword = password;
                _client.Credentials = new SqlCredential(user, SecureStringHelper.ConvertToSecureString(password));
                return this;
            }
            #endregion
        }
    }
}
