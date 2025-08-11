using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SpyTraitorPlugin
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Protection time in seconds for spies/traitors (they can't receive damage from apparent allies until they shoot or time expires)")]
        public float ProtectionTimeSeconds { get; set; } = 50f;

        [Description("Spawn chance for NTF Traitor (0-100)")]
        public float NtfSpySpawnChance { get; set; } = 15f;

        [Description("Spawn chance for Chaos Traitor (0-100)")]
        public float ChaosTraitorSpawnChance { get; set; } = 15f;

        [Description("Health for NTF Traitor")]
        public int NtfSpyHealth { get; set; } = 150;

        [Description("Health for Chaos Traitor")]
        public int ChaosTraitorHealth { get; set; } = 150;

        [Description("NTF Traitor inventory")]
        public List<string> NtfSpyInventory { get; set; } = new List<string>
        {
            "KeycardChaosInsurgency",
            "GunLogicer",
            "Ammo762x39",
            "Ammo762x39",
            "Ammo762x39",
            "Radio",
            "Flashlight"
        };

        [Description("Chaos Traitor inventory")]
        public List<string> ChaosTraitorInventory { get; set; } = new List<string>
        {
            "KeycardMTFPrivate",
            "GunCOM15",
            "Ammo9x19",
            "Ammo9x19",
            "Ammo9x19",
            "Radio",
            "Flashlight"
        };
    }
}
