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

using Parse;

namespace nextCloudAndroid
{
	[Activity (Label = "Sign Up")]
	public class RegisterActivity : Activity
	{	
		protected EditText usernameEditText;
		protected EditText passwordEditText;
		protected EditText emailEditText;
		protected Button signUpButton;
		protected Button connectButton;

		private ProgressBar mprogress;

		protected override void OnCreate (Bundle bundle)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.RegisterLayout);

			mprogress = FindViewById<ProgressBar> (Resource.Id.mprogress);

			mprogress.Visibility = ViewStates.Invisible;

			signUpButton = FindViewById<Button> (Resource.Id.signupButton);
			usernameEditText = FindViewById<EditText> (Resource.Id.usernameField);
			passwordEditText = FindViewById<EditText> (Resource.Id.passwordField);
			emailEditText = FindViewById<EditText> (Resource.Id.emailField);
			connectButton = FindViewById<Button> (Resource.Id.btn_connect);

			connectButton.Click += (object sender, EventArgs e) => 
			{
				var Intent = new Intent(this, typeof(LoginActivity));
				StartActivity(Intent);
				this.Finish();
			};

			signUpButton.Click += async delegate 
			{
				String username = usernameEditText.Text.ToString();
				String password = passwordEditText.Text.ToString();
				String email = emailEditText.Text.ToString();

				username = username.Trim();
				password = password.Trim();
				email = email.Trim();

				if (usernameEditText.Text == "" || passwordEditText.Text == "" || emailEditText.Text == "")
				{
					Toast.MakeText(this, "Please fill all the informations.", ToastLength.Short).Show();
				}
				else
				{
					var user = new ParseUser()
					{
						Username = username,
						Password = password,
						Email = email
					};

					mprogress.Visibility = ViewStates.Visible;
							
					await user.SignUpAsync();

					var Intent = new Intent(this, typeof(AppActivity));
					StartActivity(Intent);
					this.Finish();
				}
			};
		}
	}
}