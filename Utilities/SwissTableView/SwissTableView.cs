using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.CoreAnimation;

namespace FGUtils
{
	[Register("SwissTableView")]
	public partial class SwissTableView : UITableView
	{
		public SwissTableView (RectangleF frame) : base(frame) {}
		public SwissTableView (IntPtr handle) : base(handle) {}
		public SwissTableView () : base() {}

		public new SwissTableSource Source 
		{ 
			get { return (SwissTableSource)base.Source; }
			set 
			{ 
				base.Source = value; 

				expandedPath = null;
				LastPoint = PointF.Empty;
				DraggingView = null;
			}
		}

#region Pull to Refresh Functionality
		private UIRefreshControl RefreshControl;
		public bool IsRefreshing 
		{
			get { return RefreshControl.Refreshing; } 
		}

		public event EventHandler RefreshRequested
		{
			add 
			{
				InitRefreshControl();
				RefreshControl.ValueChanged += value; 
			}
			remove { RefreshControl.ValueChanged -= value; }
		}

		private void InitRefreshControl()
		{
			RefreshControl = new UIRefreshControl ();
			RefreshControl.Frame = new RectangleF (0, -1, Frame.Width, 1);
			this.AddSubview (RefreshControl);
		}
		
		public void RefreshConcluded()
		{
			 if (RefreshControl != null)
				RefreshControl.EndRefreshing ();
		}
#endregion

#region Swipe to Delete Functionality
		public bool SwipeToDeleteEnabled = false;

		private UITableViewCell DraggingView;
		private UIView Accent;
		private PointF LastPoint;

		private float _deleteThreshold = 0.5f;
		public float DeleteThreshold 
		{
			get { return _deleteThreshold; }

			set
			{
				if (0 <= value && value <= 1)
					_deleteThreshold = value;
			}
		}

		private float DelThresh { get { return DraggingView.Frame.Width * _deleteThreshold; } }

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			LastPoint = ((UITouch)touches.AnyObject).LocationInView (this);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			if (SwipeToDeleteEnabled) 
			{
				PointF activePoint = ((UITouch)touches.AnyObject).LocationInView (this);

				if (DraggingView == null) 
				{
					NSIndexPath draggingPath = IndexPathForRowAtPoint (activePoint);
					if (draggingPath != null)
					{
						DraggingView = CellAt (draggingPath);
						Accent = new AccentView(DraggingView, _deleteThreshold);
						this.AddSubview(Accent);
					}
				}
			

				if (DraggingView != null && DraggingView is UITableViewCell) 
				{
					float delX = activePoint.X - LastPoint.X;
				
					float x = DraggingView.Frame.X + delX;
					float y = DraggingView.Frame.Y;
					DraggingView.Frame = new RectangleF (new PointF(x, y) , DraggingView.Frame.Size);

					float accentX = x >= 0 ? x - Accent.Frame.Width : DraggingView.Frame.Right;
					float accentY = DraggingView.Frame.Y;
					Accent.Frame = new RectangleF(accentX, accentY, Accent.Frame.Width, Accent.Frame.Height);
				}

				LastPoint = activePoint;
			}
		}
		
		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);

			if (SwipeToDeleteEnabled && DraggingView != null) 
			{
				Swiped (DraggingView);
				DraggingView = null;

				Accent.RemoveFromSuperview();
				Accent = null;
			} 
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			if (SwipeToDeleteEnabled && DraggingView != null) 
			{
				Swiped (DraggingView);
				DraggingView = null;

				Accent.RemoveFromSuperview();
				Accent = null;
			} 
			else if (ExpandCollapseEnabled) 
			{
				NSIndexPath path = IndexPathForRowAtPoint (((UITouch)touches.AnyObject).LocationInView(this));
				if (path != null)
					ExpandCollapse(path);
			}
		}

		private void Swiped(UITableViewCell cell)
		{
			if (cell != null) 
			{
				float threshold = (cell.Frame.Width * _deleteThreshold);
				if (cell.Frame.X > threshold || cell.Frame.X < -threshold)
				{
					float x = cell.Frame.X < 0 ? -cell.Frame.Width : cell.Frame.Width;
					
					UIView.Animate (0.25, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
						cell.Frame = new RectangleF (new PointF(x, cell.Frame.Y) , cell.Frame.Size);
						cell.Alpha = .5f;
					}, () => {
						cell.RemoveFromSuperview();
						BeginUpdates();
						NSIndexPath path = IndexPathForRowAtPoint (LastPoint);
						DeleteRow(path);
						EndUpdates();
					});
				}
				else 
				{
					UIView.Animate (0.25, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
						cell.Frame = new RectangleF (new PointF(0, cell.Frame.Y), cell.Frame.Size);
						cell.Alpha = 1f;
					}, () => {
					});
				}
			} 
		}

		private void DeleteRow(NSIndexPath indexPath)
		{
			DeleteRows(new NSIndexPath[]{indexPath}, UITableViewRowAnimation.None);
			Source.DeleteRow(this, indexPath);
			
			if (expandedPath != null) 
			{
				if (expandedPath.Equals(indexPath))
				{
					expandedPath = null;
				}
				else 
				{
					if (expandedPath.GreaterThan(indexPath))
						expandedPath = Source.DecrementIndex(this, expandedPath);
				}
			}
		}

		private class AccentView : UIView
		{
			private const float AccentAlphaInitial = 0.5f;
			private const float AccentAlphaFinal = 1;

			UIImageView ImageView;
			float _delThresh;

			private bool _canDelete = false;
			private bool CanDelete {
				get { return _canDelete; }
				set
				{
					if (value != _canDelete)
					{
						Console.WriteLine("Threshold Crossed");
						_canDelete = value;
						if(_canDelete)
						{
							ImageView.Image = UIImage.FromBundle("/Content/Images/Delete/delete.png");
						}
						else 
						{
							ImageView.Image = UIImage.FromBundle("/Content/Images/Delete/deleteGray.png");
						}
					}
				}
			}

			public AccentView (UIView view, float delThreshold) : base(view.Bounds)
			{
				_delThresh = delThreshold;

				this.BackgroundColor = UIColor.FromRGBA(100, 30, 30, 255);
				this.Alpha = 0.5f;

				ImageView = new UIImageView(UIImage.FromBundle("/Content/Images/Delete/deleteGray.png"));
				this.AddSubview(ImageView);
			}

			public override RectangleF Frame {
				get { return base.Frame; }
				set 
				{
					Update(base.Frame, value);
					base.Frame = value;
				}
			}

			private void Update(RectangleF oldFrame, RectangleF newFrame)
			{
				UpdateImageView(oldFrame, newFrame);
				UpdateAlpha();
			}

			private void UpdateImageView(RectangleF oldFrame, RectangleF newFrame)
			{
				if (ImageView != null) 
				{
					UpdateFrame (oldFrame, newFrame);
					UpdateImageState (oldFrame, newFrame);
				}
			}

			private void UpdateFrame(RectangleF oldFrame, RectangleF newFrame)
			{
				float imageX = Frame.X < 0 ? this.Frame.Width - ImageView.Frame.Width - 10 : 10;
				float imageY = (this.Frame.Height - ImageView.Frame.Height) / 2;
				ImageView.Frame = new RectangleF (imageX, imageY, ImageView.Frame.Width, ImageView.Frame.Height);
			}

			private void UpdateImageState(RectangleF oldFrame, RectangleF newFrame)
			{
				if (Frame.X > 0) 
				{
					CanDelete = (Frame.Width - Frame.X) > (Frame.Width * _delThresh);
				}
				else 
				{
					CanDelete = Frame.Right > (Frame.Width * _delThresh);
				}
			}

			private void UpdateAlpha()
			{
				float adjustment = Frame.X > 0 ?
					(Frame.Width - Frame.X) / (_delThresh * Frame.Width) :
						Frame.Right / (_delThresh * Frame.Width);
				Alpha = AccentAlphaInitial + (AccentAlphaFinal - AccentAlphaInitial) * adjustment;

			}
		}
