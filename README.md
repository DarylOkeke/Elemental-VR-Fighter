# Elemental VR Fighter

Our CS 417 project: a 1v1 VR elemental arena fighter with gesture casting, physical dodging, and best-of-three rounds.

Team: Daryl Okeke, Giovanni Vera, Ethan Olvera, Suhaas Kowthalam Jois. Mentor: Julia Davis.

## Current progress

My movement and health starter, the Unity project, and the shared scene are in [PR #1](https://github.com/DarylOkeke/Elemental-VR-Fighter/pull/1), on `daryl/unity-scaffold`. Main is still the script-free skeleton. The other three prefabs are placeholders for my teammates' mechanics.

Unity compilation and all 10 Play Mode tests pass, including a synthetic-input dash test on the actual prefab. Headset, controller, Android, and multiplayer testing are still pending.

## Open the project

Check out `daryl/unity-scaffold` and open it in Unity Hub with **6000.5.6f1**. We use URP, OpenXR 1.17.1, XR Interaction Toolkit 3.5.1, and Input System 1.20.0, matching our MP stack.

Open `Assets/ElementalFighter/Shared/Scenes/ElementalArena.unity`. Run tests from **Window > General > Test Runner > PlayMode**.

## Team folders and prefabs

Everything lives under `Assets/ElementalFighter/`:

| Folder | Responsibility | Shared scene prefab |
| --- | --- | --- |
| Daryl/ | My movement, dodging, and health | Daryl_MovementRig |
| Giovanni/ | Gesture casting and elemental combat | Giovanni_ElementalCombat |
| Suhaas/ | Combos and ultimates | Suhaas_CombosUltimates |
| Ethan/ | Multiplayer and rounds | Ethan_MultiplayerVersus |
| Shared/Scenes/ | Our shared arena | |

We each work in our own folder and open small PRs for review. I'll approve this starter before it goes into main. Keep asset `.meta` files with their assets. Each teammate needs to configure their local UnityYAMLMerge driver.

## Integration

- Send hits through `Hurtbox.ApplyHit(damage, hitPoint)`. `OnHit` carries damage and world position. Don't modify health fields directly.
- `Health.OnHealthChanged` and `Health.OnKO` expose health events. Each `HealthBar` needs its own player's `Health`.
- Dash is provisionally left-controller primary button or Space. We still need to agree on the final mapping.
- Bounds clamp the XR Origin, not the tracked headset. Keep the headset safety boundary active.
- Trigger collisions need a Rigidbody on at least one side, or a physics query that calls `ApplyHit`.
- Ethan owns the networking choice and disabling input on remote-player replicas. No networking is implemented yet.

## WIP checklist

Due Friday Nov 20 at 7pm, with no late extension:

- Group Git: Unity project, shared scene with four prefabs, and a committed file from each teammate.
- Design docs: 3+ comments or suggestions per teammate; Technical doc shared as Anyone with the link / Commenter.
- Individual demo: in-engine video in the Discord thread.
- Merge requests: screenshots showing merged PRs.

The setup READMEs don't count as separate teammate contributions. We still need everyone's own commits and reviews.
