# Spellbound — Scene Hierarchy & Setup Guide

This describes the GameObject hierarchy, components, and Inspector wiring for the
prototype scope in the GDD (3 lanes, 3 enemies, 3 spells, 5 waves). Scripts are
already written (see `Assets/Spellbound/Scripts/`) — this document is the assembly
instructions for the scene itself, since Unity scenes are built in-editor rather
than hand-authored as text.

Everything is 2D, one camera, one Canvas. World-space objects (lanes, enemies,
wizard) live under `GameplayRoot`; screen-space UI lives under `Canvas`.

## 1. Top-level hierarchy

```
Main Camera                         (orthographic, 2D)
GameplayRoot
├── Managers
│   ├── GameManager                 (GameManager.cs)
│   ├── InputManager                (InputManager.cs)
│   ├── LaneManager                 (LaneManager.cs)
│   ├── Spellbook                   (Spellbook.cs)
│   ├── SpellCaster                 (SpellCaster.cs)
│   ├── ScoreManager                (ScoreManager.cs)
│   ├── WaveManager                 (WaveManager.cs)
│   └── AudioManager                (AudioManager.cs, AudioSource)
├── Wizard                          (sprite at left edge of screen)
├── TowerHealth                     (TowerHealth.cs — can live on Wizard instead)
├── Lanes
│   ├── Lane1
│   │   ├── SpawnPoint              (empty, right edge, lane 1 Y)
│   │   └── TowerPoint              (empty, at Wizard's X, lane 1 Y)
│   ├── Lane2
│   │   ├── SpawnPoint
│   │   └── TowerPoint
│   └── Lane3
│       ├── SpawnPoint
│       └── TowerPoint
Canvas                              (Screen Space - Overlay)
├── HUD
│   ├── HPBar (Image, filled)
│   ├── WaveText (Text/TMP)
│   ├── ScoreText (Text/TMP)
│   ├── ComboText (Text/TMP)
│   ├── LaneHighlights
│   │   ├── Highlight_Lane1 (Image, positioned over Lane1 row)
│   │   ├── Highlight_Lane2
│   │   └── Highlight_Lane3
│   ├── SpellbookPanel
│   │   └── RowContainer            (Vertical Layout Group; rows added at runtime)
│   └── InputFeedback
│       ├── CurrentSequenceText (Text/TMP)
│       └── FeedbackText (Text/TMP — "MISCAST" / "FIREBALL!" popups)
├── MainMenuPanel
│   └── PlayButton
├── CheckpointPanel
│   ├── ContinueButton
│   └── CashOutButton
├── GameOverPanel
│   └── RestartButton
├── VictoryPanel
│   ├── StatsText
│   └── RestartButton
└── GameStateUI (empty GameObject holding GameStateUI.cs)

EventSystem                         (auto-created with Canvas)
```

## 2. Managers — component wiring

All manager scripts use a `static Instance` singleton pattern, so **only one of
each should exist in the scene**. Drag references between them in the Inspector
as follows:

- **SpellCaster**
  - `spellbook` → the `Spellbook` GameObject
  - `laneManager` → the `LaneManager` GameObject
  - `scoreManager` → the `ScoreManager` GameObject
- **WaveManager**
  - `laneManager` → the `LaneManager` GameObject
  - `handAuthoredWaves` → the 5 `WaveData` assets (see §4), in order
  - `smallEnemy` / `mediumEnemy` / `largeEnemy` → the 3 `EnemyData` assets (used
    once hand-authored waves run out, for endless mode)
- **GameManager**
  - `waveManager` → the `WaveManager` GameObject
  - `finalWaveNumber` → `0` for endless mode, or `5` if you want the prototype
    to end cleanly after wave 5 with a Victory screen
- **AudioManager**
  - `sfxSource` → an `AudioSource` component on the same GameObject
  - `sfxEntries` → fill in `Tap`, `HoldCharge`, `CastSuccess`, `Miscast` (and
    optionally `EnemyDeath`, `TowerHit`) with placeholder clips

## 3. Lanes

Each `LaneX` GameObject:
- Add the `Lane` component.
- Set `laneIndex` to `0`, `1`, `2` respectively.
- `spawnPoint` → its child `SpawnPoint` transform (positioned at the right edge
  of the screen, e.g. x = 10)
- `towerPoint` → its child `TowerPoint` transform (positioned at the wizard's x,
  e.g. x = -8)
- Position the three lanes at different Y values (e.g. `y = 2, 0, -2`) so they
  read as stacked rows, matching the GDD's ASCII mockup.

On the `LaneManager` GameObject, assign `lanes[0..2]` to `Lane1`, `Lane2`,
`Lane3` in the Inspector, and set `rotationInterval` (start at `3` seconds per
the GDD's suggested pacing).

## 4. ScriptableObject assets to create

Use the Project window's `Assets > Create > Spellbound > ...` menu (added by
the `[CreateAssetMenu]` attributes on `SpellData`, `EnemyData`, `WaveData`).
Suggested folder: `Assets/Spellbound/Data/`.

### Spells (`Assets/Spellbound/Data/Spells/`)
| Asset name   | sequence         | effectType   | notes |
|--------------|------------------|--------------|-------|
| Firebolt     | Tap              | SingleTarget | low damage, prototype starter spell |
| Fireball     | Tap, Tap, Hold   | SingleTarget | higher damage than Firebolt |
| Meteor       | Hold, Tap, Hold  | AreaOfEffect | hits every enemy in the lane |

