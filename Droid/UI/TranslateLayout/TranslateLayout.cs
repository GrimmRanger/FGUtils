using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FGUtilsDroid
{
	public class TranslateLayout : LinearLayout
	{
		public TranslateLayout (Context context) :
			base (context)
		{
			Initialize ();
		}

		public TranslateLayout (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		private int _xOffset;
		private int _yOffset;

		void Initialize () {}

		public void SetOffset(int xOffset, int yOffset)
		{
			_xOffset = xOffset;
			_yOffset = yOffset;
		}

		public override void OffsetLeftAndRight (int offset)
		{
//			Console.WriteLine (string.Format("OffsetLeftAndRight xOffseti: {0}", _xOffset));
//			_xOffset += offset;
//			Console.WriteLine (string.Format("OffsetLeftAndRight xOffsetf: {0}", _xOffset));
			base.OffsetLeftAndRight (offset);
		}

		public override void OffsetTopAndBottom (int offset)
		{
			_yOffset += offset;
//			base.OffsetTopAndBottom (offset);
		}

		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			Console.WriteLine(string.Format("Lefti: {0}", Left));
			Console.WriteLine (string.Format("lefti: {0}", left));
			Console.WriteLine(string.Format("xOffset: {0}", _xOffset));
			if (Animation == null || (Animation != null && !Animation.HasStarted))
				base.OffsetLeftAndRight (_xOffset);

			base.OnLayout (changed, left, top, right, bottom);

//			Console.WriteLine (string.Format("OnLayout Lefti: {0}", Left));
//			if (left != _xOffset) {
//				if ( left == 0 ) {
//					Console.WriteLine (string.Format("OnLayout left == 0"));
//					Console.WriteLine (string.Format("xOffset: {0}", _xOffset));
//					left = _xOffset;
//					_xOffset = 0;
//					Console.WriteLine (string.Format("left: {0}", left));
//					Console.WriteLine (string.Format("xOffset: {0}", _xOffset));
//					base.OffsetLeftAndRight(left);
//					base.OnLayout(changed, left, top, right, bottom);
//				} else {
//					Console.WriteLine (string.Format("OnLayout left != 0"));
//					Console.WriteLine (string.Format("xOffset: {0}", _xOffset));
//					_xOffset = left;
//					base.OnLayout(changed, left, top, right, bottom);
//				}
//			} else {
//				base.OnLayout(changed, left, top, right, bottom);
//			}


//			Console.WriteLine (string.Format("onLayout left: {0}", left));
//			Console.WriteLine (string.Format("onLayout Lefti: {0}", Left));
//			if (_xOffset != 0 || _yOffset != 0) 
//			{
//				Console.WriteLine (string.Format("onLayout xOffset: {0}", _xOffset));
//				left += _xOffset;
//				top += _yOffset;
//				_xOffset = 0;
//				_yOffset = 0;
//				Console.WriteLine (string.Format("onLayout left: {0}", left));
//
//				base.OnLayout (changed, left, top, right, bottom);
//			}
//
//			base.OnLayout (changed, left, top, right, bottom);
		}
	}
}