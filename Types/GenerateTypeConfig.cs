
using Newtonsoft.Json;
using Pipliz;
using Pipliz.JSON;
using Recipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Pandaros.API.Models;

namespace Nach0.Decor
{
    [ModLoader.ModManager]
    public class GenerateTypeConfig
    {
        public const string NAME = "NACH0";
        public const string MODNAME = "Decor";
        public const string MODNAMESPACE = NAME + "." + MODNAME + ".";

        public const string GENERATE_TYPES_PREFIX = MODNAMESPACE + "GenerateTypes.";
        public const string GENERATE_RECIPES_PREFIX = MODNAMESPACE + "GenerateRecipes.";

        public static string GAMEDATA_FOLDER = @"";
        public static string GAME_ROOT = @"";
        public static string MOD_FOLDER = @"gamedata/mods/NACH0/Decor";
        public static string MOD_MESH_PATH = "./meshes";
        public static string MOD_ICON_PATH = "./textures/icons";

        public const string TYPEPREFIX = NAME + ".Types.";

        public const string MESHTYPE = ".obj";
        public const string ICONTYPE = ".png";

        public static Dictionary<string, List<DecorType>> DecorTypes { get; private set; }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, MODNAMESPACE + ".OnAssemblyLoaded")]
        public static void OnAssemblyLoaded(string path)
        {
            MOD_FOLDER = Path.GetDirectoryName(path) + "/";

            GAME_ROOT = path.Substring(0, path.IndexOf("gamedata")).Replace("\\", "/") + "/";
            GAMEDATA_FOLDER = path.Substring(0, path.IndexOf("gamedata") + "gamedata".Length).Replace("\\", "/") + "/";

            MOD_MESH_PATH = MOD_FOLDER + "gamedata/meshes/";
            MOD_ICON_PATH = MOD_FOLDER + "textures/icons/";

            var file = File.ReadAllText(MOD_FOLDER + "DecorTypes.json");
            DecorTypes = JsonConvert.DeserializeObject<Dictionary<string, List<DecorType>>>(file);

            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(GenerateTypeConfig.MOD_FOLDER, "TypeList.txt")))
            {
                outputFile.WriteLine("");
            }
        }


        /*private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            ServerLog.LogAsyncMessage(new LogMessage(args.Name, UnityEngine.LogType.Log));
            try
            {

                if (args.Name.Contains("Newtonsoft.Json"))
                    return Assembly.LoadFile(MOD_FOLDER + "Newtonsoft.Json.dll");

                if (args.Name.Contains("System.Numerics"))
                    return Assembly.LoadFile(MOD_FOLDER + "System.Numerics.dll");

                if (args.Name.Contains("System.Data"))
                    return Assembly.LoadFile(MOD_FOLDER + "System.Data.dll");

                if (args.Name.Contains("System.Runtime.Serialization"))
                    return Assembly.LoadFile(MOD_FOLDER + "System.Runtime.Serialization.dll");
            }
            catch (Exception ex)
            {
                ServerLog.LogAsyncMessage(new LogMessage(ex.Message, UnityEngine.LogType.Exception));
            }

            return null;
        }*/
    }

    /*public static class ExtentionMethods
    {
        public static JSONNode JsonSerialize<T>(this T obj)
        {
            var objStr = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            //ServerLog.LogAsyncMessage(new LogMessage(objStr, UnityEngine.LogType.Log));
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
            {
                outputFile.WriteLine("JSON - " + objStr);
                outputFile.WriteLine("");
            }
            var json = JSON.DeserializeString(objStr);

            if (obj is ICSType csType)
                json.SetAs("customData", csType.customData);

            return json;
        }

        public static T JsonDeerialize<T>(this JSONNode node)
        {
            return JsonConvert.DeserializeObject<T>(node.ToString(), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        public static void LoadRecipe(this ICSRecipe recipe)
        {
            var requirements = new List<InventoryItem>();
            var results = new List<Recipes.RecipeResult>();
            recipe.JsonSerialize();

            foreach (var ri in recipe.requires)
                if (ItemTypes.IndexLookup.TryGetIndex(ri.type, out var itemIndex))
                    requirements.Add(new InventoryItem(itemIndex, ri.amount));

            foreach (var ri in recipe.results)
                if (ItemTypes.IndexLookup.TryGetIndex(ri.type, out var itemIndex))
                    results.Add(new Recipes.RecipeResult(itemIndex, ri.amount));

            var newRecipe = new Recipe(recipe.name, requirements, results, recipe.defaultLimit, 0, (int)recipe.defaultPriority);

                ServerManager.RecipeStorage.AddLimitTypeRecipe(recipe.Job, newRecipe);
        }
    }*/
    public class TypeRecipeBase : ICSRecipe
    {
        public string name { get; set; }

        public List<RecipeItem> requires { get; set; } = new List<RecipeItem>();

        public List<RecipeResult> results { get; set; } = new List<RecipeResult>();

        public CraftPriority defaultPriority { get; set; } = CraftPriority.Medium;

        public bool isOptional { get; set; } = false;

        public int defaultLimit { get; set; } = 0;

        public string Job { get; set; }
    }
}
