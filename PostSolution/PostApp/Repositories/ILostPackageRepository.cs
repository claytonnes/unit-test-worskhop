using PostApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Repositories
{
    public interface ILostPackageRepository
    {
        /// <summary>
        /// Sends a package unable to be delivered or returned to long term storage.
        /// </summary>
        /// <param name="package"></param>
        /// <exception cref="IOException">Sometimes shit goes wrong, OK?</exception>
        /// <returns></returns>
        bool SendToStorage(Package package);
    }
}
