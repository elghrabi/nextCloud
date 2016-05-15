using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedProj;
using System.IO;

using Parse;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Animation;
using Android.Views.Animations;
using Android.Net;
using Android.Support.V4.App;

using Java.Net;

namespace nextCloudAndroid
{	
	[Activity (Label = "ListFileAdapter")]
	public class ListFileAdapter : BaseAdapter<Fichier>
	{
		Activity context;
		List<Fichier> list;

		public ListFileAdapter (Activity _context, List<Fichier> _list)
			:base()
		{
			this.context = _context;
			this.list = _list;
		}

		public override int Count {
			get { return list.Count; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Fichier this[int index] {
			get { return list [index]; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView; 

			if (view == null)
				view = context.LayoutInflater.Inflate (Resource.Layout.ListItemRow, parent, false);

			Fichier item = this [position];

			view.FindViewById<TextView> (Resource.Id.FileName).Text = item.FileName;
			view.FindViewById<TextView>(Resource.Id.FileDateCreated).Text = item.Date;

			var iconType = view.FindViewById<ImageView> (Resource.Id.Thumbnail);
			var iconOptions = view.FindViewById<ImageView> (Resource.Id.icon_options);
			var LayoutOptions = view.FindViewById<LinearLayout> (Resource.Id.options_lay);
			var DeleteItem = view.FindViewById<LinearLayout> (Resource.Id.delete_item);
			var RenameItem = view.FindViewById<LinearLayout> (Resource.Id.rename_item);
			var MoveItem = view.FindViewById<LinearLayout> (Resource.Id.move_item);

			iconType.SetImageResource (item.Photo);

			iconOptions.Focusable = false;
			iconOptions.FocusableInTouchMode = false;
			iconOptions.Clickable = true;

			iconOptions.Click += (sender, args) =>
			{
				String test = LayoutOptions.Visibility.ToString();

				Animation animationFadeIn = AnimationUtils.LoadAnimation(context, Resource.Animation.fade_in);
				Animation animationFadeOut = AnimationUtils.LoadAnimation(context, Resource.Animation.fade_out);

				if (test == "Gone")
				{
					LayoutOptions.StartAnimation(animationFadeIn);
					LayoutOptions.Visibility = ViewStates.Visible;
				}
				else
				{
					LayoutOptions.StartAnimation(animationFadeOut);
					LayoutOptions.Visibility = ViewStates.Gone;
				}
			};

			DeleteItem.Click += (sender, args) => 
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(context);
				alert.SetTitle("Do you really want to delete " + item.FileName + "?");

				alert.SetPositiveButton("Yes", new EventHandler<DialogClickEventArgs>(delegate {
					SupprimerFichier(item);
					Toast.MakeText(view.Context, "Element " + "\"" + item.FileName +"\"" + " deleted !", ToastLength.Short).Show();
				}));

				alert.SetNegativeButton("No", new EventHandler<DialogClickEventArgs>(delegate {

				}));

				alert.Show();
			};

			RenameItem.Click += (sender, args) => 
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(context);
				alert.SetTitle("Renommer " + item.FileName + "?");

				EditText input = new EditText(context);
				input.Text = item.FileName;
				alert.SetView(input);

				alert.SetPositiveButton("Ok", new EventHandler<DialogClickEventArgs>(delegate {
					String value = input.Text;
					RenommerFichier(item, value);
					Toast.MakeText(view.Context, "New name is : \"" + value + "\"", ToastLength.Short).Show();
				}));

				alert.SetNegativeButton("Cancel", new EventHandler<DialogClickEventArgs>(delegate {

				}));	

				alert.Show();
			};

			MoveItem.Click += (sender, args) => 
			{
				Android.App.Fragment fragment = null;
				fragment = new MoveFragment ();
				Android.App.FragmentManager fragmentManager = context.FragmentManager;
				Android.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
				fragmentTransaction.Replace (Resource.Id.content_frame, fragment);
				fragmentTransaction.Commit ();

				Donnee.fileName = item.FileName;
			};

			return view;
		}

		public override void NotifyDataSetChanged()
		{
			base.NotifyDataSetChanged ();
		}

		public async void AjouterFichier(String FileName, byte[] data, String FileType)
		{
			ProgressDialog progress;

			progress = new ProgressDialog(context);
			progress.Indeterminate = true;
			progress.SetProgressStyle(ProgressDialogStyle.Spinner);
			progress.SetMessage("Uploading the file. Please wait...");
			progress.SetCancelable(false);

			ParseFile file = new ParseFile(FileName, data, FileType);

			await file.SaveAsync(new Progress<ParseUploadProgressEventArgs>(e => {
				progress.Show();
			}));
			
			//Change columns names of Parse Object here.
			ParseObject fichier = new ParseObject("File"); //'File' is the name of my object.
			//File Columns (change it)
			fichier["fichier"] = file;
			fichier["user"] = ParseUser.CurrentUser;
			fichier["type"] = FileType;
			fichier["name"] = FileName;
			fichier["date"] = DateTime.Today.ToLongDateString().ToString();
			fichier["refer"] = "";

			await fichier.SaveAsync();

			progress.Dismiss();
		}

		public async void AjouterDossier(String value)
		{
			ProgressDialog progress;

			progress = new ProgressDialog(context);
			progress.Indeterminate = true;
			progress.SetProgressStyle(ProgressDialogStyle.Spinner);
			progress.SetMessage("Creating the folder...");
			progress.SetCancelable(false);

			progress.Show ();

			//Change columns names of Parse Object here.
			ParseObject fichier = new ParseObject("File"); //'File' is the name of my object.
			//File Columns (change it)
			fichier["fichier"] = null;
			fichier["user"] = ParseUser.CurrentUser;
			fichier["type"] = "folder";
			fichier["name"] = value;
			fichier["date"] = DateTime.Today.ToLongDateString().ToString();
			fichier["refer"] = "";
			await fichier.SaveAsync();

			progress.Dismiss ();
		}

		public async void SupprimerFichier(Fichier item)
		{
			var query = ParseObject.GetQuery ("File").Where (fichier => fichier.Get<ParseUser> ("user") == ParseUser.CurrentUser && fichier.Get<String> ("name") == item.FileName);
			int x = await query.CountAsync();
			IEnumerable<ParseObject> results = await query.FindAsync();

			foreach (var obj in results)
			{
				await obj.DeleteAsync();
			}


		}

		public async void RenommerFichier(Fichier item, String value)
		{
			var query = ParseObject.GetQuery ("File").Where (fichier => fichier.Get<ParseUser> ("user") == ParseUser.CurrentUser && fichier.Get<String> ("name") == item.FileName);
			int x = await query.CountAsync();
			IEnumerable<ParseObject> results = await query.FindAsync();
			foreach (var obj in results) {
				ParseObject fichier = new ParseObject ("File");
				fichier = obj;
				fichier ["name"] = value;

				await obj.SaveAsync ();
			}
		}
			
			public void DownloadFile(String fileURL, Java.IO.File directory) {
				try {
					
					Java.IO.FileOutputStream f = new Java.IO.FileOutputStream(directory);
					URL u = new URL(fileURL);
					HttpURLConnection c = (HttpURLConnection)u.OpenConnection();
					c.RequestMethod = "GET";
					c.DoOutput = true;
					c.Connect();

					System.IO.Stream inp = c.InputStream;

					byte[] buffer = new byte[1024*1024];
					int len1 = 0;
					while ((len1 = inp.Read(buffer, 0, (int)buffer.Length)) > 0)
					{
						f.Write(buffer, 0, len1);
					}
					f.Close();

				}
				catch (Exception e) {
					e.StackTrace.ToString ();
			}
	
		}
	}
}