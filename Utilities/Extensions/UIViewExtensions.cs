using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace FGUtils
{
	public static class UIViewExtensions
	{
		public static void AnimateGrow(this UIView view, PointF fromCenter, PointF toCenter)
		{
			view.Alpha = 0;
			view.Transform = CGAffineTransform.MakeScale(0.001f, 0.001f);
			view.Center = fromCenter;
			
			float wobbleDuration = 0.1f;
			UIView.Animate(0.6, 0.0, UIViewAnimationOptions.CurveEaseInOut, () => {
				view.Transform = CGAffineTransform.MakeScale(1.04f, 1.04f);
				view.Center = toCenter;
				view.Alpha = 1;
			}, () => {
				UIView.Animate(wobbleDuration, 0.0f, UIViewAnimationOptions.CurveEaseInOut, () => {
					view.Transform = CGAffineTransform.MakeScale(0.97f, 0.97f);
				}, () => {
					UIView.Animate(wobbleDuration, 0.0f, UIViewAnimationOptions.CurveEaseInOut, () => {
						view.Transform = CGAffineTransform.MakeScale(1.02f, 1.02f);
					}, () => {
						UIView.Animate(wobbleDuration, 0.0f, UIViewAnimationOptions.CurveEaseInOut, () => {
							view.Transform = CGAffineTransform.MakeScale(0.99f, 0.99f);
						}, () => {
							UIView.Animate(wobbleDuration, 0.0f, UIViewAnimationOptions.CurveEaseInOut, () => {
								view.Transform = CGAffineTransform.MakeScale(1, 1);
							}, () => {});
						});
					});
				});
			});
		}

		public static void AnimateShrink (this UIView view, PointF fromCenter, PointF toCenter)
		{
			view.Center = fromCenter;

			UIView.Animate(0.6, 0.0, UIViewAnimationOptions.CurveEaseInOut, () => {
				view.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f);
				view.Center = toCenter;
				view.Alpha = 0;
			}, () => {});
		}
	}
}