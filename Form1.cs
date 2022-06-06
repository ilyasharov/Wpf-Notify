using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        readonly string connectionString = @"Data Source=localhost; Initial Catalog=Appdata; Integrated Security=true";
        readonly DataBase database;
        readonly BackgroundWorker bw;
        SqlTableDependency<Peoples> tableDependency;

        public Form1()
        {
            InitializeComponent();
            database = new DataBase(connectionString);
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(RefreshDataGrid);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "Id");
            dataGridView1.Columns.Add("flag", "Flag");
            dataGridView1.Columns.Add("data", "Data");
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Invoke((Action)(() =>
            {
                dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2));
            }));
        }

        private void RefreshDataGrid(object sender, DoWorkEventArgs e)
        {
            try
            {

                dataGridView1.Invoke((Action)(() => { dataGridView1.Rows.Clear(); }));

                string queryString = $"select * from DataTable";

                SqlCommand command = new SqlCommand(queryString, database.GetConnection());

                database.OpenConnection();
                tableDependency = new SqlTableDependency<Peoples>(connectionString, "DataTable");
                tableDependency.OnChanged += TableDependency_Changed;
                tableDependency.OnError += TableDependency_OnError;
                tableDependency.Start();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReadSingleRow(dataGridView1, reader);
                }                
               
                reader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void TableDependency_Changed(object sender, RecordChangedEventArgs<Peoples> e)
        {
            dataGridView1.Invoke((Action)(() => { dataGridView1.Rows.Clear(); }));
            string queryString = $"select * from DataTable";
            SqlCommand command = new SqlCommand(queryString, database.GetConnection());
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ReadSingleRow(dataGridView1, reader);
            }
            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Database connection open";
            bw.RunWorkerAsync();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            database.CloseConnection();
            tableDependency.Stop();
        }

        void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "Database connection closed";
        }
    }
}
