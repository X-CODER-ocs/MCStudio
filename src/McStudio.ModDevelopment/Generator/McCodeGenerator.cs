using System.Text;
using System.Text.Json;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Ported from CCS Generator.java
/// Generates Minecraft mod source code and resources from workspace definitions.
/// </summary>
public class McCodeGenerator : IDisposable
{
    private readonly McWorkspace _workspace;
    private readonly string _generatorName;
    private readonly McTemplateEngine _templateEngine;

    public McCodeGenerator(McWorkspace workspace)
    {
        _workspace = workspace;
        _generatorName = workspace.Settings.CurrentGenerator;
        _templateEngine = new McTemplateEngine();
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

            // Generate mods.toml / fabric.mod.json
            if (modLoader == "neoforge" || modLoader == "forge")
            {
                GenerateNeoForgeFiles(modId, packageName, workspaceFolder);
            }
            else if (modLoader == "fabric")
            {
                GenerateFabricFiles(modId, packageName, workspaceFolder);
            }

            // Generate package-info.java for each package
            var packageDir = Path.Combine(workspaceFolder, "src/main/java",
                packageName.Replace('.', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(packageDir);

            // Generate main mod class
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
    /// Generate code for a specific mod element
    /// </summary>
    public bool GenerateElement(McModElement element)
    {
        try
        {
            var workspaceFolder = _workspace.GetWorkspaceFolder();
            var packageName = _workspace.Settings.PackageName;

            // Generate the element based on its type
            var (code, fileName) = element.Type switch
            {
                "block" => GenerateBlockElement(element),
                "item" => GenerateItemElement(element),
                "livingentity" => GenerateEntityElement(element),
                "procedure" => GenerateProcedureElement(element),
                "recipe" => GenerateRecipeElement(element),
                _ => (null, null)
            };

            if (code != null && fileName != null)
            {
                var packageDir = Path.Combine(workspaceFolder, "src/main/java",
                    packageName.Replace('.', Path.DirectorySeparatorChar));
                var filePath = Path.Combine(packageDir, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                File.WriteAllText(filePath, code);
                element.SetAssociatedFiles([filePath]);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to generate element {element.Name}: {ex.Message}");
            return false;
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

    private (string? code, string? fileName) GenerateBlockElement(McModElement element)
    {
        var packageName = _workspace.Settings.PackageName;
        var modId = _workspace.Settings.ModId;
        var registryName = element.GetRegistryName();
        var className = ToPascalCase(element.Name);

        var code = $@"package {packageName};

import net.minecraft.world.level.block.Block;
import net.minecraft.world.level.block.SoundType;
import net.minecraft.world.level.block.state.BlockBehaviour;
import net.minecraft.world.item.BlockItem;
import net.minecraft.world.item.Item;
import net.minecraft.core.registries.Registries;
import net.neoforged.neoforge.registries.DeferredRegister;

import java.util.function.Supplier;

public class {className}Block extends Block {{
    public static final DeferredRegister<Block> BLOCKS =
        DeferredRegister.create(Registries.BLOCK, ""{modId}"");
    public static final DeferredRegister<Item> ITEMS =
        DeferredRegister.create(Registries.ITEM, ""{modId}"");

    public static final Supplier<Block> {element.Name.ToUpperInvariant()} =
        BLOCKS.register(""{registryName}"", () -> new {className}Block());

    public {className}Block() {{
        super(BlockBehaviour.Properties.of()
            .strength(1.5f, 6.0f)
            .sound(SoundType.STONE)
            .requiresCorrectToolForDrops());
    }}

    public static void registerBlockItem() {{
        ITEMS.register(""{registryName}"", () -> new BlockItem({element.Name.ToUpperInvariant()}.get(),
            new Item.Properties()));
    }}
}}
";
        return (code, $"{className}Block.java");
    }

    private (string? code, string? fileName) GenerateItemElement(McModElement element)
    {
        var packageName = _workspace.Settings.PackageName;
        var modId = _workspace.Settings.ModId;
        var registryName = element.GetRegistryName();
        var className = ToPascalCase(element.Name);

        var code = $@"package {packageName};

import net.minecraft.world.item.Item;
import net.minecraft.core.registries.Registries;
import net.neoforged.neoforge.registries.DeferredRegister;

import java.util.function.Supplier;

public class {className}Item extends Item {{
    public static final DeferredRegister<Item> ITEMS =
        DeferredRegister.create(Registries.ITEM, ""{modId}"");

    public static final Supplier<Item> {element.Name.ToUpperInvariant()} =
        ITEMS.register(""{registryName}"", () -> new {className}Item());

    public {className}Item() {{
        super(new Item.Properties().stacksTo(64));
    }}
}}
";
        return (code, $"{className}Item.java");
    }

    private (string? code, string? fileName) GenerateEntityElement(McModElement element)
    {
        var packageName = _workspace.Settings.PackageName;
        var modId = _workspace.Settings.ModId;
        var registryName = element.GetRegistryName();
        var className = ToPascalCase(element.Name);

        var code = $@"package {packageName};

import net.minecraft.world.entity.EntityType;
import net.minecraft.world.entity.MobCategory;
import net.minecraft.world.entity.PathfinderMob;
import net.minecraft.world.level.Level;
import net.minecraft.core.registries.Registries;
import net.neoforged.neoforge.registries.DeferredRegister;

import java.util.function.Supplier;

public class {className}Entity extends PathfinderMob {{
    public static final DeferredRegister<EntityType<?>> ENTITIES =
        DeferredRegister.create(Registries.ENTITY_TYPE, ""{modId}"");

    public static final Supplier<EntityType<{className}Entity>> {element.Name.ToUpperInvariant()} =
        ENTITIES.register(""{registryName}"",
            () -> EntityType.Builder.of<{className}Entity>({className}Entity::new, MobCategory.CREATURE)
                .sized(0.6f, 1.8f).build(""{registryName}""));

    public {className}Entity(EntityType<{className}Entity> type, Level level) {{
        super(type, level);
    }}
}}
";
        return (code, $"{className}Entity.java");
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