﻿using System;
using System.Text.RegularExpressions;

namespace Finance.Web.Helpers.ExtensionMethods
{
	public static class StringExtensionMethods
	{
		public static string PascalCaseToPrettyString(this string s)
		{
			return Regex.Replace(s, @"(\B[A-Z]|[0-9]+)", " $1");
		}

        public static string ToString(this DateTime inputDate, bool showOnlyDate)
        {
            if (showOnlyDate)
            {
                string returnString;

                if (inputDate == DateTime.MinValue)
                {
                    returnString = "Please select date";
                }
                else
                {
                    returnString = inputDate.ToString("dd/MM/yyyy");
                }
                return returnString;
            }
            return inputDate.ToString();
        }

	}
}