using System;
using Android.Arch.Lifecycle;
using Java.Lang;
using UsersTest.Security.Bcrypt;
using UsersTest.Serialization.Sqlite;
using Object = Java.Lang.Object;

namespace UsersTest.ViewModels.Factories {
  /// <summary>
  /// The Factory responsible for creating ViewModels. This factory should read from the DI system to retrieve
  /// the necessary dependencies for each created view model.
  /// </summary>
  public class ViewModelFactory : Java.Lang.Object, ViewModelProvider.IFactory {
    private static ViewModelFactory _INSTANCE;
    public static ViewModelFactory Instance => _INSTANCE ?? (_INSTANCE = new ViewModelFactory());
    
    public Object Create(Class p0) {
      /* todo ahodder@praethos.com 2019-07-24: We need to bind to a DI system. */
      // This will work for now, but we will need to bind to some DI system to make the factory more robust.
      
      if (p0.IsAssignableFrom(Class.FromType(typeof(MainViewModel)))) {
        var userSerializer = SqliteUserSerializer.Get();
        return new MainViewModel(userSerializer);
      } else if (p0.IsAssignableFrom(Class.FromType(typeof(CreateUserViewModel)))) {
        var userSerializer = SqliteUserSerializer.Get();
        var passwordHasher = new BCryptPasswordHasher();
        return new CreateUserViewModel(userSerializer, passwordHasher);
      } else if (p0.IsAssignableFrom(Class.FromType(typeof(EditUserViewModel)))) {
        var userSerializer = SqliteUserSerializer.Get();
        return new EditUserViewModel(userSerializer);
      } else {
        throw new ArgumentException($"Invalid view model type: {p0.Name}");
      }
    }
  }
}