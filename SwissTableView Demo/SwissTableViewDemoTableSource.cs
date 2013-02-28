using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Drawing;

namespace FGUtils
{
	public class SwissTableViewDemoTableSource : SwissTableSource
	{
		public SwissTableViewDemoTableSource (List<string> stuff)
		{
			_stuff = stuff;
		}

		List<string> _stuff;

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _stuff.Count;
		}
		
		public override float GetHeightForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (ExpandedPath != null && indexPath.Equals(ExpandedPath))
				return 100;
			
			return 50;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			ExpandingCell cell = (ExpandingCell)tableView.DequeueReusableCell ("cell");
			if (cell == null)
				cell = ExpandingCell.View (new RectangleF (0, 0, tableView.Frame.Width, 100));
			
			cell.SetTitle (string.Format("Row: {0}", _stuff[indexPath.Row]));
			cell.SetDetail (string.Format ("Detail {0}", _stuff[indexPath.Row]));
			
			if (ExpandedPath != null && indexPath.Equals(ExpandedPath))
				cell.ToggleDetail ();
			
			return cell;
		}

		public override void DeleteRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			base.DeleteRow (tableView, indexPath);

			_stuff.RemoveAt (indexPath.Row);
		}

		public override void DidCollapseCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
		{
			base.WillCollapseCell (tableView, path);

			ExpandingCell cell = (ExpandingCell)tableView.CellAt (path);
			if (cell != null)
				cell.ToggleDetail ();
		}
	}
}

