﻿using System;
using Abp.Authorization.Users;
using Abp.AutoMapper;

namespace JK.Logs.Dto
{
    [AutoMapFrom(typeof(UserLoginAttempt))]
    public class UserLoginAttemptDto
    {
        public string TenancyName { get; set; }

        public string UserNameOrEmailAddress { get; set; }

        public string ClientIpAddress { get; set; }

        public string ClientName { get; set; }

        public string BrowserInfo { get; set; }

        public string Result { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
