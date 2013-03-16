// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace FGUtils
{
	partial class MessageHeader
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView Icon { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Title { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Time { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Icon != null) {
				Icon.Dispose ();
				Icon = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (Time != null) {
				Time.Dispose ();
				Time = null;
			}
		}
	}
}