#endregion

#region Expand Collapse Functionality
		public bool ExpandCollapseEnabled = false;

		private NSIndexPath expandedPath = null;

		public void ToggleExpanded(NSIndexPath indexPath, bool animated, UITableViewScrollPosition position)
		{
			ScrollToRow (indexPath, position, animated);
			ExpandCollapse (indexPath);
		}

		private void ExpandCollapse(NSIndexPath indexPath)
		{
			if (indexPath != null) 
			{
				bool shouldExpand = true;
				bool shouldCollapse = false;
				
				if (expandedPath != null) 
				{
					shouldCollapse = true;
					
					if (expandedPath.Equals(indexPath))
						shouldExpand = false;
				}
				
				if (shouldExpand && shouldCollapse) 
				{
					Collapse (expandedPath, () => {
						Expand (indexPath); });
				} 
				else 
				{
					if (shouldCollapse)
						Collapse(expandedPath, null);
					
					if (shouldExpand)
						Expand (indexPath);
				}
			}
		}

		private void Expand(NSIndexPath path)
		{
			CATransaction.Begin ();
			
			CATransaction.CompletionBlock = () => {
				Source.DidExpandCell(this, path);
			};
			
			expandedPath = path;
			Source.WillExpandCell (this, path);
			BeginUpdates ();
			EndUpdates ();
			
			CATransaction.Commit ();
		}
		
		private void Collapse(NSIndexPath path, Action completion)
		{
			CATransaction.Begin ();
			
			CATransaction.CompletionBlock = () => {
				Source.DidCollapseCell (this, path);
				
				if (completion != null)
				completion.Invoke ();
			};
			
			expandedPath = null;
			Source.WillCollapseCell (this, path);
			BeginUpdates ();
			EndUpdates ();
			
			CATransaction.Commit ();
		}
#endregion

		void Log(string msg)
		{
			Console.WriteLine (msg);
		}
	}	
}