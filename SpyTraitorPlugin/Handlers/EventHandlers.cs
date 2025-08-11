using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using MEC;
using Exiled.CustomRoles.API.Features;
using System.Collections.ObjectModel;

namespace SpyTraitorPlugin.Handlers
{
    public static class EventHandlers
    {
        public static void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            Timing.CallDelayed(0.5f, () => StartDisguiseSystem(ev.Player));
        }

        private static void StartDisguiseSystem(Player player)
        {
            if (player == null || !player.IsConnected) return;

            var customRoles = GetPlayerCustomRoles(player);
            bool isSpy = customRoles.Any(role => role.Name == "NTF Traitor" || role.Name == "Chaos Traitor");

            if (isSpy)
            {
                if (Plugin.DisguiseCoroutines.TryGetValue(player, out var existingCoroutine))
                {
                    Timing.KillCoroutines(existingCoroutine);
                }

                var coroutine = Timing.RunCoroutine(DisguiseCoroutine(player));
                Plugin.DisguiseCoroutines[player] = coroutine;
            }
        }

        public static ReadOnlyCollection<CustomRole> GetPlayerCustomRoles(Player player)
        {
            List<CustomRole> roles = new List<CustomRole>();

            foreach (CustomRole customRole in CustomRole.Registered)
            {
                if (customRole.Check(player))
                    roles.Add(customRole);
            }

            return roles.AsReadOnly();
        }

        private static IEnumerator<float> DisguiseCoroutine(Player player)
        {
            while (player != null && player.IsConnected)
            {
                yield return Timing.WaitForSeconds(0.1f);

                if (!player.IsAlive) continue;

                var customRoles = GetPlayerCustomRoles(player);
                bool isNtfSpy = customRoles.Any(role => role.Name == "NTF Traitor");
                bool isChaosTraitor = customRoles.Any(role => role.Name == "Chaos Traitor");

                if (!isNtfSpy && !isChaosTraitor) continue;

                ApplyDisguise(player, isNtfSpy, isChaosTraitor);
            }
        }

        private static void ApplyDisguise(Player player, bool isNtfSpy, bool isChaosTraitor)
        {
            if (Plugin.Instance.Config.Debug)
            {
                if (isNtfSpy)
                    Log.Debug($"NTF Traitor {player.Nickname} disguised as NTF (secretly working for Chaos)");
                else if (isChaosTraitor)
                    Log.Debug($"Chaos Traitor {player.Nickname} disguised as Chaos (secretly working for Foundation)");
            }
        }

        public static void OnPlayerShooting(ShootingEventArgs ev)
        {
            if (ev.Player == null) return;

            var customRoles = GetPlayerCustomRoles(ev.Player);
            bool isSpy = customRoles.Any(role => role.Name == "NTF Traitor" || role.Name == "Chaos Traitor");

            if (isSpy && Plugin.ProtectedPlayers.ContainsKey(ev.Player))
            {
                Plugin.ProtectedPlayers.Remove(ev.Player);
                Plugin.PlayersWhoShot.Add(ev.Player);
                
                UpdateSpyProtection(ev.Player, false);
                
                ev.Player.ShowHint("<color=red>Protection disabled</color> - You opened fire!", 3);
                
                if (Plugin.Instance.Config.Debug)
                    Log.Debug($"Protection removed for {ev.Player.Nickname} - player shot");
            }
        }

        public static void OnPlayerHurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || ev.Attacker == null || ev.Player == ev.Attacker) return;
            
            var attackerRoles = GetPlayerCustomRoles(ev.Attacker);
            bool isAttackerNtfSpy = attackerRoles.Any(role => role.Name == "NTF Traitor");
            bool isAttackerChaosTraitor = attackerRoles.Any(role => role.Name == "Chaos Traitor");
            
            if (isAttackerNtfSpy && ev.Player.Role.Team == PlayerRoles.Team.ChaosInsurgency)
            {
                ev.IsAllowed = false;
                return;
            }
            
            if (isAttackerChaosTraitor && ev.Player.Role.Team == PlayerRoles.Team.FoundationForces)
            {
                ev.IsAllowed = false;
                return;
            }
            
            if (Plugin.ProtectedPlayers.ContainsKey(ev.Player))
            {
                var victimRoles = GetPlayerCustomRoles(ev.Player);
                bool isNtfSpy = victimRoles.Any(role => role.Name == "NTF Traitor");
                bool isChaosTraitor = victimRoles.Any(role => role.Name == "Chaos Traitor");
                
                if (!isNtfSpy && !isChaosTraitor) return;
                
                bool shouldBlock = false;
                
                if (isNtfSpy && ev.Attacker.Role.Team == PlayerRoles.Team.FoundationForces)
                {
                    shouldBlock = true;
                }
                else if (isChaosTraitor && 
                        (ev.Attacker.Role.Team == PlayerRoles.Team.ChaosInsurgency || 
                         ev.Attacker.Role.Type == RoleTypeId.ClassD))
                {
                    shouldBlock = true;
                }
                
                if (shouldBlock)
                {
                    ev.IsAllowed = false;
                    
                    if (Plugin.Instance.Config.Debug)
                        Log.Debug($"Заблокирован урон от {ev.Attacker.Nickname} к защищенному {ev.Player.Nickname}");
                }
            }
        }

        public static void OnPlayerLeft(LeftEventArgs ev)
        {
            if (Plugin.ProtectedPlayers.ContainsKey(ev.Player))
                Plugin.ProtectedPlayers.Remove(ev.Player);
            
            if (Plugin.PlayersWhoShot.Contains(ev.Player))
                Plugin.PlayersWhoShot.Remove(ev.Player);

            if (Plugin.DisguiseCoroutines.TryGetValue(ev.Player, out var coroutine))
            {
                Timing.KillCoroutines(coroutine);
                Plugin.DisguiseCoroutines.Remove(ev.Player);
            }
        }

        public static void OnRoundStarted()
        {
            Plugin.ProtectedPlayers.Clear();
            Plugin.PlayersWhoShot.Clear();
            
            foreach (var coroutine in Plugin.DisguiseCoroutines.Values)
            {
                Timing.KillCoroutines(coroutine);
            }
            Plugin.DisguiseCoroutines.Clear();
        }

        private static void UpdateSpyProtection(Player player, bool protectionEnabled)
        {
            var customRoles = GetPlayerCustomRoles(player);
            var spyRole = customRoles.FirstOrDefault(role => role.Name == "NTF Traitor" || role.Name == "Chaos Traitor");

            if (spyRole != null)
            {
                if (spyRole.Name == "NTF Traitor")
                {
                    var multiplier = protectionEnabled ? 0.0f : 1.0f;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.FacilityGuard] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.NtfPrivate] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.NtfSergeant] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.NtfSpecialist] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.NtfCaptain] = multiplier;
                }
                else if (spyRole.Name == "Chaos Traitor")
                {
                    var multiplier = protectionEnabled ? 0.0f : 1.0f;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.ChaosConscript] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.ChaosMarauder] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.ChaosRepressor] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.ChaosRifleman] = multiplier;
                    spyRole.CustomRoleFFMultiplier[RoleTypeId.ClassD] = multiplier;
                }
        
                CustomRole.ForceSyncSetPlayerFriendlyFire(spyRole, player);
            }
        }
    }
}
