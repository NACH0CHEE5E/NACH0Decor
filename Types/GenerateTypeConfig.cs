
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
        public const string MOD_VERSION = "3.2.0_Beta4";

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
        public static string MOD_CUSTOM_TEXTURE_PATH = "./textures/custom";

        public const string DecorJobRecipe = NAME + ".Jobs.DecorMaker";

        public const string TYPEPREFIX = NAME + ".Types.";

        public const string MESHTYPE = ".obj";
        public const string ICONTYPE = ".png";

        public static bool DecorConfigFileTrue = false;
        public static bool NewDecorConfigFileTrue = false;
        public static bool NewWorld = false;

        public const string FILE_NAME = "DecorTypes.json";
        public const string BASE_FILE_NAME = FILE_NAME;
        public const string EXAMPLE_FILE_NAME = "Example" + FILE_NAME;

        public static string EXAMPLE_FILE = "";
        public static string BASE_FILE = "";
        public static string NEW_FILE = "";
        public static Dictionary<string, List<DecorType>> DecorTypes { get; private set; }
        public static List<string> TypeList = new List<string>();

        public static Dictionary<string, bool> DecorSettings { get; private set; }
        public static string SETTINGS_FILE = ""; 



        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, MODNAMESPACE + ".OnAssemblyLoaded")]
        public static void OnAssemblyLoaded(string path)
        {
            MOD_FOLDER = Path.GetDirectoryName(path) + "/";

            GAME_ROOT = path.Substring(0, path.IndexOf("gamedata")).Replace("\\", "/") + "/";
            GAMEDATA_FOLDER = path.Substring(0, path.IndexOf("gamedata") + "gamedata".Length).Replace("\\", "/") + "/";
            GAME_SAVES = GAMEDATA_FOLDER + "savegames/";

            MOD_MESH_PATH = MOD_FOLDER + "gamedata/meshes/";
            MOD_ICON_PATH = MOD_FOLDER + "gamedata/textures/icons/";
            MOD_CUSTOM_TEXTURE_PATH = MOD_FOLDER + "gamedata/textures/custom/";

            DecorLogger.LogToFile("Mod Instalation Directory: " + MOD_FOLDER);
            DecorLogger.LogToFile("Mod Mesh Directory: " + MOD_MESH_PATH);
            DecorLogger.LogToFile("Mod Icon Directory: " + MOD_ICON_PATH);
            DecorLogger.LogToFile("Game Root Directory: " + GAME_ROOT);
            DecorLogger.LogToFile("Game Gamedata Directory: " + GAMEDATA_FOLDER);
            DecorLogger.LogToFile("Game Savegame Directory: " + GAME_SAVES);


            BASE_FILE = MOD_FOLDER + BASE_FILE_NAME;
            EXAMPLE_FILE = MOD_FOLDER + EXAMPLE_FILE_NAME;
            if (!File.Exists(BASE_FILE))
            {
                File.Copy(EXAMPLE_FILE, BASE_FILE);
            }



            //DecorLogger.LogToFile("Types in TypeList" + TypeList);
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, MODNAMESPACE + ".AfterSelectedWorld")]
        public static void AfterSelectedWorld()
        {
            DecorLogger.LogToFile("MOD VERSION = " + MOD_VERSION);
            string[] files = Directory.GetFiles(MOD_MESH_PATH);
            //DecorLogger.LogToFile("Files in Mod Mesh Directory" + files);


            foreach (string file in files)
            {
                if (file.EndsWith(MESHTYPE))
                {
                    var type = file.Replace(MESHTYPE, "");
                    type = type.Replace(MOD_MESH_PATH, "");
                    TypeList.Add(type);
                    DecorLogger.LogToFile("Adding Type: \"" + type + "\" to typeList");
                }

            }


            GAME_SAVEFILE = GAME_SAVES + ServerManager.WorldName + "/";
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
                if (File.Exists(BASE_FILE))
                {
                    DecorLogger.LogToFile("Example Decor File Found");
                    File.Copy(BASE_FILE, NEW_FILE);
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
            DecorLogger.LogToFile("DecorTypes.json Contents: " + fileContents);
            DecorTypes = JsonConvert.DeserializeObject<Dictionary<string, List<DecorType>>>(fileContents);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt")))
            {
                outputFile.WriteLine("");
            }
            /*SETTINGS_FILE = GAME_SAVEFILE + "DecorSettings.json";
            if (!File.Exists(SETTINGS_FILE))
            {
                if (ServerManager.HostingSettings.NetworkType == Shared.Options.EServerNetworkType.Singleplayer)
                {
                    DecorSettings["Singleplayer"] = true;
                }
                else
                {
                    DecorSettings["Singleplayer"] = false;
                }
                var settingsJSON = JsonConvert.SerializeObject(DecorSettings, Formatting.Indented);
                File.WriteAllText(SETTINGS_FILE, settingsJSON);
            }
            else
            {
                var settingsFileContents = File.ReadAllText(SETTINGS_FILE);
                DecorSettings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(settingsFileContents);
            }*/
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
                foreach (string type in TypeList)
                {
                    if (!DecorTypes.ContainsKey(type))
                    {
                        DecorTypes.Add(type, null);
                    }
                }
                fileContents = JsonConvert.SerializeObject(DecorTypes);
                File.WriteAllText(NEW_FILE, fileContents);

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt")))
                {
                    outputFile.WriteLine("");
                }
            }
            
            DecorLogger.LogToFile("TypeList.txt Contents: " + File.ReadAllText(GenerateTypeConfig.GAME_SAVEFILE + "TypeList.txt"));
        }
    }
    public class AddTypes
    {
        public bool AddDecorTypes(string DecorType, string name, string texture)
        {
            return true;
        }
    }
    [ChatCommandAutoLoader]
    public class AddTypesCommand : IChatCommand
    {
        public bool TryDoCommand(Players.Player player, string chat, List<string> splits)
        {
            //ServerLog.LogAsyncMessage(new LogMessage("Command Imput: " + chat, LogType.Log));
            if (player == null)
            {
                return false;
            }
            if (chat.StartsWith("/decoradd"))
            {
                DecorLogger.LogToFile("Command Attempt: " + chat + " by player: " + player.ID);
                if (PermissionsManager.CheckAndWarnPermission(player, "NACH0.Decor.Add") || ServerManager.HostingSettings.NetworkType == Shared.Options.EServerNetworkType.Singleplayer)
                {
                    DecorLogger.LogToFile("Command Attempt: " + chat);
                    var inventorySlot1 = player.Inventory.Items[0];
                    var typeName = ItemTypes.GetType(inventorySlot1.Type).Name;
                    var typeCategories = ItemTypes.GetType(inventorySlot1.Type).Categories;
                    var typeIsPlaceable = ItemTypes.GetType(inventorySlot1.Type).IsPlaceable;
                    var typeSideAll = ItemTypes.GetType(inventorySlot1.Type).SideAll;
                    var typeAmounnt = inventorySlot1.Amount;
                    if (typeAmounnt != 0 && typeCategories != null && typeSideAll != null && typeIsPlaceable)
                    {
                        DecorLogger.LogToFile("Type in inventory Slot 1: " + typeName);
                        Chat.Send(player, "<color=blue>Type in inventory Slot 1: " + typeName + "</color>");
                        var requestedDecorType = chat.Replace("/decoradd2", "");
                        requestedDecorType = requestedDecorType.Replace("/decoradd", "");
                        requestedDecorType = requestedDecorType.Replace(" ", "");
                        var decorType = "";
                        if (requestedDecorType != "")
                        {
                            DecorLogger.LogToFile("Requested Decor Type: " + requestedDecorType);
                            foreach (string type in GenerateTypeConfig.TypeList)
                            {
                                if (requestedDecorType == type)
                                {
                                    DecorLogger.LogToFile("Requested Decor Type is same as Decor Type: " + type);
                                    decorType = type;
                                }
                            }
                            if (decorType == "")
                            {
                                //ServerLog.LogAsyncMessage(new LogMessage("Type " + requestedDecorType + " does not exist, defaulting to _ALL", LogType.Log));
                                DecorLogger.LogToFile("Type " + requestedDecorType + " does not exist, stopping command");
                                Chat.Send(player, "<color=blue>Type " + requestedDecorType + " does not exist, stopping command</color>");
                                return false;
                            }
                        }
                        else
                        {
                            decorType = "_ALL";
                        }
                        //ServerLog.LogAsyncMessage(new LogMessage("typetoadd = " + typeToAddTo, LogType.Log));
                        if (GenerateTypeConfig.DecorTypes.TryGetValue(decorType, out List<DecorType> blockTypes))
                        {
                            foreach (var currentType in blockTypes)
                            {
                                if (currentType.texture == typeSideAll)
                                {
                                    DecorLogger.LogToFile("There is already a registered type in " + decorType + " that has the texture: " + currentType.texture);
                                    Chat.Send(player, "<color=blue>There is already a registered type in " + decorType + " that has the texture: " + currentType.texture + "</color>");
                                    return false;
                                }
                                if (currentType.name == typeName)
                                {
                                    DecorLogger.LogToFile("There is already a registered type in " + decorType + " that has the name: " + currentType.name);
                                    Chat.Send(player, "<color=blue>There is already a registered type in " + decorType + " that has the name: " + currentType.name + "</color>");
                                    return false;
                                }
                            }
                            DecorType newDecorTypeProperties = new DecorType();
                            newDecorTypeProperties.name = typeName;
                            newDecorTypeProperties.type = typeName;
                            newDecorTypeProperties.texture = typeSideAll;
                            newDecorTypeProperties.name = newDecorTypeProperties.name.Replace("NACH0.Types.", "");
                            newDecorTypeProperties.name = newDecorTypeProperties.name.Replace("Pandaros.Settlers.AutoLoad.", "");
                            newDecorTypeProperties.name = newDecorTypeProperties.name.Replace("Pandaros.Settlers.", "");
                            newDecorTypeProperties.name = newDecorTypeProperties.name.Replace("Ulfric.ColonyAddOns.Blocks.", "");

                            GenerateTypeConfig.DecorTypes[decorType].Add(newDecorTypeProperties);
                            var jsonWithAddedType = JsonConvert.SerializeObject(GenerateTypeConfig.DecorTypes, Formatting.Indented);
                            File.WriteAllText(GenerateTypeConfig.NEW_FILE, jsonWithAddedType);
                            DecorLogger.LogToFile("Added " + newDecorTypeProperties.type + " to " + decorType);
                            Chat.Send(player, "<color=blue>" + "Added " + newDecorTypeProperties.type + " to " + decorType + "</color>");
                            Chat.Send(player, "<color=blue>World/Server needs to be restarted to see new type, world is still playable and more can be added before restarting</color>");
                            return true;
                            //ServerLog.LogAsyncMessage(new LogMessage("Texture and name do not exist, able to add new type", LogType.Log));
                        }
                    }
                    else
                    {
                        if (typeAmounnt == 0)
                        {
                            Chat.Send(player, "<color=blue>Slot 1 is empty</color>");
                        }
                        else
                        {
                            if (typeCategories == null)
                            {
                                Chat.Send(player, "<color=blue>Type in slot one had no categories (its a non obtainable type)</color>");
                            }
                            if (!typeIsPlaceable)
                            {
                                Chat.Send(player, "<color=blue>Type in slot one is a item not a block</color>");
                            }
                            if (typeSideAll == null)
                            {
                                Chat.Send(player, "<color=blue>Type in slot one does not have a sideall texture</color>");
                            }
                        }
                    }
                }
                else
                {
                    Chat.Send(player, "<color=blue>You do not have required permissions</color>");
                }
            }
            

            return false;
        }
        
    }
    [ChatCommandAutoLoader]
    public class ListTypesCommand : IChatCommand
    {
        public bool TryDoCommand(Players.Player player, string chat, List<string> splits)
        {
            //ServerLog.LogAsyncMessage(new LogMessage("Command Imput: " + chat, LogType.Log));
            if (player == null)
            {
                return false;
            }
            if (chat.StartsWith("?decortypes"))
            {
                string decorTypes = "Available Decor Types: ";
                foreach (string type in GenerateTypeConfig.TypeList)
                {
                    decorTypes = decorTypes + type + ", ";
                }
                Chat.Send(player, "<color=blue>" + decorTypes + "</color>");
                return true;
            }


            return false;
        }
    }
}
