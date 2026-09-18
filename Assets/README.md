# Spellbound — Unity 2D Project Scaffold

A one-button tower-defense/rhythm hybrid: cast Morse-code-style spells
(Tap/Hold sequences of Spacebar) into one of three continuously rotating
lanes to fend off waves of monsters. Built from the attached Game Design
Document, prototype scope (§26): 3 lanes, 3 enemy types, 3 spells, 5 waves.

## What's included

- **19 C# scripts** under `Assets/Spellbound/Scripts/`, organized by system
  (see below). Drop this whole `Assets/Spellbound/` folder into a Unity 2D
  project (2021.3 LTS or newer; uses only `UnityEngine`, `UnityEngine.UI`,
  and `UnityEngine.SceneManagement` — no external packages required).
- **`SCENE_HIERARCHY.md`** — a full walkthrough of the GameObject hierarchy,
  component wiring, and ScriptableObject assets needed to get the prototype
  running in the editor. Unity scenes are binary/YAML files best assembled
  in-editor, so that document is the "build the scene" instructions rather
  than a `.unity` file.

## Script map

| System | Script | Responsibility |
|---|---|---|
| Input | `InputSystem/InputManager.cs` | Reads Spacebar, classifies Tap vs Hold, is the *only* script that touches `Input` directly |
| Spells | `Spells/SpellSymbol.cs` | Tap/Hold enum |
| Spells | `Spells/SpellData.cs` | ScriptableObject: a spell's sequence, damage, effect type |
| Spells | `Spells/Spellbook.cs` | Which spells are unlocked; prefix/exact-match logic |
| Spells | `Spells/SpellCaster.cs` | Builds the live input sequence, resolves it against the Spellbook, decides when to cast vs. wait vs. miscast |
| Spells | `Spells/SpellEffectResolver.cs` | Applies a cast spell's effect (damage/AoE/slow/heal) to a lane |
| Lanes | `Lanes/Lane.cs` | Tracks enemies in one lane; spawn/tower endpoints |
| Lanes | `Lanes/LaneManager.cs` | Rotates the targeting indicator between lanes on a timer |
| Enemies | `Enemies/EnemyData.cs` | ScriptableObject: an enemy type's stats |
| Enemies | `Enemies/Enemy.cs` | Movement, HP, damage, slow, death/reach-tower reporting |
| Waves | `Waves/WaveData.cs` | ScriptableObject: a wave's spawn list |
| Waves | `Waves/WaveManager.cs` | Runs waves in sequence, generates endless waves after, raises checkpoints every 5 waves |
| Tower | `Tower/TowerHealth.cs` | Wizard's HP, game-over trigger |
| Scoring | `Scoring/ScoreManager.cs` | Score, combo multiplier, cast/miscast counts |
| Core | `Core/GameManager.cs` | Game state machine (menu/playing/checkpoint/game over/victory) |
| Core | `Core/AudioManager.cs` | Central SFX playback |
| UI | `UI/HUDController.cs` | HP bar, wave, score, combo text |
| UI | `UI/SpellbookUI.cs` | On-screen spellbook list with live "still possible" highlighting |
| UI | `UI/InputFeedbackUI.cs` | Shows the sequence being typed; flashes cast/miscast feedback |
| UI | `UI/TargetIndicatorUI.cs` | Highlights the currently targeted lane |
| UI | `UI/GameStateUI.cs` | Shows/hides menu/HUD/checkpoint/game-over/victory panels |

## Design notes baked into the code

- **Prefix ambiguity (Firebolt vs. Fireball):** `Spellbook` distinguishes
  "exact match with no longer spell sharing this prefix" (cast immediately)
  from "exact match, but a longer spell could still extend it" (wait
  `SpellCaster.commitDelay` seconds before committing to the shorter spell).
  This is what lets `Tap` alone be a full spell (Firebolt) even though
  `Tap` is also the start of `Tap, Tap, Hold` (Fireball).
- **Lane timing is resolved at the moment of casting**, not when the
  player starts typing: `SpellCaster.Cast()` reads
  `LaneManager.CurrentLaneIndex` only once the input sequence is complete,
  which is exactly the "interesting problem" from GDD §8 — the player must
  time when they *start* a spell so it *finishes* on the lane they want.
- **Endless play with checkpoints:** `WaveManager` uses hand-authored
  `WaveData` assets for the first N waves, then procedurally generates
  harder ones forever. `GameManager` raises a checkpoint every 5 waves
  (GDD §11) so the player can cash out or keep going.

## Next steps beyond the prototype

- Add the remaining spells from the full spellbook (Lightning, Freeze,
  Heal) as more `SpellData` assets — no code changes needed.
- Add sprites/VFX prefabs and wire `SpellData.castVfxPrefab` /
  `EnemyData.prefab` for visual polish.
- Add a proper spell/wave unlock progression instead of unlocking
  everything at once in `Spellbook`.
