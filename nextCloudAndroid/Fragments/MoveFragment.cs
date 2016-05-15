using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SharedProj;

using Android.Net;
using Android.Support.V4.App;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Parse;

namespace nextCloudAndroid
{
	public class MoveFragment : Android.App.Fragment
	{
		protected ListView flistview;
		protected ImageView folder_logo;
		protected TextView folderonly_text;
		protected ProgressBar loadfile;
		protected ListFileAdapter file_adapter;

		protected List<Fichier> mItems;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate(Resource.Layout.fragment_move, container, false);

			flistview = rootView.FindViewById<ListView> (Resource.Id.ListViewFiles);
			folder_logo = rootView.FindViewById<ImageView> (Resource.Id.folder_logo);
			folderonly_text = rootView.FindViewById<TextView> (Resource.Id.folderonly_text);
			loadfile = rootView.FindViewById<ProgressBar> (Resource.Id.loading_files);

			mItems = new List<Fichier>();

			AfficherFolder (mItems, Donnee.folderName);

			file_adapter = new ListFileAdapter (Activity, mItems);
			flistview.Adapter = file_adapter;

			return rootView;
		}

		public async void AfficherFolder(List<Fichier> mItems, String foldername)
		{
			var query = ParseObject.GetQuery ("File").Where (fichier => fichier.Get<ParseUser> ("user") == ParseUser.CurrentUser && fichier.Get<String> ("type") == "folder");
			IEnumerable<ParseObject> results = await query.FindAsync();
			int nbr = await query.CountAsync();

			if(nbr > 0)
			{
				loadfile.Visibility = ViewStates.Gone;
				folder_logo.Visibility = ViewStates.Gone;
				folderonly_text.Visibility = ViewStates.Gone;
			}
			else
			{
				loadfile.Visibility = ViewStates.Gone;
				folder_logo.Visibility = ViewStates.Visible;
				folderonly_text.Text = "Pas de dossiers créés!";
				folderonly_text.Visibility = ViewStates.Visible;
			}

			file_adapter.NotifyDataSetChanged	();

			foreach (var filez in results)
			{
				flistview.ItemClick += async delegate(object sender, AdapterView.ItemClickEventArgs e) 
				{
					var listView = sender as ListView;
					var t = mItems[e.Position];

					var qz = ParseObject.GetQuery ("File").Where (fichier => fichier.Get<ParseUser> ("user") == ParseUser.CurrentUser && fichier.Get<String> ("name") == Donnee.fileName);
					IEnumerable<ParseObject> rez = await qz.FindAsync();

					foreach(var fich in rez)
					{
						ParseObject fi = new ParseObject("File");
						fi = fich;
						fi["refer"] = t.FileName;
						await fich.SaveAsync();
					}
				};

				if (filez.Get<String> ("type") == "folder") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						//Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_folder
					});
				}
			}
		}
	}
}
