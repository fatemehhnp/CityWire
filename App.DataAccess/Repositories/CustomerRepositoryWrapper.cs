using App.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.DataAccess.Repositories
{
    public class CustomerRepositoryWrapper: IWrapper
    {
        Customer _customer = null;
        public CustomerRepositoryWrapper(Customer customer)
        {
            _customer = customer;
        }
        public void AddCustomer(Customer customer)
        {
            _customer = customer;
            CustomerRepository.AddCustomer(customer);
        }
    }
}
