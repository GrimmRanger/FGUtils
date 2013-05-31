/** 
	Copyright (c) 2013 Frank J. Grimmer II

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
**/

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
		protected bool ExpandCollapseEnabled { get; set; }
		protected bool SwipeToDeleteEnabled { get; set; }

		protected bool _isAnimating = false;
#endregion

#region SwipeToDelete Configuration
		protected View _swipeView;

		private float _swipeThresh = 25;
		private float _firstX;
		private float _lastX;

		private float Offset { get { return _lastX - _firstX; } }

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

		private bool ShouldDeleteSwipeView()
		{
			if (_swipeView != null) 
			{
				return Math.Abs (Offset) > DelThresh;
			}
			return false;
		}

		protected void ActivateSwissListItem(View view, int position)
		{
			if (ExpandCollapseEnabled)
				EnableExpandCollapse (view, position);

			if (ExpandCollapseEnabled || SwipeToDeleteEnabled)
				view.SetOnTouchListener (new SwissTouchListener(this, position));
		}

		public void PerformDelete(View view, int position)
		{
			if (ExpandCollapseEnabled)
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
			}

			DeleteRowRequested (position);

			_swipeView.ClearAnimation ();
			_swipeView = null;
		}
#endregion

#region SwissTouchListener
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
#endregion

#region ISwissTouchDelegate
		public void OnTouchDown (View view, MotionEvent evt, int position)
		{
			if (SwipeToDeleteEnabled)
			{
				_firstX = evt.RawX;
				_lastX = _firstX;
			}
		}

		public void OnTouchMove (View view, MotionEvent evt, int position)
		{
			if (SwipeToDeleteEnabled)
			{
				float activeX = evt.RawX;

				if (Math.Abs (activeX - _firstX) > _swipeThresh) {

					view.Parent.RequestDisallowInterceptTouchEvent (true);

					if (_swipeView == null) {
						_swipeView = view as View;
					}

					if (_swipeView != null) 
					{
						_swipeView.StartAnimation (SwipeDragAnimation(activeX));
					}
				}

				_lastX = activeX;
			}
		}

		public void OnTouchUp (View view, MotionEvent evt, int position)
		{
			if (ExpandCollapseEnabled && _swipeView == null) 
			{
				PerformClick (view, position);
			}
			else if (SwipeToDeleteEnabled)
			{
				Animation swipeEnd = SwipeEndAnimation ();
				if (ShouldDeleteSwipeView ()) 
				{
					swipeEnd.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) 
					{
						PerformDelete (_swipeView, position);
					};
				} 
				else 
				{
					_swipeView = null;
				}

				view.StartAnimation (swipeEnd);
			}
		}
#endregion 

#region Swipe Animations
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