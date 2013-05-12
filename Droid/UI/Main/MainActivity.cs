using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace FGUtilsDroid
{
	[Activity (Label = "Main", MainLauncher = true)]
	public class Activity1 : Activity
	{
		ListView _listView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			_listView = FindViewById(Resource.Id.listView) as ListView;
			_listView.Adapter = new MainAdapter(this);
			_listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) {
				StartActivity(typeof(SwissListActivity));
			};
		}
	}
}