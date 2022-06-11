using App.DataAccess.Models;
using App.DataAccess.Repositories;
using App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class CustomerService
    {
        private readonly ICustomerCreditService _customerCreditService;
        private readonly ICompanyRepository _companyRepository;
        public CustomerService(ICompanyRepository companyRepository,
            ICustomerCreditService customerCreditService)
        {
            _companyRepository = companyRepository;
            _customerCreditService=customerCreditService;
        }
        public bool AddCustomer(CustomerRequest customerRequest)
        {
            if (string.IsNullOrWhiteSpace(customerRequest.FirstName) || string.IsNullOrWhiteSpace(customerRequest.LastName))
            { 
                return false;
            }

            var now = DateTime.Today;
            var dateOfBirth = customerRequest.DateOfBirth;
            int age = now.Year - dateOfBirth.Year;
            //if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            if (dateOfBirth.Date > now.AddYears(-age)) age--;

            if (age < 21)
            {
                return false;
            }
            var company = _companyRepository.GetById(customerRequest.companyId);
            var customer = new Customer
            {
                Company = company,
                Firstname = customerRequest.FirstName,
                Surname = customerRequest.LastName,
                DateOfBirth = customerRequest.DateOfBirth
            };

            if (company.Classification ==Classification.Gold)
            {
                // Skip credit check
                customer.HasCreditLimit = false;
            }
            else
            {
                customer.HasCreditLimit = true;
                var creditLimit = _customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                if (company.Classification == Classification.Bronze)
                {
                    customer.CreditLimit= creditLimit * 2;
                }
                customer.CreditLimit = creditLimit;
                if (customer.CreditLimit < 500) return false;
            }
            CustomerRepository.AddCustomer(customer);

            return true;
        }
    }
}
