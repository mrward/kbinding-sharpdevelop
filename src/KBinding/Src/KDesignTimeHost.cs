// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.SharpDevelop;
using Microsoft.Framework.DesignTimeHost.Models;
using Microsoft.Framework.DesignTimeHost.Models.OutgoingMessages;

namespace ICSharpCode.KBinding
{
	public class KDesignTimeHost
	{
		KRuntimeRunner runner = new KRuntimeRunner();
		SolutionFileWatcher watcher;
		KSolution solution;
		ProcessingQueue queue;
		string hostId;
		int port = 1334;
		
		public KDesignTimeHost()
		{
			runner.Started += KRuntimeStarted;
		}

		void KRuntimeStarted(object sender, EventArgs e)
		{
			var runner = sender as KRuntimeRunner;
			queue = ProcessingQueueProvider.CreateProcessingQueue(port);
			queue.OnReceive += message => OnMessageReceived(message);
			queue.Start();
			watcher = new SolutionFileWatcher(queue, hostId);
			watcher.Start(solution);
		}

		void OnMessageReceived(Message message)
		{
			KProject project = GetProject(message.ContextId);

			if (message.MessageType == "References") {
				OnReferencesMessage(project, message.Payload.ToObject<ReferencesMessage>());
			} else if (message.MessageType == "Diagnostics") {
				// Errors and warnings
				var val = message.Payload.ToObject<DiagnosticsMessage>();
			} else if (message.MessageType == "Configurations") {
				// Configuration and compiler options
				var val = message.Payload.ToObject<ConfigurationsMessage>();
			} else if (message.MessageType == "Sources") {
				// The sources to feed to the language service
				var val = message.Payload.ToObject<SourcesMessage>();
			}
		}
		
		void OnReferencesMessage(KProject project, ReferencesMessage message)
		{
			SD.MainThread.InvokeIfRequired(() => {
				project.UpdateReferences(message);
			});
		}
		
		KProject GetProject(int contextId)
		{
			string projectPath = watcher.GetProjectPath(contextId);
			return solution.GetProject(projectPath);
		}
		
		public void Start(KSolution solution)
		{
			this.solution = solution;
			
			Stop();
			
			KRuntime runtime = KRuntime.GetDefaultRuntime();
			if (runtime == null)
				throw new ApplicationException("KRE not found.");
			
			hostId = GetHostId();
			runner.Start(runtime.Path, hostId, port);
		}
		
		string GetHostId()
		{
			return Guid.NewGuid().ToString();
		}
		
		public void Stop()
		{
			runner.Stop();
			if (watcher != null) {
				watcher.Dispose();
				watcher = null;
			}
		}
	}
}
