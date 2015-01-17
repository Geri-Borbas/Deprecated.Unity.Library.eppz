using UnityEngine;
using System.Collections;


// From http://stackoverflow.com/questions/3174152/isnulloremptyorwhitespace-problem-in-c-sharp
public static class EPPZKit_String
{


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
}