// Based on code from https://github.com/davidfowl/DesignTimeHostDemo

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.KBinding
{
	public class KRuntimeRunner
	{
		Process kreProcess;
		Task<Process> task;
		CancellationTokenSource cancellationTokenSource;
		
		public event EventHandler Started;
		
		public KRuntimeRunner()
		{
		}
		
		public void Start(string runtimePath, string hostId, int port)
		{
			Stop();
			
			TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			cancellationTokenSource = new CancellationTokenSource();
			task = new Task<Process>(
				() => StartRuntime(runtimePath, hostId, port),
				cancellationTokenSource.Token);
			task.ContinueWith(result => OnRuntimeStarted(result), scheduler);
			task.Start();
		}
		
		void OnRuntimeStarted(Task<Process> task)
		{
			if (task.IsFaulted) {
				Console.WriteLine(task.Exception);
			} else {
				kreProcess = task.Result;
				OnStarted();
			}
		}
		
		public void Stop()
		{
			if (kreProcess == null)
				return;
			
			try {
				kreProcess.Kill();
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}
		
		Process StartRuntime(string runtimePath, string hostId, int port)
		{
			var psi = new ProcessStartInfo
			{
				FileName = Path.Combine(runtimePath, "bin", "klr.exe"),
				Arguments = String.Format(
					@"--appbase ""{0}"" {1} {2} {3} {4}",
					Directory.GetCurrentDirectory(),
					Path.Combine(runtimePath, "bin", "lib", "Microsoft.Framework.DesignTimeHost", "Microsoft.Framework.DesignTimeHost.dll"),
					port,
					Process.GetCurrentProcess().Id,
					hostId),
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
				UseShellExecute = false
			};

			Console.WriteLine(psi.FileName + " " + psi.Arguments);

			var kreProcess = Process.Start(psi);
			kreProcess.BeginOutputReadLine();
			kreProcess.BeginErrorReadLine();

			kreProcess.OutputDataReceived += (sender, e) => {
				//if (verboseOutput) {
					Console.WriteLine(e.Data);
				//}
			};

			// Wait a little bit for it to conncet before firing the callback
			Thread.Sleep(1000);

			if (kreProcess.HasExited) {
				Console.WriteLine("Child process failed with {0}", kreProcess.ExitCode);
				return null;
			}

			kreProcess.EnableRaisingEvents = true;
			kreProcess.Exited += (sender, e) => {
				Console.WriteLine("Process crash trying again");

				Thread.Sleep(1000);

				StartRuntime(runtimePath, hostId, port);
			};
			
			return kreProcess;
		}
		
		void OnStarted()
		{
			if (Started != null) {
				Started(this, new EventArgs());
			}
		}
	}
}
