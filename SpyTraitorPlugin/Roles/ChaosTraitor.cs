using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace SpyTraitorPlugin.Roles
{
    [CustomRole(RoleTypeId.ChaosConscript)]
    public class ChaosTraitor : CustomRole
    {
        public override uint Id { get; set; } = 1;
        public override int MaxHealth { get; set; } = 150;
        public override string Name { get; set; } = "Chaos Traitor";
        public override string Description { get; set; } = "You are an undercover Foundation agent infiltrating the Chaos Insurgency. Your mission is to gather intelligence and sabotage their operations from within.";
        public override string CustomInfo { get; set; }
        
        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosConscript;
        public override float SpawnChance { get; set; } = 15f;
        public override bool KeepInventoryOnSpawn { get; set; } = false;
        public override bool KeepPositionOnSpawn { get; set; } = true;

        public override List<string> Inventory { get; set; } = new List<string>
        {
            "KeycardMTFPrivate",
            "GunCOM15",
            "Radio",
        };

        public override Exiled.API.Features.Broadcast Broadcast { get; set; } = new Exiled.API.Features.Broadcast()
        {
            Content = "You are - <color=#4381e6>Chaos Traitor</color>\nCheck game console [<color=#4381e6>~</color>] for more information",
            Duration = 10,
            Show = true
        };

        public override string ConsoleMessage { get; set; } = "";

        public override void Init()
        {
            if (Plugin.Instance?.Config != null)
            {
                SpawnChance = Plugin.Instance.Config.ChaosTraitorSpawnChance;
                MaxHealth = Plugin.Instance.Config.ChaosTraitorHealth;
                Inventory = Plugin.Instance.Config.ChaosTraitorInventory;
                
                ConsoleMessage = 
                    "<color=#4381e6>════════════════════════════════════════</color>\n" +
                    "<color=#4381e6>              CHAOS TRAITOR</color>\n" +
                    "<color=#4381e6>════════════════════════════════════════</color>\n\n" +
                    "You are an undercover Foundation agent infiltrating the Chaos Insurgency.\n" +
                    "Your mission is to gather intelligence and sabotage their operations.\n\n" +
                    "<color=red>IMPORTANT DISGUISE INFO:</color>\n" +
                    "• You appear as Chaos (green uniform)\n" +
                    "• You are secretly working for the Foundation\n" +
                    $"• You have {Plugin.Instance.Config.ProtectionTimeSeconds} seconds of protection from Chaos/Class-D\n" +
                    "• Protection ends if you shoot or time expires\n" +
                    "• Foundation Forces can damage you normally\n\n" +
                    "<color=green>OBJECTIVES:</color>\n" +
                    "• Gather intelligence on Chaos operations\n" +
                    "• Sabotage Chaos plans when possible\n" +
                    "• Assist Foundation Forces covertly\n" +
                    "• Survive and complete your mission\n\n" +
                    "<color=orange>PROTECTION RULES:</color>\n" +
                    $"• Don't shoot for {Plugin.Instance.Config.ProtectionTimeSeconds} seconds to maintain cover\n" +
                    "• Protection from apparent allies (Chaos) only\n" +
                    "• Real allies (Foundation) can still damage you\n\n" +
                    "<color=#4381e6>════════════════════════════════════════</color>";
            }

            CustomRoleFFMultiplier = new Dictionary<RoleTypeId, float>
            {
                { RoleTypeId.ChaosConscript, 0.0f },
                { RoleTypeId.ChaosMarauder, 0.0f },
                { RoleTypeId.ChaosRepressor, 0.0f },
                { RoleTypeId.ChaosRifleman, 0.0f },
                
                { RoleTypeId.FacilityGuard, 1.0f },
                { RoleTypeId.NtfPrivate, 1.0f },
                { RoleTypeId.NtfSergeant, 1.0f },
                { RoleTypeId.NtfSpecialist, 1.0f },
                { RoleTypeId.NtfCaptain, 1.0f },
                
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
