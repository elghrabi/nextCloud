using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using nextCloudAndroid;
using Parse;

namespace DroidTest
{
    [Activity(Label = "nextCloud", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            		
			if (ParseUser.CurrentUser == null)
			{
				loadLoginView ();
				this.Finish ();
			} 
			else
			{
				loadMainView ();
				this.Finish ();
			}
		}

		private void loadLoginView()
		{
			var Intent = new Intent(this, typeof(LoginActivity));
			StartActivity(Intent);
		}

		private void loadMainView()
		{
			var Intent = new Intent(this, typeof(AppActivity));
			StartActivity(Intent);
		}
    }
}