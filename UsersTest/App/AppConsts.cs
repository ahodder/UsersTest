using System;
using UsersTest.Models;
using Uri = Android.Net.Uri;

namespace UsersTest.App {
  public static class AppConsts {
    public const string Scheme = "UsersTest";
    public const string DataTypeUser = "User";
    
    public const string KeyId = "id";

    public static Uri UserUri(User user) {
      return new Uri.Builder().Scheme(Scheme).Authority(DataTypeUser).AppendQueryParameter(KeyId, user.Id.ToString()).Build();
    }

    /// <summary>
    /// Retrieves a user id from the given uri.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException"></exception>
    public static uint GetUserId(Uri uri) {
      if (uri.Scheme != Scheme) throw new ArgumentException($"Invalid Uri scheme: {uri.Scheme}");
      if (uri.Authority != DataTypeUser) throw new ArgumentException($"Invalid Uri authority: {uri.Authority}");
      var usId = uri.GetQueryParameter(KeyId);
      if (uint.TryParse(usId, out uint ret)) {
        return ret;
      } else {
        throw new ArgumentException($"Invalid id: {usId}");
      }
    }
  }
}