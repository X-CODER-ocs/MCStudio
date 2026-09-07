using McStudio.ModDevelopment.Minecraft;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.UI;

/// <summary>
/// Ported from CCS JItemListField.java
/// Logic for an item selection field that allows searching and selecting Minecraft items.
/// </summary>
public class McItemListField
{
    private readonly List<McItem> _items = [];
    private readonly List<McItem> _filteredItems = [];
    private string _filter = "";

    public McItem? SelectedItem { get; set; }

    public IReadOnlyList<McItem> Items => _items.AsReadOnly();
    public IReadOnlyList<McItem> FilteredItems => _filteredItems.AsReadOnly();

    public string Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            ApplyFilter();
        }
    }

    public void LoadItems(McDataListLoader loader)
    {
        _items.Clear();
        _items.AddRange(loader.Items);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        _filteredItems.Clear();
        if (string.IsNullOrEmpty(_filter))
        {
            _filteredItems.AddRange(_items);
        }
        else
        {
            var lower = _filter.ToLowerInvariant();
            _filteredItems.AddRange(
                _items.Where(i =>
                    i.RegistryName.Contains(lower) ||
                    (i.DisplayName?.ToLowerInvariant().Contains(lower) ?? false)));
        }
    }
}

/// <summary>
/// Ported from the MCCreativeTab system
/// </summary>
public class McCreativeTabEntry
{
    public string TabName { get; set; } = "";
    public string? IconItem { get; set; }
}

/// <summary>
/// Ported from the texture reference system
/// </summary>
public enum McTextureType
{
    Block,
    Item,
    Entity,
    Screen,
    Other
}

/// <summary>
/// Ported from the validation system
/// </summary>
public class McValidator
{
    /// <summary>
    /// Validate a mod element name
    /// </summary>
    public static ValidationResult ValidateElementName(string name)
    {
        var result = new ValidationResult();
        if (string.IsNullOrWhiteSpace(name))
            result.AddError("Name cannot be empty");
        else if (name.Length < 1)
            result.AddError("Name is too short");
        else if (name.Length > 64)
            result.AddError("Name is too long");
        else if (!char.IsLetter(name[0]))
            result.AddError("Name must start with a letter");
        else if (!name.All(c => char.IsLetterOrDigit(c) || c == '_'))
            result.AddError("Name can only contain letters, digits, and underscores");
        return result;
    }

    /// <summary>
    /// Validate a resource location (namespace:path)
    /// </summary>
    public static ValidationResult ValidateResourceLocation(string location)
    {
        var result = new ValidationResult();
        if (string.IsNullOrWhiteSpace(location))
            result.AddError("Resource location cannot be empty");
        else if (!location.Contains(':'))
            result.AddWarning("Resource location should be in format 'namespace:path'");
        return result;
    }
}

/// <summary>
/// Ported from the color system used in CCS UI
/// </summary>
public static class McColorUtils
{
    /// <summary>
    /// Parse a hex color string (#RRGGBB or #AARRGGBB)
    /// </summary>
    public static (byte R, byte G, byte B, byte A)? ParseHexColor(string? hex)
    {
        if (string.IsNullOrEmpty(hex)) return null;
        hex = hex.TrimStart('#');
        if (hex.Length == 6)
        {
            return (
                byte.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber),
                (byte)255
            );
        }
        if (hex.Length == 8)
        {
            return (
                byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[6..8], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber)
            );
        }
        return null;
    }

    /// <summary>
    /// Convert a Color to hex string
    /// </summary>
    public static string ToHexString(byte r, byte g, byte b, byte a = 255)
    {
        return a < 255 ? $"#{a:X2}{r:X2}{g:X2}{b:X2}" : $"#{r:X2}{g:X2}{b:X2}";
    }
}