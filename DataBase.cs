using System.Data.SqlClient;

namespace WindowsFormsApp3
{
    internal class DataBase
    {
        readonly SqlConnection sqlConnection;

        public DataBase(string connectionString)
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }
    }
}
