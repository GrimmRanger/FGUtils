using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using System.Drawing;

namespace FGUtilsDroid
{	
	public interface ISwissTouchDelegate
	{
		void OnTouchDown (View view, MotionEvent evt, int position);
		void OnTouchMove (View view, MotionEvent evt, int position);
		void OnTouchUp (View view, MotionEvent evt, int position);
	}

	public abstract class SwissListAdapter : BaseAdapter, ISwissTouchDelegate
	{
#region Abstract Methods
		protected abstract View GetExpandableView(View parent);
		protected abstract void DeleteRowRequested (int position);
#endregion

#region General Configuration
		private bool ExpandCollapseEnabled { get; set; }
		private bool SwipeToDeleteEnabled { get; set; }

		protected bool _isAnimating = false;
#endregion

#region SwipeToDelete Configuration
		protected View _swipeView;

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

		private float DelThresh { get { return _swipeView.MeasuredWidth * _deleteThreshold; } }

		protected void EnableSwipeToDelete(View view, int position)
		{
			EnableExpandCollapse (view, position);
			view.SetOnTouchListener (new SwissTouchListener(this, position));
		}
#endregion

#region ISwissTouchDelegate
		internal class SwissTouchListener : Java.Lang.Object, View.IOnTouchListener
		{
			public SwissTouchListener(ISwissTouchDelegate del, int position) : base() 
			{
				_delegate = del;
				_position = position;
			}

			private ISwissTouchDelegate _delegate;
			private int _position;

			public bool OnTouch(View view, MotionEvent evt)
			{
				switch (evt.Action) 
				{
					case MotionEventActions.Move:
					_delegate.OnTouchMove (view, evt, _position);
					break;
					case MotionEventActions.Up:
					_delegate.OnTouchUp (view, evt, _position);
					break;
					case MotionEventActions.Down:
					_delegate.OnTouchDown (view, evt, _position);
					break;
				}

				return true;
			}
		}

		private float _swipeThresh = 25;
		private float _firstX;
		private float _lastX;

		private float Offset { get { return _lastX - _firstX; } }

		private bool ShouldDeleteSwipeView()
		{
			bool shouldDelete = false;
			if (_swipeView != null) 
			{
				return Math.Abs (Offset) > DelThresh;
			}
			return shouldDelete;
		}

		public void OnTouchDown (View view, MotionEvent evt, int position)
		{
			_firstX = evt.RawX;
			_lastX = _firstX;
		}

		public void OnTouchMove (View view, MotionEvent evt, int position)
		{
			float activeX = evt.RawX;

			if (Math.Abs (activeX - _firstX) > _swipeThresh) {

				view.Parent.RequestDisallowInterceptTouchEvent (true);

				if (_swipeView == null) {
					_swipeView = view as View;
				}

				if (_swipeView != null) 
				{
					view.StartAnimation (SwipeDragAnimation(activeX));
				}
			}

			_lastX = activeX;
		}

		public void OnTouchUp (View view, MotionEvent evt, int position)
		{
			if (_swipeView == null) 
			{
				PerformClick (view, position);
			}
			else 
			{
				Animation swipeEnd = SwipeEndAnimation ();
				if (ShouldDeleteSwipeView ()) 
				{
					swipeEnd.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) 
					{
						PerformDelete(_swipeView, position);
					};
				}

				view.StartAnimation (swipeEnd);
			}
		}

		private Animation SwipeDragAnimation(float activeX)
		{
			float delX = activeX - _lastX;

			Animation translate = new TranslateAnimation (Offset, Offset + delX, 0, 0);
			translate.Duration = 0;
			translate.FillAfter = true;

			return translate;
		}

		private Animation SwipeEndAnimation()
		{
			TranslateAnimation translate = null;

			if (ShouldDeleteSwipeView()) 
			{
				if (_lastX > _firstX) 
				{
					translate = new TranslateAnimation (Offset, _swipeView.Width, 0, 0);
				}
				else 
				{
					translate = new TranslateAnimation (Offset, -_swipeView.Width, 0, 0);
				}
			}
			else 
			{
				translate = new TranslateAnimation (Offset, 0, 0, 0);
			}

			translate.Duration = 330;
			translate.FillAfter = true;

			return translate;
		}
#endregion

#region Expand/Collpase Configuration
		protected View _expandedView = null;
		protected int _expandedViewPosition = -1;

		protected void EnableExpandCollapse(View parent, int position)
		{
			View expandingView = GetExpandableView(parent);
			EnableExpandCollapse(parent, expandingView, position);
		}

