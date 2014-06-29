# ASP.NET vNext Addin for SharpDevelop

This is a simple prototype that adds [ASP.NET vNext](https://github.com/aspnet/Home) support to [SharpDevelop](http://www.icsharpcode.net/OpenSource/SD/).

Minimal support for ASP.NET vNext.

 * Adds two menu options to File - Open.
   * ASP.NET vNext Project - Used to open a project.json file.
   * ASP.NET vNext Solution Directory - Used to open a set of project.json files inside the directory.
 * Hosts klr.exe
 * References in Projects window taken from information returned from klr.exe
 
Currently does not create a solution (.sln) or project file (.kproj) nor supports opening these.
