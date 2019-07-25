using System;

namespace UsersTest.ViewModels {
  public class PasswordMatchEventArgs : EventArgs {
    public bool PasswordsMatch { get; }
    public bool PasswordValid { get; }

    public PasswordMatchEventArgs(bool passwordsMatch, bool passwordValid) {
      PasswordsMatch = passwordsMatch;
      PasswordValid = passwordValid;
    }
  }
}