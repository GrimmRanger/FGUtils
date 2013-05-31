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

namespace FGUtilsDroid
{
	public class TestAdapter : SwissListAdapter
	{
		Context _context;

		public TestAdapter(Context context) : base() { _context = context; }

		private int _count = 40;
		public override int Count {
			get { return _count; }
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{	
			View view = convertView;

			if (view == null) 
			{
				LayoutInflater inflator = (LayoutInflater)_context.GetSystemService (Context.LayoutInflaterService);
				view = inflator.Inflate (Resource.Layout.ExpandCollapseCell, null, false);
			}

			TextView title = view.FindViewById (Resource.Id.title) as TextView;
			title.Text = string.Format ("row {0}", position);

			EnableSwipeToDelete(view, position);

			return view;
		}

		protected override View GetExpandableView (View parent)
		{
			return parent.FindViewById(Resource.Id.expandView);
		}

		protected override void DeleteRowRequested(int position)
		{
			if (_count > 0)
				_count--;

			NotifyDataSetChanged ();
		}
	}
}