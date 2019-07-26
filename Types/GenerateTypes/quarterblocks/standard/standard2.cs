using System.Collections.Generic;
using static ItemTypesServer;
using Pipliz.JSON;
using Pipliz;
using System.IO;
using NACH0.Decor.GenerateTypes.Config;
using Pandaros.API.Models;
using Recipes;
using Decor.Models;
using Pandaros.API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Nach0.Decor.GenerateTypes.QuarterBlock
{
    public class LocalGenerateConfig
    {
        public const string NAME = "QuarterBlock";
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
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0f, 0.5f }, new List<float>(){ 0f, -0.5f, -0.5f }),
                },
            collidePlayer = true,
            collideSelection = true
        };
        public override int? maxStackSize => 500;
        public override bool? isPlaceable => true;
        public override bool? needsBase => false;
        public override bool? isRotatable => true;

        public override JObject customData { get; set; } = JsonConvert.DeserializeObject<JObject>("{ \"useHightMap\": true, \"useNormalMap\": true }");
        public override string mesh { get; set; } = GenerateTypeConfig.MOD_MESH_PATH + Type.NAME + GenerateTypeConfig.MESHTYPE;
        public override string sideall { get; set; }
        //public override List<OnRemove> onRemove { get => base.onRemove; set => base.onRemove = value; }
    }

    public class TypeSpecs : CSGenerateType
    {
        public override string generateType { get; set; } = "rotateBlock";
        public override ICSType baseType { get; set; } = new TypeBase();
        public override string typeName { get; set; }
    }

    public class TypeRecipe : ICSRecipe
    {
            public string name { get; set; } = GenerateTypeConfig.TYPEPREFIX + Type.NAME;

            public List<RecipeItem> requires => new List<RecipeItem>();

        public List<RecipeResult> results => new List<RecipeResult>();

            public CraftPriority defaultPriority { get; set; } = CraftPriority.Medium;

            public bool isOptional { get; set; } = false;

           public int defaultLimit { get; set; } = 0;

        public string Job { get; set; } = GenerateTypeConfig.NAME + ".Jobs." + LocalGenerateConfig.NAME + "Maker";
    }

    public class DummyJobRecipe : ICSRecipe
    {
        public string name { get; set; } = GenerateTypeConfig.NAME + ".Jobs." + LocalGenerateConfig.NAME + "Maker.dummy";

        public List<RecipeItem> requires => new List<RecipeItem>();

        public List<RecipeResult> results => new List<RecipeResult>();

        public CraftPriority defaultPriority { get; set; } = CraftPriority.Medium;

        public bool isOptional { get; set; } = true;

        public int defaultLimit { get; set; } = 0;

        public string Job { get; set; } = GenerateTypeConfig.NAME + ".Jobs." + LocalGenerateConfig.NAME + "Maker";
    }



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
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
            {
                outputFile.WriteLine("Begining " + NAME + " generation");
            }
            JSONNode list = new JSONNode(NodeType.Array);

            if (GenerateTypeConfig.DecorTypes.TryGetValue(NAME, out List<DecorType> blockTypes))
                foreach (var currentType in blockTypes)
                {
                    //ServerLog.LogAsyncMessage(new LogMessage("Found parent " + currentType.type, LogType.Log));
                    //ServerLog.LogAsyncMessage(new LogMessage("Found texture " + currentType.texture, LogType.Log));
                    var typeName = GenerateTypeConfig.TYPEPREFIX + NAME + "." + currentType.type;

                    //ServerLog.LogAsyncMessage(new LogMessage("Generating type " + typeName, LogType.Log));
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(GenerateTypeConfig.MOD_FOLDER, "Log.txt"), true))
                    {
                        outputFile.WriteLine("Generating type " + typeName);
                    }

                    var Typesbase = new TypeSpecs();
                    Typesbase.baseType.categories.Add(currentType.type);
                    Typesbase.typeName = typeName;
                    Typesbase.baseType.sideall = currentType.texture;
                    //Typesbase.baseType.onRemoveType = typeName;
                    //Typesbase.baseType.onRemove.Add.typeName;


                    list.AddToArray(Typesbase.JsonSerialize());


                }
            ItemTypesServer.BlockRotator.Patches.AddPatch(new ItemTypesServer.BlockRotator.BlockGeneratePatch(GenerateTypeConfig.MOD_FOLDER, -99999, list));
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
