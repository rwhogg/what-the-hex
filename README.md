# What The Hex?

![What The Hex?](logo.png)

Hexagonal puzzle game.

This is _not_ fully finished but I'm not intending to do active
work on this long-term.

## Rules

I'm still tweaking the specific numbers. Once we get to playing the game, what's fun/frustrating etc. will probably require adjusting this.
Maybe I'll make it configurable once the basics are working.

* You begin with an 8 x 5 grid of hexagons
* Each hexagon has one of 3 colors on each edge
* You can rotate any given hexagon 60 degrees counterclockwise with a left click, 60 degrees clockwise with a right click
* You start with 3 minutes on the clock
* If a diamond with 4 hexagons bordering it has all 4 edges the same color, you get a match
    * It's possible more than one match is made at once - if so, you get geometrically increasing points per match made (100, 200, 400, 800, etc.)
    * After the hexagons are destroyed, they are randomly regenerated
* Every 10 seconds, 2 random hexagons are regenerated
    * Hexagons are internally gray by default, turn yellow when they are selected for replacement, and turn red after 5 seconds as a warning

## Requirements

If you just want to use the provided executables, just download them
and run from the [Releases page](https://github.com/rwhogg/what-the-hex/releases).

Otherwise, you'll need at least the following:

* Python 3.9 (may work with earlier 3.x versions. Python 2.x probably _won't_ work)
* Pygame (and dependencies thereof, eg. SDL)

Get Python via whatever method you prefer.
Pygame can be acquired with the following:

```bash
% pip3 install pygame
```

To run the game on its own, just run:

```bash
% python3 game.py
```

If you want to build an installable standalone executable, you'll also need:

* Pyinstaller
* Ruby/Rake
* NSIS

Then:

```
% rake
```

## Copyright

Copyright (C) 2021, Bob "Wombat" Hogg

See LICENSE.txt for copying details.

Licenses of (possible) runtime dependencies are included in the licenses/ directory.

![Wombat OSG](wombat-osg.png)

## Acknowledgements

![Python Powered](https://www.python.org/static/community_logos/python-powered-w-140x56.png)
![Powered by pygame](pygame_powered.gif)

In addition to the above development requirements, special thanks to www.kenney.nl and to the Superpowers app team for providing a number of the assets.
