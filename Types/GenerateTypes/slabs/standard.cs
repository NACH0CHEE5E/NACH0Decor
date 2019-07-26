using System.Collections.Generic;
using static ItemTypesServer;
using Pipliz;
using System.IO;
using NACH0.Decor.GenerateTypes.Config;
using UnityEngine;
using Pandaros.API.Models;
using Recipes;
using Decor.Models;
using Pandaros.API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Nach0.Decor.GenerateTypes.Slab
{
    public class LocalGenerateConfig
    {
        public const string NAME = "Slab";
        public const string PARENT_NAME = NAME;

    }

    public class TypeParent : CSType
    {
        public override List<string> categories { get; set; } = new List<string>()
        {
            GenerateTypeConfig.NAME, GenerateTypeConfig.MODNAME, LocalGenerateConfig.PARENT_NAME, "a", LocalGenerateConfig.NAME, "b"        /*.SetAs("colors", "#ff0000->#ffffff")*/
        };

        public override int? maxStackSize => 500;
        public override bool? isPlaceable => true;
        public override bool? needsBase => false;
        public override bool? isRotatable => true;
        public override bool? isSolid => true;

        public override JObject customData { get; set; } = JsonConvert.DeserializeObject<JObject>("{ \"useHightMap\": true, \"useNormalMap\": true }");
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

    }

    public class TypeRecipe : ICSRecipe
    {
        public string name { get; set; }

        public List<RecipeItem> requires => new List<RecipeItem>();

        public List<RecipeResult> results => new List<RecipeResult>();

        public CraftPriority defaultPriority { get; set; } = CraftPriority.Medium;

        public bool isOptional { get; set; } = false;

        public int defaultLimit { get; set; } = 0;

        public string Job { get; set; } = GenerateTypeConfig.NAME + ".Jobs." + LocalGenerateConfig.NAME + "Maker";
    }



    [ModLoader.ModManager]
    public class Type
    {
        public const string NAME = LocalGenerateConfig.NAME;
        public const string GENERATE_TYPES_NAME = GenerateTypeConfig.GENERATE_TYPES_PREFIX + NAME;
        public const string GENERATE_RECIPES_NAME = GenerateTypeConfig.GENERATE_RECIPES_PREFIX + NAME;

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AddItemTypes, GENERATE_TYPES_NAME)]
        public static void generateTypes(Dictionary<string, ItemTypeRaw> types)
        {
            ServerLog.LogAsyncMessage(new LogMessage("Begining " + NAME + " type generation", LogType.Log));
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
            {
                outputFile.WriteLine("Begining " + NAME + " generation");
            }

            if (GenerateTypeConfig.DecorTypes.TryGetValue(NAME, out List<DecorType> blockTypes))
                foreach (var currentType in blockTypes)
                {
                    //ServerLog.LogAsyncMessage(new LogMessage("Found parent " + currentType.type, LogType.Log));
                    //ServerLog.LogAsyncMessage(new LogMessage("Found texture " + currentType.texture, LogType.Log));
                    var typeName = GenerateTypeConfig.TYPEPREFIX + NAME + "." + currentType.type;
                    var typeNameUp = typeName + ".up";
                    var typeNameDown = typeName + ".down";

                    //ServerLog.LogAsyncMessage(new LogMessage("Generating type " + typeName, LogType.Log));
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
                    {
                        outputFile.WriteLine("Generating type " + typeName);
                    }

                    var baseType = new TypeParent();
                    baseType.categories.Add(currentType.type);
                    baseType.name = typeName;
                    baseType.sideall = currentType.texture;
                    baseType.rotatablexn = typeNameUp;
                    baseType.rotatablexp = typeNameUp;
                    baseType.rotatablezn = typeNameDown;
                    baseType.rotatablezp = typeNameDown;

                    var typeUp = new TypeUp();
                    typeUp.name = typeNameUp;
                    typeUp.parentType = typeName;

                    var typeDown = new TypeDown();
                    typeDown.name = typeNameDown;
                    typeDown.parentType = typeName;


                    types.Add(typeName, new ItemTypeRaw(typeName, baseType.JsonSerialize()));
                    types.Add(typeNameUp, new ItemTypeRaw(typeNameUp, typeUp.JsonSerialize()));
                    types.Add(typeNameDown, new ItemTypeRaw(typeNameDown, typeDown.JsonSerialize()));
                }
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, GENERATE_RECIPES_NAME)]
        public static void generateRecipes()
        {
            //ServerLog.LogAsyncMessage(new LogMessage("Begining " + NAME + " recipe generation", LogType.Log));
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
            {
                outputFile.WriteLine("Begining " + NAME + " recipe generation");
            }

            if (GenerateTypeConfig.DecorTypes.TryGetValue(LocalGenerateConfig.NAME, out List<DecorType> blockTypes))
                foreach (var currentType in blockTypes)
                {
                    var typeName = GenerateTypeConfig.TYPEPREFIX + NAME + "." + currentType.type;
                    var typeNameRecipe = GenerateTypeConfig.TYPEPREFIX + NAME + "." + currentType.type + ".Recipe";

                    //ServerLog.LogAsyncMessage(new LogMessage("Generating recipe " + typeNameRecipe, LogType.Log));
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
                    {
                        outputFile.WriteLine("Generating recipe " + typeNameRecipe);
                    }

                    var recipe = new TypeRecipe();
                    recipe.name = typeNameRecipe;
                    recipe.requires.Add(new RecipeItem(currentType.type));
                    recipe.results.Add(new RecipeResult(typeName));


                    var requirements = new List<InventoryItem>();
                    var results = new List<RecipeResult>();
                    recipe.JsonSerialize();

                    foreach (var ri in recipe.requires)
                        if (ItemTypes.IndexLookup.TryGetIndex(ri.type, out var itemIndex))
                            requirements.Add(new InventoryItem(itemIndex, ri.amount));

                    foreach (var ri in recipe.results)
                        results.Add(ri);

                    var newRecipe = new Recipe(recipe.name, requirements, results, recipe.defaultLimit, 0, (int)recipe.defaultPriority);

                    ServerManager.RecipeStorage.AddLimitTypeRecipe(recipe.Job, newRecipe);
                }
        }
    }
}