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
	public interface ISwipeViewDelegate
	{
		void PerformClick(View view, int position);
		void PerformDelete (View view, int position);
	}

	public abstract class SwissListAdapter : BaseAdapter, ISwipeViewDelegate
	{
#region Abstract Methods
		protected abstract View GetSwipeToDeleteView (View parent);
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
			View swipeView = GetSwipeToDeleteView (view);
			swipeView.SetOnTouchListener (new SwipeViewOnTouchListener(this, position, _deleteThreshold));
		}

		internal class SwipeViewOnTouchListener : Java.Lang.Object, View.IOnTouchListener
		{
			public SwipeViewOnTouchListener(ISwipeViewDelegate del, int position ,float delThresh) : base() 
			{
				_delegate = del;
				_position = position;
				_delThresh = delThresh;
			}

			private ISwipeViewDelegate _delegate;
			private int _position;
			private float _swipeThresh = 25;
			private float _delThresh;
			private View _swipeView;
			private float _firstX;
			private float _lastX;

			private float Offset { get { return _lastX - _firstX; } }

			public bool OnTouch(View view, MotionEvent evt)
			{
				switch (evt.Action) 
				{
				case MotionEventActions.Move:
					OnTouchMove (view, evt);
					break;
				case MotionEventActions.Up:
					OnTouchUp (view, evt);
					break;
				default:
				case MotionEventActions.Down:
					OnTouchDown (view, evt);
					break;
				}

				return true;
			}

			private void OnTouchDown(View view, MotionEvent evt)
			{
				_firstX = evt.RawX;
				_lastX = _firstX;
			}

			private void OnTouchMove(View view, MotionEvent evt)
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

			private void OnTouchUp(View view, MotionEvent evt)
			{
				if (_swipeView == null) 
				{
					View viewParent = view.Parent as View;
					_delegate.PerformClick (viewParent, _position);
				}
				else 
				{
					Animation swipeEnd = SwipeEndAnimation ();
					swipeEnd.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) {
						Animation finishAnimation = new SwipeFinishAnimation(_swipeView);
						finishAnimation.AnimationEnd += delegate {
							_delegate.PerformDelete(_swipeView, _position);
							_swipeView = null;
						};
						_swipeView.StartAnimation(finishAnimation);
					};
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

				float offset = Math.Abs (Offset);

				if (offset > (_delThresh * _swipeView.Width)) 
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
		}

		internal class SwipeFinishAnimation : Animation
		{
			private View _animationView;

			private int _endHeight;
			private LinearLayout.LayoutParams _layoutParams;

			public SwipeFinishAnimation(View view) : base() { Initialize(view); }

			private void Initialize(View view)
			{
				_animationView = view;

				_endHeight = view.MeasuredHeight;
				_layoutParams = ((LinearLayout.LayoutParams)view.LayoutParameters);
			}

			protected override void ApplyTransformation (float interpolatedTime, Transformation t)
			{
				base.ApplyTransformation (interpolatedTime, t);

				if (interpolatedTime < 1)
				{
					_layoutParams.BottomMargin = -(int)(_endHeight * interpolatedTime);
					_animationView.RequestLayout();
					Console.WriteLine (string.Format("applyTransformation bm: {0}", _layoutParams.BottomMargin));
				}
				else
				{
					_layoutParams.BottomMargin = -_endHeight;
					_animationView.Visibility = ViewStates.Gone;
					_animationView.RequestLayout();
				}
			}
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

//			parent.SetOnTouchListener (new ExpandViewOnTouchListener(new Action(() => {
//				PerformClick(parent, position);
//			})));

			parent.SetOnClickListener (new ExpandViewOnClickListener(new Action(() => { 
				PerformClick(parent, position);
			})));
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

			DeleteRowRequested (position);
		}
#endregion

#region ExpandView OnTouchListener
		internal class ExpandViewOnTouchListener : Java.Lang.Object, View.IOnTouchListener
		{
			public ExpandViewOnTouchListener(Action action) : base() { _action = action; }

			private Action _action;

			public bool OnTouch(View view, MotionEvent evt)
			{
				switch (evt.Action) 
				{
				case MotionEventActions.Up:
					_action.Invoke ();
					break;
				}

				return true;
			}
		}
#endregion

#region ExpandView OnClickListener
		internal class ExpandViewOnClickListener : Java.Lang.Object, View.IOnClickListener
		{
			public ExpandViewOnClickListener(Action action) : base()
			{
				_action = action;
			}

			private Action _action;

			public void OnClick(View View)
			{
				_action.Invoke ();
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