# What The Hex?

This is What The Hex?, a geometric puzzle game made in C#, using the [Godot Game Engine](https://godotengine.org).

![Banner](banner.png)

![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/rwhogg/what-the-hex?style=social)
![Lines of code](https://img.shields.io/tokei/lines/github/rwhogg/what-the-hex?style=social)
![GitHub](https://img.shields.io/github/license/rwhogg/what-the-hex?style=social)

![Screenshot](screenshot.png)

_The pursuit of true hexcellence requires that you clear your mind and become one with the geometry of the universe..._

## Rules

* You begin with an 8 x 5 grid of hexagons

* Each hexagon has one of 4 colors on each edge

* You can rotate any given hexagon 60 degrees counterclockwise with a left click, 60 degrees clockwise with a right click

* You start with 100 seconds on the clock

* If a rhombus with 4 hexagons bordering it has all 4 edges the same color, you get a match

* Most matches are worth 100 points, but each match you make with the current advantage color gives you 300 instead

FIXME verify true

* Every 10 seconds, random hexagons are regenerated

* Hexagons are internally black by default but turn blue when they are selected for replacement
* The number of hexagons refreshed per cycle increases by 1 every 3 refresh cycles

* You win if you make 20 matches

FIXME unimplemented

## Controls

Joystick (Supported on all platforms):

* D-Pad: Change currently selected hexagon
* Left shoulder buttons/triggers (L, L1, L2, LB, ZL, etc.): Rotate currently selected hexagon counterclockwise
* Right shoulder buttons/triggers (R, R1, R2, RB, ZR, etc.): Rotate currently selected hexagon clockwise

Support for using analog sticks to change the currently selected hexagon is planneed.

Mouse (Supported on desktop and web only):

* Left Click: Rotate hexagon under mouse cursor counterclockwise
* Right Click: Rotate hexagon under mouse cursor clockwise

Touch (Supported on Android only, _might_ work on desktop platforms with a touch screen):

* Tap on any hexagon: Select
* Specific touch buttons in the top-right handle rotation of the current hexagon

## Supported Platforms

Tier 1 (Should Work Well):

* Android/Chrome OS/Fire OS (ARM)
* GNU/Linux (amd64)
* Windows (x86)

Tier 2 (Will Probably Work Well):

* macOS (Can't test this at the moment, but no reason to believe it will be an issue)

Tier 3 (Works With Significant Issues)

* Desktop HTML5 (Currently works on Firefox and Edge. It works on Brave, but only with Shields Down. The alignment is currently all wrong.)

Tier 4 (Might work if compiled from source, but no promises)

* iOS

Tier 5 (Might work in the future, but I don't currently expect it to)

* Mobile HTML5 (The resolution is all wrong and touch events don't work. Will require a lot of work to get this up to snuff)
* Xbox One and Xbox Series X/S: Currently UWP targets do not support Mono. If this is fixed in the future I'm definitely interested. The relevant GitHub Issue is https://github.com/godotengine/godot/issues/20271 . For some reason the Edge browser here doesn't work either - maybe it will after the Chromium-based Edge is made available to all Xbox users.
* Raspberry Pi and other non-Android ARM devices: I don't want to compile my own build templates, so this is unsupported until they add one by default.

For mobile devices, a tablet is recommended, as the screen needs to be able to display at 1024 * 600 resolution smoothly

## Reporting Bugs

To report a bug, please send an email to Bob "Wombat" Hogg &lt;wombat@rwhogg.site&gt;.

## Created By

![Wombat OSG](wombat-osg.png)
A Division Of
![Boarish Entertainment](boarish-entertainment.png)

## Copyright

See the files [COPYING](COPYING) and [LICENSE.txt](LICENSE.txt)

## Third-Party Licenses

See the file [THIRD-PARTY-LICENSES.txt](project/THIRD-PARTY-LICENSES.txt)

## Acknowledgements

What The Hex? is built on the Godot Engine and on [Mono](https://www.mono-project.com/).

![Godot Logo](godot_logo.svg)

Special thanks to www.kenney.nl and to the [Superpowers app](http://superpowers-html5.com/) team for providing a number of the assets.
