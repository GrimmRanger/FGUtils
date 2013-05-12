// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace FGUtils
{
	partial class MessageCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView Header { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView Content { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Header != null) {
				Header.Dispose ();
				Header = null;
			}

			if (Content != null) {
				Content.Dispose ();
				Content = null;
			}
		}
	}
}
