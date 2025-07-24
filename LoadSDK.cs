using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using UnityEditor.PackageManager;

public static class LoadSDK
{
    public class PackageList : Dictionary<string, string> { }

    public static PackageList SDKList = new()
    {
        { "li.lightingtools.lightmapswitcher", "https://github.com/arteranos/lightmap-switching-tool.git" },
        { "com.willneedit.ipfs.iface", "https://github.com/arteranos/Arteranos-IPFS-interface.git" },
        { "com.willneedit.arteranos.common", "https://github.com/arteranos/com.willneedit.arteranos.common.git" },
        { "com.willneedit.arteranos.sdk", "https://github.com/arteranos/com.willneedit.arteranos.sdk.git" }
    };

    // [MenuItem("Tools/Load Arteranos SDK")]
    [InitializeOnLoadMethod]
    public static void Loader()
    {
        bool modified = false;

        string json = File.ReadAllText("Packages/manifest.json");
        string c = ParseManifestString(json);
        PackageList manifest = ParseManifestLines(c);

        foreach (KeyValuePair<string, string> entry in SDKList)
        {
            if (!manifest.ContainsKey(entry.Key))
            {
                manifest.Add(entry.Key, entry.Value);
                modified = true;
            }
        }

        if (modified)
        {
            json = EmitManifest(manifest);
            File.WriteAllText("Packages/manifest.json", json);
            Debug.Log("Written manifest - wait for package loading...");
            Client.Resolve();
            Debug.Log("...Package loading done.");

            // ... and, clean up behind yourself.
            Directory.Delete("Assets/LoadSDK", true);
            File.Delete("Assets/LoadSDk.meta");
        }
    }


    // Minimalistic JSON parser - Unity's inbuilt JSON parser cannot handle dictionaries!
    private static string ParseManifestString(string json)
    {
        json = json.Replace('\n', ' ');
        string pat = "{\\s+\"dependencies\":\\s+{\\s+(.+?)\\s+}\\s+}";

        Regex r = new(pat);
        Match m = r.Match(json);
        Group g = m.Groups[1];
        string c = g.Captures[0].ToString();
        return c;
    }

    private static PackageList ParseManifestLines(string jsonValue)
    {
        PackageList result = new();

        string pat = "\"([^\"]*?)\": \"([^\"]*?)\"";
        Regex r = new(pat);
        Match m = r.Match(jsonValue);
        while (m.Success)
        {
            result[m.Groups[1].ToString()] = m.Groups[2].ToString();
            m = m.NextMatch();
        }
        return result;
    }

    private static string EmitManifest(PackageList manifest)
    {
        StringBuilder sb = new();

        sb.Append("{\n  \"dependencies\": {\n");

        List<string> entryLines = new();
        foreach (KeyValuePair<string, string> entry in manifest)
            entryLines.Add($"    \"{entry.Key}\": \"{entry.Value}\"");

        sb.Append(string.Join(",\n", entryLines));
        sb.Append("\n");

        sb.Append("  }\n}\n");

        return sb.ToString();
    }
}
