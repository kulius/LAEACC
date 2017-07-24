using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;

namespace FastReport.Web
{
  public static class WebUtils
  {
    /// <summary>
    /// Determines whether the path is an absolute physical path.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns><b>true</b> if the path is absolute physical path.</returns>
    public static bool IsAbsolutePhysicalPath(string path)
    {
      if ((path == null) || (path.Length < 3))
      {
        return false;
      }
      return (path.StartsWith(@"\\", StringComparison.Ordinal) || ((char.IsLetter(path[0]) && (path[1] == ':')) && (path[2] == '\\')));
    }

  }
}
