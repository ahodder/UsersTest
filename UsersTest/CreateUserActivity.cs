using System;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Hardware.Input;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using UsersTest.App;
using UsersTest.Models.Exceptions;
using UsersTest.Models.Extensions;
using UsersTest.ViewModels;
using UsersTest.ViewModels.Factories;

namespace UsersTest {
  [Activity(Label="@string/activity_create_user", Theme="@style/AppTheme.NoActionBar", NoHistory=true)]
  public class CreateUserActivity : AppCompatActivity {
    private const string TAG = nameof(CreateUserActivity);
    
    private CreateUserViewModel _viewModel;

    private View _contentView;
    private TextView _userName;
    private TextView _password;
    private TextView _passwordVerify;

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_create_user);
      
      Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
      SetSupportActionBar(toolbar);

      _viewModel = ViewModelProviders.Of(this, ViewModelFactory.Instance)
        .Get(Java.Lang.Class.FromType(typeof(CreateUserViewModel))) as CreateUserViewModel;

      _contentView = FindViewById(Resource.Id.create_user);
      _userName = FindViewById<TextView>(Resource.Id.user_name);
      _password = FindViewById<TextView>(Resource.Id.password);
      _passwordVerify = FindViewById<TextView>(Resource.Id.password_verify);
    }

    public override bool OnCreateOptionsMenu(IMenu menu) {
      MenuInflater.Inflate(Resource.Menu.menu_create_user, menu);
      return true;
    }

    public override bool OnOptionsItemSelected(IMenuItem item) {
      int id = item.ItemId;
      if (id == Resource.Id.action_save) {
        SaveUser();
        return true;
      }

      return base.OnOptionsItemSelected(item);
    }

    protected override void OnResume() {
      base.OnResume();

      _contentView.Click += ContentViewClicked;
      _userName.TextChanged += UserNameChanged;
      _password.TextChanged += PasswordChanged;
      _passwordVerify.TextChanged += PasswordVerifyChanged;

      if (_viewModel != null) {
        _viewModel.OnPasswordVerifyChanged += PasswordVerifyChanged;
      }
    }

    protected override void OnPause() {
      base.OnPause();

      _contentView.Click -= ContentViewClicked;
      _userName.TextChanged -= UserNameChanged;
      _password.TextChanged -= PasswordChanged;
      _passwordVerify.TextChanged -= PasswordVerifyChanged;
      
      if (_viewModel != null) {
        _viewModel.OnPasswordVerifyChanged -= PasswordVerifyChanged;
      }
    }

    /// <summary>
    /// Attempt to create the user. Failure to do so will result present a reason to the user.
    /// </summary>
    /// <returns></returns>
    public async void SaveUser() {
      /* todo ahodder@praethos.com 2019-07-24: ProgressDialog was deprecated. */
      // However, it really is the best option at this juncture; we are performing a potentially long term action (maybe
      // hitting the network). During this time, we don't want the user interacting with the UI. An alternative is to
      // create a modal activity or loading screen which could be made to look nice- the latter being my preferred choice.
      // A loading screen can have a cute little animation that fits the company theme and is also the perfect place to
      // instantiate the rest of the app state for the logged in user.
      var dialog = new ProgressDialog(this);
      dialog.SetTitle(Resource.String.creating_user);
      dialog.SetMessage(GetString(Resource.String.please_wait));
      dialog.Show();

      try {
        var user = await _viewModel.CreateUserAsync();
        var intent = new Intent(Intent.ActionEdit);
        intent.SetData(AppConsts.UserUri(user));
        StartActivity(intent);
      } catch (UserCreateException e) {
        Log.Error(TAG, "Failed to create user", e);
        switch (e.UserCreateError) {
          case EUserCreateError.InvalidPassword:
            UpdatePasswordError(GetString(Resource.String.error_user_create_exception_password_invalid));
            break;
          
          case EUserCreateError.NoUserName:
            UpdateUserNameError(GetString(Resource.String.error_user_create_exception_no_user_name));
            break;
          
          case EUserCreateError.UserAlreadyExists:
            UpdateUserNameError(GetString(Resource.String.error_user_create_exception_already_exists));
            break;
          
          case EUserCreateError.PasswordsDoNotMatch:
            UpdatePasswordError(GetString(Resource.String.error_user_create_exception_passwords_do_not_match));
            break;
          
          default:
            Toast.MakeText(this, $"{GetString(Resource.String.error_failed_to_create_user)}: {e.UserCreateError.ToLocalizedString(this)}", ToastLength.Short).Show();
            break;
        }
      } finally {
        dialog.Dismiss();
      }
    }

    private void UpdateUserNameError(string msg) {
      /* note ahodder@praethos.com 2019-07-24: This method is a little excessive. Remove? */
      _userName.Error = msg;
    }

    private void UpdatePasswordError(string msg) {
      _password.Error = msg;
      _passwordVerify.Error = msg;
    }

    private void UserNameChanged(object obj, TextChangedEventArgs args) {
      _viewModel.UserName = _userName.Text;
      
      /* note ahodder@praethos.com 2019-07-24: Rather than create a callback loop for when the user name changes,
       simply check whether or not the user name is valid here. If not then we will need to present an error. */

      if (!_viewModel.IsUserNameValid) {
        UpdateUserNameError(GetString(Resource.String.error_user_create_exception_user_name_invalid));
      } else {
        UpdateUserNameError(null);
      }
    }

    private void ContentViewClicked(object obj, EventArgs args) {
      var im = (InputMethodManager)GetSystemService(Context.InputMethodService);
      im.HideSoftInputFromWindow(_contentView.WindowToken, 0);
    }
    
    private void PasswordChanged(object obj, TextChangedEventArgs args) => _viewModel.Password = _password.Text;
    private void PasswordVerifyChanged(object obj, TextChangedEventArgs args) => _viewModel.PasswordVerify = _passwordVerify.Text;
    
    private void PasswordVerifyChanged(object obj, PasswordMatchEventArgs args) {
      if (!args.PasswordsMatch) {
        UpdatePasswordError(GetString(Resource.String.error_user_create_exception_passwords_do_not_match));
      } else if (!args.PasswordValid) {
        UpdatePasswordError(GetString(Resource.String.error_user_create_exception_password_invalid));
      } else {
        UpdatePasswordError(null);
      }
    }
  }
}