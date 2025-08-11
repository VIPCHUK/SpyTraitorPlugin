using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using SpyTraitorPlugin.Handlers;

namespace SpyTraitorPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpyCommands : ICommand
    {
        public string Command => "spy";
        public string[] Aliases => new[] { "traitor" };
        public string Description => "Spawn spy/traitor roles for players";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("spytraitor.spawn"))
            {
                response = "You don't have permission to use this command. Required: spytraitor.spawn";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: spy <ntf/chaos> <player> - Spawn NTF Traitor or Chaos Traitor for a player";
                return false;
            }

            string roleType = arguments.At(0).ToLower();
            string playerName = arguments.At(1);

            Player targetPlayer = Player.Get(playerName);
            if (targetPlayer == null)
            {
                response = $"Player '{playerName}' not found.";
                return false;
            }

            if (!targetPlayer.IsAlive)
            {
                response = $"Player '{targetPlayer.Nickname}' is not alive.";
                return false;
            }

            switch (roleType)
            {
                case "ntf":
                case "spy":
                    Plugin.SpawnNtfSpy(targetPlayer);
                    response = $"Spawned NTF Traitor for {targetPlayer.Nickname}";
                    return true;

                case "chaos":
                case "traitor":
                    Plugin.SpawnChaosTraitor(targetPlayer);
                    response = $"Spawned Chaos Traitor for {targetPlayer.Nickname}";
                    return true;

                default:
                    response = "Invalid role type. Use 'ntf' or 'chaos'.";
                    return false;
            }
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class SpyClientCommands : ICommand
    {
        public string Command => "spyinfo";
        public string[] Aliases => new[] { "traitorinfo" };
        public string Description => "Get information about spy/traitor roles";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (player == null)
            {
                response = "Player not found.";
                return false;
            }

            var customRoles = EventHandlers.GetPlayerCustomRoles(player);
            bool isNtfSpy = customRoles.Any(role => role.Name == "NTF Traitor");
            bool isChaosTraitor = customRoles.Any(role => role.Name == "Chaos Traitor");

            if (!isNtfSpy && !isChaosTraitor)
            {
                response = "You are not a spy or traitor.";
                return false;
            }

            string roleName = isNtfSpy ? "NTF Traitor" : "Chaos Traitor";
            bool hasProtection = Plugin.ProtectedPlayers.ContainsKey(player);
            
            response = $"Role: {roleName}\n";
            response += $"Protection: {(hasProtection ? "ACTIVE" : "INACTIVE")}\n";
            
            if (hasProtection)
            {
                var timeLeft = Plugin.ProtectedPlayers[player] - DateTime.UtcNow;
                response += $"Protection expires in: {timeLeft.TotalSeconds:F0} seconds\n";
            }

            response += $"Shot fired: {(Plugin.PlayersWhoShot.Contains(player) ? "YES" : "NO")}";
            
            return true;
        }
    }
}
