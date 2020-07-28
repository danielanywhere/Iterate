using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Iterate.IterateUtil;

namespace Iterate
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Main instance of the application.
	/// </summary>
	public class Program
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	ProcessDirectories																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process the currently selected directory and optionally, any
		/// subdirectories.
		/// </summary>
		private void ProcessDirectories()
		{
			DirectoryInfo[] directories = null;
			FileInfo[] files = null;
			string relDir = "";
			string value = "";

			if(mDirectory != null)
			{
				//	Get the relative directory.
				if(mDirectory.FullName.Length != mStartFolderName.Length)
				{
					relDir = GetRelativeDirectory(mStartFolderName, mDirectory.FullName);
					mVariables.Set("RelativeFolderName", relDir);
				}
				Console.WriteLine($"Directory: {relDir}");
				//	Get the list of files for the current folder.
				files = mDirectory.GetFiles(mFileMask);
				value = mDirectory.FullName;
				if(value.EndsWith(@"\") || value.EndsWith("/"))
				{
					value = value.Substring(0, value.Length - 1);
				}
				mVariables.Set("FullFoldername", value);
				foreach(FileInfo file in files)
				{
					//	Each matching file.
					mIndex++;
					mVariables.Set("Index", mIndex.ToString());
					mVariables.Set("Extension", file.Extension);
					mVariables.Set("Filename", file.Name);
					mVariables.Set("FilenameOnly", GetFilenameOnly(file));
					mVariables.Set("FullFilename", file.FullName);
					Console.WriteLine($" {file.Name}");
					RunCommands();
				}
				//	Process sub-directories.
				if(mRecurse)
				{
					directories = mDirectory.GetDirectories();
					foreach(DirectoryInfo directory in directories)
					{
						mDirectory = directory;
						ProcessDirectories();
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RunCommands																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the commands list with the currently configured variables.
		/// </summary>
		public void RunCommands()
		{
			Process command = null;
			string content = "";
			ProcessStartInfo process = null;

			foreach(string commandValue in mCommands)
			{
				content = ReplaceVariables(commandValue, mVariables);

				process = new ProcessStartInfo("cmd.exe");
				process.Arguments = $"/S /C \"{content}\"";
				process.UseShellExecute = false;
				if(mLogFilename?.Length > 0)
				{
					process.RedirectStandardError = true;
					process.RedirectStandardOutput = true;
				}
				command = Process.Start(process);
				if(mLogFilename?.Length > 0)
				{
					content = command.StandardOutput.ReadToEnd() +
						command.StandardError.ReadToEnd();
				}
				command.WaitForExit();
				if(mLogFilename?.Length > 0)
				{
					File.AppendAllText(mLogFilename,
						DateTime.Now.ToString("yyyyMMdd.HHmmss") + "\r\n" +content);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Main																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Configure and run the application.
		/// </summary>
		public static void Main(string[] args)
		{
			bool bShowHelp = false; //	Flag - Explicit Show Help.
			string content = "";    //	Working content.
			string key = "";        //	Current Parameter Key.
			string lowerArg = "";   //	Current Lowercase Argument.
			MatchCollection matches = null;
			string message = "";    //	Message to display in Console.
			string name = "";				//	Current working name.
			Program prg = new Program();  //	Initialized instance.
			string value = "";			//	Current working value.

			Console.WriteLine("Iterate.exe");
			foreach(string arg in args)
			{
				lowerArg = arg.ToLower();
				key = "/?";
				if(lowerArg == key)
				{
					bShowHelp = true;
					continue;
				}
				key = "/commands:";
				if(lowerArg.StartsWith(key))
				{
					content = arg.Substring(key.Length).Replace('^', '"');
					matches = Regex.Matches(content, ResourceMain.rxCommands);
					foreach(Match match in matches)
					{
						value = GetValue(match, "v");
						if(value?.Length > 0)
						{
							prg.Commands.Add(value);
						}
					}
					continue;
				}
				key = "/filemask:";
				if(lowerArg.StartsWith(key))
				{
					prg.mFileMask = arg.Substring(key.Length);
					if(prg.mFileMask.Length == 0)
					{
						prg.mFileMask = "*";
					}
					continue;
				}
				key = "/folder:";
				if(lowerArg.StartsWith(key))
				{
					prg.StartFolderName = arg.Substring(key.Length);
					continue;
				}
				key = "/log:";
				if(lowerArg.StartsWith(key))
				{
					prg.mLogFilename = arg.Substring(key.Length);
					continue;
				}
				key = "/recurse:";
				if(lowerArg.StartsWith(key))
				{
					prg.Recurse = "1yestrue".Contains(lowerArg.Substring(key.Length));
					continue;
				}
				key = "/variables:";
				if(lowerArg.StartsWith(key))
				{
					content = arg.Substring(key.Length).Replace('^', '"');
					matches = Regex.Matches(content, ResourceMain.rxVariables);
					foreach(Match match in matches)
					{
						name = GetValue(match, "n");
						value = GetValue(match, "v");
						if(name?.Length > 0)
						{
							prg.mVariables.Add(name, value);
						}
					}
					continue;
				}
				key = "/wait";
				if(lowerArg.StartsWith(key))
				{
					prg.mWaitAfterEnd = true;
					continue;
				}
			}
			if(prg.mStartFolderName.Length == 0)
			{
				//	No folder was specified. Start in the current folder.
				prg.mStartFolderName = System.Environment.CurrentDirectory;
			}
			//	Truncate trailing slash.
			if(prg.mStartFolderName.EndsWith("\"") ||
				prg.mStartFolderName.EndsWith("/"))
			{
				prg.mStartFolderName =
					prg.mStartFolderName.Substring(0, prg.mStartFolderName.Length - 1);
			}
			if(bShowHelp)
			{
				//	Display Syntax.
				Console.WriteLine(message.ToString() + "\r\n" + ResourceMain.Syntax);
			}
			else
			{
				//	Run the configured application.
				prg.Run();
			}
			if(prg.mWaitAfterEnd)
			{
				Console.WriteLine("Press [Enter] to exit...");
				Console.ReadLine();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Commands																															*
		//*-----------------------------------------------------------------------*
		private List<string> mCommands = new List<string>();
		/// <summary>
		/// Get a reference to the list of commands in this session.
		/// </summary>
		public List<string> Commands
		{
			get { return mCommands; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Directory																															*
		//*-----------------------------------------------------------------------*
		private DirectoryInfo mDirectory = null;
		/// <summary>
		/// Get/Set a reference to the currently selected directory.
		/// </summary>
		public DirectoryInfo Directory
		{
			get { return mDirectory; }
			set { mDirectory = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FileMask																															*
		//*-----------------------------------------------------------------------*
		private string mFileMask = "*";
		/// <summary>
		/// Get/Set the mask for files to select.
		/// </summary>
		public string FileMask
		{
			get { return mFileMask; }
			set { mFileMask = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Index																																	*
		//*-----------------------------------------------------------------------*
		private int mIndex = 0;
		/// <summary>
		/// Get/Set the file index.
		/// </summary>
		/// <remarks>
		/// This value is one-based when files have been found, and is equal to
		/// zero before the first file is found.
		/// </remarks>
		public int Index
		{
			get { return mIndex; }
			set { mIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	LogFilename																														*
		//*-----------------------------------------------------------------------*
		private string mLogFilename = "";
		/// <summary>
		/// Get/Set the filename of the logfile to generate.
		/// </summary>
		public string LogFilename
		{
			get { return mLogFilename; }
			set { mLogFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Recurse																																*
		//*-----------------------------------------------------------------------*
		private bool mRecurse = false;
		/// <summary>
		/// Get/Set a value indicating whether to recurse sub-directories.
		/// </summary>
		public bool Recurse
		{
			get { return mRecurse; }
			set { mRecurse = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured application.
		/// </summary>
		public void Run()
		{
			//	Add the built-in variables.
			//	AddUnique and Set are used redundantly for the sake of clarity.
			mVariables.AddUnique(new string[]
			{
				"Extension",
				"Filename",
				"FilenameOnly",
				"FullFilename",
				"FullFoldername",
				"Index",
				"RelativeFolderName",
				"StartFolderName",
				"Username"
			});
			mVariables.Set("FullFolderName", mStartFolderName);
			mVariables.Set("Index", "0");
			mVariables.Set("StartFolderName", mStartFolderName);
			mVariables.Set("Username", RightOf(Environment.UserName, @"\"));
			//	Initialize the directory.
			mStartFolderName = ReplaceVariables(mStartFolderName, mVariables);
			mDirectory = new DirectoryInfo(mStartFolderName);
			if(mDirectory.Exists)
			{
				//	Iterate through all files and folders in the scope.
				ProcessDirectories();
			}
			else
			{
				Console.WriteLine($"Folder {mStartFolderName} does not exist...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StartFolderName																												*
		//*-----------------------------------------------------------------------*
		private string mStartFolderName = "";
		/// <summary>
		/// Get/Set the name of the starting folder.
		/// </summary>
		public string StartFolderName
		{
			get { return mStartFolderName; }
			set { mStartFolderName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Variables																															*
		//*-----------------------------------------------------------------------*
		private NameValueCollection mVariables = new NameValueCollection();
		/// <summary>
		/// Get a reference to the variables defined in this session.
		/// </summary>
		public NameValueCollection Variables
		{
			get { return mVariables; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WaitAfterEnd																													*
		//*-----------------------------------------------------------------------*
		private bool mWaitAfterEnd = false;
		/// <summary>
		/// Get/Set a value indicating whether the application will wait for user
		/// keypress after finishing.
		/// </summary>
		public bool WaitAfterEnd
		{
			get { return mWaitAfterEnd; }
			set { mWaitAfterEnd = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
