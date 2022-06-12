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

            var age = GetAge(customerRequest.DateOfBirth);

            if (age < 21)
            {
                return null;
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
                if (customer.CreditLimit < 500) return null;
            }
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
