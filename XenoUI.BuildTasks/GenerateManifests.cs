using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace XenoUI.BuildTasks
{
    public class GenerateManifests : Task
    {
        [Required] public string AppId { get; set; }
        [Required] public string AppName { get; set; }
        [Required] public string IntermediatePath { get; set; } // obj/ folder
        public ITaskItem[] Permissions { get; set; }

        public override bool Execute()
        {
            // 1. Generate AndroidManifest.xml
            var androidPath = Path.Combine(IntermediatePath, "AndroidManifest.xml");
            var permString = "";
            if (Permissions != null)
                foreach (var p in Permissions)
                    permString += $"<uses-permission android:name=\"android.permission.{p.ItemSpec.ToUpper()}\" />\n";

            File.WriteAllText(androidPath, $@"<manifest xmlns:android=""http://schemas.android.com/apk/res/android"" package=""{AppId}"">
                    <application android:label=""{AppName}"">
                        {permString}
                    </application>
                </manifest>");

            // 2. Generate Info.plist (iOS)
            var iosPath = Path.Combine(IntermediatePath, "Info.plist");
            File.WriteAllText(iosPath, $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                <plist version=""1.0"">
                <dict>
                    <key>CFBundleIdentifier</key><string>{AppId}</string>
                    <key>CFBundleName</key><string>{AppName}</string>
                </dict>
                </plist>");

            return true;
        }
    }
}
