using System;
using MonoTouch.UIKit;

namespace FGUtils
{
	public static class UIScreenExtensions
	{
		public static bool IsTall (this UIScreen screen)
		{
			if (screen.Bounds.Size.Height > 480f)
				return true;

			return false;
		}
	}
}