using Moq;
using PostApp.Common;
using PostApp.Models;
using PostApp.Repositories;
using PostApp.Services;


namespace PostAppTests
{
    public class PackageServiceTests
    {
        private Mock<IPackageSenderRepository> _packageSenderRepository;
        private Mock<ILostPackageRepository> _lostPackageRepository;
        private Mock<ILoggerService> _loggerService;
        private Mock<IPackageCalculationService> _packageCalculationService;

        private PackageService _packageService;

        [Theory]
        [InlineData(41, 41)]
        [InlineData(41, 99128)]
        public void HandleIncomingPackage_ShouldTryToSendPackages_WithAppropriateFeePaid(int calculatedValue, int paidValue)
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = paidValue, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(calculatedValue);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>()))
                .Returns(true);
            //Act
            var result = _packageService.HandleIncomingPackage(package);
            //Assert
            _packageSenderRepository.Verify(mock => mock.SendPackage(It.IsAny<Package>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(41, 40)]
        [InlineData(22, 0)]
        [InlineData(41, -1247)]
        public void HandleIncomingPackage_ShouldNotSendPackages_WithTooLittlePaidFee(int calculatedValue, int paidValue)
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = paidValue };
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(calculatedValue);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>()))
                .Returns(true);
            //Act
            var result = _packageService.HandleIncomingPackage(package);
            //Assert
            _packageSenderRepository.Verify(mock => mock.SendPackage(It.IsAny<Package>()), Times.Never);
        }

        [Theory]
        [InlineData(41, 40)]
        [InlineData(22, 0)]
        [InlineData(41, -1247)]
        public void HandleIncomingPackage_ShouldTryToReturnPackage_IfTooLittleFeeIsPaid(int calculatedValue, int paidValue)
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = paidValue, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(calculatedValue);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(true);
            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _packageSenderRepository.Verify(mock => mock.ReturnPackage(It.IsAny<Package>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldReturnPackage_IfSendToReceiverFailed()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(true);

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _packageSenderRepository.Verify(mock => mock.ReturnPackage(It.IsAny<Package>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldSendToLostStorage_IfReturnToSenderFailed()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(false);
            _lostPackageRepository.Setup(_ => _.SendToStorage(It.IsAny<Package>())).Returns(true);

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _lostPackageRepository.Verify(mock => mock.SendToStorage(It.IsAny<Package>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldThrowException_IfSendToLostPackageStorageFails()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(false);
            _lostPackageRepository.Setup(_ => _.SendToStorage(It.IsAny<Package>())).Returns(false);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _packageService.HandleIncomingPackage(package));
        }

        [Fact]
        public void HandleIncomingPackage_ShouldAlwaysGenerateLogStartAndLogEnd_WhenSuccessfulSend()
        {
            //Assignt
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(true);

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogStart(It.IsAny<Guid>()), Times.Exactly(1));
            _loggerService.Verify(mock => mock.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldAlwaysGenerateLogStartAndLogEnd_WhenPackageReturned()
        {
            //Assignt
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(true);

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogStart(It.IsAny<Guid>()), Times.Exactly(1));
            _loggerService.Verify(mock => mock.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldAlwaysGenerateLogStartAndLogEnd_WhenPackageSentToStorage()
        {
            //Assignt
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(false);
            _lostPackageRepository.Setup(_ => _.SendToStorage(It.IsAny<Package>())).Returns(true);
            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogStart(It.IsAny<Guid>()), Times.Exactly(1));
            _loggerService.Verify(mock => mock.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldAlwaysGenerateLogStartAndLogEnd_WhenPackageFailedSendToStorage()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), It.IsAny<bool>()));
            _loggerService.Setup(_ => _.LogException(It.IsAny<Guid>(), It.IsAny<InvalidOperationException>()));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(false);
            _lostPackageRepository.Setup(_ => _.SendToStorage(It.IsAny<Package>())).Returns(false);

            //Act + Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _packageService.HandleIncomingPackage(package));
            _loggerService.Verify(mock => mock.LogStart(It.IsAny<Guid>()), Times.Exactly(1));
            _loggerService.Verify(mock => mock.LogEnd(It.IsAny<Guid>(), false), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldLogExceptions_WhenCalculationExceptionIsCaught()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()));
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Throws<Exception>();

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()), Times.Once);

        }

        [Fact]
        public void HandleIncomingPackage_ShouldLogExceptions_WhenSendExceptionIsCaught()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()));
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>()))
                .Throws<Exception>();

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldLogExceptions_WhenReturnExceptionIsCaught()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()));
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Throws<Exception>();

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldLogException_WhenSendToStorageExceptionIsCaught()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()));
            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(false);
            _lostPackageRepository.Setup(_ => _.SendToStorage(It.IsAny<Package>())).Throws<Exception>();

            //Act
            var result = _packageService.HandleIncomingPackage(package);

            //Assert
            _loggerService.Verify(mock => mock.LogException(It.IsAny<Guid>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldReturnSuccessString_WhenSendToReceiverSucceeds()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), true));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(true);

            var expected = "Successfully sent package to receiver";

            //Act
            var actual = _packageService.HandleIncomingPackage(package);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldReturnReturnedToUserString_WhenReturnSuceeds()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 21, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), true));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(true);

            var expected = "Package returned to sender.";

            //Act
            var actual = _packageService.HandleIncomingPackage(package);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldReturnSentToLostStorage_WhenSendToLostStorageSucceeds()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), true));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
                .Returns(22);
            _packageSenderRepository.Setup(_ => _.SendPackage(It.IsAny<Package>())).Returns(false);
            _packageSenderRepository.Setup(_ => _.ReturnPackage(It.IsAny<Package>())).Returns(false);
            _lostPackageRepository.Setup(_ => _.SendToStorage(It.IsAny<Package>())).Returns(true);

            var expected = "Failed to send to receiver or return. Sent to lost storage.";

            //Act
            var actual = _packageService.HandleIncomingPackage(package);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HandleIncomingPackage_ShouldReturnErrorString_WhenExceptionIsCaught()
        {
            //Assign
            Setup();
            var package = new Package() { PaidPostFee = 22, Dimensions = PackageFactory.GenerateDimensionsInstance(22, 22, 22) };
            _loggerService.Setup(_ => _.LogStart(It.IsAny<Guid>()));
            _loggerService.Setup(_ => _.LogEnd(It.IsAny<Guid>(), true));

            _packageCalculationService.Setup(_ => _.CalculatePostageFee(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), false))
               .Throws<Exception>();

            var expected = "Ran into error when processing package. Please see logs";

            //Act
            var actual = _packageService.HandleIncomingPackage(package);

            //Assert
            Assert.Equal(expected, actual);
        }


        private void Setup()
        {
            _packageSenderRepository = new Mock<IPackageSenderRepository>();
            _lostPackageRepository = new Mock<ILostPackageRepository>();
            _loggerService = new Mock<ILoggerService>();
            _packageCalculationService = new Mock<IPackageCalculationService>();

            _packageService =
                new PackageService(
                    _packageSenderRepository.Object,
                    _lostPackageRepository.Object,
                    _packageCalculationService.Object,
                    _loggerService.Object);
        }
    }
}
