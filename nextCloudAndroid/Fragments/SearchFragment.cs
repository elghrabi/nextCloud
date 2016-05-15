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
using System.Net;

namespace nextCloudAndroid
{
	public class SearchFragment : Fragment
	{
		protected ListView flistview;
		protected ImageView search_logo;
		protected TextView search_text;
		protected ProgressBar loadfile;
		protected ListFileAdapter file_adapter;
		protected ProgressDialog dialog_progress;

		protected List<Fichier> mItems;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate(Resource.Layout.fragment_search, container, false);

			flistview = rootView.FindViewById<ListView> (Resource.Id.ListViewFiles);
			search_logo = rootView.FindViewById<ImageView> (Resource.Id.search_logo);
			search_text = rootView.FindViewById<TextView> (Resource.Id.search_text);
			loadfile = rootView.FindViewById<ProgressBar> (Resource.Id.loading_files);

			mItems = new List<Fichier>();

			AfficherResultat (mItems);

			file_adapter = new ListFileAdapter (Activity, mItems);
			flistview.Adapter = file_adapter;

			flistview.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				dialog_progress = new ProgressDialog(Activity);
				var listView = sender as ListView;
				var t = mItems[e.Position];

				dialog_progress.Indeterminate = true;
				dialog_progress.SetProgressStyle(ProgressDialogStyle.Spinner);
				dialog_progress.SetMessage("File loading. Please wait...");
				dialog_progress.SetCancelable(false);

				if (t.Photo == Resource.Drawable.item_folder)
				{
					Donnee.folderName = t.FileName;
					Android.Support.V4.App.Fragment fragment = null;
					fragment = new DossierFragment ();
					Android.Support.V4.App.FragmentManager fragmentManager = Activity.SupportFragmentManager;
					Android.Support.V4.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction ();
					fragmentTransaction.Replace (Resource.Id.content_frame, fragment);
					fragmentTransaction.AddToBackStack(null);
					fragmentTransaction.Commit ();
				}
				else
				{	
					dialog_progress.Show();

					var response = await HttpWebRequest.CreateHttp(t.Link).GetResponseAsync();

					Stream stream = response.GetResponseStream();

					String extStorageDirectory = Android.OS.Environment.ExternalStorageDirectory.ToString ();

					using (var client = new WebClient())
					using (var file = File.Create(extStorageDirectory + "/media/" + t.FileName))
					{
						stream.CopyTo(file);
					}

					dialog_progress.Dismiss();

					Intent intent = new Intent (Intent.ActionView);

					String filePath = "file://" + extStorageDirectory + "/media/" + t.FileName;

					if (t.FileName.Contains(".doc") || t.FileName.Contains(".docx"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "application/msword");
					}
					else if (t.FileName.Contains(".pdf"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "application/pdf");
					}
					else if (t.FileName.Contains(".ppt") || t.FileName.Contains(".pptx"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "application/vnd.ms-powerpoint");
					}
					else if (t.FileName.Contains(".xls") || t.FileName.Contains(".xlsx"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "application/vnd.ms-excel");
					}
					else if (t.FileName.Contains(".zip") || t.FileName.Contains(".rar"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "application/*");
					}
					else if (t.FileName.Contains(".rtf"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "application/rtf");
					}
					else if (t.FileName.Contains(".wav") || t.FileName.Contains(".mp3"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "audio/*");
					}
					else if (t.FileName.Contains(".gif"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "image/gif");
					}
					else if (t.FileName.Contains(".jpg") || t.FileName.Contains(".jpeg") || t.FileName.Contains(".png"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "image/jpeg");
					}
					else if (t.FileName.Contains(".txt"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "text/plain");
					}
					else if (t.FileName.Contains(".3gp") || t.FileName.Contains(".mpg") || t.FileName.Contains(".mpg") || t.FileName.Contains(".mpeg") || t.FileName.Contains(".mpe") || t.FileName.Contains(".mp4") || t.FileName.Contains(".avi"))
					{
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "video/*");
					}
					else
					{	
						intent.SetDataAndType(Android.Net.Uri.Parse(filePath), "*/*");
					}
					intent.SetFlags(ActivityFlags.NewTask);
					StartActivity (intent);
				}
			};

			return rootView;
		}

		public async void AfficherResultat(List<Fichier> mItems)
		{
			var query = ParseObject.GetQuery ("Fichier").Where (filez => filez.Get<ParseUser> ("user") == ParseUser.CurrentUser && filez.Get<String>("name").Contains(Donnee.searchData));
			IEnumerable<ParseObject> results = await query.FindAsync();
			int nbr = await query.CountAsync();

			if(nbr > 0)
			{
				loadfile.Visibility = ViewStates.Gone;
				search_logo.Visibility = ViewStates.Gone;
				search_text.Visibility = ViewStates.Gone;
			}
			else
			{
				loadfile.Visibility = ViewStates.Gone;
				search_logo.Visibility = ViewStates.Visible;
				search_text.Text = "File not found !";
				search_text.Visibility = ViewStates.Visible;
			}

			file_adapter.NotifyDataSetChanged	();

			foreach (var filez in results)
			{
				ParseFile ok = filez.Get<ParseFile>("fichier");

				if (filez.Get<String> ("type") == ".jpg" || filez.Get<String> ("type") == ".png" || filez.Get<String> ("type") == ".jpeg" || filez.Get<String> ("type") == ".gif" || filez.Get<String> ("type") == ".bmp") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_img,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".mp3" || filez.Get<String> ("type") == ".wav" || filez.Get<String> ("type") == ".aac" || filez.Get<String> ("type") == ".m4a" || filez.Get<String> ("type") == ".ogg" || filez.Get<String> ("type") == ".wma" || filez.Get<String> ("type") == ".rm") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_son,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".mp4" || filez.Get<String> ("type") == ".avi" || filez.Get<String> ("type") == ".mov" || filez.Get<String> ("type") == ".flv" || filez.Get<String> ("type") == ".webm" || filez.Get<String> ("type") == ".mkv" || filez.Get<String> ("type") == ".wmv" || filez.Get<String> ("type") == ".mpg" || filez.Get<String> ("type") == ".3gp" || filez.Get<String> ("type") == ".mpeg") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_vid,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".doc" || filez.Get<String> ("type") == ".docx") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_doc,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".ppt" || filez.Get<String> ("type") == ".pptx") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_ppt,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".xls" || filez.Get<String> ("type") == ".xlsx") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_xls,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".pdf") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_pdf,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".txt") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_txt,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".zip") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_zip,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == ".rar") {
					mItems.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_rar,
						Link = ok.Url
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
						Photo = Resource.Drawable.item_fichier,
						Link = ok.Url
					});
				}
			}
		}
	}
}