using Atomus.Windows.Controls.Login.Models;
using Atomus.Database;
using Atomus.Service;

namespace Atomus.Windows.Controls.Login.Controllers
{
    internal static class DefaultLoginController
    {
        internal static IResponse Search(this ICore core, DefaultLoginSearchModel search)
        {
            IServiceDataSet serviceDataSet;

            //serviceDataSet = new ServiceDataSet { ServiceName = core.GetAttribute("ServiceName"), TransactionScope = false };
            serviceDataSet = new ServiceDataSet { ServiceName = core.GetAttribute("ServiceName") };
            serviceDataSet["LOGIN"].ConnectionName = core.GetAttribute("DatabaseName");
            serviceDataSet["LOGIN"].CommandText = core.GetAttribute("ProcedureLogin");
            //serviceDataSet["LOGIN"].SetAttribute("DatabaseName", core.GetAttribute("DatabaseName"));
            //serviceDataSet["LOGIN"].SetAttribute("ProcedureID", core.GetAttribute("ProcedureLogin"));
            serviceDataSet["LOGIN"].AddParameter("@EMAIL", DbType.NVarChar, 100);
            serviceDataSet["LOGIN"].AddParameter("@ACCESS_NUMBER", DbType.NVarChar, 4000);

            serviceDataSet["LOGIN"].NewRow();
            serviceDataSet["LOGIN"].SetValue("@EMAIL", search.EMAIL);
            serviceDataSet["LOGIN"].SetValue("@ACCESS_NUMBER", search.ACCESS_NUMBER);

            return core.ServiceRequest(serviceDataSet);
        }
    }
}