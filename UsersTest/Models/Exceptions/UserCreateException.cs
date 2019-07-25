using System;

namespace UsersTest.Models.Exceptions {
  public class UserCreateException : Exception {
    public EUserCreateError UserCreateError { get; }

    public UserCreateException(EUserCreateError userCreateError) {
      UserCreateError = userCreateError;
    }

    public UserCreateException(Exception e) : base("Unknown Exception", e) {
      UserCreateError = EUserCreateError.Unknown;
    }
  }
}