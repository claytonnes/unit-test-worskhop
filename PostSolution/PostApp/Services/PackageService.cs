using PostApp.Common;
using PostApp.Models;
using PostApp.Repositories;

namespace PostApp.Services
{
    public class PackageService
    {
        private readonly IPackageSenderRepository _packageSenderRepository;
        private readonly ILostPackageRepository _lostPackageRepository;
        private readonly ILoggerService _loggerService;
        private readonly IPackageCalculationService _packageCalculationService;

        public PackageService(
            IPackageSenderRepository packageSenderRepository,
            ILostPackageRepository lostMailRepository,
            IPackageCalculationService packageCalculationService,
            ILoggerService loggerService)
        {
            _packageSenderRepository = packageSenderRepository;
            _lostPackageRepository = lostMailRepository;
            _loggerService = loggerService;
            _packageCalculationService = packageCalculationService;
        }

        public string HandleIncomingPackage(Package package)
        {
            var packageId = Guid.NewGuid();
            _loggerService.LogStart(packageId);
            try
            {
                var isAbroad = _packageCalculationService.IsAbroad(package.ReceiverAddressInformation);
                var requriedFee = _packageCalculationService.CalculatePostageFee(package.WeightInGrams, package.Dimensions.Length, package.Dimensions.Height, package.Dimensions.Width, isAbroad);
                if (package.PaidPostFee >= requriedFee)
                {
                    var sendResult = _packageSenderRepository.SendPackage(package);
                    if (sendResult)
                    {
                        _loggerService.LogEnd(packageId, true);
                        return "Successfully sent package to receiver";
                    }
                    else
                    {
                        return HandleSendFailure(package, packageId);
                    }
                }
                else
                {
                    return HandleSendFailure(package, packageId);
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogEnd(packageId, false);
                _loggerService.LogException(packageId, ex);
                if (ex.Message == "All backups failed.. shit")
                {
                    throw;
                }

                return "Ran into error when processing package. Please see logs";
            }
        }

        private string HandleSendFailure(Package package, Guid packageId)
        {

            var returnResult = _packageSenderRepository.ReturnPackage(package);
            if (returnResult)
            {
                _loggerService.LogEnd(packageId, false);
                return "Package returned to sender.";
            } 

            return HandleReturnFailure(package, packageId);
        }

        private string HandleReturnFailure(Package package, Guid packageId)
        {
            var lostStorageResult = _lostPackageRepository.SendToStorage(package);
            if(lostStorageResult)
            {
                _loggerService.LogEnd(packageId, false);
                return "Failed to send to receiver or return. Sent to lost storage.";
            }
            return HandleSendToLostStorageFailure(package, packageId);
        }

        private string HandleSendToLostStorageFailure(Package package, Guid packageId)
        {
            throw new InvalidOperationException("All backups failed.. shit");
        }
    }
}
