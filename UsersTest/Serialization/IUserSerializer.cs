using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UsersTest.Models;

namespace UsersTest.Serialization {
  /// <summary>
  /// The contract for reading / writing users to a backing data store.
  /// </summary>
  public interface IUserSerializer {
    
    /// <summary>
    /// Determines whether or not the serializer contains a user with the given username.
    /// </summary>
    /// <param name="userName">The name to compare</param>
    /// <returns>True if the user exists.</returns>
    /// <exception cref="IOException">If the serializer fails to perform the query.</exception>
    Task<bool> ContainsUserAsync(string userName);

    /// <summary>
    /// Reads all users from the backing data store.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="IOException">If serializer fails to read any users from the data store.</exception>
    Task<List<User>> ReadAllAsync();

    /// <summary>
    /// Reads a specific user from the backing store.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="IOException">If the serializer fails to read the particular user</exception>
    Task<User> ReadAsync(uint id);
    
    /// <summary>
    /// Saves the user to the backing data store.
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="ArgumentException">If the provided user is null.</exception>
    /// <exception cref="IOException">If serializer fails to save.</exception>
    Task SaveAsync(User user);
    
    /// <summary>
    /// Deletes the user from the backing data store.
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="ArgumentException">If the provided user is null.</exception>
    /// <exception cref="IOException">If serializer fails to delete.</exception>
    Task DeleteAsync(User user);
  }
}