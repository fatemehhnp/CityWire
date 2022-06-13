using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App;
using App.DataAccess.Repositories;
using Moq;
using NUnit.Framework;
using NUnit;
using App.Models;
using App.DataAccess.Models;

namespace Test
{
    [TestFixture]
    public class CustomerServicesTests
    {
        private  Mock<ICompanyRepository> _mockCompanyRepository;
        private Mock<ICustomerCreditService> _mockCustomerCreditService;
        private Mock<IWrapper> _mockWrapper;
        private CustomerService _sut;
        [SetUp]
        public void SetUp()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockCustomerCreditService = new Mock<ICustomerCreditService>();
            _mockWrapper = new Mock<IWrapper>();
            _sut = new CustomerService(_mockCompanyRepository.Object,_mockCustomerCreditService.Object,_mockWrapper.Object);
        }
        [Test]
        public void AddCustomer_WithEmptyFirstNameInRequest_ReturnsNull()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            customerRequest.FirstName = "";
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void AddCustomer_WithEmptySureNameInRequest_ReturnsNull()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            customerRequest.LastName = "";
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void AddCustomer_WithEmptySureNameAndFirstNameInRequest_ReturnsNull()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            customerRequest.LastName = "";
            customerRequest.FirstName = "";
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void AddCustomer_WithInvalidEmail_ReturnsNull()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            customerRequest.EmailAddress = "fatemehhnp";
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void AddCustomer_WithBelowAgeLimite_ReturnsNull()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            customerRequest.DateOfBirth = new DateTime(2003, 12, 09);
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void AddCustomer_WithGoldCompanyClassification_SetsCreditLimitToFalse()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            var company = GetCompany();
            company.Classification = Classification.Gold;
            _mockCompanyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            _mockWrapper.Setup(x => x.AddCustomer(It.IsAny<Customer>()));
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            _mockCompanyRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _mockWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once());
            Assert.IsFalse(result.HasCreditLimit);
        }
        [Test]
        public void AddCustomer_WithSilverCompanyClassificationAndOverCreditLimite_SetsCreditLimitToTrue()
        {
            //Arrange
            var customerRequest=GetCustomerRequest();
            var company = GetCompany();
            company.Classification = Classification.Silver;
            _mockCompanyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            _mockWrapper.Setup(x => x.AddCustomer(It.IsAny<Customer>()));
            _mockCustomerCreditService.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>())).Returns(800);
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            _mockCompanyRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _mockWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once());
            _mockCustomerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
            Assert.IsTrue(result.HasCreditLimit);
        }
        [Test]
        public void AddCustomer_WithSilverCompanyClassificationAndOverCreditLimite_MultipliesCreditelimite()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            var company = GetCompany();
            company.Classification = Classification.Silver;
            var creditLimit = 800;
            _mockCompanyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            _mockWrapper.Setup(x => x.AddCustomer(It.IsAny<Customer>()));
            _mockCustomerCreditService.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>())).Returns(creditLimit);
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            _mockCompanyRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _mockWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once());
            _mockCustomerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
            Assert.AreEqual((creditLimit*2), result.CreditLimit);
        }
        [Test]
        public void AddCustomer_WithBronzeCompanyClassificationAndUnderCreditLimite_ReturnsNull()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            var company = GetCompany();
            company.Classification = Classification.Bronze;
            _mockCompanyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            _mockWrapper.Setup(x => x.AddCustomer(It.IsAny<Customer>()));
            _mockCustomerCreditService.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>())).Returns(300);
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            _mockCompanyRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _mockWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never());
            _mockCustomerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
            Assert.Null(result);
        }
        [Test]
        public void AddCustomer_WithBronzeCompanyClassificationAndOverCreditLimite_ReturnsCreditLimit()
        {
            //Arrange
            var customerRequest = GetCustomerRequest();
            var company = GetCompany();
            company.Classification = Classification.Bronze;
            var creditLimit = 650;
            _mockCompanyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            _mockWrapper.Setup(x => x.AddCustomer(It.IsAny<Customer>()));
            _mockCustomerCreditService.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>())).Returns(creditLimit);
            //Act
            var result = _sut.AddCustomer(customerRequest);
            //Assert
            _mockCompanyRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _mockWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once());
            _mockCustomerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
            Assert.AreEqual(creditLimit,result.CreditLimit);
        }
        private CustomerRequest GetCustomerRequest()
        {
            return new CustomerRequest
            {
                FirstName = "Fatemeh",
                LastName = "Hoseinpoor",
                EmailAddress="Fatemehhnp_1365@yahoo.com",
                companyId = 2,
                DateOfBirth = new DateTime(1990, 12, 05),
            };
        }
        private Company GetCompany()
        {
            return new Company { Id = 2, Name =Constants.ImportantClient };
        }
    }
}
