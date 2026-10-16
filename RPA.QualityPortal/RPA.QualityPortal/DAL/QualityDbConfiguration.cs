using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace RPA.QualityPortal.DAL
{
    // Code-based replacement for the <entityFramework> section of Web.config.
    public class QualityDbConfiguration : DbConfiguration
    {
        public QualityDbConfiguration()
        {
            SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new SqlConnectionFactory());
        }
    }
}
