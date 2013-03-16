using System;
using MonoTouch.UIKit;
using Firefly;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace FGUtils
{
	public class FGUtilsTableSource : UITableViewSource
	{
		private const string MainMenuCell = "MainMenuRow";

		private UIViewController _controller;
		private Menu _menu;

		public FGUtilsTableSource (UIViewController controller) 
		{
			_controller = controller;

			_menu = new Menu () {
				Sections = new List<Section>() {
					new Section() {
						Title = "Utilities",
						ViewableBy = MenuViewableKind.Everyone,
						Rows = new List<Row>() {
							new Row() {
								Title = "SwissTableView Demo",
								OnSelect = () => { 
									LoadViewControllerByType("FGUtils.SwissTableViewDemoViewController");
								}
							}
						}
					}
				}
			};
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _menu.Sections[section].Rows.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(MainMenuCell) as MainMenuRow;
			
			if (cell == null)
			{
				cell = new MainMenuRow();
				var views = NSBundle.MainBundle.LoadNib(MainMenuCell, cell, null);
				cell = Runtime.GetNSObject( views.ValueAt(0) ) as MainMenuRow;
			}
			
			cell.BindDataToCell(_menu.Sections[indexPath.Section].Rows[indexPath.Row].Title);
			
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			_menu.Sections[indexPath.Section].Rows[indexPath.Row].OnSelect();
		}

		private void LoadViewControllerByType (string typeString)
		{
			Type type = Type.GetType(typeString);
			
			using (UIViewController instance = (UIViewController)Activator.CreateInstance(type, new object[] {}))
			{
				_controller.NavigationController.PushViewController(instance, true);
			}
		}
	}
}

