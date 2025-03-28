using SQLite;

namespace wfaSQLLite
{
    public partial class Form1 : Form
    {
        private readonly SQLiteConnection db;

        public Form1()
        {
            InitializeComponent();

            db = new SQLiteConnection("myDB.db");
            db.CreateTable<Log>();
            db.CreateTable<City>();

            db.Insert(new Log { DateTime = DateTime.Now });

            lvLogs.Columns.Add("DateTime", 250);
            lvLogs.View = View.Details;

            foreach (var log in db.Table<Log>())
            {
                lvLogs.Items.Add(log.DateTime.ToString());
            }

            buCityAdd.Click += (s, e) => db.Insert(new City { Name = edCityName.Text });
            buCityShow.Click += (s, e) => dataGridView1.DataSource = db.Table<City>().ToList();
        }
    }
}
