// Based on from https://github.com/davidfowl/DesignTimeHostDemo

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using Microsoft.Framework.DesignTimeHost.Models.OutgoingMessages;

namespace ICSharpCode.KBinding
{
	public class ProcessingQueueProvider
	{
		public static ProcessingQueue CreateProcessingQueue(int port)
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(new IPEndPoint(IPAddress.Loopback, port));

			var networkStream = new NetworkStream(socket);

			Console.WriteLine("Connected");

			var mapping = new Dictionary<int, string>();
			var queue = new ProcessingQueue(networkStream);

//			queue.OnReceive += m =>
//			{
//				// Get the project associated with this message
//				var projectPath = mapping[m.ContextId];
//
//				// This is where we can handle messages and update the
//				// language service
//				if (m.MessageType == "References")
//				{
//					// References as well as the dependency graph information
//					var val = m.Payload.ToObject<ReferencesMessage>();
//				}
//				else if (m.MessageType == "Diagnostics")
//				{
//					// Errors and warnings
//					var val = m.Payload.ToObject<DiagnosticsMessage>();
//				}
//				else if (m.MessageType == "Configurations")
//				{
//					// Configuration and compiler options
//					var val = m.Payload.ToObject<ConfigurationsMessage>();
//				}
//				else if (m.MessageType == "Sources")
//				{
//					// The sources to feed to the language service
//					var val = m.Payload.ToObject<SourcesMessage>();
//				}
//			};
			
			return queue;
		}
	}
}
