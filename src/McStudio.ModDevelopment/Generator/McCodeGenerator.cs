using System.Text;
using System.Text.Json;
using McStudio.ModDevelopment.Element;
using McStudio.ModDevelopment.Element.Types;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Ported from CCS Generator.java
/// Generates Minecraft mod source code and resources from workspace definitions.
/// Uses Scriban template engine for template-based generation.
/// </summary>
public class McCodeGenerator : IDisposable
{
    private readonly McWorkspace _workspace;
    private readonly string _generatorName;
    private readonly McScribanTemplateEngine _templateEngine;

    public McCodeGenerator(McWorkspace workspace)
    {
        _workspace = workspace;
        _generatorName = $"{workspace.Settings.ModLoader}-{workspace.Settings.MinecraftVersion}";
        _templateEngine = new McScribanTemplateEngine(_generatorName);
    }

    public McWorkspace Workspace => _workspace;
    public string GeneratorName => _generatorName;

    /// <summary>
    /// Generate all base mod files (gradle, mod metadata, etc.)
    /// </summary>
    public bool GenerateBase()
    {
        try
        {
            var modId = _workspace.Settings.ModId;
            var packageName = _workspace.Settings.PackageName;
            var modLoader = _workspace.Settings.ModLoader;
            var mcVersion = _workspace.Settings.MinecraftVersion;
            var workspaceFolder = _workspace.GetWorkspaceFolder();

            // Generate build.gradle / build.gradle.kts
            var gradleContent = GenerateGradleFile(modId, modLoader, mcVersion);
            var gradleExt = modLoader == "fabric" ? ".kts" : "";
            File.WriteAllText(Path.Combine(workspaceFolder, $"build.gradle{gradleExt}"), gradleContent);

            // Generate mod metadata using Scriban templates
            if (modLoader == "neoforge" || modLoader == "forge")
            {
                GenerateNeoForgeFiles(modId, packageName, workspaceFolder);
            }
            else if (modLoader == "fabric")
            {
                GenerateFabricFiles(modId, packageName, workspaceFolder);
            }

            // Generate main mod class using Scriban template
            var packageDir = Path.Combine(workspaceFolder, "src/main/java",
                packageName.Replace('.', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(packageDir);
            GenerateMainModClass(modId, packageName, modLoader, packageDir);

            // Generate gradle.properties
            var gradleProps = $"modId={modId}\nmodName={_workspace.Settings.ModName}\n" +
                              $"modVersion=1.0.0\nmodGroup={packageName}\nmodAuthor={_workspace.Settings.Author}\n" +
                              $"modDescription={_workspace.Settings.Description}\n";
            File.WriteAllText(Path.Combine(workspaceFolder, "gradle.properties"), gradleProps);

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to generate base files: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Generate code for a specific mod element using Scriban templates
    /// </summary>
    public bool GenerateElement(McModElement element)
    {
        try
        {
            var workspaceFolder = _workspace.GetWorkspaceFolder();
            var packageName = _workspace.Settings.PackageName;

            // Map element type to template name and generate
            var (code, fileName) = GenerateFromTemplate(element);

            if (code != null && fileName != null)
            {
                var packageDir = Path.Combine(workspaceFolder, "src/main/java",
                    packageName.Replace('.', Path.DirectorySeparatorChar));
                var filePath = Path.Combine(packageDir, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                File.WriteAllText(filePath, code);
                element.SetAssociatedFiles([filePath]);
            }

            // Generate JSON resources (models, blockstates, loot tables)
            GenerateJsonResources(element, workspaceFolder);

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to generate element {element.Name}: {ex.Message}");
            return false;
        }
    }

    private (string? code, string? fileName) GenerateFromTemplate(McModElement element)
    {
        var registryName = element.GetRegistryName();
        var className = ToPascalCase(element.Name);
        var modId = _workspace.Settings.ModId;
        var packageName = _workspace.Settings.PackageName;

        var dataModel = new Dictionary<string, object>
        {
            ["modid"] = modId,
            ["package"] = packageName,
            ["registryname"] = registryName,
            ["classname"] = className,
            ["JavaModName"] = ToPascalCase(modId),
            ["name"] = element.Name,
        };

        // Add element-specific data
        var genElement = element.GetGeneratableElement();
        if (genElement != null)
        {
            // Add element properties to data model
            AddElementProperties(dataModel, genElement);
        }

        // Determine template path based on element type
        var templateName = element.Type switch
        {
            "block" => "block/block.java.scriban",
            "item" => "item/item.java.scriban",
            "livingentity" => "livingentity/livingentity.java.scriban",
            _ => null
        };

        if (templateName == null)
            return (null, null);

        var code = _templateEngine.Render(templateName, dataModel);
        // Remove .scriban extension from the template path to get the output file name
        var outputFile = $"{className}{ToPascalCase(element.Type)}.java";
        return (code, outputFile);
    }

    private void AddElementProperties(Dictionary<string, object> dataModel, McGeneratableElement element)
    {
        if (element is McBlockElement block)
        {
            dataModel["block_base_class"] = GetBlockBaseClass(block.BlockBase);
            dataModel["hardness"] = block.Hardness;
            dataModel["resistance"] = block.Resistance;
            dataModel["luminance"] = block.Luminance;
            dataModel["light_opacity"] = block.LightOpacity;
            dataModel["is_transparent"] = block.IsTransparent;
            dataModel["is_waterloggable"] = false; // Simplified
            dataModel["is_redstone_conductor"] = block.IsRedstoneConductor;
            dataModel["rotation_mode"] = block.RotationMode;
            dataModel["enable_pitch"] = block.EnablePitch;
            dataModel["requires_tool"] = block.RequiresCorrectTool;
            dataModel["has_inventory"] = false; // Simplified
            dataModel["map_color"] = block.BlockMapColor ?? "";
            dataModel["sound"] = "";
        }
        else if (element is McItemElement item)
        {
            dataModel["stack_size"] = item.StackSize;
            dataModel["durability"] = 0;
            dataModel["is_food"] = item.IsFood ?? false;
            dataModel["nutrition"] = item.Nutrition ?? 0;
            dataModel["saturation_mod"] = item.SaturationMod ?? 0.6;
            dataModel["always_edible"] = item.AlwaysEdible ?? false;
            dataModel["is_meat"] = item.IsMeat ?? false;
            dataModel["is_fast_food"] = item.IsFastFood ?? false;
        }
        else if (element is McLivingEntityElement entity)
        {
            dataModel["entity_base_class"] = "PathfinderMob";
            dataModel["health"] = entity.Health;
            dataModel["movement_speed"] = entity.MovementSpeed;
            dataModel["attack_damage"] = entity.AttackDamage;
            dataModel["armor_base_value"] = entity.ArmorBaseValue ?? 0;
            dataModel["knockback_resistance"] = entity.KnockbackResistance ?? 0;
            dataModel["follow_range"] = entity.FollowRange ?? 0;
            dataModel["xp_reward"] = 0;
        }
    }

    private void GenerateJsonResources(McModElement element, string workspaceFolder)
    {
        var modId = _workspace.Settings.ModId;
        var registryName = element.GetRegistryName();
        var resourcesDir = Path.Combine(workspaceFolder, "src/main/resources");

        if (element.Type == "block")
        {
            // Blockstate JSON
            var blockstateData = new Dictionary<string, object>
            {
                ["modid"] = modId,
                ["registryname"] = registryName
            };
            var blockstate = _templateEngine.Render("json/blockstate.json.scriban", blockstateData);
            if (blockstate != null)
            {
                var blockstateDir = Path.Combine(resourcesDir, "assets", modId, "blockstates");
                Directory.CreateDirectory(blockstateDir);
                File.WriteAllText(Path.Combine(blockstateDir, $"{registryName}.json"), blockstate);
            }

            // Block model JSON
            var modelData = new Dictionary<string, object>
            {
                ["parent"] = "block/cube_all",
                ["textures"] = new[] { new { key = "all", value = $"{modId}:block/{registryName}" } },
                ["render_type"] = ""
            };
            var model = _templateEngine.Render("json/block_model.json.scriban",
                new Dictionary<string, object> { ["parent"] = "block/cube_all", ["textures"] = "", ["render_type"] = "" });
            if (model != null)
            {
                var modelsDir = Path.Combine(resourcesDir, "assets", modId, "models", "block");
                Directory.CreateDirectory(modelsDir);
                File.WriteAllText(Path.Combine(modelsDir, $"{registryName}.json"), model);
            }

            // Item model for block item
            var itemModelData = new Dictionary<string, object>
            {
                ["modid"] = modId,
                ["texture"] = registryName
            };
            var itemModel = _templateEngine.Render("json/item_model.json.scriban", itemModelData);
            if (itemModel != null)
            {
                var itemModelsDir = Path.Combine(resourcesDir, "assets", modId, "models", "item");
                Directory.CreateDirectory(itemModelsDir);
                File.WriteAllText(Path.Combine(itemModelsDir, $"{registryName}.json"), itemModel);
            }

            // Loot table JSON
            var lootData = new Dictionary<string, object>
            {
                ["modid"] = modId,
                ["registryname"] = registryName
            };
            var lootTable = _templateEngine.Render("json/loot_table.json.scriban", lootData);
            if (lootTable != null)
            {
                var lootDir = Path.Combine(resourcesDir, "data", modId, "loot_table", "blocks");
                Directory.CreateDirectory(lootDir);
                File.WriteAllText(Path.Combine(lootDir, $"{registryName}.json"), lootTable);
            }
        }
        else if (element.Type == "item")
        {
            // Item model JSON
            var itemModelData = new Dictionary<string, object>
            {
                ["modid"] = modId,
                ["texture"] = registryName
            };
            var itemModel = _templateEngine.Render("json/item_model.json.scriban", itemModelData);
            if (itemModel != null)
            {
                var itemModelsDir = Path.Combine(resourcesDir, "assets", modId, "models", "item");
                Directory.CreateDirectory(itemModelsDir);
                File.WriteAllText(Path.Combine(itemModelsDir, $"{registryName}.json"), itemModel);
            }
        }
    }

    private string GenerateGradleFile(string modId, string modLoader, string mcVersion)
    {
        return modLoader switch
        {
            "neoforge" => $@"plugins {{
    id 'java'
    id 'net.neoforged.gradle.userdev' version '7.0.145'
}}

base {{
    archivesName = '{modId}'
}}

java.toolchain.languageVersion = JavaLanguageVersion.of(21)

repositories {{
    mavenCentral()
}}

dependencies {{
    implementation ""net.neoforged:neoforge:"" + neo_version + """" 
}}

tasks.withType(JavaCompile).configureEach {{
    options.encoding = 'UTF-8'
    options.release = 21
}}",
            "fabric" => $@"plugins {{
    id 'fabric-loom' version '1.8-SNAPSHOT'
    id 'java'
}}

base {{
    archivesName = '{modId}'
}}

java.toolchain.languageVersion = JavaLanguageVersion.of(21)

repositories {{
    mavenCentral()
}}

dependencies {{
    minecraft ""com.mojang:minecraft:{mcVersion}""
    mappings ""net.fabricmc:yarn:{mcVersion}+build.1:v2""
    modImplementation ""net.fabricmc:fabric-loader:0.16.0""
    modImplementation ""net.fabricmc.fabric-api:fabric-api:0.102.0+{mcVersion}""
}}

tasks.withType(JavaCompile).configureEach {{
    options.encoding = 'UTF-8'
    options.release = 21
}}",
            _ => ""
        };
    }

    private void GenerateNeoForgeFiles(string modId, string packageName, string workspaceFolder)
    {
        // mods.toml
        var modName = _workspace.Settings.ModName;
        var author = _workspace.Settings.Author;
        var description = _workspace.Settings.Description;
        var modsToml = $@"modLoader=""javafml""
loaderVersion=""[4,)""
license=""All rights reserved""

[[mods]]
modId=""{modId}""
version=""1.0.0""
displayName=""{modName}""
authors=""{author}""
description='{description}'
";
        File.WriteAllText(Path.Combine(workspaceFolder, "src/main/resources/META-INF/neoforge.mods.toml"), modsToml);

        // pack.mcmeta
        var packMcmeta = JsonSerializer.Serialize(new
        {
            pack = new
            {
                description = _workspace.Settings.ModName,
                pack_format = mcVersionToPackFormat(_workspace.Settings.MinecraftVersion)
            }
        }, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.Combine(workspaceFolder, "src/main/resources"));
        File.WriteAllText(Path.Combine(workspaceFolder, "src/main/resources/pack.mcmeta"), packMcmeta);
    }

    private void GenerateFabricFiles(string modId, string packageName, string workspaceFolder)
    {
        var fabricJson = JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            id = modId,
            version = "1.0.0",
            name = _workspace.Settings.ModName,
            description = _workspace.Settings.Description,
            authors = new[] { _workspace.Settings.Author },
            entrypoints = new
            {
                main = new[] { $"{packageName}.{ToPascalCase(modId)}" }
            },
            depends = new
            {
                fabricloader = ">=0.16.0",
                minecraft = $"~{_workspace.Settings.MinecraftVersion}"
            }
        }, new JsonSerializerOptions { WriteIndented = true });

        Directory.CreateDirectory(Path.Combine(workspaceFolder, "src/main/resources"));
        File.WriteAllText(Path.Combine(workspaceFolder, "src/main/resources/fabric.mod.json"), fabricJson);

        var packMcmeta = JsonSerializer.Serialize(new
        {
            pack = new
            {
                description = _workspace.Settings.ModName,
                pack_format = mcVersionToPackFormat(_workspace.Settings.MinecraftVersion)
            }
        }, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(workspaceFolder, "src/main/resources/pack.mcmeta"), packMcmeta);
    }

    private void GenerateMainModClass(string modId, string packageName, string modLoader, string packageDir)
    {
        var className = ToPascalCase(modId);

        var code = modLoader switch
        {
            "neoforge" => $@"package {packageName};

import net.neoforged.bus.api.IEventBus;
import net.neoforged.fml.common.Mod;

@Mod({modId})
public class {className} {{
    public static final String MODID = ""{modId}"";

    public {className}(IEventBus modEventBus) {{
        // Registration will be added here
    }}
}}
",
            "fabric" => $@"package {packageName};

import net.fabricmc.api.ModInitializer;
import net.fabricmc.fabric.api.event.lifecycle.v1.ServerLifecycleEvents;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class {className} implements ModInitializer {{
    public static final String MODID = ""{modId}"";
    public static final Logger LOGGER = LoggerFactory.getLogger(MODID);

    @Override
    public void onInitialize() {{
        LOGGER.info(""Initializing "" + MODID);
    }}
}}
",
            _ => ""
        };

        var filePath = Path.Combine(packageDir, $"{className}.java");
        File.WriteAllText(filePath, code);
    }

    private (string? code, string? fileName) GenerateProcedureElement(McModElement element)
    {
        var packageName = _workspace.Settings.PackageName;
        var className = ToPascalCase(element.Name);

        var code = $@"package {packageName};

import net.minecraft.world.level.LevelAccessor;

public class {className}Procedure {{
    public static void execute(LevelAccessor world, double x, double y, double z) {{
        // TODO: Add procedure logic here
    }}
}}
";
        return (code, $"{className}Procedure.java");
    }

    private (string? code, string? fileName) GenerateRecipeElement(McModElement element)
    {
        var registryName = element.GetRegistryName();
        var packageName = _workspace.Settings.PackageName;
        var modId = _workspace.Settings.ModId;

        var code = $@"{{
    ""type"": ""minecraft:crafting_shaped"",
    ""group"": ""{modId}"",
    ""pattern"": [
        ""AAA"",
        ""ABA"",
        ""AAA""
    ],
    ""key"": {{
        ""A"": {{ ""item"": ""minecraft:stone"" }},
        ""B"": {{ ""item"": ""minecraft:diamond"" }}
    }},
    ""result"": {{
        ""item"": ""{modId}:{registryName}"",
        ""count"": 1
    }}
}}
";
        return (code, $"{registryName}.json");
    }

    private static string GetBlockBaseClass(string? blockBase)
    {
        return blockBase switch
        {
            "Stairs" => "StairBlock",
            "Slab" => "SlabBlock",
            "Fence" => "FenceBlock",
            "Wall" => "WallBlock",
            "TrapDoor" => "TrapDoorBlock",
            "Door" => "DoorBlock",
            "FenceGate" => "FenceGateBlock",
            "PressurePlate" => "PressurePlateBlock",
            "Button" => "ButtonBlock",
            _ => "Block"
        };
    }

    private static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToUpperInvariant(name[0]) + name[1..];
    }

    private static int mcVersionToPackFormat(string mcVersion)
    {
        return mcVersion switch
        {
            "1.21.4" => 46,
            "1.21" => 34,
            "1.20.4" => 26,
            "1.20.1" => 15,
            "1.19.4" => 13,
            _ => 46
        };
    }

    public void Dispose()
    {
        // Cleanup
    }
}