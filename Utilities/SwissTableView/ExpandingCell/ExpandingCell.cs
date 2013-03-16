using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace FGUtils
{
	public abstract class ExpandingCell : UITableViewCell
	{
		public ExpandingCell (UITableViewCellStyle style, string reuseId) : base(style, reuseId) {}
		public ExpandingCell (RectangleF frame) : base (frame) {}
		public ExpandingCell (IntPtr handle) : base(handle) {}
		public ExpandingCell () : base() {}

		public abstract void Expand ();
		public abstract void Collapse ();
	}
}