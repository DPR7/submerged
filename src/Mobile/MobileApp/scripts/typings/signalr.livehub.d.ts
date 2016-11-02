// Autogenerated by SignalrTypescriptGenerator.exe at 16-8-2016 09:17:01
// Hubs
interface SignalR
{
	liveHub : Repsaj.Submerged.API.Hubs.LiveHub;
}

// Service contracts

declare module Repsaj.Submerged.API.Hubs
{

	interface LiveHub
	{
		server : Repsaj.Submerged.API.Hubs.LiveHubServer;
		client : any;
	}

	interface LiveHubServer
	{
		sendMessage(name : string, message : string) : JQueryPromise<void>;
		getServerTime() : JQueryPromise<string>;
	}
}


// Clients

// Data contracts

// Enums