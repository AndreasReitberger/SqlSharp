using AndreasReitberger.Shared.Core.Utilities;
using System.Data;
using System.Security;

namespace AndreasReitberger.SQL.NUnitTest
{
    public class Tests
    {
        string _server = "server";
        string _databaseName = "database";
        string _user = "user";
        string _password = "MyPassword";
        string _domain = "workgroup";
        string _connectionString = string.Empty;

        [SetUp]
        public void Setup()
        {
            _connectionString = $"Data Source={_server};Initial Catalog={_databaseName};Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        }

        [Test]
        public void InitConnectionTest()
        {
            try
            {
                SqlClientSharp.InitDatabase(_connectionString, _user, _password, _domain);
                Assert.That(SqlClientSharp.Current.IsInitialized);
            }
            catch (Exception exc)
            {
                Assert.Fail(exc.Message);
            }
        }

        [Test]
        public async Task QueryDataTestAsync()
        {
            try
            {
                SqlClientSharp.InitDatabase(_connectionString, _user, _password, _domain);

                string table = "Logs l";
                string data =
                    "l.log_id, " +
                    "min(l.log_reporter) as 'reporter', " +
                    "min(l.log_date_time) as 'date', " +
                    "min(l.log_description) as 'log_desc', " +
                    "min(u.q_number) as 'qnumber', " +
                    "min(l.log_userrole) as 'role', " +
                    "min(id.identifier) as 'id', " +
                    "min(id.description) as 'id_desc', " +
                    "min(convert(int,id.systemlocked)) as 'id_Systemlocked', " +
                    "min(id.state) as 'id_state', " +
                    "min(id.level) as 'id_level', " +
                    "min(l.log_status) as 'status', " +
                    "min(l.duration) as 'duration', " +
                    "min(u.firstname) as 'firstname', " +
                    "min(u.lastname) as 'lastname', " +
                    "min(u.email) as 'email', " +
                    "min(u.role) as 'user_role', " +
                    "min(u.status) as 'user_status', " +
                    "min(u.oe) as 'oe'";
                //string filter = "";
                string join =
                    "inner join users u on l.log_worker = u.q_number " +
                    "inner join identifiers id on l.log_identifier = id.identifier";

                string filter = "";

                string group = "group by log_id";

                string cmd = $"select {data} from {table} {join} {(string.IsNullOrEmpty(filter) ? string.Empty : $"where {filter}")} {group};";
                DataTable logs = await SqlClientSharp.Current.QueryCommandAsync(cmd).ConfigureAwait(false);
                Assert.That(logs != null && logs.Rows.Count > 0);
            }
            catch (Exception exc)
            {
                Assert.Fail(exc.Message);
            }
        }
    }
}