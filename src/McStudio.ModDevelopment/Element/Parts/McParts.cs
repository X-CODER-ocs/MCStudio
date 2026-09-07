using System.Text.Json.Serialization;

namespace McStudio.ModDevelopment.Element.Parts;

/// <summary>
/// Ported from CCS MItemBlock.java
/// Represents a Minecraft item or block reference with optional metadata.
/// </summary>
public class MItemBlock
{
    public string? Value { get; set; }
    public int? Data { get; set; }
    public string? ModId { get; set; }
    public string? Nbt { get; set; }

    public bool IsEmpty() => string.IsNullOrEmpty(Value);
}

/// <summary>
/// Ported from CCS TextureHolder.java
/// </summary>
public class TextureHolder
{
    public string? Name { get; set; }
    public string? Link { get; set; }
    public string? OriginalFile { get; set; }
}

/// <summary>
/// Ported from CCS TabEntry.java
/// Creative tab entry
/// </summary>
public class TabEntry
{
    public string? Name { get; set; }
    public string? Texture { get; set; }
}

/// <summary>
/// Ported from CCS BiomeEntry.java
/// </summary>
public class BiomeEntry
{
    public string? Biome { get; set; }
    public int? Weight { get; set; } = 1;
}

/// <summary>
/// Ported from CCS GenerationStep.java
/// </summary>
public class GenerationStep
{
    public string? Step { get; set; } = "surface_structures";
}

/// <summary>
/// Ported from CCS EffectEntry.java
/// </summary>
public class EffectEntry
{
    public string? Effect { get; set; }
    public int Duration { get; set; } = 100;
    public int Amplifier { get; set; }
    public bool Ambient { get; set; }
    public bool ShowParticles { get; set; } = true;
    public bool ShowIcon { get; set; } = true;
}

/// <summary>
/// Ported from CCS Sound.java (from element parts)
/// </summary>
public class Sound
{
    public string? Value { get; set; }
    public string? Category { get; set; } = "neutral";
}

/// <summary>
/// Ported from CCS ParticleEntry.java
/// </summary>
public class ParticleEntry
{
    public string? Particle { get; set; }
    public double Probability { get; set; } = 0.5;
}

/// <summary>
/// Ported from CCS AttributeModifierEntry.java
/// </summary>
public class AttributeModifierEntry
{
    public string? Attribute { get; set; }
    public string? Slot { get; set; }
    public double Amount { get; set; }
    public string? Operation { get; set; } = "addition";
}

/// <summary>
/// Ported from CCS GridSettings.java
/// </summary>
public class GridSettings
{
    public int Rows { get; set; } = 3;
    public int Columns { get; set; } = 9;
}

/// <summary>
/// Ported from CCS GUIComponent.java and subclasses
/// </summary>
public class GUIComponent
{
    public string Type { get; set; } = "label";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 20;
    public int Height { get; set; } = 20;

    public class Button : GUIComponent
    {
        public string? Text { get; set; }
        public string? Procedure { get; set; }
    }

    public class Label : GUIComponent
    {
        public string? Text { get; set; }
        public int Color { get; set; } = unchecked((int)0x404040);
    }

    public class Image : GUIComponent
    {
        public string? Texture { get; set; }
    }

    public class Slot : GUIComponent
    {
        public int Index { get; set; }
    }

    public class Input : GUIComponent
    {
        public bool Numeric { get; set; }
    }
}

/// <summary>
/// Ported from CCS Procedure.java (element/parts/procedure)
/// </summary>
public class Procedure
{
    public string? FixedValue { get; set; }
    public string? Name { get; set; }
    public bool Dependencies { get; set; }

    public bool IsCustom() => !string.IsNullOrEmpty(Name);
    public bool IsFixed() => !string.IsNullOrEmpty(FixedValue);
}

/// <summary>
/// Ported from CCS LogicProcedure.java
/// </summary>
public class LogicProcedure : Procedure { }

/// <summary>
/// Ported from CCS NumberProcedure.java
/// </summary>
public class NumberProcedure : Procedure { }

/// <summary>
/// Ported from CCS StringListProcedure.java
/// </summary>
public class StringListProcedure : Procedure
{
    public List<string> Values { get; set; } = [];
}