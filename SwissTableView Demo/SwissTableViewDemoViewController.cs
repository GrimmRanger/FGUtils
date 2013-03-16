
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

		private List<Message> _messages;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			InitMessages ();
			InitTableView ();
			UpdateTableSource ();
		}
		
		private void InitMessages()
		{
			_messages = new List<Message> ();
			
			for (int i = 0; i < 4; i++) 
			{
				Message msg = new Message() 
				{
					Id = i,
					Title = string.Format("Message {0}", i),
					Detail = string.Format("Message {0}: This message has a really big detail, full of all sorts of information and data, enought to make your brain bleed and your eyes pop.", i)
				};
				
				_messages.Add(msg);
			}
		}
		
		private void InitTableView()
		{
			TableView.Frame = new RectangleF (0, 0, this.View.Frame.Width, this.View.Frame.Height);
			//			TableView.ContentInset = new UIEdgeInsets (0, 0, -80, 0);
			
			TableView.RefreshRequested += delegate {
				RefreshData();
			};
			TableView.SwipeToDeleteEnabled = true;
			TableView.ExpandCollapseEnabled = true;
			
			TableView.BackgroundColor = UIColor.FromRGBA (25, 27, 28, 255);
		}
		
		private void RefreshData()
		{
			int count = _messages.Count;
			for (int i = count; i < count + 5; i++) 
			{
				Message msg = new Message() 
				{
					Id = i,
					Title = string.Format("Message {0}", i),
					Detail = string.Format("Message {0}: This message has a really big detail, full of all sorts of information and data, enought to make your brain bleed and your eyes pop.", i)
				};
				
				_messages.Add(msg);
			}
			
			TableView.RefreshConcluded ();
			UpdateTableSource ();
		}
		
		private void UpdateTableSource()
		{
			TableView.Source = new SwissTableViewDemoTableSource (_messages);
		}
	}
}