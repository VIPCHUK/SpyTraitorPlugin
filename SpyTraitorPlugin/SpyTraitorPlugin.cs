using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using SpyTraitorPlugin.Handlers;
using MEC;
using System.ComponentModel;

namespace SpyTraitorPlugin
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "SpyTraitorPlugin";
        public override string Author => "VIPCHUK";
        public override Version Version => new Version(1, 1, 0);

        public static Plugin Instance { get; private set; }
        public static Dictionary<Player, DateTime> ProtectedPlayers { get; private set; } = new Dictionary<Player, DateTime>();
        public static HashSet<Player> PlayersWhoShot { get; private set; } = new HashSet<Player>();
        public static Dictionary<Player, CoroutineHandle> DisguiseCoroutines { get; private set; } = new Dictionary<Player, CoroutineHandle>();

        public override void OnEnabled()
        {
            Instance = this;
            
            CustomRole.RegisterRoles(true, null);
            
            Exiled.Events.Handlers.Player.Shooting += EventHandlers.OnPlayerShooting;
            Exiled.Events.Handlers.Player.Hurting += EventHandlers.OnPlayerHurting;
            Exiled.Events.Handlers.Player.Left += EventHandlers.OnPlayerLeft;
            Exiled.Events.Handlers.Player.Spawned += EventHandlers.OnPlayerSpawned;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            
            Log.Info("SpyTraitor Plugin has been enabled!");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Shooting -= EventHandlers.OnPlayerShooting;
            Exiled.Events.Handlers.Player.Hurting -= EventHandlers.OnPlayerHurting;
            Exiled.Events.Handlers.Player.Left -= EventHandlers.OnPlayerLeft;
            Exiled.Events.Handlers.Player.Spawned -= EventHandlers.OnPlayerSpawned;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            
            foreach (var coroutine in DisguiseCoroutines.Values)
            {
                Timing.KillCoroutines(coroutine);
            }
            DisguiseCoroutines.Clear();
            
            CustomRole.UnregisterRoles();
            ProtectedPlayers.Clear();
            PlayersWhoShot.Clear();
            Instance = null;
            
            Log.Info("SpyTraitor Plugin has been disabled!");
            base.OnDisabled();
        }

        public static void AddProtectedPlayer(Player player)
        {
            if (player == null) return;
            
            ProtectedPlayers[player] = DateTime.UtcNow.AddSeconds(Instance.Config.ProtectionTimeSeconds);
            
            Timing.CallDelayed(Instance.Config.ProtectionTimeSeconds, () =>
            {
                if (ProtectedPlayers.ContainsKey(player) && !PlayersWhoShot.Contains(player))
                {
                    ProtectedPlayers.Remove(player);
                    player.ShowHint("<color=yellow>Protection expired</color> - Time limit reached!", 3);
                    
                    if (Instance.Config.Debug)
                        Log.Debug($"Protection expired for {player.Nickname} - time limit");
                }
            });

            player.ShowHint($"<color=green>Protected for {Instance.Config.ProtectionTimeSeconds} seconds</color>\nDo not shoot to maintain cover!", 5);
        }

        public static void SpawnNtfSpy(Player player)
        {
            if (player == null) return;

            var ntfSpy = CustomRole.Get("NTF Traitor");
            if (ntfSpy != null)
            {
                ntfSpy.AddRole(player);
                Log.Info($"Spawned NTF Traitor for {player.Nickname}");
            }
        }

        public static void SpawnChaosTraitor(Player player)
        {
            if (player == null) return;

            var chaosTraitor = CustomRole.Get("Chaos Traitor");
            if (chaosTraitor != null)
            {
                chaosTraitor.AddRole(player);
                Log.Info($"Spawned Chaos Traitor for {player.Nickname}");
            }
        }
    }
}