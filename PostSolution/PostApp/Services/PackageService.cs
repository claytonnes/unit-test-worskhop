using PostApp.Models;
using PostApp.Repositories;

namespace PostApp.Services
{
    public class PackageService
    {
        private readonly IPackageSenderRepository _packageSenderRepository;
        private readonly ILostPackageRepository _lostMailRepository;
        private readonly ILoggerService _loggerService;
        private readonly IPackageCalculationService _packageCalculationService;

        public PackageService(
            IPackageSenderRepository packageSenderRepository,
            ILostPackageRepository lostMailRepository, 
            IPackageCalculationService packageCalculationService,
            ILoggerService loggerService)
        {
            _packageSenderRepository = packageSenderRepository;
            _lostMailRepository = lostMailRepository;
            _loggerService = loggerService;
            _packageCalculationService = packageCalculationService;
        }

        public string HandleIncomingPackage(Package packages)
        {
            return "";
        }
    }
}
