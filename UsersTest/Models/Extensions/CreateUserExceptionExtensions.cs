using Android.Content;
using UsersTest.Models.Exceptions;

namespace UsersTest.Models.Extensions {
  public static class CreateUserExceptionExtensions {
    /// <summary>
    /// Provides localized string representations for the given exception reason.
    /// </summary>
    /// <param name="userCreateError"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string ToLocalizedString(this EUserCreateError userCreateError, Context context) {
      switch (userCreateError) {
        case EUserCreateError.NoUserName:
          return context.GetString(Resource.String.error_user_create_exception_no_user_name);
        
        case EUserCreateError.InvalidPassword:
          return context.GetString(Resource.String.error_user_create_exception_password_invalid);
        
        case EUserCreateError.PasswordsDoNotMatch:
          return context.GetString(Resource.String.error_user_create_exception_passwords_do_not_match);
        
        case EUserCreateError.UserAlreadyExists:
          return context.GetString(Resource.String.error_user_create_exception_already_exists);

        case EUserCreateError.Unknown: // Fall through
        default:
          return context.GetString(Resource.String.error_user_create_exception_unknown);
      }
    }
  }
}