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
	public class MainAdapter : BaseAdapter
	{
		public MainAdapter(Context context) : base() { _context = context; }

		private Context _context;

		public override int Count { get { return 1; } }

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			return new TextView (_context) {
				Text = "SwissTable"
			};
		}
	}
}