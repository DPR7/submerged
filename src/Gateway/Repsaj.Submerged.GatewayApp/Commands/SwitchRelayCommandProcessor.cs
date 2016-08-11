﻿using RemoteArduino.Hardware;
using Repsaj.Submerged.GatewayApp;
using Repsaj.Submerged.GatewayApp.Modules;
using Repsaj.Submerged.GatewayApp.Modules.Connections;
using Repsaj.Submerged.GatewayApp.Universal.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteArduino.Commands
{
    public class SwitchRelayCommandProcessor : ICommandProcessor
    {
        IModuleConnectionFactory _moduleConnectionFactory;
        public event Action<int, bool> RelaySwitched;

        public SwitchRelayCommandProcessor(IModuleConnectionFactory connectionFactory)
        {
            this._moduleConnectionFactory = connectionFactory;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<CommandProcessingResult> ProcessCommand(DeserializableCommand command)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (command.CommandName == CommandNames.SWITCH_RELAY)
            {
                try
                {
                    dynamic parameters = command.Command.Parameters;

                    int? relayNumber = parameters.RelayNumber;
                    bool? relayState = parameters.RelayState;

                    // TODO: should include the module name in the command payload
                    string moduleName = "Cabinet Module";// parameters.ModuleName;

                    if (relayNumber == null || relayState == null)
                        return CommandProcessingResult.CannotComplete;

                    // fire an event which notifies the device manager of the relay switch 
                    // this is done before it's actually switched so the state is still saved even when the 
                    // module might be disconnected at this point
                    RelaySwitched(relayNumber.Value, relayState.Value);

                    CabinetModuleConnection connection = (CabinetModuleConnection)_moduleConnectionFactory.GetModuleConnection(moduleName);

                    // if the module could not be found or is not connected, return cannot complete
                    if (connection == null || connection.ModuleStatus != ModuleConnectionStatus.Connected)
                        return CommandProcessingResult.CannotComplete;

                    // execute the command, switch the relay
                    connection.SwitchRelay(relayNumber.Value, relayState.Value);
                }
                catch (Exception)
                {
                    return CommandProcessingResult.CannotComplete;
                }
            }

            return CommandProcessingResult.CannotComplete;
        }
    }
}