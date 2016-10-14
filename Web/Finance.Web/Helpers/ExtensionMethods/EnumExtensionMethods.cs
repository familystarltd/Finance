using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Finance.Web.Helpers.ExtensionMethods
{
    public static class EnumExtensionMethods
	{
		public static SelectList ToSelectList<TEnum>(this TEnum enumObj, Func<TEnum, bool> predicate = null, bool addPleaseSelect = false,string selectMsg = "--- please select ---", bool shuffleList = false)
		{
			IEnumerable<TEnum> values = (from TEnum e in Enum.GetValues(typeof(TEnum))
											select e);

			if (predicate != null)
				values = (from TEnum e in values
							where predicate(e)
							select e);

			if (shuffleList)
				values = values.ToList().ShuffleList();

			var outputs = (from TEnum e in values
							select new SelectListItem { Value = e.ToString(), Text = (e as Enum).GetDescriptionString() });

			if (addPleaseSelect)
			{
                var pleaseSelect = new List<SelectListItem> { new SelectListItem { Text = selectMsg, Value = "" } };
				outputs = pleaseSelect.Concat(outputs).ToList();
			}

			return new SelectList(outputs, "Value", "Text", enumObj);
		}

		public static string GetDescriptionString(this Enum val)
		{
			try
			{
				var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

				return attributes.Length > 0 ? attributes[0].Description : val.ToString().PascalCaseToPrettyString();
			}
			catch (Exception)
			{
				return val.ToString().PascalCaseToPrettyString();
			}
		}

	}
}