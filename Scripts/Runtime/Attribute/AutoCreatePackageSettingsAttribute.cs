using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICKX
{
    public class AutoCreatePackageSettingsAttribute : System.Attribute
    {
        public string FilePath;

        public AutoCreatePackageSettingsAttribute(string filePath = null)
        {
            FilePath = filePath;
        }
    }
}
