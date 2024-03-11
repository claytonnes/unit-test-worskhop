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
        /// <exception cref="DirectoryNotFoundException">Thrown when the post delivery person cannot find a receiver</exception>
        void SendPackages(Package package);


        /// <summary>
        /// Tries to return a package to its sender
        /// </summary>
        /// <param name="package">The package you are tying to return</param><
        /// <exception cref="DirectoryNotFoundException">Thrown when the post delivery person cannot find a receiver</exception>
        void ReturnPackage(Package package);
    }
}
