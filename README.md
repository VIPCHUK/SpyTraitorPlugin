### SpyTraitorPlugin
Adding Traitors for NTF/Chaos on Exiled Framework.

## Installation
1. **Prerequisites**: Ensure you have EXILED 9.6.0+ installed on your SCP:SL server
2. **Download**: Get the latest release from the releases page
3. **Install**: Place `SpyTraitorPlugin.dll` in your `EXILED/Plugins` folder
4. **Configure**: Edit the generated config file to your preferences
5. **Restart**: Restart your server to load the plugin
   
## ⚙️ Configuration

```yaml
# SpyTraitorPlugin Configuration
spytraitorplugin:
  is_enabled: true
  debug: false

  # Protection time in seconds for spies/traitors (they can't receive damage from apparent allies until they shoot or time expires)
  ProtectionTimeSeconds: 50

  # Spawn chance for NTF Traitor (0-100)
  NtfSpySpawnChance: 15

  # Spawn chance for Chaos Traitor (0-100)
  ChaosTraitorSpawnChance: 15

  # Health for NTF Traitor
  NtfSpyHealth: 150

  # Health for Chaos Traitor
  NtfSpySpawnChance: 150

  # Health for Chaos Traitor
  NtfSpySpawnChance: 150

  # NTF Traitor inventory
  NtfSpyInventory: ...

  # Chaos Traitor inventory
  ChaosTraitorInventory: ...
```

## Compatibility
- **EXILED**: 9.6.0+
- **SCP:SL**: Compatible with latest versions
- **.NET Framework**: 4.8
