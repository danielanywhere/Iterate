/*
 * IterateUtil.cs
 * Copyright(c) 2020. Daniel Patterson, MCSD (danielanywhere)
 * This file is licensed under GNU General Public License version 3.
 * https://opensource.org/licenses/gpl-3.0.html
 * Please see the LICENSE file in this project.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Iterate
{
	//*-------------------------------------------------------------------------*
	//*	IterateUtil																															*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Common functionality for the Iterate application.
	/// </summary>
	public class IterateUtil
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	GetFilenameOnly																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the left portion of the filename not including the extension.
		/// </summary>
		/// <param name="file">
		/// Reference to the file information object to inspect.
		/// </param>
		/// <returns>
		/// The portion of the filename to the left of the extension.
		/// </returns>
		public static string GetFilenameOnly(FileInfo file)
		{
			string result = "";

			if(file != null)
			{
				if(file.Name.IndexOf('.') > -1)
				{
					//	The filename has an extension.
					result = file.Name.Substring(0, file.Name.LastIndexOf('.'));
				}
				else
				{
					//	Filename doesn't have an extension.
					result = file.Name;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetRelativeDirectory																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the relative portion of the directory name between the new and
		/// base names.
		/// </summary>
		/// <param name="baseName">
		/// Base directory name.
		/// </param>
		/// <param name="newName">
		/// Full name of the sub-directory.
		/// </param>
		/// <returns>
		/// Relative offset name of the two directories.
		/// </returns>
		public static string GetRelativeDirectory(string baseName, string newName)
		{
			int index = 0;
			string result = "";

			if(baseName?.Length > 0 && newName?.Length > 0 &&
				newName.ToLower().StartsWith(baseName.ToLower()))
			{
				//	The new directory is an extension of the base.
				index = baseName.Length;
				result = newName.Substring(index, newName.Length - index);
				if(result.StartsWith(@"\") || result.StartsWith("/"))
				{
					result = result.Substring(1, result.Length - 1);
				}
				if(result.EndsWith(@"\") || result.EndsWith("/"))
				{
					result = result.Substring(0, result.Length - 1);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified group from the caller's match item.
		/// </summary>
		public static string GetValue(Match match, string groupName)
		{
			string result = "";

			if(match != null && groupName?.Length > 0 &&
				match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName].Value;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReplaceVariables																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Replace the variable names in the caller's template with values from
		/// the variables list.
		/// </summary>
		public static string ReplaceVariables(string template,
			NameValueCollection variables)
		{
			MatchCollection matches = null;
			string result = "";

			if(template?.Length > 0)
			{
				result = template;
				if(variables?.Count > 0)
				{
					matches = Regex.Matches(template, ResourceMain.rxVariableNames);
					foreach(Match match in matches)
					{
						result = result.Replace(GetValue(match, "f"),
							variables[GetValue(match, "n")].Value);
					}
					matches = Regex.Matches(result, ResourceMain.rxVariableNames);
					if(matches.Count > 0)
					{
						//	Continue to re-resolve until all variables are gone.
						result = ReplaceVariables(result, variables);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RightOf																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the portion of the string to the right of the specified pattern.
		/// </summary>
		/// <param name="value">
		/// Value to inspect.
		/// </param>
		/// <param name="pattern">
		/// The pattern to be excluded, along with all content to the left of it.
		/// </param>
		/// <returns>
		/// The portion of the string to the right of the specified pattern.
		/// </returns>
		public static string RightOf(string value, string pattern)
		{
			int index = 0;
			string result = "";

			if(value?.Length > 0)
			{
				result = value;		//	By default, the entire value is returned.
				if(pattern?.Length > 0 && value.IndexOf(pattern) > -1)
				{
					//	The pattern is found in the string.
					index = value.LastIndexOf(pattern) + pattern.Length;
					result = value.Substring(index, value.Length - index);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*
}
