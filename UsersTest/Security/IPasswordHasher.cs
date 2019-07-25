namespace UsersTest.Security {
  /// <summary>
  /// This contract provides a simple way of hashing password into a safe storage medium.
  /// </summary>
  /// <code>
  /// // Simple use as follows.
  ///
  /// IPasswordHasher hasher = ...
  /// string userEnteredPassword = ...
  /// string hashedPassword = hasher.HashPassword(userEnteredPassword);
  ///
  /// // Some time later, you will need to compare the passwords
  /// string userEnteredPassword = ...
  /// if (hasher.VerifyPassword(userEnteredPassword, hashedPassword)) {
  ///   // Perform accept logic
  /// } else {
  ///   // Perform reject logic
  /// }
  /// </code>
  public interface IPasswordHasher {
    /// <summary>
    /// Hash the given password into a string.
    /// </summary>
    /// <param name="unhashedPassword">The password that will be hashed.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">If the password hashing fails for any reason.</exception>
    string HashPassword(string unhashedPassword);
    
    /// <summary>
    /// Compares the unhashed password to the hashed password using the backing security algorithm.
    /// </summary>
    /// <param name="unhashedPassword">The unsafe provided password</param>
    /// <param name="hashedPassword">The password to compare against.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">If the baking algorithm fails to compare the password. This can occur if the
    /// backing algorithm changed.</exception>
    bool Verify(string unhashedPassword, string hashedPassword);
  }
}