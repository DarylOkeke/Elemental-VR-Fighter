# Elemental VR Fighter

CS 417 open-ended project. Multiplayer PvP 1v1 arena battler: Avatar-style elemental benders, gesture casting, physical dodging, best-of-three rounds.

Team: Daryl Okeke, Giovanni Vera, Ethan Olvera, Suhaas Kowthalam Jois. Mentor: Julia Davis.

## Status

This branch adds the Unity project, shared arena scene, four connected prefabs, and Daryl's movement/health starter. Unity Editor compilation and ten Play Mode tests pass on Daryl's Windows PC. Main remains the script-free skeleton until Daryl approves a merge. Headset, controller, multiplayer and Android build testing are not yet complete. The three teammate prefabs are deliberately empty extension points, not implemented mechanics.

## Unity setup

Unity 6000.5.6f1, URP, OpenXR 1.17.1, XR Interaction Toolkit 3.5.1, Input System 1.20. Same stack as the course MP repos. Open the project folder in Unity Hub with 6000.5.6f1.

## Folder layout

Assets/ElementalFighter/
- Daryl/ : Movement and Dodging (hurtbox, health, dash, arena bounds, health bar)
- Giovanni/ : Elemental Bending (gesture recognition, projectiles, damage, effects)
- Suhaas/ : Combos and Ultimate Attacks (combo counter, ultimate meter, combo UI)
- Ethan/ : Multiplayer Versus (networked session, synced combat, rounds)
- Shared/Scenes/ : the shared arena scene every teammate's prefab lives in

Each teammate works in their own folder and opens a pull request. Another teammate reviews before merging to main.

## Shared scene prefabs

The shared scene holds one prefab per teammate:

- Daryl_MovementRig : player root, hurtbox, health, dash, arena bounds
- Giovanni_ElementalCombat : gesture recognition and projectile spawning
- Suhaas_CombosUltimates : combo counter, ultimate meter, combo UI
- Ethan_MultiplayerVersus : network session, synced combat, round flow

## Shared contracts

Health and damage: Daryl's Health component owns hit points. Anything that deals damage calls TakeDamage() on it, or calls ApplyHit() on a Hurtbox. Giovanni's projectiles and Suhaas's combo counter listen for hit events from these components, so keep combat logic in separate components Ethan can sync.

Hit event format: a hit is a damage amount plus a hit position. Hurtbox.ApplyHit(damage, hitPoint) is the single entry point. Do not reach into Health fields directly from other mechanics.

## Branch and PR plan

One branch per mechanic, named after the owner and mechanic. Keep PRs small and merge early. Screenshots of merged PRs are the Merge Requests extra credit evidence, so do not squash away the merge record.

## WIP extra credit checklist (due Fri Nov 20, 7pm, no late extension)

- Group Git: repo with Unity project, gitignore and gitattributes, shared scene with 4 prefabs, one committed file per teammate.
- Rescoped Design Docs: 3+ comments or suggested edits per teammate on the Technical doc, doc shared Anyone with the link as Commenter.
- Individual Progress Demo: in-engine demo posted to the Discord thread.
- Merge Requests: merged PR screenshots showing the purple merged logo.

## Open and test

Open Assets/ElementalFighter/Shared/Scenes/ElementalArena.unity. The four named root objects are connected prefab instances. Daryl's rig includes an XR Origin, head/body trigger hurtboxes, camera-relative health HUD, and dash. Dash uses left-controller primary button or Space for Editor smoke tests; the team must confirm this provisional mapping before integration. The movement tests are in Assets/ElementalFighter/Daryl/Tests and run with Window > General > Test Runner > PlayMode.

The hit event is Hurtbox.OnHit (UnityEvent<float, Vector3>), with damage and world hit position. Health.OnHealthChanged and Health.OnKO are public event accessors. Every HealthBar must reference its own player's Health explicitly. Scene scaffolding does not implement network authority, projectile collision policy, cooldown UI, or opponent logic.

UnityYAMLMerge is configured locally on the setup PC. Each teammate must configure Git's unityyamlmerge driver to their Editor's UnityYAMLMerge executable; .gitattributes alone cannot install a local merge driver. Keep .meta files with every asset.

The initial per-teammate READMEs were committed by Daryl during setup; they are not evidence of separate teammate contributions. Each teammate still needs their own meaningful committed file and review activity for the course requirements.

## Known integration limits

ArenaBounds constrains the XR Origin, not the physical tracked head. Keep the headset boundary active and use a clear play area; this is not a physical-safety boundary. Hurtboxes are triggers; projectile trigger-based collision needs a Rigidbody on at least one side, or explicit physics queries that call ApplyHit. No networking or authoritative damage arbitration has been chosen. The desktop preview can render without a headset, but OpenXR reports XR_ERROR_RUNTIME_UNAVAILABLE on this PC without an active runtime. Real controller/headset behavior and an Android build remain unverified.

Test coverage includes the generated prefab's Space action, using a synthetic keyboard with test-only headless focus handling; those input settings are restored afterward. This is not a hardware controller test. The scene and full health HUD were also visually checked in the interactive Editor. Each DashController owns a cloned action instance so disabling one rig does not disable another rig's input; Ethan still must disable local input on non-owned network replicas.
