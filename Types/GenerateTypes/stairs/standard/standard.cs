﻿using System.Collections.Generic;
using static ItemTypesServer;
using Pipliz.JSON;
using Pipliz;
using System.IO;
using Pandaros.API.Models;
using Recipes;
using Pandaros.API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nach0.Decor;

namespace Nach0.Decor.GenerateTypes.Stairs
{
    public class LocalGenerateConfig
    {
        public const string NAME = "Stairs";
        public const string PARENT_NAME = NAME;
    }

    public class TypeBase : CSType
    {
        public override List<string> categories { get; set; } = new List<string>()
        {
            GenerateTypeConfig.NAME, GenerateTypeConfig.MODNAME, LocalGenerateConfig.PARENT_NAME, "a", LocalGenerateConfig.NAME, "b"
        };
        public override Colliders colliders { get; set; } = new Colliders()
        {
            boxes = new List<Colliders.Boxes>()
                {
                    new Colliders.Boxes(new List<float>(){ 0.5f, -0.25f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0f, 0.5f }, new List<float>(){ -0.25f, -0.25f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.25f, 0.5f }, new List<float>(){ 0f, 0f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, 0.5f }, new List<float>(){ 0.25f, 0.25f, -0.5f })
                },
            collidePlayer = true,
            collideSelection = true
        };
        public override int? maxStackSize => 500;
        public override bool? isPlaceable => true;
        public override bool? needsBase => false;
        public override bool? isRotatable => true;

        public override JObject customData { get; set; } = JsonConvert.DeserializeObject<JObject>("{ \"useHeightMap\": true, \"useNormalMap\": true }");
        public override string mesh { get; set; } = GenerateTypeConfig.MOD_MESH_PATH + Type.NAME + GenerateTypeConfig.MESHTYPE;
        public override string sideall { get; set; }
        public override string icon { get; set; } = GenerateTypeConfig.MOD_ICON_PATH + Type.NAME + GenerateTypeConfig.ICONTYPE;
    }

    public class TypeSpecs : CSGenerateType
    {
        public override string generateType { get; set; } = "rotateBlock";
        public override ICSType baseType { get; set; } = new TypeBase();
        public override string typeName { get; set; }
    }

    /*public class TypeRecipe : ICSRecipe
    {
        public string name { get; set; }

        public List<RecipeItem> requires { get; set; } = new List<RecipeItem>();

        public List<RecipeResult> results { get; set; } = new List<RecipeResult>()
        {
        };

        public CraftPriority defaultPriority { get; set; } = CraftPriority.Medium;

            public bool isOptional { get; set; } = false;

           public int defaultLimit { get; set; } = 0;

        public string Job { get; set; } = GenerateTypeConfig.NAME + ".Jobs." + LocalGenerateConfig.NAME + "Maker";
    }*/



    [ModLoader.ModManager]
    public class Type
    {
        public const string NAME = LocalGenerateConfig.NAME;
        public const string GENERATE_TYPES_NAME = GenerateTypeConfig.GENERATE_TYPES_PREFIX + NAME;
        public const string GENERATE_RECIPES_NAME = GenerateTypeConfig.GENERATE_RECIPES_PREFIX + NAME;

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, GENERATE_TYPES_NAME)]
        public static void generateTypes()
        {
            //ServerLog.LogAsyncMessage(new LogMessage("Begining " + NAME + " generation", LogType.Log));
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
            {
                outputFile.WriteLine(NAME + " types:");
            }
            DecorLogger.LogToFile("Begining " + NAME + " generation");
            JSONNode list = new JSONNode(NodeType.Array);

            if (GenerateTypeConfig.DecorConfigFileTrue && GenerateTypeConfig.DecorTypes.TryGetValue(NAME, out List<DecorType> blockTypes))
                foreach (var currentType in blockTypes)
                {
                    //ServerLog.LogAsyncMessage(new LogMessage("Found parent " + currentType.type, LogType.Log));
                    //ServerLog.LogAsyncMessage(new LogMessage("Found texture " + currentType.texture, LogType.Log));
                    var typeName = GenerateTypeConfig.TYPEPREFIX + NAME + "." + currentType.name;

                    //ServerLog.LogAsyncMessage(new LogMessage("Generating type " + typeName, LogType.Log));
                    
                    DecorLogger.LogToFile("Generating type \"" + typeName + "\" with \"name\": \"" + currentType.name + "\" \"type\": \"" + currentType.type + "\" \"texture\": \"" + currentType.texture + "\"");

                    var Typesbase = new TypeSpecs();
                    Typesbase.baseType.categories.Add(currentType.texture);
                    Typesbase.typeName = typeName;
                    Typesbase.baseType.sideall = currentType.texture;

                    list.AddToArray(Typesbase.JsonSerialize());
                    DecorLogger.LogToFile("JSON - " + Typesbase.JsonSerialize().ToString());
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
                    {
                        outputFile.WriteLine("Type \"" + typeName + "\" has texture \"" + currentType.texture + "\"");
                    }

                }
            ItemTypesServer.BlockRotator.Patches.AddPatch(new ItemTypesServer.BlockRotator.BlockGeneratePatch(GenerateTypeConfig.MOD_FOLDER, -99999, list));
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
            {
                outputFile.WriteLine("");
            }

        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, GENERATE_RECIPES_NAME)]
        public static void generateRecipes()
        {
            //ServerLog.LogAsyncMessage(new LogMessage("Begining " + NAME + " recipe generation", LogType.Log));
            
            DecorLogger.LogToFile("Begining " + NAME + " recipe generation");

            if (GenerateTypeConfig.DecorConfigFileTrue && GenerateTypeConfig.DecorTypes.TryGetValue(LocalGenerateConfig.NAME, out List<DecorType> blockTypes))
                foreach (var currentType in blockTypes)
                {
                    var typeName = GenerateTypeConfig.TYPEPREFIX + NAME + "." + currentType.name;
                    var typeNameRecipe = typeName + ".Recipe";

                    //ServerLog.LogAsyncMessage(new LogMessage("Generating recipe " + typeNameRecipe, LogType.Log));
                    
                    DecorLogger.LogToFile("Generating recipe " + typeNameRecipe);

                    var recipe = new TypeRecipeBase();
                    recipe.name = typeNameRecipe;
                    recipe.requires.Add(new RecipeItem(currentType.type));
                    recipe.results.Add(new RecipeResult(typeName));

                    recipe.Job = GenerateTypeConfig.NAME + ".Jobs." + LocalGenerateConfig.NAME + "Maker";


                    var requirements = new List<InventoryItem>();
                    var results = new List<RecipeResult>();
                    recipe.JsonSerialize();
                    DecorLogger.LogToFile("JSON - " + recipe.JsonSerialize().ToString());

                    foreach (var ri in recipe.requires)
                    {
                        if (ItemTypes.IndexLookup.TryGetIndex(ri.type, out var itemIndex))
                        {
                            requirements.Add(new InventoryItem(itemIndex, ri.amount));
                        }
                        else
                        {
                            DecorLogger.LogToFile("\"" + typeNameRecipe + "\" bad requirement \"" + ri.type + "\"");
                        }
                    }
                    foreach (var ri in recipe.results)
                        results.Add(ri);

                    var newRecipe = new Recipe(recipe.name, requirements, results, recipe.defaultLimit, 0, (int)recipe.defaultPriority);

                    ServerManager.RecipeStorage.AddLimitTypeRecipe(recipe.Job, newRecipe);
                }
        }
    }
}
