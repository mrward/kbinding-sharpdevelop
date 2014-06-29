// Based on code from https://github.com/davidfowl/DesignTimeHostDemo

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop.Project;
using Microsoft.Framework.DesignTimeHost.Models;
using Microsoft.Framework.DesignTimeHost.Models.IncomingMessages;
using Newtonsoft.Json.Linq;

namespace ICSharpCode.KBinding
{
	public class SolutionFileWatcher : IDisposable
	{
		ProcessingQueue queue;
		string hostId;
		FileWatcher fileWatcher;
		Dictionary<string, int> projects;
		Dictionary<int, string> mapping;

		int contextId;
		string activeTargetFramework = "net45";
		
		public SolutionFileWatcher(ProcessingQueue queue, string hostId)
		{
			this.queue = queue;
			this.hostId = hostId;
		}
		
		public void Start(KSolution solution)
		{
			fileWatcher = new FileWatcher(solution.Directory);
			WatchSolutionFiles(solution);
			fileWatcher.OnChanged += changedPath => OnFileChanged(changedPath);
		}
		
		void WatchSolutionFiles(KSolution solution)
		{
			projects = new Dictionary<string, int>();
			mapping = new Dictionary<int, string>();
			
			foreach (KProject project in solution.Projects) {
				string projectPath = project.Directory.ToString().TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

				// Send an InitializeMessage for each project
				var initializeMessage = new InitializeMessage
				{
					ProjectFolder = projectPath,
					TargetFramework = activeTargetFramework
				};

				// Create a unique id for this project
				int projectContextId = contextId++;

				// Create a mapping from path to contextid and back
				projects[projectPath] = projectContextId;
				mapping[projectContextId] = projectPath;

				// Initialize this project
				queue.Post(new Message {
					ContextId = projectContextId,
					MessageType = "Initialize",
					Payload = JToken.FromObject(initializeMessage),
					HostId = hostId
				});

				// Watch the project.json file
				fileWatcher.WatchFile(project.FileName.ToString());
				fileWatcher.WatchDirectory(project.Directory, ".cs");

				// Watch all directories for cs files
				foreach (ProjectItem item in project.GetCodeItems()) {
					fileWatcher.WatchFile(item.FileName);
				}

				foreach (string directory in Directory.GetDirectories(project.Directory, "*.*", SearchOption.AllDirectories)) {
					fileWatcher.WatchDirectory(directory, ".cs");
				}
			}
		}

		void OnFileChanged(string changedPath)
		{
			foreach (var project in projects) {
				if (changedPath.StartsWith(project.Key, StringComparison.OrdinalIgnoreCase)) {
					queue.Post(
						new Message {
							ContextId = project.Value,
							MessageType = "FilesChanged",
							HostId = hostId
						});
				}
			}
		}
		
		public void Dispose()
		{
			if (fileWatcher != null) {
				fileWatcher.Dispose();
			}
		}
		
		public string GetProjectPath(int contextId)
		{
			return mapping[contextId];
		}
	}
}
