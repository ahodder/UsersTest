namespace UsersTest.Models.Exceptions {
  public enum EUserCreateError {
    Unknown,
      
    NoUserName,
    InvalidPassword,
    PasswordsDoNotMatch,
    UserAlreadyExists,
  }
}