using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//This entire piece of code was ripped pretty much wholesale from the mod Pearlcat
namespace SlugTemplate
{
        public static class AssetLoader
        {
            public static Dictionary<string, Texture2D> Textures { get; } = new Dictionary<string, Texture2D>();

          
            public static string GetUniqueName(string name)
            {
                return "snowcat_" + name;
            }

     
            public static FAtlas GetAtlas(string atlasName)
            {
                string uniqueName = AssetLoader.GetUniqueName(atlasName);
                if (Futile.atlasManager.DoesContainAtlas(uniqueName))
                {
                    return Futile.atlasManager.LoadAtlas(uniqueName);
                }
                string atlasDirName = "snowcat_atlases" + Path.AltDirectorySeparatorChar.ToString() + "snowcat_" + atlasName;
                if (!Futile.atlasManager.DoesContainAtlas(atlasDirName))
                {
                    UnityEngine.Debug.LogError("Atlas not found! (" + uniqueName + ")");
                    return null;
                }
                return Futile.atlasManager.LoadAtlas(atlasDirName);
            }

            public static Texture2D GetTexture(string textureName)
            {
                if (!AssetLoader.Textures.ContainsKey(textureName))
                {
                    return null;
                }
                Texture2D originalTexture = AssetLoader.Textures[textureName];
                Texture2D copiedTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
                Graphics.CopyTexture(originalTexture, copiedTexture);
                return copiedTexture;
            }

            public static void LoadAssets()
            {
                AssetLoader.LoadAtlases();
            //AssetLoader.LoadSprites();
            //AssetLoader.LoadTextures();
            Debug.Log("LoadAssets called");
        }

            public static void LoadAtlases()
            {
                foreach (string filePath in AssetManager.ListDirectory("snowcat_atlases", false, false))
                {
                    if (!(Path.GetExtension(filePath) != ".txt"))
                    {
                        string atlasName = Path.GetFileNameWithoutExtension(filePath);
                        Futile.atlasManager.LoadAtlas("snowcat_atlases" + Path.AltDirectorySeparatorChar.ToString() + atlasName);
                    }
                }
            }

        /*    public static void LoadSprites()
            {
                foreach (string filePath in AssetManager.ListDirectory("snowcat_sprites", false, false))
                {
                    if (!(Path.GetExtension(filePath).ToLower() != ".png"))
                    {
                        string atlasName = Path.GetFileNameWithoutExtension(filePath);
                        Texture2D texture = AssetLoader.FileToTexture2D(filePath);
                        if (!(texture == null))
                        {
                            Futile.atlasManager.LoadAtlasFromTexture(atlasName, texture, false);
                        }
                    }
                }
            }*/

         /*   public static void LoadTextures()
            {
                foreach (string filePath in AssetManager.ListDirectory("snowcat_textures", false, false))
                {
                    if (!(Path.GetExtension(filePath).ToLower() != ".png"))
                    {
                        string textureName = Path.GetFileNameWithoutExtension(filePath);
                        Texture2D texture = AssetLoader.FileToTexture2D(filePath);
                        if (!(texture == null))
                        {
                            AssetLoader.Textures.Add(textureName, texture);
                        }
                    }
                }
            }

            public static Texture2D FileToTexture2D(string filePath)
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, false)
                {
                    anisoLevel = 0,
                    filterMode = 0
                };
                //image conversion is a default method in unity, this should work
                if (!ImageConversion.LoadImage(texture, fileData))
                {
                    return null;
                }
                return texture;
            }*/

            public const string ATLASES_DIRPATH = "snowcat_atlases";

            public const string SPRITES_DIRPATH = "snowcat_sprites";
 
            public const string TEXTURES_DIRPATH = "snowcat_textures";

            public const TextureFormat TEXTURE_FORMAT = TextureFormat.RGBA32;
        }
}
