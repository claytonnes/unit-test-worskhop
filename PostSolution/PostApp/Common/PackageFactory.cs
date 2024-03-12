using PostApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PostApp.Common
{
    public static class PackageFactory
    {
        public static Dimensions GenerateDimensionsInstance(double length, double width, double height)
        {
            return new Dimensions() { Length = length, Width = width, Height = height };
        }

        public static AddressInformation GenerateAddressInformationInstance(string country)
        {
            return new AddressInformation() { Country = country };
        } 

        public static Package GeneratePackageInstance(Dimensions dimensions)
        {
            return new Package()
            {
                Dimensions = dimensions
            };
        }
    }
}
