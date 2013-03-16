using System;
using MonoTouch.CoreAnimation;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace FGUtils
{
	public static class LayerHelper
	{
		// Drop Shadow Constants
		public const int DropShadowTop 			= 		1111;
		public const int DropShadowRight		=		1112;
		public const int DropShadowBottom 		= 		1113;
		public const int DropShadowLeft			=		1114;

		public static CAGradientLayer SimpleGradientLayer(RectangleF frame, float radius, UIColor highColor, UIColor lowColor)
		{
			CAGradientLayer layer = new CAGradientLayer();
			layer.Frame = frame;
			layer.CornerRadius = radius;
			layer.Colors = new MonoTouch.CoreGraphics.CGColor[]
			{
				highColor.CGColor,
				lowColor.CGColor
			};

			return layer;
		}

		public static CAGradientLayer SimpleGradientLayer(RectangleF frame, float radius, UIColor highColor, UIColor lowColor,
		                                                  PointF startPoint, PointF endPoint)
		{
			CAGradientLayer layer = SimpleGradientLayer(frame, radius, highColor, lowColor);
			layer.StartPoint = startPoint;
			layer.EndPoint = endPoint;

			return layer;
		}

		public static CAGradientLayer SimpleGradientLayer(RectangleF frame, float radius, UIColor tint)
		{
          	Func<float, int> rgbToAlpha = x => (int)(255 * x);
			Func<int, int> upper = x => x + Math.Min(50, 255 - x);
			Func<int, int> lower = x => x - Math.Min(50, x);

			float[] colors = tint.CGColor.Components;
			int r = rgbToAlpha(colors[0]);
			int g = rgbToAlpha(colors[1]);
			int b = rgbToAlpha(colors[2]);
			int a = rgbToAlpha(colors[3]);

			UIColor high = UIColor.FromRGBA(upper(r), upper(g), upper(b), a);
			UIColor low = UIColor.FromRGBA(lower(r), lower(g), lower(b), a);

			return SimpleGradientLayer(frame, radius, high, low);
		}

		public static CALayer SimpleHighlightLayer(RectangleF frame, float radius)
		{
			CALayer layer = new CALayer();
			layer.Frame = frame;
			layer.CornerRadius = radius;

			layer.BackgroundColor = UIColor.FromRGBA(0.25f, 0.25f, 0.25f, 0.75f).CGColor;
			
			layer.ShadowColor = UIColor.LightGray.CGColor;
			layer.ShadowRadius = 2f;
			layer.ShadowOffset = new SizeF(1f, 1f);
			layer.ShadowOpacity = 1f;

			return layer;
		}

		public static CALayer SimpleHighlightLayer(RectangleF frame, float radius, UIColor backgroundColor)
		{
			CALayer layer = LayerHelper.SimpleHighlightLayer(frame, radius);
			layer.BackgroundColor = backgroundColor.CGColor;

			return layer;
		}

		public static CAGradientLayer GlossyLayer(RectangleF frame, float radius)
		{
			CAGradientLayer layer = new CAGradientLayer();
			layer.Frame = frame;
			layer.CornerRadius = radius;
			
			layer.Colors = new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromWhiteAlpha(1f, 0.4f).CGColor,
				UIColor.FromWhiteAlpha(1f, 0.2f).CGColor,
				UIColor.FromWhiteAlpha(0.75f, 0.2f).CGColor,
				UIColor.FromWhiteAlpha(0.4f, 0.2f).CGColor,
				UIColor.FromWhiteAlpha(1f, 0.4f).CGColor
			};
			layer.Locations = new NSNumber[]
			{
				NSNumber.FromFloat(0f),
				NSNumber.FromFloat(0.2f),
				NSNumber.FromFloat(0.5f),
				NSNumber.FromFloat(0.8f),
				NSNumber.FromFloat(1f),
			};

			return layer;
		}

		public static CAGradientLayer GlossyLayer(RectangleF frame, float radius, PointF startPoint, PointF endPoint)
		{
			CAGradientLayer layer = GlossyLayer(frame, radius);
			layer.StartPoint = startPoint;
			layer.EndPoint = endPoint;

			return layer;
		}

		public static CALayer PopContourLayer(RectangleF frame, float radius)
		{
			CALayer layer = new CALayer();
			layer.Frame = frame;
			layer.CornerRadius = radius;

			CALayer topLayer = new CALayer();
			topLayer.Frame = new RectangleF(0, -1f, frame.Width, frame.Height);
			topLayer.CornerRadius = radius;
			topLayer.BackgroundColor = UIColor.FromRGBA(40, 40, 40, 255).CGColor;
			layer.InsertSublayerAbove(topLayer, layer);

			CALayer bottomLayer = new CALayer();
			bottomLayer.Frame = new RectangleF(0, 1f, frame.Width, frame.Height);
			bottomLayer.CornerRadius = radius;
			bottomLayer.BackgroundColor = UIColor.FromRGBA(150, 150, 150, 255).CGColor;
			layer.InsertSublayerAbove(bottomLayer, layer);
			
			CAGradientLayer leftLayer = new CAGradientLayer();
			leftLayer.Frame = new RectangleF(-1f, 0, frame.Width, frame.Height);
			leftLayer.CornerRadius = radius;
			leftLayer.Colors =  new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromRGBA(40, 40, 40, 255).CGColor,
				UIColor.FromRGBA(100, 100, 100, 255).CGColor
			};
			layer.InsertSublayerAbove(leftLayer, layer);
			
			CAGradientLayer rightLayer = new CAGradientLayer();
			rightLayer.Frame = new RectangleF(1f, 0, frame.Width, frame.Height);
			rightLayer.CornerRadius = radius;
			rightLayer.Colors =  new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromRGBA(40, 40, 40, 255).CGColor,
				UIColor.FromRGBA(100, 100, 100, 255).CGColor
			};
			layer.InsertSublayerAbove(rightLayer, layer);

			return layer;
		}

		public static CALayer SinkContourLayer(RectangleF frame, float radius)
		{
			CALayer layer = new CALayer();
			layer.Frame = frame;
			layer.CornerRadius = radius;
			
			CALayer topLayer = new CALayer();
			topLayer.Frame = new RectangleF(0, -1f, frame.Width, frame.Height);
			topLayer.CornerRadius = radius;
			topLayer.BackgroundColor = UIColor.FromRGBA(150, 150, 150, 255).CGColor;
			layer.InsertSublayerAbove(topLayer, layer);
			
			CALayer bottomLayer = new CALayer();
			bottomLayer.Frame = new RectangleF(0, 1f, frame.Width, frame.Height);
			bottomLayer.CornerRadius = radius;
			bottomLayer.BackgroundColor = UIColor.FromRGBA(40, 40, 40, 255).CGColor;
			layer.InsertSublayerAbove(bottomLayer, layer);
			
			CAGradientLayer leftLayer = new CAGradientLayer();
			leftLayer.Frame = new RectangleF(-1f, 0, frame.Width, frame.Height);
			leftLayer.CornerRadius = radius;
			leftLayer.Colors =  new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromRGBA(100, 100, 100, 255).CGColor,
				UIColor.FromRGBA(40, 40, 40, 255).CGColor
			};
			layer.InsertSublayerAbove(leftLayer, layer);
			
			CAGradientLayer rightLayer = new CAGradientLayer();
			rightLayer.Frame = new RectangleF(1f, 0, frame.Width, frame.Height);
			rightLayer.CornerRadius = radius;
			rightLayer.Colors =  new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromRGBA(100, 100, 100, 255).CGColor,
				UIColor.FromRGBA(40, 40, 40, 255).CGColor
			};
			layer.InsertSublayerAbove(rightLayer, layer);

			return layer;
		}

		public static CAGradientLayer StoresGrayGradient(RectangleF frame)
		{
			CAGradientLayer backgroundLayer = new CAGradientLayer();
			backgroundLayer.Frame = new RectangleF(0, 0, frame.Width, frame.Height);
			backgroundLayer.Colors =  new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromRGBA(65, 75, 73, 255).CGColor,
				UIColor.FromRGBA(40, 46, 45, 255).CGColor
			};

			CALayer topEdge = new CALayer();
			topEdge.Frame = new RectangleF(0, 0, frame.Width, 1.5f);
			topEdge.BackgroundColor = UIColor.FromRGBA(90, 99, 97, 255).CGColor;
			backgroundLayer.AddSublayer(topEdge);

			return backgroundLayer;
		}

		public static CALayer StoreCellLayer(RectangleF frame)
		{
			CALayer layer = new CALayer();
			layer.Frame = frame;

			CALayer backgroundLayer = new CALayer();
			backgroundLayer.Frame = new RectangleF(0, 0, frame.Width, frame.Height);
			backgroundLayer.BackgroundColor = UIColor.FromRGBA(19, 19, 19, 255).CGColor;
			layer.InsertSublayer(backgroundLayer, 0);

			CALayer topLayer1 = new CALayer();
			topLayer1.Frame = new RectangleF(0, 0, frame.Width, 1);
			topLayer1.BackgroundColor = UIColor.FromRGBA(25, 27, 28, 255).CGColor;
			layer.InsertSublayerAbove(topLayer1, backgroundLayer);
			
			CALayer topLayer2 = new CALayer();
			topLayer2.Frame = new RectangleF(0, 1, frame.Width, 1);
			topLayer2.BackgroundColor = UIColor.FromRGBA(10, 10, 10, 255).CGColor;
			layer.InsertSublayerAbove(topLayer2, backgroundLayer);

			CALayer bottomLayer1 = new CALayer();
			bottomLayer1.Frame = new RectangleF(0, frame.Height - 2, frame.Width, 1);
			bottomLayer1.BackgroundColor = UIColor.FromRGBA(23, 25, 26, 255).CGColor;
			layer.InsertSublayerAbove(bottomLayer1, backgroundLayer);

			CALayer bottomLayer2 = new CALayer();
			bottomLayer2.Frame = new RectangleF(0, frame.Height - 1, frame.Width, 1);
			bottomLayer2.BackgroundColor = UIColor.FromRGBA(42, 43, 44, 255).CGColor;
			layer.InsertSublayerAbove(bottomLayer2, backgroundLayer);

			return layer;
		}

		public static CAGradientLayer DepthLayer(RectangleF frame, float radius)
		{
			CAGradientLayer layer = new CAGradientLayer();
			layer.Frame = frame;
			layer.CornerRadius = radius;
			
			layer.Colors = new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromWhiteAlpha(0.4f, 1).CGColor,
				UIColor.FromWhiteAlpha(0.3f, 1).CGColor,
				UIColor.FromWhiteAlpha(0.2f, 1).CGColor,
				UIColor.FromWhiteAlpha(0.1f, 1).CGColor,
				UIColor.FromWhiteAlpha(0, 1).CGColor
			};
			layer.Locations = new NSNumber[]
			{
				NSNumber.FromFloat(0f),
				NSNumber.FromFloat(0.3f),
				NSNumber.FromFloat(0.5f),
				NSNumber.FromFloat(0.7f),
				NSNumber.FromFloat(1f),
			};
			
			return layer;
		}

		public static CAGradientLayer DropShadowLayer(float length, int side)
		{
			int shadowDepth = 6;

			CAGradientLayer layer = new CAGradientLayer();
			layer.Colors = new MonoTouch.CoreGraphics.CGColor[]
			{
				UIColor.FromRGBA(0, 0, 0, 120).CGColor,
				UIColor.FromRGBA(32, 34, 35, 0).CGColor
			};

			// set layer frame based on direction
			if (side == DropShadowTop || side == DropShadowBottom)
			{
				layer.Frame = new RectangleF(0, 0, length, shadowDepth);
			}
			else
			{
				layer.Frame = new RectangleF(0, 0, shadowDepth, length);
			}

			// set shadow direction
			// layer default equivalent to DropShadowTop: start = (0.5, 0), end = (0.5, 1)
			if (side != DropShadowTop)
			{
				float xi, xf, yi, yf;

				if (side == DropShadowBottom)
				{
					yi = 1;
					yf = 0;
					xi = 0.5f;
					xf = 0.5f;
				}
				else
				{
					yi = 0.5f;
					yf = 0.5f;
					xi = side == DropShadowLeft ? 0 : 1;
					xf = xi == 0 ? 1 : 0;
				}
				layer.StartPoint = new PointF(xf, yf);
				layer.EndPoint = new PointF(xi, yi);
			}
			
			return layer;
		}

		public static CAShapeLayer RoundedMask(RectangleF frame, UIRectCorner corners, float radius)
		{
			UIBezierPath path = UIBezierPath.FromRoundedRect(frame, corners, new SizeF(radius, radius));
			CAShapeLayer layer = new CAShapeLayer();
			layer.Frame = frame;
			layer.Path = path.CGPath;

			return layer;
		}
	}
}