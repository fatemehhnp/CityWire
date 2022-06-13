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
        private readonly IWrapper _wrapper;
        public CustomerService(ICompanyRepository companyRepository,
            ICustomerCreditService customerCreditService
            ,IWrapper wrapper)
        {
            _companyRepository = companyRepository;
            _customerCreditService=customerCreditService;
            _wrapper = wrapper;
        }
        public Customer AddCustomer(CustomerRequest customerRequest)
        {
            if (string.IsNullOrWhiteSpace(customerRequest.FirstName) || string.IsNullOrWhiteSpace(customerRequest.LastName))
            { 
                return null;
            }
            if (!customerRequest.EmailAddress.Contains("@") && !customerRequest.EmailAddress.Contains("."))
            {
                return null;
            }
            if (GetAge(customerRequest.DateOfBirth) < Constants.AgeLimite)
            {
                return null;
            }
            var company = _companyRepository.GetById(customerRequest.companyId);
            var customer = new Customer
            {
                Company = company,
                Firstname = customerRequest.FirstName,
                Surname = customerRequest.LastName,
                DateOfBirth = customerRequest.DateOfBirth,
                EmailAddress=customerRequest.EmailAddress
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
                if (company.Classification == Classification.Silver)
                {
                    customer.CreditLimit = creditLimit * Constants.CreditMultiplyNumber;
                }
                else
                {
                    customer.CreditLimit = creditLimit;
                }
            }
            if (customer.HasCreditLimit && customer.CreditLimit < Constants.CreditLimite) return null;
            _wrapper.AddCustomer(customer);

            return customer;
        }
        private int GetAge(DateTime dateOfBirth)
        {
            var now = DateTime.Today;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            return age;
        }
    }
}
