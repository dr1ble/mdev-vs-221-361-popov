using SQLite;

namespace wfaSQLLite
{
    [Table("Logs")]
    public class Log
    {
        public DateTime DateTime { get; set; }
    }

    [Table("Cities")]
    public class City
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}