# Suhaas: Combos and Ultimate Attacks

Owner folder. Combo counter, ultimate meter, combo UI.

Contract: listen for hit events from Hurtbox (onHit) and Health (onHealthChanged). Agree on the hit-event format with Giovanni early: damage amount plus hit position, through Hurtbox.ApplyHit().
