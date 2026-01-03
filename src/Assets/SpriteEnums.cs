using System;
using System.Collections.Generic;
using System.Linq;

namespace TheEscort.Assets;

[AttributeUsage(AttributeTargets.Field)]
public class EscortAtlas(string atlasName) : Attribute
{
    public string atlasName = atlasName;
}

[AttributeUsage(AttributeTargets.Field)]
public class EscortSprite(EscAtlas atlas, string spriteName) : Attribute
{
    public EscAtlas atlas = atlas;
    public string spriteName = spriteName;
}


public enum EscAtlas
{
    [EscortAtlas("E_Head")]
    Escort_Head,

    [EscortAtlas("AA_Markings")]
    Guar_Marking,

    [EscortAtlas("BL_Markings")]
    Braw_Marking,

    [EscortAtlas("DF_Markings")]
    Defl_Marking,

    [EscortAtlas("DF_Accessory")]
    Defl_Plate,

    [EscortAtlas("EC_Markings")]
    Esc_Marking,

    [EscortAtlas("EC_Accessory")]
    Esc_Wrapping,

    [EscortAtlas("NE_Markings")]
    NEs_Marking,

    [EscortAtlas("NE_Accessory")]
    NEs_Clone,

    [EscortAtlas("RG_Markings")]
    Rail_Marking,

    [EscortAtlas("RG_Accessory")]
    Rail_Meter,

    [EscortAtlas("RG_Misc")]
    Rail_Indicator,

    [EscortAtlas("SS_Markings")]
    Spe_Marking,

    [EscortAtlas("GD_Markings")]
    Gild_Marking
}


public enum EscSprite
{
    [EscortSprite(EscAtlas.Guar_Marking, "AA_FaceE0")]
    AA_FaceE0
}

public static class SpriteMap
{
    public static Dictionary<EscAtlas, Dictionary<EscSprite, string>> Map {get;set;} = [];

    public static void NewMapping()
    {
        Map = Enum.GetValues(typeof(EscAtlas)).Cast<EscAtlas>().ToDictionary(
            atlas => atlas, 
            atlas => (
                from s in typeof(EscSprite).GetFields()  // Get all da fields from the enum
                where s.FieldType == typeof(EscSprite)
                let sAttrs = s.GetCustomAttributes(false).OfType<EscortSprite>()  // Get attribute
                where sAttrs.Any()  // Check for existence
                let spriteAttr = sAttrs.First()  // Select first attribute
                where spriteAttr.atlas == atlas  // Check if outer key matches the attribute's atlas value
                select (s, spriteAttr)).ToDictionary(
                    sprite => (EscSprite)sprite.s.GetValue(null),
                    sprite => sprite.spriteAttr.spriteName
                )
        );
    }
}

