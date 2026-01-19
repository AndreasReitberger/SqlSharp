using AndreasReitberger.Shared.Core.Utilities;
using AndreasReitberger.SQL.Events;
using AndreasReitberger.SQL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace AndreasReitberger.SQL
{
    public partial class SqlClientSharp : ObservableObject, ISqlClientSharp
    {
        #region Instance
        private static SqlClientSharp? _instance;
        public static SqlClientSharp Current
        {
            get
            {
                _instance ??= new SqlClientSharp();
                return _instance;
            }
            private set
            {
                if (_instance == value) return;
                _instance = value;
            }
        }
        #endregion

        #region Properties
        [ObservableProperty]
        public partial bool IsInitialized { get; set; }

        [ObservableProperty]
        public partial string ConnectionString { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string UserDomain { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string UserId { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string UserPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial SqlCredential? Credentials { get; set; }

        [ObservableProperty]
        public partial SqlConnection? Connection { get; set; }
        #endregion

        #region Ctor
        public SqlClientSharp()
        {
            _instance = this;
        }
        public SqlClientSharp(string connectionString) : this()
        {
            ConnectionString = connectionString;
            IsInitialized = true;
        }
        public SqlClientSharp(string connectionString, string username, string password) : this(connectionString)
        {
            ConnectionString = connectionString;
            Credentials = new SqlCredential(username, SecureStringHelper.ConvertToSecureString(password));

            UserId = username;
            UserPassword = password;

            IsInitialized = true;
        }
        #endregion

        #region Dtor
        ~SqlClientSharp()
        {
            Dispose();
        }
        #endregion

        #region Events

        public event EventHandler? Error;
        protected virtual void OnError()
        {
            Error?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }
        protected virtual void OnError(UnhandledExceptionEventArgs e)
        {
            Error?.Invoke(this, e);
        }
        public event EventHandler<DatabaseQueryResultEventArgs>? DatabaseQueryCompleted;
        protected virtual void OnDatabaseQueryCompleted(DatabaseQueryResultEventArgs e)
        {
            DatabaseQueryCompleted?.Invoke(this, e);
        }
        #endregion
       
        #region Methods

        public void InitDatabase(string connectionString, string username, string password)
        {
            _instance = new SqlClientSharp(connectionString, username, password)
            {
                IsInitialized = true
            };
        }

        public async Task<double> ExecuteCommandAsync(string queryString)
        {
            Stopwatch timer = new();
            timer.Start();
            using (Connection = new(ConnectionString))
            {
                SqlCommand cmd = new(queryString, Connection);
                await cmd.Connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            OnDatabaseQueryCompleted(new DatabaseQueryResultEventArgs()
            {
                SqlCommand = queryString,
                Duration = timeTaken
            });
#if DEBUG
            Console.WriteLine($"{nameof(ExecuteCommandAsync)} took {timeTaken} to run");
#endif
            return timeTaken.TotalMilliseconds;
        }

        public double ExecuteCommand(string queryString)
        {
            Stopwatch timer = new();
            timer.Start();
            using (Connection = new(ConnectionString))
            {
                SqlCommand cmd = new(queryString, Connection);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            OnDatabaseQueryCompleted(new DatabaseQueryResultEventArgs()
            {
                SqlCommand = queryString,
                Duration = timeTaken
            });
#if DEBUG
            Console.WriteLine($"{nameof(ExecuteCommandAsync)} took {timeTaken} to run");
#endif
            return timeTaken.TotalMilliseconds;
        }

        [SupportedOSPlatform("windows")]
        public long ExecuteScalarCommand(string queryString)
        {
            Stopwatch timer = new();
            timer.Start();
            long id = -1;
            using (Connection = new(ConnectionString))
            {
                SqlCommand cmd = new(queryString, Connection);
                cmd.Connection.Open();
                var res = cmd.ExecuteScalar();
                if (res.GetType() != typeof(DBNull))
                    id = Convert.ToInt32(res);
                else
                    id = -1;
            }
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            OnDatabaseQueryCompleted(new DatabaseQueryResultEventArgs()
            {
                SqlCommand = queryString,
                Duration = timeTaken
            });
#if DEBUG
            Console.WriteLine($"{nameof(ExecuteScalarCommandAsync)} took {timeTaken} to run");
#endif
            return id;
        }

        public async Task<long> ExecuteScalarCommandAsync(string queryString)
        {

            Stopwatch timer = new();
            timer.Start();

            long id = -1;
            using (Connection = new(ConnectionString))
            {
                SqlCommand cmd = new(queryString, Connection);
                await cmd.Connection.OpenAsync();
                object? res = await cmd.ExecuteScalarAsync();
                if (res?.GetType() != typeof(DBNull))
                    id = Convert.ToInt32(res);
                else
                    id = -1;
            }
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            OnDatabaseQueryCompleted(new DatabaseQueryResultEventArgs()
            {
                SqlCommand = queryString,
                Duration = timeTaken
            });
#if DEBUG
            Console.WriteLine($"{nameof(ExecuteScalarCommandAsync)} took {timeTaken} to run");
#endif
            return id;
        }

        public DataTable QueryCommand(string tableName, string filter = "")
        {
            return string.IsNullOrEmpty(filter)
                ? QueryCommand($"SELECT * FROM {tableName}")
                : QueryCommand($"SELECT * FROM {tableName} WHERE {filter}");
        }

        public async Task<DataTable> QueryCommandAsync(string tableName, string filter = "")
        {
            return string.IsNullOrEmpty(filter)
                ? await QueryCommandAsync($"SELECT * FROM {tableName}")
                : await QueryCommandAsync($"SELECT * FROM {tableName} WHERE {filter}");
        }

        public DataTable QueryCommand(string queryString)
        {
            Stopwatch timer = new();
            timer.Start();
            DataTable result = new();
            using (Connection = new(ConnectionString))
            {
                using SqlCommand cmd = new(queryString, Connection);
                cmd.Connection.Open();
                SqlDataAdapter dataAdapter = new() { SelectCommand = cmd };
                dataAdapter.Fill(result);
            }          
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            OnDatabaseQueryCompleted(new DatabaseQueryResultEventArgs()
            {
                SqlCommand = queryString,
                Duration = timeTaken
            });
#if DEBUG
            Console.WriteLine($"{nameof(QueryCommandAsync)} took {timeTaken} to run");
#endif
            return result;
        }

        public async Task<DataTable> QueryCommandAsync(string queryString)
        {
            Stopwatch timer = new();
            timer.Start();

            DataTable result = new();
            using (Connection = new(ConnectionString))
            {
                using SqlCommand cmd = new(queryString, Connection);
                await cmd.Connection.OpenAsync();
                SqlDataAdapter dataAdapter = new() { SelectCommand = cmd };
                dataAdapter.Fill(result);
            }
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            OnDatabaseQueryCompleted(new DatabaseQueryResultEventArgs()
            {
                SqlCommand = queryString,
                Duration = timeTaken
            });
#if DEBUG
            Console.WriteLine($"{nameof(QueryCommandAsync)} took {timeTaken} to run");
#endif
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }
        #endregion
    }
}
