# Elemental VR Fighter

CS 417 open-ended project. Multiplayer PvP 1v1 arena battler: Avatar-style elemental benders, gesture casting, physical dodging, best-of-three rounds.

Team: Daryl Okeke, Giovanni Vera, Ethan Olvera, Suhaas Kowthalam Jois. Mentor: Julia Davis.

## Status

Skeleton only. This repo currently holds the folder layout, gitignore and gitattributes, and the per-teammate plans. The Unity project, shared scene, and prefabs come next, built and tested in the Unity Editor on Daryl's machine (Jeff). No scripts are in the repo yet; Daryl's starter scripts are staged and land through a pull request after an Editor test.

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
