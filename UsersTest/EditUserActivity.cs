using System;
using System.IO;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using UsersTest.App;
using UsersTest.Models;
using UsersTest.ViewModels;
using UsersTest.ViewModels.Factories;

namespace UsersTest {
  [Activity(Label="@string/activity_edit_user", Theme="@style/AppTheme.NoActionBar")]
  [IntentFilter(new []{Intent.ActionView, Intent.ActionEdit},
    Categories = new []{Intent.CategoryDefault, Intent.CategoryBrowsable},
    DataScheme= AppConsts.Scheme,
    DataHost=AppConsts.DataTypeUser)]
  public class EditUserActivity : AppCompatActivity {
    private const string TAG = nameof(EditUserActivity);

    private EditUserViewModel _viewModel;

    private TextView _userName;
    private TextView _firstName;
    private TextView _lastName;
    private TextView _address1;
    private TextView _address2;
    private TextView _city;
    private TextView _state;
    private TextView _country;
    
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_edit_user);
      
      Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
      SetSupportActionBar(toolbar);

      _viewModel = (EditUserViewModel)ViewModelProviders.Of(this, ViewModelFactory.Instance)
        .Get(Java.Lang.Class.FromType(typeof(EditUserViewModel)));

      _userName = FindViewById<TextView>(Resource.Id.user_name);
      _firstName = FindViewById<TextView>(Resource.Id.first_name);
      _lastName = FindViewById<TextView>(Resource.Id.last_name);
      _address1 = FindViewById<TextView>(Resource.Id.address1);
      _address2 = FindViewById<TextView>(Resource.Id.address2);
      _city = FindViewById<TextView>(Resource.Id.city);
      _state = FindViewById<TextView>(Resource.Id.state);
      _country = FindViewById<TextView>(Resource.Id.country);
    }
    
    public override bool OnCreateOptionsMenu(IMenu menu) {
      MenuInflater.Inflate(Resource.Menu.menu_edit_user, menu);
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
      if (_viewModel.User == null) {
        try {
          var id = AppConsts.GetUserId(Intent.Data);
          LoadUser(id);
        } catch (ArgumentException e) {
          Log.Error(TAG, "Failed to retrieve user from intent");
          Toast.MakeText(this, Resource.String.error_failed_to_provide_user, ToastLength.Long).Show();
          Finish();
          return;
        }
      }
    }

    public async void LoadUser(uint id) {
      /* todo ahodder@praethos.com 2019-07-24: ProgressDialog was deprecated. */
      // However, it really is the best option at this juncture; we are performing a potentially long term action (maybe
      // hitting the network). During this time, we don't want the user interacting with the UI. An alternative is to
      // create a modal activity or loading screen which could be made to look nice- the latter being my preferred choice.
      // A loading screen can have a cute little animation that fits the company theme and is also the perfect place to
      // instantiate the rest of the app state for the logged in user.
      var dialog = new ProgressDialog(this);
      dialog.SetTitle(Resource.String.creating_user);
      dialog.SetMessage(GetString(Resource.String.please_wait));
      dialog.SetCancelable(false);
      dialog.Show();

      try {
        await _viewModel.LoadUserAsync(id);
        Populate(_viewModel.User);
      } catch (IOException e) {
        Log.Error(TAG, "Failed to create user");
        Toast.MakeText(this, Resource.String.error_failed_to_load_user, ToastLength.Long).Show();
      } finally {
        dialog.Dismiss();
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
      dialog.SetCancelable(false);
      dialog.Show();

      try {
        _viewModel.FirstName = _firstName.Text;
        _viewModel.LastName = _lastName.Text;
        _viewModel.Address1 = _address1.Text;
        _viewModel.Address2 = _address2.Text;
        _viewModel.City = _city.Text;
        _viewModel.State = _state.Text;
        _viewModel.Country = _country.Text;
        
        await _viewModel.SaveUserAsync();
        Finish();
      } catch (IOException e) {
        Log.Error(TAG, "Failed to create user");
        Toast.MakeText(this, Resource.String.error_failed_to_save_user, ToastLength.Long).Show();
      } finally {
        dialog.Dismiss();
      }
    }

    private void Populate(User user) {
      _userName.Text = user.UserName;
      _firstName.Text = user.FirstName;
      _lastName.Text = user.LastName;
      _address1.Text = user.Address1;
      _address2.Text = user.Address2;
      _city.Text = user.City;
      _state.Text = user.State;
      _country.Text = user.Country;
    }
  }
}