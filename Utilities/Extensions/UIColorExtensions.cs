using System;
using MonoTouch.UIKit;

namespace ExtensionMethods
{
	public static class UIColorExtensions
	{
		public static bool EquivalentColor(this UIColor c, UIColor d)
		{
			float ca = 0;
			float cr = 0; 
			float cg = 0; 
			float cb = 0;
			float da = 0; 
			float dr = 0;
			float dg = 0; 
			float db = 0;
			c.GetRGBA(out cr, out cg, out cb, out ca);
			d.GetRGBA(out dr, out dg, out db, out da);

			if (cr == dr && cg == dg && cb == db && ca == da)
				return true;

			return false;
		}
	}
}

