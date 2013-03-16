using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using System.Linq;

namespace FGUtils
{
	[Register("MessageCell")]
	public partial class MessageCell : ExpandingCell
	{
		public MessageCell (string ReuseId) : base(UITableViewCellStyle.Default, ReuseId) {}
		public MessageCell (IntPtr handle) : base(handle) {}

		// Static "Constructor" to internalize loading from xib
		public static MessageCell View(RectangleF frame, string reuseId) 
		{
			MessageCell view = new MessageCell (reuseId);
			var views = NSBundle.MainBundle.LoadNib("MessageCell", view, null);
			view = Runtime.GetNSObject(views.ValueAt(0)) as MessageCell;

			view.Frame = frame;
			view.Initialize ();
			
			return view;
		}

		private const float CollapsedHeight = 80;
		private const float ExpandedHeight = 160;

		private MessageHeader _header;
		private UIView _webView;
		public Message Message { get; set; }

		public void SetHeader (UIView header)
		{	
			if (Header.Subviews.Count() == 0)
				Header.AddSubview (header);
		}

		public void SetContent (UIView content)
		{
			if (Content.Subviews.Count() == 0) 
			{
				Content.Frame = new RectangleF (0, Header.Frame.Bottom, content.Frame.Width, content.Frame.Height);
				Content.AddSubview (content);
			}
		}

		private void ClearCell()
		{
			if (Header.Subviews.Count () > 0) {
				Header.Subviews.ToList().ForEach(v => v.RemoveFromSuperview());
			}

			if (Content.Subviews.Count() > 0)
				Content.Subviews.ToList().ForEach(v => v.RemoveFromSuperview());
		}

		private void Initialize ()
		{
			SelectionStyle = UITableViewCellSelectionStyle.None;

			_header = MessageHeader.View (Header.Frame);
			SetHeader (_header);
		}

		public void UpdateCell(Message msg)
		{
			ClearCell ();

			Message = msg;

			if (Message != null) 
			{
				_header.UpdateHeader (Message);
				SetHeader(_header);

				_webView = new UIView(new RectangleF(0, 0, Frame.Width, 80));
				UILabel label = new UILabel(_webView.Bounds);
				label.Text = Message.Detail;
				_webView.AddSubview(label);
				SetContent(_webView);
			}
		}

#region ExpandingCell Methods
		public override void Expand ()
		{
			Frame = new RectangleF (Frame.X, Frame.Y, Frame.Width, ExpandedHeight);
		}

		public override void Collapse ()
		{
			Frame = new RectangleF (Frame.X, Frame.Y, Frame.Width, CollapsedHeight);
		}
#endregion
	}
}