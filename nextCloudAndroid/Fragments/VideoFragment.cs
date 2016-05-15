﻿using System;
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
	public class VideoFragment : Fragment
	{
		protected Button button_add;
		protected LinearLayout popup_new;
		protected LinearLayout folder_lay;
		protected LinearLayout join_lay;
		protected ListView flistview;
		protected TextView video_text;
		protected ImageView video_logo;
		protected ProgressBar loadfile;
		protected ListFileAdapter file_adapter;
		protected ProgressDialog dialog_progress;

		protected List<Fichier> list;

		public VideoFragment(){
			this.RetainInstance = true;
			this.HasOptionsMenu = (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate(Resource.Layout.fragment_video, container, false);

			button_add = rootView.FindViewById<Button> (Resource.Id.newitem_btn);
			popup_new = rootView.FindViewById<LinearLayout> (Resource.Id.popup_new);
			folder_lay = rootView.FindViewById<LinearLayout> (Resource.Id.new_folder_layout);
			join_lay = rootView.FindViewById<LinearLayout> (Resource.Id.import_file_layout);
			flistview = rootView.FindViewById<ListView> (Resource.Id.ListViewFiles);
			video_text = rootView.FindViewById<TextView> (Resource.Id.mesvideos_text);
			loadfile = rootView.FindViewById<ProgressBar> (Resource.Id.loading_files);
			video_logo = rootView.FindViewById<ImageView> (Resource.Id.mesvideos_logo);

			list = new List<Fichier>();

			file_adapter = new ListFileAdapter (Activity, list);
			flistview.Adapter = file_adapter;

			AfficherVideo (list);
				
			popup_new.Animate().TranslationY(300);

			button_add.Click += (object sender, EventArgs e) =>
			{
				popup_new.Visibility = ViewStates.Visible;
				popup_new.Animate().TranslationY(0);
			};

			folder_lay.Click += delegate
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(Activity);
				alert.SetTitle("Create a new folder:");

				EditText input = new EditText(Activity);
				alert.SetView(input);

				alert.SetPositiveButton("Create", new EventHandler<DialogClickEventArgs>(delegate {
					popup_new.Animate().TranslationY(300);
					String value = input.Text;
					file_adapter.AjouterDossier(value);

					Handler handler = new Handler();
					handler.PostDelayed(RefreshThis, 5);

					Toast.MakeText(Activity, "Folder : \"" + value + "\" created" , ToastLength.Short).Show();
				}));

				alert.SetNegativeButton("Cancel", new EventHandler<DialogClickEventArgs>(delegate {

				}));	

				alert.Show();
			};

			join_lay.Click += (s, e) =>
			{
				var intent = new Intent(Activity, typeof(FilePickerActivity));
				StartActivityForResult(intent, FilePickerActivity.ResultCodeDirSelected);
			};

			flistview.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				dialog_progress = new ProgressDialog(Activity);
				var listView = sender as ListView;
				var t = list[e.Position];

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

			rootView.Touch += (sender, e) => 
			{
				var handled = false;
				if (e.Event.Action == MotionEventActions.Down)
				{
					popup_new.Animate().TranslationY(300);
					handled = true;
				}
				else if (e.Event.Action == MotionEventActions.Up)
				{
					popup_new.Animate().TranslationY(300);
					handled = true;
				}

				e.Handled = handled;
			};

			return rootView;
		}

		public void RefreshThis()
		{
			Android.Support.V4.App.Fragment fragment = null;
			fragment = new VideoFragment();
			Android.Support.V4.App.FragmentManager fragmentManager = Activity.SupportFragmentManager;
			Android.Support.V4.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction ();
			fragmentTransaction.Replace (Resource.Id.content_frame, fragment);
			fragmentTransaction.Commit ();
		}

		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			switch (requestCode)
			{
			case FilePickerActivity.ResultCodeDirSelected:
				switch (resultCode)
				{
				case (int) Result.Canceled:
					break;
				case (int) Result.FirstUser:
					break;
				case (int) Result.Ok:
					String filename = data.GetStringExtra (FilePickerActivity.ResultSelectedDir);

					byte[] bytes = File.ReadAllBytes (filename);

					String file_name = filename.Substring (filename.LastIndexOf ('/') + 1);
					String file_type = filename.Substring (filename.LastIndexOf ('.'));

					file_adapter.AjouterFichier (file_name, bytes, file_type);

					Handler handler = new Handler();
					handler.PostDelayed(RefreshThis, 5);

					popup_new.Animate ().TranslationY (300);

					break;
				default:
					throw new ArgumentOutOfRangeException("resultCode");
				}
				break;
			}
		}

		public async void AfficherVideo(List<Fichier> list)
		{
			var query = ParseObject.GetQuery ("Fichier").Where (filez => filez.Get<ParseUser> ("user") == ParseUser.CurrentUser && (filez.Get<String> ("type") == ".mp4" || filez.Get<String> ("type") == ".avi" || filez.Get<String> ("type") == ".mov" || filez.Get<String> ("type") == ".flv" || filez.Get<String> ("type") == ".webm" || filez.Get<String> ("type") == ".mkv" || filez.Get<String> ("type") == ".wmv" || filez.Get<String> ("type") == ".mpg" || filez.Get<String> ("type") == ".3gp" || filez.Get<String> ("type") == ".mpeg") && filez.Get<String> ("refer") == "");
			IEnumerable<ParseObject> results = await query.FindAsync();
			int nbr = await query.CountAsync();

			if (nbr > 0)
			{
				loadfile.Visibility = ViewStates.Gone;
				video_logo.Visibility = ViewStates.Gone;
				video_text.Visibility = ViewStates.Gone;
			}
			else if (nbr == 0)
			{
				loadfile.Visibility = ViewStates.Gone;
				video_logo.Visibility = ViewStates.Visible;
				video_text.Text = "No video available !";
				video_text.Visibility = ViewStates.Visible;
			}

			file_adapter.NotifyDataSetChanged	();

			foreach (var filez in results)
			{
				ParseFile ok = filez.Get<ParseFile>("fichier");

				if (filez.Get<String> ("type") == ".mp4" || filez.Get<String> ("type") == ".avi" || filez.Get<String> ("type") == ".mov" || filez.Get<String> ("type") == ".flv" || filez.Get<String> ("type") == ".webm" || filez.Get<String> ("type") == ".mkv" || filez.Get<String> ("type") == ".wmv" || filez.Get<String> ("type") == ".mpg" || filez.Get<String> ("type") == ".3gp" || filez.Get<String> ("type") == ".mpeg") {
					list.Add (new Fichier () {
						FileName = filez.Get<String> ("name"),
						Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_vid,
						Link = ok.Url
					});
				} else if (filez.Get<String> ("type") == "folder") {
					list.Add (new Fichier () {

						FileName = filez.Get<String> ("name"),
						//Date = filez.Get<String> ("date"),
						Photo = Resource.Drawable.item_folder
					});
				}
			}
		}
	}
}