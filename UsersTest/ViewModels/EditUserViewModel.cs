using System.Threading.Tasks;
using Android.Arch.Lifecycle;
using UsersTest.Models;
using UsersTest.Serialization;

namespace UsersTest.ViewModels {
  public class EditUserViewModel : ViewModel {
    public User User { get; private set; } 
    
    public string FirstName {
      get => User.FirstName;
      set => User.FirstName = value;
    }

    public string LastName {
      get => User.LastName;
      set => User.LastName = value;
    }

    public string Address1 {
      get => User.Address1;
      set => User.Address1 = value;
    }

    public string Address2 {
      get => User.Address2;
      set => User.Address2 = value;
    }

    public string City {
      get => User.City;
      set => User.City = value;
    }
    public string State {
      get => User.State;
      set => User.State = value;
    }
    
    public string Country {
      get => User.Country;
      set => User.Country = value;
    }

    private IUserSerializer _userSerializer;

    public EditUserViewModel(IUserSerializer userSerializer) {
      _userSerializer = userSerializer;
    }

    /// <summary>
    /// Saves the user.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException"></exception>
    public async Task SaveUserAsync() {
      await _userSerializer.SaveAsync(User);
    }

    public async Task LoadUserAsync(uint id) {
      User = await _userSerializer.ReadAsync(id);
    }
  }
}