(Lightning, Freeze, and Heal from the full spellbook in the GDD can be added
the same way once the prototype's 3-spell loop feels good — just create more
`SpellData` assets and add them to the `Spellbook.unlockedSpells` list.)

Add all created `SpellData` assets to `Spellbook.unlockedSpells` in the
Inspector.

### Enemies (`Assets/Spellbound/Data/Enemies/`)
| Asset name | size   | maxHP | moveSpeed | damageToTower | scoreValue |
|------------|--------|-------|-----------|----------------|------------|
| Goblin     | Small  | 1     | 3         | 5              | 100        |
| Orc        | Medium | 3     | 2         | 10             | 150        |
| Ogre       | Large  | 10    | 1         | 20             | 300        |

Each needs a `prefab` reference (see §5).

### Waves (`Assets/Spellbound/Data/Waves/`)
Create `Wave1` … `Wave5` as `WaveData` assets. Each has a `spawns` array of
`SpawnEntry {enemyData, laneIndex, spawnTime}`. Suggested prototype progression:
- **Wave 1**: 3× Goblin, one lane, spaced 1.5s apart — teaches "press Space to cast."
- **Wave 2**: Goblins across 2 lanes — teaches watching the lane rotation.
- **Wave 3**: introduce 1–2 Orcs — teaches that Firebolt isn't always enough.
- **Wave 4**: mixed Goblins/Orcs across all 3 lanes — teaches prioritization.
- **Wave 5**: an Ogre plus a Goblin swarm — teaches planning a Meteor cast ahead
  of the lane you need.

Assign all 5 to `WaveManager.handAuthoredWaves` in order.

## 5. Enemy prefab

Create one prefab (`Assets/Spellbound/Prefabs/EnemyBase.prefab`) with:
- `SpriteRenderer`
- `Enemy` component (leave `data`/`laneIndex` empty — set at runtime by
  `Enemy.Initialize()`)
- Optionally a simple HP bar (`Canvas` + `Image`, world space, child of the
  prefab) if you want per-enemy health feedback beyond death.

You can either use one shared prefab for all three enemy types (differing only
via the `EnemyData` asset's stats and a sprite swap in `Initialize`), or make
three prefab variants (`Goblin`, `Orc`, `Ogre`) with different sprites/scale
and point each `EnemyData.prefab` at its matching variant. The three-variant
approach is simpler for a first pass.

## 6. UI wiring

- **HUDController**: assign `towerHealth`, `scoreManager`, `waveManager`, and
  the `hpFillImage` / `waveText` / `scoreText` / `comboText` UI elements.
- **TargetIndicatorUI**: assign `laneManager` and the 3 `Highlight_LaneX`
  GameObjects in the same order as `LaneManager.lanes`.
- **SpellbookUI**: assign `spellbook`, `spellCaster`, a `rowPrefab` (a small
  prefab with an `Image` on its root and two child `Text` objects named
  `Name` and `Pattern`), and `rowContainer` (the `RowContainer` transform
  under `SpellbookPanel`).
- **InputFeedbackUI**: assign `spellCaster`, `currentSequenceText`,
  `feedbackText`.
- **GameStateUI**: assign `gameManager` and all five panels
  (`MainMenuPanel`, `HUD`, `CheckpointPanel`, `GameOverPanel`,
  `VictoryPanel`). Wire each panel's buttons in the Inspector:
  - `PlayButton.OnClick` → `GameStateUI.OnPlayButtonPressed`
  - `ContinueButton.OnClick` → `GameStateUI.OnContinueButtonPressed`
  - `CashOutButton.OnClick` → `GameStateUI.OnCashOutButtonPressed`
  - `RestartButton.OnClick` (both panels) → `GameStateUI.OnRestartButtonPressed`

## 7. Wizard & Tower

- `Wizard` sprite sits at the shared left-edge X used by every lane's
  `TowerPoint`.
- Put `TowerHealth` on the `Wizard` GameObject (or a dedicated `TowerHealth`
  empty parented to it) and set `maxHP` (e.g. `100`).

## 8. Camera & layout

- `Main Camera`: orthographic, sized so all 3 lanes plus the HUD comfortably
  fit (e.g. orthographic size `5`, positioned at `(0, 0, -10)`).
- Keep world-space lane Y positions symmetric around 0 so the camera doesn't
  need to move.

## 9. Play-test checklist

1. Enter Play mode; `MainMenuPanel` should show, everything else hidden.
2. Press the Play button → `HUD` appears, Wave 1 starts, `LaneManager` begins
   rotating its highlight every `rotationInterval` seconds.
3. Tap Space once → Firebolt fires into whatever lane is currently
   highlighted.
4. Tap, Tap, Hold → Fireball fires (verify it doesn't fire early after the
   first Tap, thanks to the `commitDelay` prefix-ambiguity handling in
   `SpellCaster`).
5. Let a Goblin reach the tower → HP bar drops; at 0 HP, `GameOverPanel`
   shows.
6. Clear all 5 waves → since `finalWaveNumber = 5` (if set), `VictoryPanel`
   shows.
