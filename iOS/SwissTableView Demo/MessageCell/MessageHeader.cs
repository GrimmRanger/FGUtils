using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace FGUtils
{
	[Register("MessageHeader")]
	public partial class MessageHeader : UIView
	{
		public MessageHeader (RectangleF frame) : base(frame) {}
		public MessageHeader (IntPtr handle) : base(handle) {}
		public MessageHeader () : base() {}
		
		// Static "Constructor" to internalize loading from xib
		public static MessageHeader View(RectangleF frame) 
		{
			MessageHeader view = new MessageHeader(frame);
			var views = NSBundle.MainBundle.LoadNib("MessageHeader", view, null);
			view = Runtime.GetNSObject(views.ValueAt(0)) as MessageHeader;

			view.Initialize ();
		
			return view;
		}

		private Message _message;

		private void Initialize()
		{
			this.Layer.InsertSublayer (LayerHelper.SimpleGradientLayer (this.Bounds, 0, 
			                                                         UIColor.FromRGBA (250, 250, 250, 255), 
			                                                         UIColor.FromRGBA (225, 225, 225, 255)), 0);

			Icon.Image = UIImage.FromBundle("/UI/gear.png");
		}

		public void UpdateHeader(Message msg)
		{
			_message = msg;

			Title.Text = _message.Title;
		}
	}
}