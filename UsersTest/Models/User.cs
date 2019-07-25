using System;

namespace UsersTest.Models {
  public class User {
    public int Id { get; set; }
    public string UserName { get; set; }
    /// <summary>
    /// The Base64 string of the hashed password.
    /// </summary>
    public string HashedPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
  }
}