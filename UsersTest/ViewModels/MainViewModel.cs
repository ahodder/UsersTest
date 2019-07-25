using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Arch.Lifecycle;
using UsersTest.Models;
using UsersTest.Serialization;

namespace UsersTest.ViewModels {
  public class MainViewModel : ViewModel {
    private readonly IUserSerializer _userSerializer;

    public MainViewModel(IUserSerializer userSerializer) {
      _userSerializer = userSerializer;
    }

    /// <summary>
    /// Queries all available users.
    /// </summary>
    /// <returns></returns>
    public async Task<List<User>> GetAllUsers() {
      return await _userSerializer.ReadAllAsync();
    }

    /// <summary>
    /// Saves the given user. Used in undo-ing a deleted user.
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="IOException"></exception>
    public async Task SaveUserAsync(User user) {
      await _userSerializer.SaveAsync(user);
    }

    /// <summary>
    /// Deletes the given user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task DeleteUserAsync(User user) {
      await _userSerializer.DeleteAsync(user);
    }
  }
}