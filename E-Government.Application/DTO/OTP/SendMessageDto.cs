﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.OTP
{
    public class SendMessageDto
    {
        public string PhoneNumber { get; set; }

        public string Body { get; set; }
    }
}
