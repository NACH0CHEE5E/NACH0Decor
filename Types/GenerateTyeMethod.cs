
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
    public class TypeRecipeBase : ICSRecipe
    {
        public string name { get; set; }

        public List<RecipeItem> requires { get; set; } = new List<RecipeItem>();

        public List<RecipeResult> results { get; set; } = new List<RecipeResult>();

        public CraftPriority defaultPriority { get; set; } = CraftPriority.Medium;

        public bool isOptional { get; set; } = false;

        public int defaultLimit { get; set; } = 0;

        public string Job { get; set; } = GenerateTypeConfig.DecorJobRecipe;
    }
    public class categoryBase
    {
        public static List<string> categories = new List<string>()
        {
            "decorative2", GenerateTypeConfig.NAME
        };
    }
    public class DecorTypeBase : CSType
    {
        public override List<string> categories { get; set; }
        public override Colliders colliders { get; set; } = new Colliders()
        {
            collidePlayer = true,
            collideSelection = true
        };
        public override int? maxStackSize => 500;
        public override bool? isPlaceable => true;
        public override bool? needsBase => false;
        public override bool? isRotatable => true;

        public override JObject customData { get; set; } = JsonConvert.DeserializeObject<JObject>("{ \"useHeightMap\": true, \"useNormalMap\": true }");
        public override string mesh { get; set; }
        public override string sideall { get; set; }
        public override string icon { get; set; }

    }
    public class DecorTypeSpecs : CSGenerateType
    {
        public override string generateType { get; set; } = "rotateBlock";
        public override ICSType baseType { get; set; } = new DecorTypeBase();
        public override string typeName { get; set; }
    }
    public class generateDecorTypes
    {
        public static void generateTypes(string name, List<Colliders.Boxes> colliders)
        {


            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
            {
                outputFile.WriteLine(name + " types:");
            }
            DecorLogger.LogToFile("Begining " + name + " generation");


            List<string> typesAdded = new List<string>();
            List<string> categories = new List<string>(categoryBase.categories);
            categories.Add(name);
            if (name != "Slab")
            {
                JSONNode list = new JSONNode(NodeType.Array);
                var Typesbase = new DecorTypeSpecs();
                Typesbase.baseType.colliders.boxes = colliders;
                Typesbase.baseType.mesh = GenerateTypeConfig.MOD_MESH_PATH + name + GenerateTypeConfig.MESHTYPE;
                if (!File.Exists(GenerateTypeConfig.MOD_ICON_PATH + name + GenerateTypeConfig.ICONTYPE))
                {
                    Typesbase.baseType.icon = GenerateTypeConfig.MOD_ICON_PATH + "_NoIcon" + GenerateTypeConfig.ICONTYPE;
                }
                else
                {
                    Typesbase.baseType.icon = GenerateTypeConfig.MOD_ICON_PATH + name + GenerateTypeConfig.ICONTYPE;
                }

                if (GenerateTypeConfig.DecorTypes.TryGetValue("_ALL", out List<DecorType> allBlockTypes))
                {
                    foreach (var currentType in allBlockTypes)
                    {
                        var typeName = GenerateTypeConfig.TYPEPREFIX + name + "." + currentType.name;
                        if (!typesAdded.Contains(typeName))
                        {

                            DecorLogger.LogToFile("Generating type \"" + typeName + "\" with \"name\": \"" + currentType.name + "\" \"type\": \"" + currentType.type + "\" \"texture\": \"" + currentType.texture + "\"");

                            Typesbase.typeName = typeName;
                            Typesbase.baseType.categories = new List<string>(categories);
                            Typesbase.baseType.categories.Add(currentType.texture);
                            Typesbase.baseType.sideall = currentType.texture;

                            list.AddToArray(Typesbase.JsonSerialize());
                            typesAdded.Add(typeName);


                            DecorLogger.LogToFile("JSON - " + Typesbase.JsonSerialize().ToString());
                            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
                            {
                                outputFile.WriteLine("Type \"" + typeName + "\" has texture \"" + currentType.texture + "\"");
                            }
                        }
                        else
                        {
                            DecorLogger.LogToFile("Type with \"name\": \"" + currentType.name + "\" already exists for \"" + name + "\" check decor file for duplicates, type skipped");
                        }

                    }
                }
                if (GenerateTypeConfig.DecorTypes.TryGetValue(name, out List<DecorType> blockTypes))
                {
                    foreach (var currentType in blockTypes)
                    {
                        var typeName = GenerateTypeConfig.TYPEPREFIX + name + "." + currentType.name;
                        if (!typesAdded.Contains(typeName))
                        {

                            DecorLogger.LogToFile("Generating type \"" + typeName + "\" with \"name\": \"" + currentType.name + "\" \"type\": \"" + currentType.type + "\" \"texture\": \"" + currentType.texture + "\"");


                            Typesbase.typeName = typeName;
                            Typesbase.baseType.categories = categories;
                            Typesbase.baseType.categories.Add(currentType.texture);
                            Typesbase.baseType.sideall = currentType.texture;

                            list.AddToArray(Typesbase.JsonSerialize());
                            typesAdded.Add(typeName);

                            DecorLogger.LogToFile("JSON - " + Typesbase.JsonSerialize().ToString());
                            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
                            {
                                outputFile.WriteLine("Type \"" + typeName + "\" has texture \"" + currentType.texture + "\"");
                            }
                        }
                        else
                        {
                            DecorLogger.LogToFile("Type with \"name\": \"" + currentType.name + "\" already exists for \"" + name + "\" check decor file for duplicates, type skipped");
                        }

                    }
                }
                ItemTypesServer.BlockRotator.Patches.AddPatch(new ItemTypesServer.BlockRotator.BlockGeneratePatch(GenerateTypeConfig.MOD_FOLDER, -99999, list));
            }
            else
            {
                var TypeParent = new DecorTypeBase();
                TypeParent.icon = GenerateTypeConfig.MOD_ICON_PATH + name + GenerateTypeConfig.ICONTYPE;

                var TypeUp = new DecorTypeBase();
                TypeUp.mesh = GenerateTypeConfig.MOD_MESH_PATH + name + ".up" + GenerateTypeConfig.MESHTYPE;
                TypeUp.colliders.boxes = typeColliders.Colliders_Dict[name + "Up"];

                var TypeDown = new DecorTypeBase();
                TypeUp.mesh = GenerateTypeConfig.MOD_MESH_PATH + name + ".down" + GenerateTypeConfig.MESHTYPE;
                TypeUp.colliders.boxes = typeColliders.Colliders_Dict[name + "Down"];

                if (GenerateTypeConfig.DecorTypes.TryGetValue("_ALL", out List<DecorType> allBlockTypes))
                {
                    foreach (var currentType in allBlockTypes)
                    {
                        var typeName = GenerateTypeConfig.TYPEPREFIX + name + "." + currentType.name;
                        var typeDownName = typeName + ".down";
                        var typeUpName = typeName + ".up";
                        if (!typesAdded.Contains(typeName))
                        {

                            DecorLogger.LogToFile("Generating type \"" + typeName + "\" with \"name\": \"" + currentType.name + "\" \"type\": \"" + currentType.type + "\" \"texture\": \"" + currentType.texture + "\"");

                            TypeParent.name = typeName;
                            TypeParent.categories = new List<string>(categories);
                            TypeParent.categories.Add(currentType.texture);
                            TypeParent.sideall = currentType.texture;
                            TypeParent.rotatablexn = typeUpName;
                            TypeParent.rotatablexp = typeUpName;
                            TypeParent.rotatablezn = typeDownName;
                            TypeParent.rotatablezp = typeDownName;

                            TypeUp.name = typeUpName;
                            TypeDown.name = typeDownName;

                            typesAdded.Add(typeName);


                            DecorLogger.LogToFile("JSON - " + TypeParent.JsonSerialize().ToString());
                            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
                            {
                                outputFile.WriteLine("Type \"" + typeName + "\" has texture \"" + currentType.texture + "\"");
                            }
                        }
                        else
                        {
                            DecorLogger.LogToFile("Type with \"name\": \"" + currentType.name + "\" already exists for \"" + name + "\" check decor file for duplicates, type skipped");
                        }

                    }
                }

            }

            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
            {
                outputFile.WriteLine("");
            }

        }
        public static void generateRecipes(string name)
        {
            DecorLogger.LogToFile("Begining " + name + " recipe generation");

            List<string> recipesAdded = new List<string>();

            if (GenerateTypeConfig.DecorTypes.TryGetValue("_ALL", out List<DecorType> allBlockTypes))
            {
                foreach (var currentType in allBlockTypes)
                {
                    var typeName = GenerateTypeConfig.TYPEPREFIX + name + "." + currentType.name;
                    var typeNameRecipe = typeName + ".Recipe";

                    if (!recipesAdded.Contains(typeNameRecipe))
                    {
                        DecorLogger.LogToFile("Generating recipe " + typeNameRecipe);

                        var recipe = new TypeRecipeBase();
                        recipe.name = typeNameRecipe;
                        recipe.requires.Add(new RecipeItem(currentType.type));
                        recipe.results.Add(new RecipeResult(typeName));


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
            if (GenerateTypeConfig.DecorTypes.TryGetValue(name, out List<DecorType> blockTypes))
            {
                foreach (var currentType in blockTypes)
                {
                    var typeName = GenerateTypeConfig.TYPEPREFIX + name + "." + currentType.name;
                    var typeNameRecipe = typeName + ".Recipe";

                    if (!recipesAdded.Contains(typeNameRecipe))
                    {
                        DecorLogger.LogToFile("Generating recipe " + typeNameRecipe);

                        var recipe = new TypeRecipeBase();
                        recipe.name = typeNameRecipe;
                        recipe.requires.Add(new RecipeItem(currentType.type));
                        recipe.results.Add(new RecipeResult(typeName));


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
    }

    [ModLoader.ModManager]
    public class callGenerateMethod
    {
        static List<string> IlligalTypes = new List<string>()
        {
            "Slab.down",
            "Slab.up"
        };
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, GenerateTypeConfig.MODNAMESPACE + "GenerateTypes")]
        public static void generateTypes()
        {
            if (GenerateTypeConfig.DecorConfigFileTrue)
            {

                foreach (string type in IlligalTypes)
                {
                    GenerateTypeConfig.TypeList.Remove(type);
                };
                foreach (string type in GenerateTypeConfig.TypeList)
                {
                    if (!typeColliders.Colliders_Dict.ContainsKey(type))
                    {
                        generateDecorTypes.generateTypes(type, typeColliders.Colliders_Dict["Generic"]);
                    }
                    else
                    {
                        generateDecorTypes.generateTypes(type, typeColliders.Colliders_Dict[type]);
                    }
                }
            }
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, GenerateTypeConfig.MODNAMESPACE + "GenerateRecipes")]
        public static void generateRecipes()
        {
            if (GenerateTypeConfig.DecorConfigFileTrue)
            {
                GenerateTypeConfig.TypeList.Add("Slab");
                foreach (string type in GenerateTypeConfig.TypeList)
                {
                    generateDecorTypes.generateRecipes(type);
                }
            }
        }
    }
}
