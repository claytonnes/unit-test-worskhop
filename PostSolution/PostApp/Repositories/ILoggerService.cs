using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Repositories
{
    public interface ILoggerService
    {
        /// <summary>
        /// Starts a logging trace for a package with provided unique packageId
        /// </summary>
        /// <param name="packageId"></param>
        void LogStart(Guid packageId);
        /// <summary>
        /// Logs an exception associated with provided packageId for tracability
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="exception"></param>
        void LogException(Guid packageId, Exception exception);
        /// <summary>
        /// Logs end of package trace handling by PackageService, isSuccess indicated success of handling (success = successfuly sent package to receiver)
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="isSuccess"></param>
        void LogEnd(Guid packageId, bool isSuccess);
    }
}
