using E_Government.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface IMailSettings
    {
        void SendEmail(Email email); 
    }
}
