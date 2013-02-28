
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace FGUtils
{
	public partial class SwissTableViewDemoViewController : UIViewController
	{
		public SwissTableViewDemoViewController () : base ("SwissTableViewDemoViewController", null) {}

		List<string> items;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "SwissTableView Demo";

			InitItems ();
			InitSwissTableView ();
		}

		private void InitItems()
		{
			items = new List<string> ();
			for (int i = 0; i < 100; i++) 
			{
				items.Add(string.Format("{0}", i));
			}
		}

		private void InitSwissTableView()
		{
			TableView.Source = new SwissTableViewDemoTableSource (items);
			TableView.RefreshRequested += delegate {
				RefreshRequrested();
			};
			TableView.SwipeToDeleteEnabled = true;
			TableView.ExpandCollapseEnabled = true;
		}
		
		private void RefreshRequrested()
		{
			Console.WriteLine ("Refresh Requested");
			NSTimer.CreateScheduledTimer(new TimeSpan(0, 0, 2),
			                             delegate { TableView.RefreshConcluded(); });
		}
	}
}