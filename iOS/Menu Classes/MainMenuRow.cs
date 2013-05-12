using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Firefly
{
	[Register("MainMenuRow")]
	public partial class MainMenuRow : UITableViewCell
	{
		public MainMenuRow () : base()
		{
			this.Initialize();
		}

		public MainMenuRow(IntPtr handle) : base(handle)
		{
			this.Initialize();
		}

		private void Initialize ()
		{
//			var background = UIImage.FromBundle("/Content/Images/SideMenu/menu-item-bg.png");
//			this.ContentView.BackgroundColor = UIColor.FromPatternImage(background);
//
//			// get rid of the blue selection view
//			this.SelectedBackgroundView = new UIView();
//			this.SelectedBackgroundView.BackgroundColor = UIColor.FromPatternImage(background);
		}

		public void BindDataToCell (string heading)
		{
			this.Heading.TextColor = UIColor.Black;
			this.Heading.Text = heading;
		}
	}
}

