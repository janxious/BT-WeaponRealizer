# BT-WeaponRealizer

Battletech:Game Mod - experiments in restoring lost functions to weapons, and adding new ones.

This supercedes my previous mod [Weapon Variance](https://github.com/janxious/BT-WeaponVariance)

## Features

* Multishot Ballistics - see `ShotsWhenFired` in def
* Multishot Lasers - see `ShotsWhenFired` in def
* Damage variance +/- around specified damage - see `DamageVariance` in def
* Increased damage done when overheated - see `OverheatedDamageMultiplier` in def
* Heat damage converted to normal damage vs. vehicles, buildings, and turrets
* Damage dropoff by distance - configured by tag on weapon def of format `WR-variance_by_distance-X`; `X` is a multiplier that controls the rate at which damage diminishes with distance

## LICENSE
MIT © 2018 Joel Meador