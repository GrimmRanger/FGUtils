using System;
using MonoTouch.Foundation;

namespace FGUtils
{
	public static class NSIndexExtensions
	{
		public static bool GreaterThan(this NSIndexPath left, NSIndexPath right)
		{
			if (left.Section > right.Section) 
			{
				return true;
			} 
			else 
			{
				return left.Section == right.Section && left.Row > right.Row;
			}
		}
	}
}