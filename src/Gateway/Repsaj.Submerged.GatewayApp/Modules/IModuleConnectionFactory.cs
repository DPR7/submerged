﻿using Repsaj.Submerged.GatewayApp.Modules;
using Repsaj.Submerged.GatewayApp.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Modules
{
    public interface IModuleConnectionFactory
    {
        Task Init();
        IModuleConnection GetModuleConnection(string moduleName);
        IModuleConnection GetModuleConnection(Module module);
    }
}