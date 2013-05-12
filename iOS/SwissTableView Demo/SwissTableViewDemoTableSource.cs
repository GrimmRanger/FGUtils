using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace FGUtils
{
	public class SwissTableViewDemoTableSource : SwissTableSource
	{
		public SwissTableViewDemoTableSource (List<Message> messages) 
		{
			_messages = messages; 
		}
		
		public List<Message> _messages;
		
		public override int RowsInSection (UITableView tableview, int section)
		{
			return _messages.Count;
		}
		
		public override float GetHeightForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (ExpandedPath != null && indexPath.Equals(ExpandedPath) && indexPath.Row != _messages.Count)
				return 160;
			
			return 80;
		}
		
		public override float GetHeightForFooter (UITableView tableView, int section)
		{
			return 1;
		}
		
		public override UIView GetViewForFooter (UITableView tableView, int section)
		{
			return new UIView (new RectangleF (0, 0, tableView.Frame.Width, 1));
		}
		
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			MessageCell cell = (MessageCell)tableView.DequeueReusableCell ("MessageCell");
			if (cell == null)
				cell = MessageCell.View (new RectangleF(0, 0, tableView.Frame.Width, 80), "MessageCell");
			
			cell.UpdateCell (_messages [indexPath.Row]);
			
			return cell;
		}
		
		public override void DeleteRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (indexPath.Row != _messages.Count) 
			{
				base.DeleteRow (tableView, indexPath);
				
				// TODO remove from messageService as well
				_messages.RemoveAt (indexPath.Row);
			}
		}
	}
}