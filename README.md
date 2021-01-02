# What The Hex?

![What The Hex?](logo.png)

![Python application](https://github.com/rwhogg/what-the-hex/workflows/Python%20application/badge.svg)

## Notes

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
* Every 10 seconds, random hexagons are regenerated
    * Hexagons are internally black by default but turn blue when they are selected for replacement
* The number of hexagons refreshed per cycle increases by 1 every 20 seconds

## Requirements

You'll need at least the following:

* Python 3.9 (may work with earlier 3.x versions. Python 2.x probably _won't_ work)
* Pygame (and dependencies thereof, eg. SDL)

Get Python via whatever method you prefer.
Pygame can be acquired with the following:

```bash
% pip3 install -r requirements.txt
```

To run the game on its own, just run:

```bash
% python3 game.py
```

If you want to build an installable standalone executable, you'll also need:

* Pyinstaller
* Ruby/Rake
* NSIS (if you're on Windows)
* 7Zip (if you're on macOS)

Then:

```
% rake # for Windows
% rake -f Rakefile.macos.rb # for macOS
# Didn't bother to make Linux builds for this one... Nothing against it, but I wrote this on Windows so that's where I play it
```

## Created By

![Wombat OSG](wombat-osg.png)

## Copyright

Copyright (C) 2021, Bob "Wombat" Hogg

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

Licenses of (possible) runtime dependencies are included in the licenses/ directory.

## Acknowledgements

![Python Powered](https://www.python.org/static/community_logos/python-powered-w-140x56.png)
![Powered by pygame](pygame_powered.gif)

In addition to the above development requirements, special thanks to www.kenney.nl and to the [Superpowers app](http://superpowers-html5.com/) team for providing a number of the assets.
