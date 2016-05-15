using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Parse;

namespace nextCloudAndroid
{
	[Activity (Label = "Sign In")]			
	public class LoginActivity : Activity
	{
		protected EditText usernameEditText;
		protected EditText passwordEditText;
		protected Button loginButton;
		protected TextView signUpButton;

		protected String username;
		protected String password;

		private ProgressBar myProgress;

		protected override void OnCreate (Bundle bundle)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.LoginLayout);

			myProgress = FindViewById<ProgressBar> (Resource.Id.myProgress);

			myProgress.Visibility = ViewStates.Invisible;

			signUpButton = FindViewById<TextView> (Resource.Id.btnsignup);
			usernameEditText = FindViewById<EditText> (Resource.Id.usrinput);
			passwordEditText = FindViewById<EditText> (Resource.Id.pwinput);
			loginButton = FindViewById<Button> (Resource.Id.btnlogin);

			signUpButton.Click += (object sender, EventArgs e) =>
			{
				var Intent = new Intent(this, typeof(RegisterActivity));
				StartActivity(Intent);
				this.Finish();
			};

			passwordEditText.EditorAction += (sender, e) => 
			{
				if (e.ActionId == ImeAction.Done) 
				{
					InputMethodManager manager = (InputMethodManager) GetSystemService(InputMethodService);
					manager.HideSoftInputFromWindow(passwordEditText.WindowToken, 0);
					TryToLogin(username, password);
				}
				else
				{
					e.Handled = false;
				}
			};

			loginButton.Click += delegate 
			{
				if (usernameEditText.Text == "")
				{
					Toast.MakeText(this, "Please enter a valid username.", ToastLength.Short).Show();
				}
				else if (passwordEditText.Text == "")
				{
					Toast.MakeText(this, "Please enter the password.", ToastLength.Short).Show();
				}
				else
				{
					TryToLogin(username, password);
				}
			};
		}

		public async void TryToLogin(String username, String password)
		{
			username = usernameEditText.Text.ToString();
			password = passwordEditText.Text.ToString();

			username = username.Trim();
			password = password.Trim();

			try
			{
				myProgress.Visibility = ViewStates.Visible;
				loginButton.Enabled = false;
				signUpButton.Enabled = false;
				await ParseUser.LogInAsync(username, password);
				var Intent = new Intent(this, typeof(AppActivity));
				StartActivity(Intent);
				this.Finish();
			}
			catch (Exception e)
			{
				myProgress.Visibility = ViewStates.Invisible;
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetMessage(Resource.String.login_error_message);
				alert.SetTitle(Resource.String.login_error_title);
				alert.SetPositiveButton("OK", delegate {});

				AlertDialog dialog = alert.Create();
				dialog.Show();

				loginButton.Enabled = true;
				signUpButton.Enabled = true;
			}
		}
	}
}