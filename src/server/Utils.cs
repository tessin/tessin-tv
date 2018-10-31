using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
  static class Utils
  {
    public static Uri ParseUri(string url)
    {
      Uri uri;
      if (Uri.TryCreate(url, UriKind.Absolute, out uri))
      {
        return uri;
      }
      return null;
    }
  }
}