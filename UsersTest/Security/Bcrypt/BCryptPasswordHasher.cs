using System;
using BCrypt.Net;
using Crypt = BCrypt.Net.BCrypt;

namespace UsersTest.Security.Bcrypt {
  /// <summary>
  /// A password hasher that uses BCrypt as the workhorse for performing password encryption.
  /// </summary>
  public class BCryptPasswordHasher : IPasswordHasher {
    /// <summary>
    /// This constant represents how long BCrypt will "spend" generating salt and password hashes.
    /// </summary>
    private const int WORK_FACTOR = 4;
    private const bool ENHANCED_ENTROPY = false;
    private const HashType HASH_TYPE = HashType.SHA256;
    
    public string HashPassword(string unhashedPassword) {
      try {
        var salt = Crypt.GenerateSalt(WORK_FACTOR);
        var pass = Crypt.HashPassword(unhashedPassword, salt, ENHANCED_ENTROPY, HASH_TYPE);
        return pass;
      } catch (ArgumentException arg) {
        throw new Exception("Did not correctly provide BCrypt arguments", arg);
      } catch (SaltParseException se) {
        throw new Exception("Failed to parse generated salt. This should never happen", se);
      }
    }

    public bool Verify(string unhashedPassword, string hashedPassword) {
      try {
        return Crypt.Verify(unhashedPassword, hashedPassword, ENHANCED_ENTROPY, HASH_TYPE);
      } catch (ArgumentException arg) {
        throw new Exception("Did not correctly provide BCrypt arguments", arg);
      } catch (SaltParseException se) {
        throw new Exception("Failed to parse generated salt. This should never happen", se);
      }
    }
  }
}