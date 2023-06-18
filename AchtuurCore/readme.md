# AchtuurCore

Core mod required for most of my other mods, available on [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/16827).

## Events included (`IApi` wip)

* `EventPublisher.onFinishedWateringSoil` - Fired when a player waters a tile of previously unwatered(!!) soil.


# Changelog


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
