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
using Fragment = Android.Support.V4.App.Fragment;

using Parse;

namespace nextCloudAndroid
{
	public class DossierFragment : Fragment
	{
		protected ListView flistview;
		protected ImageView folder_logo;
		protected TextView folder_text;
		protected ProgressBar loadfile;
		protected ListFileAdapter file_adapter;

		protected List<Fichier> mItems;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate(Resource.Layout.fragment_dossier, container, false);

			flistview = rootView.FindViewById<ListView> (Resource.Id.ListViewFiles);
			folder_logo = rootView.FindViewById<ImageView> (Resource.Id.folder_logo);
			folder_text = rootView.FindViewById<TextView> (Resource.Id.folder_text);
			loadfile = rootView.FindViewById<ProgressBar> (Resource.Id.loading_files);

			mItems = new List<Fichier>();

			AfficherFileFolder (mItems, Donnee.folderName);

			file_adapter = new ListFileAdapter (Activity, mItems);
			flistview.Adapter = file_adapter;

			return rootView;
		}

		public async void AfficherFileFolder(List<Fichier> mItems, String foldername)
		{
			var query = ParseObject.GetQuery ("File").Where (fichier => fichier.Get<ParseUser> ("user") == ParseUser.CurrentUser && fichier.Get<String> ("refer") == foldername);
			IEnumerable<ParseObject> results = await query.FindAsync();
			int nbr = await query.CountAsync();

			if(nbr > 0)
			{
				loadfile.Visibility = ViewStates.Gone;
				folder_logo.Visibility = ViewStates.Gone;
				folder_text.Visibility = ViewStates.Gone;
			}
			else
			{
				loadfile.Visibility = ViewStates.Gone;
				folder_logo.Visibility = ViewStates.Visible;
				folder_text.Text = "Empty folder !";
				folder_text.Visibility = ViewStates.Visible;
			}

			file_adapter.NotifyDataSetChanged	();

			flistview.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				/*Intent intent = new Intent (Intent.ActionView);
					intent.SetDataAndType(Android.Net.Uri.Parse ("https://wordpress.org/plugins/about/readme.txt"), "text/*");
					intent.SetFlags(ActivityFlags.GrantReadUriPermission);
					intent.SetFlags(ActivityFlags.NewTask);
					intent.SetFlags(ActivityFlags.ClearWhenTaskReset);
					StartActivity (intent); */
			};

			foreach (var filez in results)
			{
				if (filez.Get<String> ("type") == ".jpg" || filez.Get<String> ("type") == ".png" || filez.Get<String> ("type") == ".jpeg" || filez.Get<String> ("type") == ".gif" || filez.Get<String> ("type") == ".bmp") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_img
					});
				} else if (filez.Get<String> ("type") == ".mp3" || filez.Get<String> ("type") == ".wav" || filez.Get<String> ("type") == ".aac" || filez.Get<String> ("type") == ".m4a" || filez.Get<String> ("type") == ".ogg" || filez.Get<String> ("type") == ".wma" || filez.Get<String> ("type") == ".rm") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_son
					});
				} else if (filez.Get<String> ("type") == ".mp4" || filez.Get<String> ("type") == ".avi" || filez.Get<String> ("type") == ".mov" || filez.Get<String> ("type") == ".flv" || filez.Get<String> ("type") == ".webm" || filez.Get<String> ("type") == ".mkv" || filez.Get<String> ("type") == ".wmv" || filez.Get<String> ("type") == ".mpg" || filez.Get<String> ("type") == ".3gp" || filez.Get<String> ("type") == ".mpeg") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_vid
					});
				} else if (filez.Get<String> ("type") == ".doc" || filez.Get<String> ("type") == ".docx") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_doc
					});
				} else if (filez.Get<String> ("type") == ".ppt" || filez.Get<String> ("type") == ".pptx") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_ppt
					});
				} else if (filez.Get<String> ("type") == ".xls" || filez.Get<String> ("type") == ".xlsx") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_xls
					});
				} else if (filez.Get<String> ("type") == ".pdf") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_pdf
					});
				} else if (filez.Get<String> ("type") == ".txt") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_txt
					});
				} else if (filez.Get<String> ("type") == ".zip") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_zip
					});
				} else if (filez.Get<String> ("type") == ".rar") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_rar
					});
				} else if (filez.Get<String> ("type") == "folder") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.ic_photo
					});
				} else {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_fichier
					});
				}
			}
		}
	}
}