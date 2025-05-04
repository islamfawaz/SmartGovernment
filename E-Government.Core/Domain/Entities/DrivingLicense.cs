using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
    public class DrivingLicense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NID {  get; set; }

        public DateOnly DateOfBirth { get; set; } = new DateOnly();
        public string Address { get; set; }
        public string LicenseType { get; set; }

        public string photo { get; set; }

        public string MedicalTest { get; set; }

        public string TheoryTest { get; set; }
        public string PracticalTest { get; set; }

        public DateOnly IssueDate { get; set; } = new DateOnly();
        public DateOnly ExpiryDate { get; set; } = new DateOnly();

    }
}