		private void EnableExpandCollapse(View parent, View expandingView, int position)
		{
			if (parent == _expandedView && position != _expandedViewPosition) 
			{
				// _expandedView has been recycled, 
				_expandedView = null;
			}
			if (position == _expandedViewPosition)
			{
				// reset reference to _expandedView to maintain state and animation
				_expandedView = parent;
			}

			expandingView.Measure(parent.Width, parent.Height);

			LinearLayout.LayoutParams pms = (LinearLayout.LayoutParams)expandingView.LayoutParameters;

			if (position == _expandedViewPosition)
			{
				expandingView.Visibility = ViewStates.Visible;
				pms.BottomMargin = 0;
			}
			else
			{
				expandingView.Visibility = ViewStates.Gone;
				pms.BottomMargin = -expandingView.MeasuredHeight;
			}
		}

		public void PerformClick(View parent, int position)
		{
			if (_isAnimating)
				return;

			_isAnimating = true;

			View view = GetExpandableView(parent);

			ExpandCollapseAnimation.ExpandCollapseAnimationType type = view.Visibility == ViewStates.Visible
				? ExpandCollapseAnimation.ExpandCollapseAnimationType.Collpase
				: ExpandCollapseAnimation.ExpandCollapseAnimationType.Expand;

			Animation viewAnimation = ECAnimation(view, type);
			Animation expandedViewAnimation = null;

			if (type == ExpandCollapseAnimation.ExpandCollapseAnimationType.Expand)
			{
				viewAnimation.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) 
				{
					_expandedView = parent;
					_expandedViewPosition = position;
					_isAnimating = false;
				};

				if (_expandedView != null)
				{
					View expandSubView = GetExpandableView(_expandedView);
					expandedViewAnimation = ECAnimation(expandSubView, ExpandCollapseAnimation.ExpandCollapseAnimationType.Collpase);
					expandedViewAnimation.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) 
					{
						_expandedView = null;
						_expandedViewPosition = -1;
						view.StartAnimation(viewAnimation);
					};
					expandSubView.StartAnimation(expandedViewAnimation);
				}
				else
				{
					view.StartAnimation(viewAnimation);
				}
			}
			else
			{
				viewAnimation.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) 
				{
					_expandedView = null;
					_expandedViewPosition = -1;
					_isAnimating = false;
				};
				view.StartAnimation(viewAnimation);
			}
		}

		public void PerformDelete(View view, int position)
		{
			if (position == _expandedViewPosition) 
			{
				_expandedViewPosition = -1;
				_expandedView = null;
			}
			else if (position < _expandedViewPosition) 
			{
				_expandedViewPosition--;
			}

			DeleteRowRequested (position);

			_swipeView.ClearAnimation ();
			_swipeView = null;
		}
#endregion

#region Expand/Collapse Animation
		private Animation ECAnimation(View view, ExpandCollapseAnimation.ExpandCollapseAnimationType type)
		{
			Animation animation = new ExpandCollapseAnimation(view as LinearLayout, type);
			animation.Duration = 330;

			return animation;
		}

		internal class ExpandCollapseAnimation : Animation
		{
			public enum ExpandCollapseAnimationType
			{
				Expand = 0,
				Collpase = 1
			}

			private View _animationView;
			private ExpandCollapseAnimationType _type;

			private int _endHeight;
			private LinearLayout.LayoutParams _layoutParams;

			public ExpandCollapseAnimation(View view, ExpandCollapseAnimationType type) : base() { Initialize(view, type); }

			private void Initialize(View view, ExpandCollapseAnimationType type)
			{
				_animationView = view;
				_type = type;

				_endHeight = view.MeasuredHeight;
				_layoutParams = ((LinearLayout.LayoutParams)view.LayoutParameters);

				if (_type == ExpandCollapseAnimationType.Expand)
				{
					_layoutParams.BottomMargin = -_endHeight;
				}
				else
				{
					_layoutParams.BottomMargin = 0;
				}
				view.Visibility = ViewStates.Visible;
			}

			protected override void ApplyTransformation (float interpolatedTime, Transformation t)
			{
				base.ApplyTransformation (interpolatedTime, t);

				if (interpolatedTime < 1)
				{
					if (_type == ExpandCollapseAnimationType.Expand)
					{
						_layoutParams.BottomMargin = -_endHeight + (int)(_endHeight * interpolatedTime);
					}
					else
					{
						_layoutParams.BottomMargin = -(int)(_endHeight * interpolatedTime);
					}
					_animationView.RequestLayout();
				}
				else
				{
					if (_type == ExpandCollapseAnimationType.Expand)
					{
						_layoutParams.BottomMargin = 0;
					}
					else
					{
						_layoutParams.BottomMargin = -_endHeight;
						_animationView.Visibility = ViewStates.Gone;
					}
					_animationView.RequestLayout();
				}
			}
		}
#endregion
	}
}