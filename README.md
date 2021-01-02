# What The Hex

Hexagonal puzzle game

## Rules

I'm still tweaking the specific numbers. Once we get to playing the game, what's fun/frustrating etc. will probably require adjusting this.
Maybe I'll make it configurable once the basics are working.

* You begin with an 8 x 5 grid of hexagons
* Each hexagon has one of 3 colors on each edge
* You can rotate any given hexagon 60 degrees counterclockwise with a left click, 60 degrees clockwise with a right click
* You start with 3 minutes on the clock
* If a diamond with 4 hexagons bordering has all 4 edges the same color, you get a match
    * It's possible more than one match is made at once - if so, you get geometrically increasing points per match made (100, 200, 400, 800, etc.)
    * After the hexagons are destroyed, they are randomly regenerated
* Every 10 seconds, 2 random hexagons are regenerated
    * Hexagons are internally gray by default, turn yellow when they are selected for replacement, and turn red after 5 seconds as a warning

## Development Requirements

You'll definitely need the following:

* Python 3.9 (may work with earlier 3.x versions)
* Pygame (and dependencies thereof, eg. SDL)

Get Python via whatever method you prefer.
Pygame can be acquired with the following:

```bash
% pip install pygame
```

If you want to build an installable standalone executable, you'll also need:

* Pyinstaller
* Ruby/Rake
* NSIS

## Copyright

Copyright (C) 2021, Bob "Wombat" Hogg

See LICENSE.txt for copying details.

Licenses of (possible) runtime dependencies are included in the licenses/ directory.

![Wombat OSG](wombat-osg.png)

## Acknowledgements

In addition to the above development requirements, special thanks to www.kenney.nl and to the Superpowers app team for providing a number of the assets.

![Python Powered](https://www.python.org/static/community_logos/python-powered-w-140x56.png)
![Powered by pygame](pygame_powered.gif)
