using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Models
{
    public class CustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int companyId { get; set; }
    }
}
