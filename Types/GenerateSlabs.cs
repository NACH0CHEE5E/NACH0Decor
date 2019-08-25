using System.Collections.Generic;
using static ItemTypesServer;
using Pipliz;
using System.IO;
using UnityEngine;
using Pandaros.API.Models;
using Recipes;
using Pandaros.API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Nach0.Decor.GenerateTypes.Slab
{
    /*public class LocalGenerateConfig
    {
        public const string NAME = "Slab";

    }

    public class TypeParent : CSType
    {
        public override List<string> categories { get; set; } = new List<string>();

        public override int? maxStackSize => 500;
        public override bool? isPlaceable => true;
        public override bool? needsBase => false;
        public override bool? isRotatable => true;
        public override bool? isSolid => true;

        public override JObject customData { get; set; } = JsonConvert.DeserializeObject<JObject>("{ \"useHeightMap\": true, \"useNormalMap\": true }");
        public override string icon { get; set; } = GenerateTypeConfig.MOD_ICON_PATH + Type.NAME + GenerateTypeConfig.ICONTYPE;
    }

    public class TypeUp : CSType
    {
        public override string mesh { get; set; } = GenerateTypeConfig.MOD_MESH_PATH + Type.NAME + ".up" + GenerateTypeConfig.MESHTYPE;
        public override Colliders colliders { get; set; } = new Colliders()
        {
            boxes = new List<Colliders.Boxes>()
            {
                new Colliders.Boxes(new List<float>(){ -0.5f, 0f, -0.5f }, new List<float>(){ 0.5f, 0.5f, 0.5f })
            },
            collidePlayer = true,
            collideSelection = true
        };
    }

    public class TypeDown : CSType
    {
        public override string mesh { get; set; } = GenerateTypeConfig.MOD_MESH_PATH + Type.NAME + ".down" + GenerateTypeConfig.MESHTYPE;
        public override Colliders colliders { get; set; } = new Colliders()
        {
            boxes = new List<Colliders.Boxes>()
            {
                new Colliders.Boxes(new List<float>(){ 0.5f, 0f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f })
            },
            collidePlayer = true,
            collideSelection = true
        };

    }*/




    [ModLoader.ModManager]
    public class Type
    {
        public const string NAME = "Slab";
        public const string GENERATE_TYPES_NAME = GenerateTypeConfig.GENERATE_TYPES_PREFIX + NAME;
       //public const string GENERATE_RECIPES_NAME = GenerateTypeConfig.GENERATE_RECIPES_PREFIX + NAME;

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AddItemTypes, GENERATE_TYPES_NAME)]
        public static void generateTypes(Dictionary<string, ItemTypeRaw> types)
        {

            var name = "Slab";
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
            {
                outputFile.WriteLine(name + " types:");
            }
            DecorLogger.LogToFile("Begining " + name + " generation");


            List<string> typesAdded = new List<string>();
            List<string> categories = new List<string>(categoryBase.categories);
            categories.Add(name);
            var TypeParent = new DecorTypeBase();
            TypeParent.icon = GenerateTypeConfig.MOD_ICON_PATH + name + GenerateTypeConfig.ICONTYPE;

            var TypeUp = new DecorTypeBase();
            TypeUp.mesh = GenerateTypeConfig.MOD_MESH_PATH + name + ".up" + GenerateTypeConfig.MESHTYPE;
            TypeUp.colliders.boxes = typeColliders.Colliders_Dict[name + "Up"];

            var TypeDown = new DecorTypeBase();
            TypeDown.mesh = GenerateTypeConfig.MOD_MESH_PATH + name + ".down" + GenerateTypeConfig.MESHTYPE;
            TypeDown.colliders.boxes = typeColliders.Colliders_Dict[name + "Down"];

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
                        TypeUp.parentType = typeName;
                        TypeDown.name = typeDownName;
                        TypeDown.parentType = typeName;

                        typesAdded.Add(typeName);
                        types.Add(typeName, new ItemTypeRaw(typeName, TypeParent.JsonSerialize()));
                        types.Add(typeUpName, new ItemTypeRaw(typeUpName, TypeUp.JsonSerialize()));
                        types.Add(typeDownName, new ItemTypeRaw(typeDownName, TypeDown.JsonSerialize()));


                        DecorLogger.LogToFile("JSON - " + TypeParent.JsonSerialize().ToString() + TypeUp.JsonSerialize().ToString() + TypeDown.JsonSerialize().ToString());
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
            if (GenerateTypeConfig.DecorTypes.TryGetValue(name, out List<DecorType> BlockTypes))
            {
                foreach (var currentType in BlockTypes)
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
                        types.Add(typeName, new ItemTypeRaw(typeName, TypeParent.JsonSerialize()));
                        types.Add(typeUpName, new ItemTypeRaw(typeUpName, TypeUp.JsonSerialize()));
                        types.Add(typeDownName, new ItemTypeRaw(typeDownName, TypeDown.JsonSerialize()));


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



            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.GAME_SAVEFILE, "TypeList.txt"), true))
            {
                outputFile.WriteLine("");
            }

        }

        /*[ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, GENERATE_RECIPES_NAME)]
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
                    recipe.Job = GenerateTypeConfig.DecorJobRecipe;


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
        }*/
    }
}