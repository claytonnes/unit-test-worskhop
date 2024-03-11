using PostApp.Models;
using PostApp.Repositories;

namespace PostApp.Services
{
    public class PackageService
    {
        private readonly IMailManRepository _mailManRepository;
        private readonly ILostMailRepository _lostMailRepository;
        private readonly ILoggerService _loggerService;

        public PackageService(
            IMailManRepository mailManRepository,
            ILostMailRepository lostMailRepository, 
            ILoggerService loggerService)
        {
            _mailManRepository = mailManRepository;
            _lostMailRepository = lostMailRepository;
            _loggerService = loggerService;
        }

        public string HandleIncomingPackages(List<Package> packages)
        {
            return "";
        }
    }
}
