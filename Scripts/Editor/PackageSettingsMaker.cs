using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ICKX
{
    [InitializeOnLoad]
    public static class PackageSettingsMaker
    {
        static PackageSettingsMaker()
        {
			EditorApplication.delayCall += () =>
			{
				if (Application.isPlaying) return;

				Dictionary<string, string> classNameFilePathTable = new Dictionary<string, string>();

				string[] guids = AssetDatabase.FindAssets("AutoCreatePackageSettings");
				foreach (var guid in guids)
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					if (string.IsNullOrEmpty(path)) continue;

					TextAsset text = AssetDatabase.LoadMainAssetAtPath(path) as TextAsset;
					if (text == null) continue;
					if (System.IO.Path.GetExtension(path) == ".cs") continue;

					foreach (var line in text.text.Split('\n'))
					{
						string[] data = line.Split(':');
						if (data.Length != 2) continue;
						string typeName = data[0];
						if (!classNameFilePathTable.ContainsKey(data[0]))
						{
							classNameFilePathTable[data[0]] = data[1];
						}
						else
						{
							Debug.LogError("同名のClassが設定されています : " + data[0]);
						}
					}
				}

                AssetDatabase.StartAssetEditing();
                foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!type.IsSubclassOf(typeof(ScriptableObject))) continue;

                        //create from attribute
                        foreach (var attribute in System.Attribute.GetCustomAttributes(type))
                        {
                            if (attribute is AutoCreatePackageSettingsAttribute)
                            {
                                var settingsAttribute = attribute as AutoCreatePackageSettingsAttribute;
                                if (settingsAttribute == null) continue;
                                CreatePackageSettings(type, settingsAttribute.FilePath);
                            }
                        }
                        
                        //create from text
                        if (classNameFilePathTable.TryGetValue(type.FullName, out string filePath))
                        {
                            CreatePackageSettings(type, filePath);
                        }
                    }
                }
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            };
        }

        private static void CreatePackageSettings(System.Type type, string filePath)
        {
            if (!type.IsSubclassOf(typeof(ScriptableObject))) return;

            if(!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = $"Assets/Resources/{type.Name}.asset";
            }

            if (System.IO.File.Exists(filePath)) return;

            var asset = ScriptableObject.CreateInstance(type);
            if (asset == null) return;

            Debug.Log("AutoCreatePackageSettings : " + filePath + " : " + asset.GetType());

            AssetDatabase.CreateAsset(asset, filePath);
        }
    }
}


