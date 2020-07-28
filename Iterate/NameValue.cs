using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iterate
{
	//*-------------------------------------------------------------------------*
	//*	NameValueCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of NameValueItem Items.
	/// </summary>
	public class NameValueCollection : List<NameValueItem>
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
		//*	_Indexer																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the item with the matching name from the collection.
		/// </summary>
		/// <remarks>
		/// If the item is not found, a new item is returned. This version is
		/// case-insensitive.
		/// </remarks>
		public NameValueItem this[string name]
		{
			get
			{
				NameValueItem result = null;

				result = this.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
				if(result == null)
				{
					result = new NameValueItem();
					result.Name = name;
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection by member values.
		/// </summary>
		/// <param name="name">
		/// Name of the item to add.
		/// </param>
		/// <param name="value">
		/// Value to add.
		/// </param>
		/// <returns>
		/// Newly created and added NameValueItem.
		/// </returns>
		public NameValueItem Add(string name, string value)
		{
			NameValueItem result = new NameValueItem();

			result.Name = name;
			result.Value = value;
			this.Add(result);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AddUnique																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection if its name will be unique.
		/// </summary>
		/// <param name="name">
		/// Name of the item to add.
		/// </param>
		/// <param name="value">
		/// Value of the item to add.
		/// </param>
		/// <returns>
		/// Reference to the newly created and item, if added. Otherwise, a
		/// reference to the previously existing item.
		/// </returns>
		public NameValueItem AddUnique(string name, string value = "")
		{
			NameValueItem result = null;

			if(name?.Length > 0)
			{
				result = this.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
				if(result == null)
				{
					//	The item didn't exist.
					result = new NameValueItem();
					result.Name = name;
					this.Add(result);
				}
				if(result != null && value?.Length > 0)
				{
					result.Value = value;
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add one or more items to the collection if each of their names will be
		/// unique.
		/// </summary>
		/// <param name="names">
		/// Array of names to add.
		/// </param>
		public void AddUnique(string[] names)
		{
			if(names?.Length > 0)
			{
				foreach(string name in names)
				{
					AddUnique(name);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Set																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the specified item.
		/// </summary>
		/// <param name="name">
		/// Name of the item to set. If it does not exist, it will be created.
		/// </param>
		/// <param name="value">
		/// Value to place on the specified item.
		/// </param>
		public void Set(string name, string value)
		{
			AddUnique(name, value);
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	NameValueItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// A simple name and value.
	/// </summary>
	public class NameValueItem
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
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the item.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of the item.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
