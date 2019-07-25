using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Arch.Lifecycle;
using UsersTest.Models;
using UsersTest.Models.Exceptions;
using UsersTest.Security;
using UsersTest.Serialization;

namespace UsersTest.ViewModels {
  /// <summary>
  /// This view model is responsible for maintaining the state of a create user session. The view model is operated by
  /// providing updates to UserName, Password and PasswordVerify. As changes come in, the view model will indicate whether
  /// or not the a user can be created. 
  /// </summary>
  public class CreateUserViewModel : ViewModel {
    /// <summary>
    /// The min and max password lengths.
    /// </summary>
    private const int MIN_PLEN = 5, MAX_PLEN = 12;
    
    /// <summary>
    /// The event handler that fires off when either the password or the password verify change.
    /// </summary>
    public event EventHandler<PasswordMatchEventArgs> OnPasswordVerifyChanged;
    
    /// <summary>
    /// The user provided name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// The first entered password. 
    /// </summary>
    public string Password {
      get => _password;
      set {
        _password = value;
        NotifyPasswordEvent();
      }
    }
    
    /// <summary>
    /// The doubly entered password.
    /// </summary>
    public string PasswordVerify {
      get => _passwordVerify;
      set {
        _passwordVerify = value;
        NotifyPasswordEvent();
      }
    }

    /// <summary>
    /// Determines whether or not the user name is valid. We didn't have any specific rules for this, so I made some up.
    /// </summary>
    public bool IsUserNameValid => !string.IsNullOrEmpty(UserName) && Regex.IsMatch(UserName, "^[a-zA-Z0-9]{3,35}$");
    
    /// <summary>
    /// Whether or not Password is equal to Password verify.
    /// </summary>
    public bool PasswordsMatch => string.Equals(Password, PasswordVerify);

    /// <summary>
    /// Compares the Password with the PasswordVerify to ensure that the passwords are similar.
    /// </summary>
    public bool PasswordValid {
      get {
        if (Password == null || !PasswordsMatch ) return false;
        return Password != null && PasswordsMatch && Password.Length >= MIN_PLEN && Password.Length <= MAX_PLEN &&
               Regex.IsMatch(Password, "\\d") && 
               Regex.IsMatch(Password, "[a-zA-Z]") && 
               !Regex.IsMatch(Password, "([a-zA-Z0-9]+)\\1"); 
      }
    }

    private string _password;
    private string _passwordVerify;

    private readonly IUserSerializer _userSerializer;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserViewModel(IUserSerializer serializer, IPasswordHasher passwordHasher) {
      _userSerializer = serializer;
      _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Attempts to create and return the user. If the user cannot be created, we will throw a UserCreateException.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserCreateException">Documents the reason the user could not be created.</exception>
    public async Task<User> CreateUserAsync() {
      if (string.IsNullOrEmpty(UserName)) {
        throw new UserCreateException(EUserCreateError.NoUserName);
      } else if (await _userSerializer.ContainsUserAsync(UserName)) {
        throw new UserCreateException(EUserCreateError.UserAlreadyExists);
      } else if (!PasswordValid) {
        throw new UserCreateException(EUserCreateError.InvalidPassword);
      }

      try {
        var user = new User() {
          UserName = UserName,
          HashedPassword = _passwordHasher.HashPassword(Password),
        };
        
        await _userSerializer.SaveAsync(user);
        
        return user;
      } catch (Exception e) {
        throw new UserCreateException(e);
      }
    }

    private void NotifyPasswordEvent() {
      OnPasswordVerifyChanged?.Invoke(this, new PasswordMatchEventArgs(PasswordsMatch, PasswordValid));
    }
  }
}