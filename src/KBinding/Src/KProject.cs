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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using Microsoft.Framework.DesignTimeHost.Models.OutgoingMessages;

namespace ICSharpCode.KBinding
{
	public class KProject : IProject
	{
		SimpleModelCollection<ProjectItem> items = new SimpleModelCollection<ProjectItem>();
		
		public KProject(string fileName, KSolution parentSolution)
		{
			FileName = new FileName(fileName);
			ParentSolution = parentSolution;
		}
		
		public void LoadFiles()
		{
			foreach (string fileName in System.IO.Directory.GetFiles(Directory, "*.*", SearchOption.AllDirectories)) {
				var item = new FileProjectItem(this, ItemType.None) {
					FileName = new FileName(fileName)
				};
				items.Add(item);
			}
		}
		
		public event EventHandler<ParseInformationEventArgs> ParseInformationUpdated;
		public event EventHandler Disposed;
		public event EventHandler ActiveConfigurationChanged;
		
		public object SyncRoot {
			get { return items; }
		}
		
		public IMutableModelCollection<ProjectItem> Items {
			get { return items; }
		}
		
		public IReadOnlyCollection<ItemType> AvailableFileItemTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IMutableModelCollection<SolutionSection> ProjectSections {
			get {
				throw new NotImplementedException();
			}
		}
		
		public FileName FileName { get; set; }
		
		public string Name {
			get {
				return Path.GetFileName(FileName.GetParentDirectory());
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public DirectoryName Directory {
			get {
				return FileName.GetParentDirectory();
			}
		}
		
		public bool IsReadOnly { get; private set; }
		
		public string AssemblyName {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string RootNamespace {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public FileName OutputAssemblyFullPath {
			get {
				return null;
			}
		}
		
		public string Language {
			get { return "C#"; }
		}
		
		public string AppDesignerFolder {
			get { return String.Empty; }
		}
		
		public ConfigurationMapping ConfigurationMapping {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStartable {
			get { return false; }
		}
		
		public Properties Preferences {
			get { return new Properties(); }
		}
		
		public SolutionFormatVersion MinimumSolutionVersion {
			get { return SolutionFormatVersion.VS2012; }
		}
		
		public IProjectContent ProjectContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ILanguageBinding LanguageBinding {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IAssemblyModel AssemblyModel {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDisposed { get; private set; }
		
		public ISolutionFolder ParentFolder {
			get {
				return ParentSolution;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ISolution ParentSolution { get; private set; }
		
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
		
		public ConfigurationAndPlatform ActiveConfiguration {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<ProjectItem> GetItemsOfType(ItemType type)
		{
			yield break;
		}
		
		public ItemType GetDefaultItemType(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public void Save()
		{
			throw new NotImplementedException();
		}
		
		public bool IsFileInProject(FileName fileName)
		{
			return FindFile(fileName) != null;
		}
		
		public FileProjectItem FindFile(FileName fileName)
		{
			lock (SyncRoot) {
				foreach (ProjectItem item in this.Items) {
					var fileItem = item as FileProjectItem;
					if (fileItem != null) {
						if (fileItem.FileName == fileName) {
							return fileItem;
						}
					}
				}
			}
			
			return null;
		}
		
		public void SavePreferences()
		{
			throw new NotImplementedException();
		}
		
		public void Start(bool withDebugging)
		{
			try {
				ProcessStartInfo processStartInfo = CreateProcessStartInfo();
				if (withDebugging) {
					SD.Debugger.Start(processStartInfo);
				} else {
					SD.Debugger.StartWithoutDebugging(processStartInfo);
				}
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
			}
		}
		
		ProcessStartInfo CreateProcessStartInfo()
		{
			return KRuntimeProcessStartInfo.CreateConsoleStartInfo(Directory);
		}
		
		public ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken)
		{
			yield break;
		}
		
		public void ProjectCreationComplete()
		{
		}
		
		public void ProjectLoaded()
		{
		}
		
		public XElement LoadProjectExtensions(string name)
		{
			return null;
		}
		
		public void SaveProjectExtensions(string name, XElement element)
		{
		}
		
		public bool HasProjectType(Guid projectTypeGuid)
		{
			return false;
		}
		
		public string GetDefaultNamespace(string fileName)
		{
			return String.Empty;
		}
		
		public CodeDomProvider CreateCodeDomProvider()
		{
			throw new NotImplementedException();
		}
		
		public void GenerateCodeFromCodeDom(System.CodeDom.CodeCompileUnit compileUnit, System.IO.TextWriter writer)
		{
			throw new NotImplementedException();
		}
		
		public IAmbience GetAmbience()
		{
			throw new NotImplementedException();
		}
		
		public ISymbolSearch PrepareSymbolSearch(ISymbol entity)
		{
			throw new NotImplementedException();
		}
		
		public void OnParseInformationUpdated(ParseInformationEventArgs args)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			throw new NotImplementedException();
		}
		
		public Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			throw new NotImplementedException();
		}
		
		public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
		}
		
		public IEnumerable<ProjectItem> GetCodeItems()
		{
			lock (SyncRoot) {
				return Items
					.Where(item => item is FileProjectItem)
					.Where(item => item.FileName.GetExtension().Equals(".cs", StringComparison.OrdinalIgnoreCase))
					.ToList();
			}
		}
		
		public void UpdateReferences(ReferencesMessage message)
		{
			RemoveExistingReferences();
			
			foreach (KeyValuePair<string, ReferenceDescription> dependency in message.Dependencies) {
				if (dependency.Value.Type != "Project") {
					var referenceItem = new ReferenceProjectItem(this) {
						FileName = GetFileName(dependency.Value),
						Include = dependency.Key
					};
					Items.Add(referenceItem);
					RaiseProjectItemAdded(referenceItem);
				}
			}
			
		}
		
		FileName GetFileName(ReferenceDescription reference)
		{
			if (reference.Type == "Assembly") {
				return new FileName(reference.Path);
			}
			return null;
		}
		
		void RemoveExistingReferences()
		{
			lock (SyncRoot) {
				Items.RemoveAll(item => item is ReferenceProjectItem);
			}
		}
		
		void RaiseProjectItemAdded(ProjectItem projectItem)
		{
			IProjectServiceRaiseEvents projectEvents = SD.GetService<IProjectServiceRaiseEvents>();
			if (projectEvents != null) {
				projectEvents.RaiseProjectItemAdded(new ProjectItemEventArgs(this, projectItem));
			}
		}
	}
}
