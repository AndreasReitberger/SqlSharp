using AndreasReitberger.SQL.Events;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace AndreasReitberger.SQL.Interfaces
{
    public interface ISqlClientSharp : IDisposable
    {
        #region Properties
        public static ISqlClientSharp? Current { get; private set; }
        public bool IsInitialized { get; set; }
        public string ConnectionString { get; set; }
        public string UserDomain { get; set; }
        public string UserId { get; set; }
        public string UserPassword { get; set; }
        public SqlCredential? Credentials { get; set; }
        public SqlConnection? Connection { get; set; }
        #endregion

        #region Events
        public event EventHandler? Error;
        public event EventHandler<DatabaseQueryResultEventArgs>? DatabaseQueryCompleted;
        #endregion

        #region Methods
        public void InitDatabase(string connectionString, string username, string password);
        public Task<double> ExecuteCommandAsync(string queryString);
        public double ExecuteCommand(string queryString);
        public long ExecuteScalarCommand(string queryString);
        public Task<long> ExecuteScalarCommandAsync(string queryString);
        public DataTable QueryCommand(string tableName, string filter = "");
        public Task<DataTable> QueryCommandAsync(string tableName, string filter = "");
        public DataTable QueryCommand(string queryString);
        public Task<DataTable> QueryCommandAsync(string queryString);
        #endregion
    }
}
