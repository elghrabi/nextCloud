using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;

namespace nextCloudAndroid
{
	public class ParametreFragment : Fragment
	{
		public ParametreFragment(){
			this.RetainInstance = true;
			this.HasOptionsMenu = (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate(Resource.Layout.fragment_parametre, container, false);

			return rootView;
		}
	}
}