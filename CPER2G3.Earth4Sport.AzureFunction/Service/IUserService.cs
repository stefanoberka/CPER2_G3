﻿using CPER2G3.Earth4Sport.AzureFunction.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPER2G3.Earth4Sport.AzureFunction.Service {
    public interface IUserService {
        Task<LoginResponse> Login(string username, string password);
        Task<string> Register(User user, string clockUuid);
        Task<List<string>> UserClocks(string userId);

    }
}
