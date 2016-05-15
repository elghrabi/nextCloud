using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SharedProj;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace nextCloudAndroid
{
	[Activity (Label = "NavDrawerListAdapter")]			
	public class NavDrawerListAdapter : BaseAdapter<NavDrawerItem>
	{
		Activity context;
		List<NavDrawerItem> navDrawerItems;

		public NavDrawerListAdapter(Activity context, List<NavDrawerItem> navDrawerItems)
			: base()
		{
			this.context = context;
			this.navDrawerItems = navDrawerItems;
		}

		public override int Count {
			get {return navDrawerItems.Count;}
		}
			
		public override NavDrawerItem this[int position] {      
			get { return navDrawerItems[position]; }
		}

		public override long GetItemId(int position) {
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = navDrawerItems [position];
			View view = convertView;

			if (view == null)
				view = context.LayoutInflater.Inflate(Resource.Layout.drawer_list_item, null);
			view.FindViewById<ImageView>(Resource.Id.icon).SetImageResource(item.getIcon());
			view.FindViewById<TextView>(Resource.Id.title).Text = item.getTitle();

			return view;
		}
	}
}

