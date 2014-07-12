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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.KBinding
{
	public class KProjectService
	{
		public KProjectService()
		{
			//ProjectService.SolutionClosed += SolutionClosed;
			WorkbenchSingleton.MainWindow.Closed += MainWindowClosed;
		}

		void MainWindowClosed(object sender, EventArgs e)
		{
			KServices.Host.Stop();
		}
		
		void SolutionClosed(object sender, EventArgs e)
		{
			KServices.Host.Stop();
		}
		
		public void OpenDirectory(string path)
		{
			try {
				CheckDirectoryExists(path);
				if (!CloseSolution())
					return;
				
				OpenDirectoryAsSolution(path);
			} catch (Exception ex) {
				LoggingService.Error("Failed to open directory.", ex);
				MessageService.ShowError(ex.Message);
			}
		}
		
		void CheckDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
				throw new ApplicationException(string.Format("Directory does not exist {0}", path));
		}
		
		bool CloseSolution()
		{
			KServices.Host.Stop();
			return true;
			//return SD.ProjectService.CloseSolution(true);
		}
		
		void OpenDirectoryAsSolution(string path)
		{
			var solution = new KSolution(path);
			solution.LoadProjects();
			//SD.ProjectService.OpenSolution(solution);
			ProjectBrowserPad.Instance.ProjectBrowserControl.ViewSolution(solution);
			
			KServices.Host.Start(solution);
		}
		
		public void OpenProject(string fileName)
		{
			var solution = new KSolution(Path.GetDirectoryName(fileName));
			solution.LoadProject(fileName);
			//SD.ProjectService.OpenSolution(solution);
			ProjectBrowserPad.Instance.ProjectBrowserControl.ViewSolution(solution);
			
			KServices.Host.Start(solution);
		}
		
		public KProject CurrentProject {
			get { return ProjectService.CurrentProject as KProject; }
		}
	}
}
