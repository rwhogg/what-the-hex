# Hexasexy

Hexagonal puzzle game (VERY VERY WIP)

## Rules

I'm still tweaking the specific numbers. Once we get to playing the game, what's fun/frustrating etc. will probably require adjusting this.
Maybe I'll make it configurable once the basics are working.

* You begin with an 8 x 5 grid of hexagons
* Each hexagon has one of 3 colors on each edge
* You can rotate any given hexagon 60 degrees counterclockwise with a left click, 60 degrees clockwise with a right click
* You start with 3 minutes on the clock
* If any single hexagon is surrounded by six other hexagons with all the same color facing inwards, all seven hexagons are destroyed and you get 100 points and another minute on the clock
    * It's possible more than one match is made at once - if so, you get geometrically increasing points per central hexagon involved (100, 200, 400, 800, etc.)
    * After the hexagons are destroyed, they are randomly regenerated
* Every 10 seconds, 2 random hexagons are regenerated
    * Hexagons are internally green by default, turn yellow when they are selected for replacement, and turn red after 5 seconds as a warning

## Development Requirements

* Python 3.9
* Ruby/Rake
* Pyinstaller
* Pygame
* NSIS

## Copyright

Copyright (C) 2020, Bob "Wombat" Hogg
See LICENSE.txt for copying details.
