﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

namespace ICSharpCode.KBinding
{
	public class KRuntime
	{
		public KRuntime(string path)
		{
			Path  = path;
		}
		
		public string Path { get; private set; }
		
		public override string ToString()
		{
			return Path;
		}
		
		public string KlrPath()
		{
			return System.IO.Path.Combine(Path, "bin", "klr.exe");
		}
		
		public static KRuntime GetDefaultRuntime()
		{
			KRuntimeHome home = KRuntimeHome.Find();
			return GetDefaultRuntime(home.GetRuntimePaths());
		}
		
		public static KRuntime GetDefaultRuntime(IEnumerable<string> runtimePaths)
		{
			string[] directories = GetPathEnvironmentVariableDirectories();
			string defaultRuntimePath = runtimePaths
				.Where(path => directories.Any(directory => directory.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
				.FirstOrDefault();
			
			if (defaultRuntimePath != null) {
				return new KRuntime(defaultRuntimePath);
			}
			return null;
		}
		
		static string[] GetPathEnvironmentVariableDirectories()
		{
			string value = Environment.GetEnvironmentVariable("PATH");
			if (value != null) {
				return value.Split(';');
			}
			return new string[0];
		}
	}
}
