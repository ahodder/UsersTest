using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using UsersTest.Models;

namespace UsersTest.Serialization.Sqlite {
  /// <summary>
  /// A SQLite implementation of user serializer. This will store all of the user's in a table using the user's Id is
  /// the primary key.
  /// </summary>
  public class SqliteUserSerializer : IUserSerializer {
    private const string DEFAULT_DATABASE_NAME = "user_database.sqlite"; 
      
    private static SQLiteAsyncConnection _CONNECTION_INSTANCE;
    
    private SQLiteAsyncConnection _database;

    public SqliteUserSerializer(SQLiteAsyncConnection database) {
      _database = database;
      _database.CreateTableAsync<SqliteUserTable>();
    }

    public async Task<bool> ContainsUserAsync(string userName) {
      var count = await _database.Table<SqliteUserTable>().Where(u => u.UserName == userName).CountAsync();
      return count > 0;
    }

    public async Task<List<User>> ReadAllAsync() {
      try {
        var users = await _database.Table<SqliteUserTable>().ToListAsync();

        var ret = new List<User>();
        foreach (var user in users) {
          ret.Add(user.ToUser());
        }

        return ret;
      } catch (SQLiteException e) {
        throw new IOException("Failed to query for all users", e);
      }
    }

    public async Task<User> ReadAsync(uint id) {
      try {
        var cnt = await _database.Table<SqliteUserTable>().CountAsync();
        var users = await _database.Table<SqliteUserTable>().Where(c => c.Id == id).ToListAsync();
        var user = users.FirstOrDefault();
        if (user == null) {
          throw new IOException("User doesn't exist");
        }

        return user.ToUser();
      } catch (SQLiteException e) {
        throw new IOException("Failed to read user for id", e);
      }
    }

    public async Task SaveAsync(User user) {
      try {
        var sql = new SqliteUserTable(user);
        var cnt = await _database.Table<SqliteUserTable>().CountAsync(c => c.Id == user.Id);
        if (cnt == 0) {
          var inserted = await _database.InsertAsync(sql);
          user.Id = sql.Id;
        } else {
          var updated = await _database.UpdateAsync(sql);
        }
      } catch (SQLiteException e) {
        throw new IOException("Failed to save user", e);
      }
    }

    public async Task DeleteAsync(User user) {
      try {
        if (user.Id != 0) {
          var deleted = await _database.DeleteAsync(new SqliteUserTable(user));
          /* todo ahodder@praethos.com 2019-07-24: Assert that deleted deleted exactly one item. */
        }
      } catch (SQLiteException e) {
        throw new IOException("Failed to delete user", e);
      }
    }

    /// <summary>
    /// Creates a new SqliteUserSerializer at the given path.
    /// </summary>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    /// <exception cref="IOException">If the serializer cannot be created.</exception>
    public static SqliteUserSerializer Get(string databaseName = DEFAULT_DATABASE_NAME) {
      try {
        if (_CONNECTION_INSTANCE == null) {
          var dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
          var path = Path.Combine(dir, databaseName);

          _CONNECTION_INSTANCE = new SQLiteAsyncConnection(path, false);
        }

        return new SqliteUserSerializer(_CONNECTION_INSTANCE);
      } catch (SQLiteException sql) {
        throw new IOException("Failed to create serializer due to sqlite exception", sql);
      } catch (Exception e) {
        throw new IOException("Failed to create serializer", e);
      }
    }

    [Table("Users")]
    private class SqliteUserTable {
      [PrimaryKey, AutoIncrement, Column("_id")]
      public int Id { get; set; }
      [Indexed]
      public string UserName { get; set; }
      public string HashedPassword { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
    
      public string Address1 { get; set; }
      public string Address2 { get; set; }
      public string City { get; set; }
      public string State { get; set; }
      public string Country { get; set; }
    
      public SqliteUserTable() {
      }

      public SqliteUserTable(User user) {
        Id = user.Id;
        UserName = user.UserName;
        HashedPassword = user.HashedPassword;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Address1 = user.Address1;
        Address2 = user.Address2;
        City = user.City;
        State = user.State;
        Country = user.Country;
      }

      public User ToUser() {
        var user = new User();
        user.Id = Id;
        user.UserName = UserName;
        user.HashedPassword = HashedPassword;
        user.FirstName = FirstName;
        user.LastName = LastName;
        user.Address1 = Address1;
        user.Address2 = Address2;
        user.City = City;
        user.State = State;
        user.Country = Country;
        return user;
      }
    }
  }
}
