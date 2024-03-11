using PostApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Repositories
{
    public interface IReturnRepository
    {
        /// <summary>
        /// Attempts to return a package to its sender
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if return is unable</exception>
        /// <param name="package"></param>
        void ReturnToSender(Package package);
    }
}
