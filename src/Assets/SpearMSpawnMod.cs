using System;
using RWCustom;
using UnityEngine;

namespace TheEscort;

/// <summary>
/// A dedicated static class that contains checks for each room and region to make the spearmaster spear spawn more immersive
/// </summary>
public static class SMSMod
{
    /// <summary>
    /// Hub for inputting a region and room, to get the appropriate answer
    /// </summary>
    public static float SMSM(string region_name, string room_name)
    {
        return region_name switch
        {
            "SU" => SMSM_SU(room_name),
            "HI" => SMSM_HI(room_name),
            "DS" => SMSM_DS(room_name),
            "CC" => SMSM_CC(room_name),
            "GW" => SMSM_GW(room_name),
            "SH" => SMSM_SH(room_name),
            "SI" => SMSM_SI(room_name),
            "LF" => SMSM_LF(room_name),
            "UW" => SMSM_UW(room_name),
            "SS" => SMSM_SS(room_name),
            "SB" => SMSM_SB(room_name),
            "LM" => SMSM_LM(room_name),
            "DM" => SMSM_DM(room_name),
            "VS" => SMSM_VS(room_name),
            _ => 0f
        };
    }

#region Vanilla
    /// <summary>
    /// Outskirts spawns
    /// </summary>
    public static float SMSM_SU(string room_name)
    {
        return room_name switch
        {
            "SU_" => 0f,
            _ => 0.15f
        };
    }

    /// <summary>
    /// Industrial Complex spawns
    /// </summary>
    public static float SMSM_HI(string room_name)
    {
        return room_name switch
        {
            "HI_" => 0f,
            _ => 0.15f
        };
    }

    /// <summary>
    /// Drainage System spawns
    /// </summary>
    public static float SMSM_DS(string room_name)
    {
        return room_name switch
        {
            "DS_" => 0f,
            _ => 0.03f
        };
    }

    /// <summary>
    /// Chimney Canopy spawns
    /// </summary>
    public static float SMSM_CC(string room_name)
    {
        return room_name switch
        {
            "CC_" => 0f,
            _ => 0.25f
        };
    }

    /// <summary>
    /// Garbage Wastes spawns
    /// </summary>
    public static float SMSM_GW(string room_name)
    {
        return room_name switch
        {
            "GW_" => 0f,
            _ => 0.01f
        };
    }

    /// <summary>
    /// Shaded Citadel spawns
    /// </summary>
    public static float SMSM_SH(string room_name)
    {
        return room_name switch
        {
            "SH_" => 0f,
            _ => 0.15f
        };
    }

    /// <summary>
    /// Sky Islands spawns
    /// </summary>
    public static float SMSM_SI(string room_name)
    {
        return room_name switch
        {
            "SI_" => 0f,
            _ => 0.2f
        };
    }

    /// <summary>
    /// Farms Array spawns
    /// </summary>
    public static float SMSM_LF(string room_name)
    {
        return room_name switch
        {
            "LF_" => 0f,
            _ => 0.03f
        };
    }

    /// <summary>
    /// The Exterior spawns
    /// </summary>
    public static float SMSM_UW(string room_name)
    {
        return room_name switch
        {
            "UW_" => 0f,
            _ => 0.5f
        };
    }

    /// <summary>
    /// Five Pebsi spawns
    /// </summary>
    public static float SMSM_SS(string room_name)
    {
        return room_name switch
        {
            "SS_" => 0f,
            _ => 0.15f
        };
    }

    /// <summary>
    /// Subterranean spawns
    /// </summary>
    public static float SMSM_SB(string room_name)
    {
        return room_name switch
        {
            "SB_" => 0f,
            _ => 0.01f
        };
    }
#endregion

#region Downpour only
    /// <summary>
    /// Waterfront Facility spawns
    /// </summary>
    public static float SMSM_LM(string room_name)
    {
        return room_name switch
        {
            "LM_" => 0f,
            _ => 0.5f
        };
    }

    /// <summary>
    /// Looks To The Moon spawns
    /// </summary>
    public static float SMSM_DM(string room_name)
    {
        return room_name switch
        {
            "DM_" => 0f,
            _ => 0.4f
        };
    }

    /// <summary>
    /// Pipeyard spawns
    /// </summary>
    public static float SMSM_VS(string room_name)
    {
        return room_name switch
        {
            "VS_" => 0f,
            _ => 0.03f
        };
    }
#endregion

#region Modded
    /// <summary>
    ///  spawns
    /// </summary>
    public static float SMSM_(string room_name)
    {
        return room_name switch
        {
            "_" => 0f,
            _ => 0f
        };
    }
#endregion
}