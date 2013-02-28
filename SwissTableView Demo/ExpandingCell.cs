using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.ObjCRuntime;

namespace FGUtils
{
	[Register("ExpandingCell")]
	public partial class ExpandingCell : UITableViewCell
	{
		public ExpandingCell (RectangleF frame) : base(frame) {}
		public ExpandingCell (IntPtr handle) : base(handle) {}
		public ExpandingCell () : base() {}

		// Static "Constructor" to internalize loading from xib
		public static ExpandingCell View(RectangleF frame) 
		{
			ExpandingCell view = new ExpandingCell(frame);
			var views = NSBundle.MainBundle.LoadNib("ExpandingCell", view, null);
			view = Runtime.GetNSObject(views.ValueAt(0)) as ExpandingCell;

			return view;
		}

		public void SetTitle(string title)
		{
			Title.Text = title;
		}

		public void SetDetail(string detail)
		{
			Detail.Text = detail;
		}

		public void ToggleDetail()
		{
			Detail.Hidden = Detail.Hidden ? false : true;
		}
	}
}

