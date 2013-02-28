using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace FGUtils
{
	public abstract class SwissTableSource : UITableViewSource
	{
		public SwissTableSource () : base() {}

#region Swipe to Delete Methods
		public virtual void DeleteRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (ExpandedPath != null) 
			{
				if (ExpandedPath.Equals(indexPath))
				{
					ExpandedPath = null;
				}
				else 
				{
					if (ExpandedPath.GreaterThan(indexPath))
						ExpandedPath = DecrementIndex(tableView, ExpandedPath);
				}
			}

			// Descendent class in charge of updating backing data
			// Example
			// Data[indexPath.Section].RemoveAt(indexPath.Row);
		}
#endregion

#region Expand/Collapse Methods
		protected NSIndexPath ExpandedPath = null;

		public virtual void WillExpandCell(UITableView tableView, NSIndexPath path)
		{
			ExpandedPath = path;
		}
		
		public virtual void DidExpandCell(UITableView tableView, NSIndexPath path) {}
		
		public virtual void WillCollapseCell(UITableView tableView, NSIndexPath path)
		{
			ExpandedPath = null;
		}
		
		public virtual void DidCollapseCell(UITableView tableView, NSIndexPath path) {}
#endregion

#region TableView Helper Methods
		public NSIndexPath DecrementIndex(UITableView tableView, NSIndexPath path)
		{
			if (path.Section == 0 && path.Row == 0)
				return path;
			
			if (path.Row != 0) 
			{
				return NSIndexPath.FromRowSection(path.Row - 1, path.Section);
			} 
			else
			{
				return NSIndexPath.FromRowSection(RowsInSection(tableView, path.Section - 1), path.Section - 1);
			}
		}
#endregion
	}
}