
namespace Infrastructures.Data
{
    public abstract class BaseDAC
    {
        //const string connName = "DefaultConnection";
        //protected IDbConnection GetConnection()
        //{
        //    return new SqlConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
        //}
        protected ApplicationDbContext _db;
        public BaseDAC()
        {
            _db = new ApplicationDbContext();
        }
    }
}
