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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using Microsoft.Framework.DesignTimeHost.Models.OutgoingMessages;

namespace ICSharpCode.KBinding
{
	public class KProject : CompilableProject
	{
		public KProject(string fileName, KSolution solution)
			: base(CreateProjectCreateInfo(fileName, solution))
		{
		}
		
		static ProjectCreateInformation CreateProjectCreateInfo(string fileName, KSolution solution)
		{
			return new ProjectCreateInformation {
				Solution = solution,
				OutputProjectFileName = fileName,
				ProjectName = Path.GetFileName(Path.GetDirectoryName(fileName))
			};
		}
		
		public void LoadFiles()
		{
			foreach (string fileName in System.IO.Directory.GetFiles(Directory, "*.*", SearchOption.AllDirectories)) {
				var item = new FileProjectItem(this, GetDefaultItemType(fileName)) {
					FileName = new FileName(fileName)
				};
				ProjectService.AddProjectItem(this, item);
			}
		}
		
		public override string Language {
			get { return "C#"; }
		}
		
		public override bool IsStartable {
			get { return false; }
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (IsCSharpFile(fileName)) {
				return ItemType.Compile;
			}
			return ItemType.None;
		}
		
		static bool IsCSharpFile(string fileName)
		{
			if (fileName == null)
				return false;
			
			string extension = Path.GetExtension(fileName);
			return String.Equals(".cs", extension, StringComparison.OrdinalIgnoreCase);
		}
		
		public override void Start(bool withDebugging)
		{
			try {
				ProcessStartInfo processStartInfo = CreateProcessStartInfo();
				if (withDebugging) {
					DebuggerService.CurrentDebugger.Start(processStartInfo);
				} else {
					DebuggerService.CurrentDebugger.StartWithoutDebugging(processStartInfo);
				}
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
			}
		}
		
		ProcessStartInfo CreateProcessStartInfo()
		{
			return KRuntimeProcessStartInfo.CreateConsoleStartInfo(Directory);
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
					ProjectService.AddProjectItem(this, referenceItem);
				}
			}
		}
		
		FileName GetFileName(ReferenceDescription reference)
		{
			if (reference.Type == "Assembly") {
				return new FileName(reference.Path);
			} else if (reference.Type == "Package") {
				return new FileName(Path.Combine(reference.Path, "lib", "contract", reference.Name + ".dll"));
			}
			return null;
		}
		
		void RemoveExistingReferences()
		{
			lock (SyncRoot) {
				List<ProjectItem> items = GetItemsOfType(ItemType.Reference).ToList();
				foreach (ProjectItem item in items) {
					ProjectService.RemoveProjectItem(this, item);
				}
			}
		}
		
		public override void Save(string fileName)
		{
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.CSharp; }
		}
		
		public IEnumerable<ProjectItem> GetCodeItems()
		{
			return Items.Where(item => IsCSharpFile(item.FileName));
		}
	}
}
