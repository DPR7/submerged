﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.Common.Models.Commands
{
    public static class DeviceCommandConstants
    {
        public const string NAME = "Name";
        public const string MESSAGE_ID = "MessageId";
        public const string CREATED_TIME = "CreatedTime";
        public const string UPDATED_TIME = "UpdatedTime";
        public const string RESULT = "Result";
        public const string ERROR_MESSAGE = "ErrorMessage";
        public const string PARAMETERS = "Parameters";
    }

    public static class DeviceCommandTypes
    {
        public const string SWITCH_RELAY = "SwitchRelay";
        public const string UPDATE_INFO = "UpdateInfo";
        public const string MODULE_COMMAND = "ModuleCommand";
    }
}