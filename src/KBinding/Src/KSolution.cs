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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Evaluation;

namespace ICSharpCode.KBinding
{
	public class KSolution : ISolution
	{
		SimpleModelCollection<ISolutionItem> items = new SimpleModelCollection<ISolutionItem>();
		SimpleModelCollection<KProject> projects = new SimpleModelCollection<KProject>();
		
		public KSolution(string directory)
		{
			Directory = new DirectoryName (directory);
		}
		
		public void LoadProjects()
		{
			foreach (string fileName in System.IO.Directory.EnumerateFiles(Directory, "project.json", SearchOption.AllDirectories)) {
				LoadProject(fileName);
			}
		}
		
		public event EventHandler FileNameChanged;
		public event EventHandler StartupProjectChanged;
		public event EventHandler PreferencesSaving;
		public event EventHandler IsDirtyChanged;
		public event EventHandler ActiveConfigurationChanged;
		
		public ProjectCollection MSBuildProjectCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public FileName FileName {
			get { return Projects.First().FileName; }
		}
		
		public DirectoryName Directory { get; private set; }
		
		public IProject StartupProject {
			get {
				return null;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IModelCollection<IProject> Projects {
			get { return projects; }
		}
		
		public IEnumerable<ISolutionItem> AllItems {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IMutableModelCollection<SolutionSection> GlobalSections {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Properties Preferences {
			get { return new Properties(); }
		}
		
		public Properties SDSettings {
			get { return new Properties(); }
		}
		
		public bool IsReadOnly { get; private set; }
		
		public string Name {
			get {
				return System.IO.Path.GetFileName(Directory.ToString());
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IMutableModelCollection<ISolutionItem> Items {
			get { return items; }
		}
		
		public ISolutionFolder ParentFolder {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ISolution ParentSolution {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Guid IdGuid {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Guid TypeGuid {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDirty { get; private set; }
		
		public IConfigurationOrPlatformNameCollection ConfigurationNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IConfigurationOrPlatformNameCollection PlatformNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ConfigurationAndPlatform ActiveConfiguration { get; set; }
		
		public ISolutionItem GetItemByGuid(Guid guid)
		{
			throw new NotImplementedException();
		}
		
		public void SavePreferences()
		{
		}
		
		public void Save()
		{
		}
		
		public bool IsAncestorOf(ISolutionItem item)
		{
			throw new NotImplementedException();
		}
		
		public IProject AddExistingProject(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		public ISolutionFileItem AddFile(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		public ISolutionFolder CreateFolder(string name)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
		}
		
		public void LoadProject(string fileName)
		{
			var project = new KProject(fileName, this);
			project.LoadFiles();
			items.Add(project);
			projects.Add(project);
		}
		
		public KProject GetProject(string directory)
		{
			var projectDirectory = new DirectoryName(directory);
			return projects.FirstOrDefault(project => project.Directory == projectDirectory);
		}
	}
}
