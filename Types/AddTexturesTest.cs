
using Newtonsoft.Json;
using Pipliz;
using Pipliz.JSON;
using Recipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Pandaros.API.Models;
using Pandaros.API;
using Chatting;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Nach0.Decor
{
    public class TextureBase : CSTextureMapping
    {
        public override string name { get; }
        public override string albedo { get; }
    }
    [ModLoader.ModManager]
    public class generateTextures
    {
        public static List<string> TextureList = new List<string>();

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterStartup, GenerateTypeConfig.MODNAMESPACE + "GenerateRecipes")]
        public static void AddTextureMappingEntries()
        {
            string[] files = Directory.GetFiles(GenerateTypeConfig.MOD_CUSTOM_TEXTURE_PATH);
            foreach (string file in files)
            {
                if (file.EndsWith(GenerateTypeConfig.ICONTYPE))
                {
                    TextureList.Add(file);
                    var texture = file.Replace(GenerateTypeConfig.ICONTYPE, "");
                    texture = texture.Replace(GenerateTypeConfig.MOD_CUSTOM_TEXTURE_PATH, "");
                    DecorLogger.LogToFile("Adding Texture: \"" + texture + "\" to TextureList");
                }
            }

            foreach (string texture in TextureList)
            {
                var newTexture = new TextureBase();
                newTexture.name = "Nach0Decor." + texture;

                    }
        }
    }
}
