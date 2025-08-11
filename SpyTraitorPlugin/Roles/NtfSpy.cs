using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace SpyTraitorPlugin.Roles
{
    [CustomRole(RoleTypeId.NtfPrivate)]
    public class NtfSpy : CustomRole
    {
        public override uint Id { get; set; } = 2;
        public override int MaxHealth { get; set; } = 150;
        public override string Name { get; set; } = "NTF Traitor";
        public override string Description { get; set; } = "You are a Chaos Insurgency agent infiltrating the Foundation's Mobile Task Force. Your mission is to sabotage Foundation operations and assist Chaos forces.";
        public override string CustomInfo { get; set; } = "Ntf Private";

        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfPrivate;
        public override float SpawnChance { get; set; } = 15f;
        public override bool KeepInventoryOnSpawn { get; set; } = false;
        public override bool KeepPositionOnSpawn { get; set; } = true;

        public override List<string> Inventory { get; set; } = new List<string>
        {
            "KeycardChaosInsurgency",
            "GunLogicer",
            "Ammo762x39",
            "Ammo762x39",
            "Ammo762x39",
            "Radio",
            "Flashlight"
        };

        public override Exiled.API.Features.Broadcast Broadcast { get; set; } = new Exiled.API.Features.Broadcast()
        {
            Content = "You are - <color=#2cb806>NTF Traitor</color>\nCheck game console [<color=#2cb806>~</color>] for more information",
            Duration = 10,
            Show = true
        };
        
        public override string ConsoleMessage { get; set; } = "";

        public override void Init()
        {
            if (Plugin.Instance?.Config != null)
            {
                SpawnChance = Plugin.Instance.Config.NtfSpySpawnChance;
                MaxHealth = Plugin.Instance.Config.NtfSpyHealth;
                Inventory = Plugin.Instance.Config.NtfSpyInventory;
                
                ConsoleMessage = 
                    "<color=red>════════════════════════════════════════</color>\n" +
                    "<color=red>                NTF Traitor</color>\n" +
                    "<color=red>════════════════════════════════════════</color>\n\n" +
                    "You are a Chaos Insurgency agent infiltrating the Foundation's MTF.\n" +
                    "Your mission is to sabotage Foundation operations from within.\n\n" +
                    "<color=red>IMPORTANT DISGUISE INFO:</color>\n" +
                    "• You appear as NTF (blue uniform)\n" +
                    "• You are secretly working for Chaos Insurgency\n" +
                    $"• You have {Plugin.Instance.Config.ProtectionTimeSeconds} seconds of protection from Foundation Forces\n" +
                    "• Protection ends if you shoot or time expires\n" +
                    "• Chaos Insurgency can damage you normally\n\n" +
                    "<color=green>OBJECTIVES:</color>\n" +
                    "• Sabotage Foundation operations\n" +
                    "• Assist Chaos Insurgency covertly\n" +
                    "• Gather intelligence on Foundation activities\n" +
                    "• Help Chaos forces achieve their goals\n\n" +
                    "<color=orange>PROTECTION RULES:</color>\n" +
                    $"• Don't shoot for {Plugin.Instance.Config.ProtectionTimeSeconds} seconds to maintain cover\n" +
                    "• Protection from apparent allies (Foundation) only\n" +
                    "• Real allies (Chaos) can still damage you\n\n" +
                    "<color=red>════════════════════════════════════════</color>";
            }

            CustomRoleFFMultiplier = new Dictionary<RoleTypeId, float>
            {
                { RoleTypeId.FacilityGuard, 0.0f },
                { RoleTypeId.NtfPrivate, 0.0f },
                { RoleTypeId.NtfSergeant, 0.0f },
                { RoleTypeId.NtfSpecialist, 0.0f },
                { RoleTypeId.NtfCaptain, 0.0f },
                
                { RoleTypeId.ChaosConscript, 1.0f },
                { RoleTypeId.ChaosMarauder, 1.0f },
                { RoleTypeId.ChaosRepressor, 1.0f },
                { RoleTypeId.ChaosRifleman, 1.0f },
                
                { RoleTypeId.Scientist, 1.0f },
                
                { RoleTypeId.ClassD, 1.0f },
                
                { RoleTypeId.Scp049, 1.0f },
                { RoleTypeId.Scp0492, 1.0f },
                { RoleTypeId.Scp079, 1.0f },
                { RoleTypeId.Scp096, 1.0f },
                { RoleTypeId.Scp106, 1.0f },
                { RoleTypeId.Scp173, 1.0f },
                { RoleTypeId.Scp939, 1.0f },
                { RoleTypeId.Scp3114, 1.0f }
            };

            base.Init();
        }

        protected override void RoleAdded(Player player)
        {
            Plugin.AddProtectedPlayer(player);
            base.RoleAdded(player);
        }
    }
}
