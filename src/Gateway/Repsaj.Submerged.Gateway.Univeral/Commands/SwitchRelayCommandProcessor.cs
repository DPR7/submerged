﻿using Repsaj.Submerged.GatewayApp.Universal.Models;
using Repsaj.Submerged.GatewayApp.Universal.Modules;
using System;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Universal.Commands
{
    public class SwitchRelayCommandProcessor : ICommandProcessor
    {
        IModuleConnectionFactory _moduleConnectionFactory;
        public event Action<string, bool> RelaySwitched;

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

                    string moduleName = parameters.ModuleName;
                    string relayName = parameters.RelayName;
                    bool? relayState = parameters.RelayState;

                    if (relayName == null || relayState == null)
                        return CommandProcessingResult.CannotComplete;

                    // fire an event which notifies the device manager of the relay switch 
                    // this is done before it's actually switched so the state is still saved even when the 
                    // module might be disconnected at this point
                    RelaySwitched(relayName, relayState.Value);

                    IModuleConnection connection = _moduleConnectionFactory.GetModuleConnection(moduleName);

                    // if the module could not be found or is not connected, return cannot complete
                    if (connection == null || connection.ModuleStatus != ModuleConnectionStatus.Connected)
                        return CommandProcessingResult.CannotComplete;

                    IRelayModule relayConnection = (IRelayModule)connection;

                    // execute the command, switch the relay
                    relayConnection.SwitchRelay(relayName, relayState.Value);

                    return CommandProcessingResult.Success;
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
