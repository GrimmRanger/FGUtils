using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FGUtils
{
	public partial class FGUtilsViewController : UIViewController
	{
		public FGUtilsViewController () : base ("FGUtilsViewController", null) {}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "FGUtils Main Menu";
			NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, null);

			TableView.Source = new FGUtilsTableSource (this);
		}
	}
}