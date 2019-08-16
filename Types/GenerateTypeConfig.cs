
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

        public const string DecorJobRecipe = NAME + ".Jobs.DecorMaker";

        public const string TYPEPREFIX = NAME + ".Types.";

        public const string MESHTYPE = ".obj";
        public const string ICONTYPE = ".png";

        public static bool DecorConfigFileTrue = false;
        public static bool NewDecorConfigFileTrue = false;
        public static bool NewWorld = false;

        public const string FILE_NAME = "DecorTypes.json";
        public const string EXAMPLE_FILE_NAME = "Example" + FILE_NAME;

        public static string EXAMPLE_FILE = "";
        public static string NEW_FILE = "";
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




        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, MODNAMESPACE + ".AfterSelectedWorld")]
        public static void AfterSelectedWorld()
        {
            GAME_SAVEFILE = GAME_SAVES + ServerManager.WorldName + "/";
            EXAMPLE_FILE = MOD_FOLDER + EXAMPLE_FILE_NAME;
            NEW_FILE = GAME_SAVEFILE + FILE_NAME;
            DecorLogger.LogToFile("Game SaveFile Directory: " + GAME_SAVEFILE);
            if (!Directory.Exists(GAME_SAVEFILE))
            {
                Directory.CreateDirectory(GAME_SAVEFILE);
                //NewWorld = true;
                //return;
            }
            if (!File.Exists(NEW_FILE))
            {
                DecorLogger.LogToFile("Decor Config file not found for save game " + ServerManager.WorldName + " named " + FILE_NAME);
                if (File.Exists(EXAMPLE_FILE))
                {
                    DecorLogger.LogToFile("Example Decor File Found");
                    File.Copy(EXAMPLE_FILE, NEW_FILE);
                    DecorLogger.LogToFile("Example File Coppied to savegame");
                    //ServerLog.LogAsyncMessage(new LogMessage("<color = blue>Restart server/game for decor mod to be functional</color>", LogType.Log));
                    NewDecorConfigFileTrue = true;
                    DecorConfigFileTrue = true;
                }
                else
                {
                    DecorLogger.LogToFile("Example Decor File not found can not copy file");
                    DecorConfigFileTrue = false;
                    return;
                }
            }
            DecorConfigFileTrue = true;
            DecorLogger.LogToFile("Decor Config file found for save game " + ServerManager.WorldName + " named " + FILE_NAME);
            var fileContents = File.ReadAllText(NEW_FILE);
            DecorTypes = JsonConvert.DeserializeObject<Dictionary<string, List<DecorType>>>(fileContents);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt")))
            {
                outputFile.WriteLine("");
            }
        }
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, MODNAMESPACE + ".AfterWorldLoad")]
        public static void AfterWorldLoad()
        {
            if (NewWorld)
            {
                DecorConfigFileTrue = false;
                if (!Directory.Exists(GAME_SAVEFILE))
                {
                    return;
                }
                if (!File.Exists(NEW_FILE))
                {
                    DecorLogger.LogToFile("Decor Config file not found for save game " + ServerManager.WorldName + " named " + FILE_NAME);
                    if (File.Exists(EXAMPLE_FILE))
                    {
                        DecorLogger.LogToFile("Example Decor File Found");
                        File.Copy(EXAMPLE_FILE, NEW_FILE);
                        DecorLogger.LogToFile("Example File Coppied to savegame");
                        ServerLog.LogAsyncMessage(new LogMessage("<color = blue>Restart server/game for decor mod to be functional</color>", LogType.Log));
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


            return true;
        }
    }
}
