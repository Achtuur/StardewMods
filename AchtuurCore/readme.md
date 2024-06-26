# AchtuurCore

Core mod required for most of my other mods, available on [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/16827).

## Events included (`IApi` wip)

* `EventPublisher.onFinishedWateringSoil` - Fired when a player waters a tile of previously unwatered(!!) soil.


# Changelog

## 1.3.0
* Internal rework of particles

## 1.2.4
* New/Changed
  * Added extra drawing stuff
  * Added lights to particles


## 1.2.3
* New/Changed
  * Added borders from Hoverlabels here

## 1.2.2
* Fixes
 * Fix Vector2.ContainsObject() to work with qualified id

## 1.2.1
* Fixes
  * Fix compatibility issue with 1.6.3

## 1.2.0
* Updated to support Stardew 1.6.0

## 1.1.5
* New/Changed
  * `Overlay.Enable/Disable` are now virtual methods
  * Add Building extensions

## 1.1.4
* New/Changed
  * Decaying text now properly decays
  * Add extension methods for gamelocation

## 1.1.3
* New/Changed
  * Added red tile placement texture to overlay

## 1.1.2
* New/Changed
  * Add VectorHelper class

## 1.1.1
* New/Changed
  * Added placement Tile to overlay
  * Added more helper functions to overlay
  * Added random/unique colors to colorhelper
* Fixes
  * Particles now despawn when changing location

## 1.1.0
* New/Changed
  * Add skill utility class
  * Add GetTilesInRadius method
  * Add Particle and TrailParticle
  * Add Generic Overlay

## 1.0.7
* New/Changed
  * Expand Logger 
  * Change patcher to not take monitor as argument
  * Add SliderRange

* Fixes
  * Fix error in watering event when not holding a tool


## 1.0.6
* New/Changed
  * Add helper functions for transpiling
	
* Fixes
  * Fixed WateringPatch erroring with flexible sprinklers mod.

## 1.0.5
* Fixes
  * Fix EventPublisher null exception

## 1.0.4
* Add gmcm api interface to this

## 1.0.3
* Add utility classes
* Rename `Debug` to `Logger`

## 1.0.2
* Minor changes to `GenericPatcher` class

## 1.0.1
* Fixes
	* Fix error showing in console on WateringPatcher

## 1.0.0
Initial release

<!-- ### (unreleased) Changes made:

* WateringPatcher now uses pass-through prefix method (thanks to Shockah)
* Removed leftover watering debug message -->
