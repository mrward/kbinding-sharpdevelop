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

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.KBinding
{
	public class KRuntimeHome
	{
		string[] paths;
		
		public KRuntimeHome(params string[] paths)
		{
			this.paths = paths;
		}
		
		public IEnumerable<string> GetPaths()
		{
			return paths;
		}
		
		public static KRuntimeHome Find()
		{
			string path = Environment.GetEnvironmentVariable("KRE_HOME");
			if (path != null) {
				return KRuntimeHome.FromEnvironmentVariable(path);
			}
			return new KRuntimeHome(GetGlobalKRuntimePath(), GetUserProfileKRuntimePath());
		}
		
		static KRuntimeHome FromEnvironmentVariable(string value)
		{
			string[] directories = value
				.Split(';')
				.Select(directory => Environment.ExpandEnvironmentVariables(directory))
				.ToArray();
			return new KRuntimeHome(directories);
		}
		
		static string GetGlobalKRuntimePath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "KRE");
		}
		
		static string GetUserProfileKRuntimePath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".kre");
		}
		
		public IEnumerable<string> GetRuntimePaths()
		{
			return paths
				.Select(path => Path.Combine(path, "packages"))
				.Where(path => Directory.Exists(path))
				.SelectMany(path => Directory.EnumerateDirectories(path, "KRE-*", SearchOption.TopDirectoryOnly));
		}
	}
}
