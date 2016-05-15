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
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using SharedProj;

using Parse;

namespace nextCloudAndroid
{
	[Activity (Label = "nextCloud", Theme="@style/nextCloudTheme")]			
	public class AppActivity : FragmentActivity
	{
		DrawerLayout mDrawerLayout;
		ListView mDrawerList;
		MyActionBarDrawerToggle mDrawerToggle;

		String mDrawerTitle;
		String mTitle;

		List<NavDrawerItem> navDrawerItems;
		NavDrawerListAdapter adapter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.TheMain);

			this.mTitle = this.mDrawerTitle = this.Title;

			this.mDrawerLayout = this.FindViewById<DrawerLayout> (Resource.Id.myDrawer);
			this.mDrawerList = this.FindViewById<ListView> (Resource.Id.leftListView);

			navDrawerItems = new List<NavDrawerItem>();

			navDrawerItems.Add (new NavDrawerItem("My Space", Resource.Drawable.ic_espace));
			navDrawerItems.Add (new NavDrawerItem("Pictures", Resource.Drawable.ic_photo));
			navDrawerItems.Add (new NavDrawerItem("Videos", Resource.Drawable.ic_video));
			navDrawerItems.Add (new NavDrawerItem("Sounds", Resource.Drawable.ic_music));
			navDrawerItems.Add (new NavDrawerItem("Documents", Resource.Drawable.ic_doc));
			navDrawerItems.Add (new NavDrawerItem("Parameters", Resource.Drawable.ic_settings));

			adapter = new NavDrawerListAdapter (this, navDrawerItems);
			mDrawerList.SetAdapter (adapter);

			this.mDrawerList.ItemClick += (sender, args) => ListItemClicked(args.Position);

			this.mDrawerToggle = new MyActionBarDrawerToggle (this, this.mDrawerLayout, Resource.Drawable.ic_drawer, Resource.String.open_drawer, Resource.String.close_drawer);

			this.mDrawerToggle.DrawerClosed += (o, args) => {
				this.ActionBar.Title = this.Title;
				this.InvalidateOptionsMenu ();
			};

			this.mDrawerToggle.DrawerOpened += (o, args) => {
				this.ActionBar.Title = this.mDrawerTitle;
				this.InvalidateOptionsMenu ();
			};

			this.mDrawerLayout.SetDrawerListener (this.mDrawerToggle);

			if (savedInstanceState == null)
			{
				ListItemClicked(0);
			}

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);
		}

		private void ListItemClicked(int position)
		{
			Android.Support.V4.App.Fragment fragment = null;
			switch (position)
			{
			case 0:
				fragment = new EspaceFragment();
				break;
			case 1:
				fragment = new PhotoFragment();
				break;
			case 2:
				fragment = new VideoFragment();
				break;
			case 3:
				fragment = new SonFragment();
				break;
			case 4:
				fragment = new DocumentFragment();
				break;
			case 5:
				fragment = new ParametreFragment();
				break;
			}
			SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.content_frame, fragment)
				.Commit();
			this.mDrawerList.SetItemChecked(position, true);

			this.mDrawerLayout.CloseDrawer(this.mDrawerList);
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			var drawerOpen = this.mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left);
			for (int i = 0; i < menu.Size (); i++)
				menu.GetItem (i).SetVisible (!drawerOpen);
			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.activity_main_actions, menu);

			var item = menu.FindItem (Resource.Id.action_search);

			return true;
		}

		public override bool OnMenuItemSelected (int featureId, IMenuItem item)
		{
			if (item.ItemId == Resource.Id.action_search) 
				{
					//Toast.MakeText(this, "Veuillez saisir un nom d'utilisateur.", ToastLength.Short).Show();
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("File/Folder name:");

					EditText input = new EditText(this);
					alert.SetView(input);

					alert.SetPositiveButton("Search", new EventHandler<DialogClickEventArgs>(delegate {
						Donnee.searchData = input.Text;
						Android.Support.V4.App.Fragment fragment = null;
						fragment = new SearchFragment();
						SupportFragmentManager.BeginTransaction()
							.Replace(Resource.Id.content_frame, fragment)
							.AddToBackStack(null)
							.Commit();
					}));

					alert.Show();
				}
			return base.OnMenuItemSelected (featureId, item);
		}


		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			this.mDrawerToggle.SyncState ();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			this.mDrawerToggle.OnConfigurationChanged (newConfig);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (this.mDrawerToggle.OnOptionsItemSelected (item))
			{
				return true;
			}
				return base.OnOptionsItemSelected (item);
		}
	}
}