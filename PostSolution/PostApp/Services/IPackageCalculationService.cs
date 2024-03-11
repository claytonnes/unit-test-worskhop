using PostApp.Common;
using PostApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Services
{
    public interface IPackageCalculationService
    {
        /// <summary>
        /// returns true if the AddressInformation is for an abroad address, else false
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        bool IsAbroad(AddressInformation addressInformation);

        /// <summary>
        /// Calculates the postage fee for the package
        /// </summary>
        /// <param name="weightInGrams"></param>
        /// <param name="lengthInCm"></param>
        /// <param name="heightInCm"></param>
        /// <param name="widthInCm"></param>
        /// <param name="abroad"></param>
        /// <exception cref="ArgumentException">Thrown if arguments for calculation are invalid, e.g. the package cannot be handled by this PackageService</exception>
        /// <returns></returns>
        int CalculatePostageFee(int weightInGrams, double lengthInCm, double heightInCm, double widthInCm, bool abroad);
    }
}
