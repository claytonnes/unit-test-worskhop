using PostApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Repositories
{
    public interface IPackageSenderRepository
    {
        /// <summary>
        /// Tries to deliver a package
        /// </summary>
        /// <param name="package">The package you are tying to send</param>
        /// <returns>Bool to indicate success. True = success, false = failure</returns>
        bool SendPackage(Package package);


        /// <summary>
        /// Tries to return a package to its sender
        /// </summary>
        /// <param name="package">The package you are tying to return</param>
        /// <returns>Bool to indicate success. True = success, false = failure</returns>
        bool ReturnPackage(Package package);
    }
}
