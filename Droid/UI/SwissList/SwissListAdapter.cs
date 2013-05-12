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

namespace FGUtilsDroid
{
	public abstract class SwissListAdapter : BaseAdapter
	{
		protected View _expandedView = null;
		protected int _expandedViewPosition = -1;

		protected bool _isAnimating = false;

		protected abstract View GetExpandableView(View parent);

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

			parent.SetOnClickListener (new ExpandViewOnClickListener(new Action(() => { 
				PerformClick(parent, position);
			})));
		}

		protected void PerformClick(View parent, int position)
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
				viewAnimation.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) {
					_expandedView = parent;
					_expandedViewPosition = position;
					_isAnimating = false;
				};

				if (_expandedView != null)
				{
					View expandSubView = GetExpandableView(_expandedView);
					expandedViewAnimation = ECAnimation(expandSubView, ExpandCollapseAnimation.ExpandCollapseAnimationType.Collpase);
					expandedViewAnimation.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) {
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
				viewAnimation.AnimationEnd += delegate(object sender, Animation.AnimationEndEventArgs e) {
					_expandedView = null;
					_expandedViewPosition = -1;
					_isAnimating = false;
				};
				view.StartAnimation(viewAnimation);
			}
		}

		private Animation ECAnimation(View view, ExpandCollapseAnimation.ExpandCollapseAnimationType type)
		{
			Animation animation = new ExpandCollapseAnimation(view as LinearLayout, type);
			animation.Duration = 330;
		
			return animation;
		}

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