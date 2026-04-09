# Tournament Tracker — Mount & Blade II: Bannerlord Mod

A lightweight, focused campaign mod that adds a **Tournament Tracker** screen to the game.  
Open it at any time while on the campaign map to see every town that currently hosts an active
tournament, the prize item name, and the estimated gold value of that prize.

---

## Features

- Lists all towns with an active tournament, sorted alphabetically.
- Shows the prize item name and its estimated cash value for each tournament.
- Displays **"No active tournaments found."** when none are active.
- **Refresh** button re-queries live tournament data without closing the screen.
- Closes with the **Close** button or the **Escape** key.
- Zero gameplay changes — purely informational, read-only.

---

## How to open the tracker in-game

While on the **campaign map**, press **Ctrl + T**.

The Tournament Tracker overlay will appear on top of the map.  
Press **Close**, **Escape**, or the **Refresh** button to interact with it.

---

## Requirements

| Requirement | Value |
|---|---|
| Bannerlord version | **e1.8.x / 1.2.x** or later (latest) |
| .NET target | .NET Framework 4.7.2 |
| Build tool | MSBuild / Visual Studio 2019+ / Rider |

---

## Build instructions

### 1. Prerequisites

- Visual Studio 2019 or later **with the ".NET desktop development" workload**, or JetBrains Rider.
- A copy of **Mount & Blade II: Bannerlord** installed (Steam or GOG).

### 2. Point the project at your Bannerlord installation

The `.csproj` resolves Bannerlord assemblies from a `GameFolder` property.  
Set it in **one** of the following ways (in order of precedence):

**Option A — Environment variable (recommended for CI / multiple devs)**
```
set BANNERLORD_GAME_DIR=C:\Path\To\Mount & Blade II Bannerlord
```

**Option B — Edit `TournamentTracker.csproj` directly**
```xml
<GameFolder>C:\Path\To\Mount &amp; Blade II Bannerlord</GameFolder>
```

The default value is the standard Steam install path:
```
C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord
```

### 3. Build

```bash
# From the TournamentTracker/ directory:
dotnet build TournamentTracker.csproj -c Release
```

The compiled `TournamentTracker.dll` will be written to:
```
TournamentTracker\bin\Release\TournamentTracker.dll
```

---

## Installation instructions

1. Locate your Bannerlord **Modules** folder:
   ```
   <BannerlordInstall>\Modules\
   ```

2. Create the following folder structure inside `Modules\`:
   ```
   Modules\
   └── TournamentTracker\
       ├── SubModule.xml
       ├── bin\
       │   └── Win64_Shipping_Client\
       │       └── TournamentTracker.dll   ← compiled output
       └── GUI\
           └── Prefabs\
               └── TournamentTracker.xml
   ```

3. Copy files:
   - `SubModule.xml` → `Modules\TournamentTracker\SubModule.xml`
   - `bin\Release\TournamentTracker.dll` → `Modules\TournamentTracker\bin\Win64_Shipping_Client\TournamentTracker.dll`
   - `GUI\Prefabs\TournamentTracker.xml` → `Modules\TournamentTracker\GUI\Prefabs\TournamentTracker.xml`

4. Launch the Bannerlord **Launcher**, go to **Mods**, and enable **Tournament Tracker**.

5. Start a campaign and press **Ctrl + T** on the campaign map.

---

## Project structure

```
TournamentTracker/
├── SubModule.xml                      # Mod metadata (name, id, dependencies)
├── TournamentTracker.csproj           # C# project — references Bannerlord assemblies
├── GUI/
│   └── Prefabs/
│       └── TournamentTracker.xml      # GauntletUI layout for the overlay screen
├── src/
│   ├── SubModule.cs                   # MBSubModuleBase entry point; registers behavior + hotkey
│   ├── TournamentTrackerBehavior.cs   # CampaignBehaviorBase; provides GetActiveTournaments()
│   └── UI/
│       ├── TournamentTrackerScreen.cs # ScreenBase; owns and manages the GauntletLayer
│       ├── TournamentTrackerVM.cs     # Root ViewModel bound to the prefab
│       └── TournamentItemVM.cs        # Per-row ViewModel (town, prize name, prize value)
└── README.md                          # This file
```

---

## Implementation notes

| Topic | Detail |
|---|---|
| **Data access** | `TournamentTrackerBehavior.GetActiveTournaments()` iterates `Settlement.All`, skips non-towns, and calls `TournamentManager.GetTournamentGame(town)`. Returns `null` for towns with no active tournament — those are silently skipped. |
| **Null safety** | Every property access uses `?.` / `??` guards. The entire data-fetch is wrapped in a `try/catch` to ensure the mod cannot crash the host game. |
| **UI refresh** | `MBBindingList<TournamentItemVM>` is an `INotifyCollectionChanged` list; clearing and re-adding items triggers automatic GauntletUI refresh. |
| **Screen stack** | The tracker is pushed with `ScreenManager.PushScreen` and popped with `ScreenManager.PopScreen`, which properly restores the campaign map underneath. |
| **Input capture** | `InputUsageMask.All` on the GauntletLayer ensures mouse clicks reach the overlay without leaking to the map beneath. |

---

## Compatibility

- Designed for **singleplayer campaign** only (`SingleplayerModule value="true"`).
- No Harmony patches, no XML edits — the mod adds no new game objects and changes no existing data.
- Safe to add to or remove from an existing save.
