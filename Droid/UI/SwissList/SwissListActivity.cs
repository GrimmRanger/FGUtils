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
	[Activity (Label = "SwissListActivity")]			
	public class SwissListActivity : Activity
	{
		ListView _listView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SwissList);

			// Get our button from the layout resource,
			// and attach an event to it
			_listView = FindViewById(Resource.Id.listView) as ListView;
			_listView.Adapter = new TestAdapter(this);
		}
	}
}