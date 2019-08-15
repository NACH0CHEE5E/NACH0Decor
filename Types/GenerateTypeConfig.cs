
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
        public static string GAME_SAVES = @"";
        public static string GAME_SAVEFILE = @"";
        public static string GAME_ROOT = @"";
        public static string MOD_FOLDER = @"gamedata/mods/NACH0/Decor";
        public static string MOD_MESH_PATH = "./meshes";
        public static string MOD_ICON_PATH = "./textures/icons";

        public const string TYPEPREFIX = NAME + ".Types.";

        public const string MESHTYPE = ".obj";
        public const string ICONTYPE = ".png";

        public static bool DecorConfigFileTrue = true;
        public static bool NewDecorConfigFileTrue = false;

        public const string FILE_NAME = "DecorTypes.json";
        public const string EXAMPLE_FILE_NAME = "Example" + FILE_NAME;
        public static Dictionary<string, List<DecorType>> DecorTypes { get; private set; }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, MODNAMESPACE + ".OnAssemblyLoaded")]
        public static void OnAssemblyLoaded(string path)
        {
            MOD_FOLDER = Path.GetDirectoryName(path) + "/";

            GAME_ROOT = path.Substring(0, path.IndexOf("gamedata")).Replace("\\", "/") + "/";
            GAMEDATA_FOLDER = path.Substring(0, path.IndexOf("gamedata") + "gamedata".Length).Replace("\\", "/") + "/";
            GAME_SAVES = GAMEDATA_FOLDER + "savegames/";

            MOD_MESH_PATH = MOD_FOLDER + "gamedata/meshes/";
            MOD_ICON_PATH = MOD_FOLDER + "gamedata/textures/icons/";

            DecorLogger.LogToFile("Mod Instalation Directory: " + MOD_FOLDER);
            DecorLogger.LogToFile("Mod Mesh Directory: " + MOD_MESH_PATH);
            DecorLogger.LogToFile("Mod Icon Directory: " + MOD_ICON_PATH);
            DecorLogger.LogToFile("Game Root Directory: " + GAME_ROOT);
            DecorLogger.LogToFile("Game Gamedata Directory: " + GAMEDATA_FOLDER);
            DecorLogger.LogToFile("Game Savegame Directory: " + GAME_SAVES);

            //if (!Directory.Exists()
            //var file = File.ReadAllText(MOD_FOLDER + "DecorTypes.json");
            //DecorTypes = JsonConvert.DeserializeObject<Dictionary<string, List<DecorType>>>(file);



        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, MODNAMESPACE + ".AfterSelectedWorld")]
        public static void AfterSelectedWorld()
        {
            GAME_SAVEFILE = GAME_SAVES + ServerManager.WorldName + "/";
            var EXAMPLE_FILE = MOD_FOLDER + EXAMPLE_FILE_NAME;
            var NEW_FILE = GAME_SAVEFILE + FILE_NAME;
            DecorLogger.LogToFile("Game SaveFile Directory: " + GAME_SAVEFILE);

            if (!File.Exists(NEW_FILE))
            {
                DecorLogger.LogToFile("Decor Config file not found for save game " + ServerManager.WorldName + " named " + FILE_NAME);
                if (File.Exists(EXAMPLE_FILE))
                {
                    DecorLogger.LogToFile("Example Decor File Found");
                    File.Copy(EXAMPLE_FILE, NEW_FILE);
                    DecorLogger.LogToFile("Example File Coppied to savegame");
                    NewDecorConfigFileTrue = true;
                }
                else
                {
                    DecorLogger.LogToFile("Example Decor File not found can not copy file");
                    DecorConfigFileTrue = false;
                    return;
                }
            }
            DecorLogger.LogToFile("Decor Config file found for save game " + ServerManager.WorldName + " named " + FILE_NAME);
            var fileContents = File.ReadAllText(NEW_FILE);
            DecorTypes = JsonConvert.DeserializeObject<Dictionary<string, List<DecorType>>>(fileContents);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt")))
            {
                outputFile.WriteLine("");
            }
        }
    }
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
        [ChatCommandAutoLoader]
        public class Command : IChatCommand
        {
            public bool TryDoCommand(Players.Player player, string chat, List<string> splits)
            {
            if (player == null)
            {
                return false;
            }

            if (!chat.StartsWith("?decoradd"))
            {
                return false;
            }
             if (!PermissionsManager.CheckAndWarnPermission(player, "Nach0.decoradd"))
            {
                return false;
            }
                return true;
            }
        }
}
