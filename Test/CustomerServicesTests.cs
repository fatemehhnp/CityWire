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
        private CustomerService _sut;
        [SetUp]
        public void SetUp()
        {;
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _sut = new CustomerService(_mockCompanyRepository.Object,_mockCustomerCreditService.Object);
        }
        [Test]
        public void AddCustomer_WithGoldCompanyClassification_SetsCreditLimitToFalse()
        {
            //Arrange
            var customerRequest = new CustomerRequest
            {
                FirstName = "Fatemeh",
                LastName = "Hoseinpoor",
                companyId = 2,
                DateOfBirth = new DateTime(1990, 12, 05),
            };
            var company = new Company { Id = 2, Name = "", Classification = Classification.Gold };
            _mockCompanyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            var result = _sut.AddCustomer(customerRequest);
        }
    }
}
