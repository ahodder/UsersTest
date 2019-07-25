using System;
using System.IO;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Util;
using Android.Views;
using Android.Widget;
using UsersTest.Adapters;
using UsersTest.App;
using UsersTest.Models;
using UsersTest.ViewModels;
using UsersTest.ViewModels.Factories;

namespace UsersTest {
  [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
  public class MainActivity : AppCompatActivity, UserAdapter.IOnUserInteraction {
    private const string TAG = nameof(MainActivity);

    private MainViewModel _viewModel;
    private ProgressBar _progressBar;
    private UserAdapter _adapter;
    private ItemTouchHelper _itemTouchHelper;
    private UserAdapter.SwipeToDelete _swipeToDelete;
    private CoordinatorLayout _coordinator;
    private RecyclerView _list;

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      Xamarin.Essentials.Platform.Init(this, savedInstanceState);
      SetContentView(Resource.Layout.activity_main);
      
      Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
      SetSupportActionBar(toolbar);
      
      _viewModel = ViewModelProviders.Of(this, ViewModelFactory.Instance)
        .Get(Java.Lang.Class.FromType(typeof(MainViewModel))) as MainViewModel;

      _coordinator = FindViewById<CoordinatorLayout>(Resource.Id.coordinator);
      
      _list = FindViewById<RecyclerView>(Resource.Id.recycler_view);
      _list.HasFixedSize = true;
      _list.SetLayoutManager(new LinearLayoutManager(this));

      _progressBar = FindViewById<ProgressBar>(Resource.Id.progress_bar);

      FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
      fab.Click += FabOnClick;
    }

    public override void OnRequestPermissionsResult(int requestCode, string [] permissions, [GeneratedEnum] Android.Content.PM.Permission [] grantResults) {
      Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnResume() {
      base.OnResume();
      RefreshUsers();
    }

    protected override void OnPause() {
      base.OnPause();
      _swipeToDelete?.Release();
      _itemTouchHelper?.AttachToRecyclerView(null);
    }

    /// <summary>
    /// Refreshes the content of the users adapter.
    /// </summary>f
    public async void RefreshUsers() {
      /* todo ahodder@praethos.com 2019-07-24: We can be more clever about this and dynamically insert users that don't already exist in the adapter */
      try {
        Window.AddFlags(WindowManagerFlags.NotTouchable);
        _progressBar.Visibility = ViewStates.Visible;
        var users = await _viewModel.GetAllUsers();
        
        /* todo ahodder@praethos.com 2019-07-24: Extract into new method */
        // Release the old instances
        _swipeToDelete?.Release();
        _itemTouchHelper?.AttachToRecyclerView(null);
        
        _adapter = new UserAdapter(this, users, this);
        _itemTouchHelper = new ItemTouchHelper(_swipeToDelete = new UserAdapter.SwipeToDelete(_adapter));
        
        _list.SetAdapter(_adapter);
        _itemTouchHelper.AttachToRecyclerView(_list);
      } catch (IOException e) {
        Log.Error(TAG, "Failed to load users");
        Toast.MakeText(this, GetString(Resource.String.error_failed_to_load_users), ToastLength.Short).Show();
      } finally {
        _progressBar.Visibility = ViewStates.Gone;
        Window.ClearFlags(WindowManagerFlags.NotTouchable);
      }
    }

    public void NavigateToUserDetails(User user) {
      var intent = new Intent(Intent.ActionEdit);
      intent.SetData(AppConsts.UserUri(user));
      StartActivity(intent);
    }
    
    private void FabOnClick(object sender, EventArgs eventArgs) {
      /* todo ahodder@praethos.com 2019-07-23: We would want to fire off an intent with a user uri if we wanted a more versatile */
      StartActivity(typeof(CreateUserActivity));
    }

    public void UserClicked(int position, User user) {
      NavigateToUserDetails(_adapter[position]);
    }

    public async void UserDeleted(int position, User user) {
      try {
        await _viewModel.DeleteUserAsync(user);
        var snack = Snackbar.Make(_coordinator, Resource.String.deleted_user, Snackbar.LengthLong);
        snack.SetAction(GetString(Resource.String.undo_delete), async (v) => {
          try {
            await _viewModel.SaveUserAsync(user);
            _adapter.InsertItem(position, user);
          } catch (IOException e) {
            Log.Error(TAG, "Failed to undo delete");
            Toast.MakeText(this, Resource.String.error_failed_to_undo_delete, ToastLength.Long).Show();
          }
        });
        snack.Show();
      } catch (IOException e) {
        Log.Error(TAG, "Failed to delete user");
        _adapter.InsertItem(position, user);
        Toast.MakeText(this, Resource.String.error_failed_to_delete_user, ToastLength.Long).Show();
      }
    }
  }
}

