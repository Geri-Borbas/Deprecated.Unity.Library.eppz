using UnityEngine;
using System.Collections;


public static class _String
{


	// Mainly from http://stackoverflow.com/questions/3174152/isnulloremptyorwhitespace-problem-in-c-sharp


	public static bool IsNullOrWhiteSpace(this string value)
	{
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool IsNullOrEmptyOrWhiteSpace(this string value)
	{
		return string.IsNullOrEmpty(value) ||
			ReferenceEquals(value, null) ||
				string.IsNullOrEmpty(value.Trim(' '));
	}

	public static string RemoveLastWord(this string value)
	{
		if (value == null) return ""; // Only if any

		string result = value;
		int index = value.LastIndexOf(" ");
		if (index > -1) result = value.Remove(index);
		return result;
	}
}