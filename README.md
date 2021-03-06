# What The Hex?

![What The Hex?](logo.png)

![Python application](https://github.com/rwhogg/what-the-hex/workflows/Python%20application/badge.svg)

![Screenshot](screenshot.png)

## Downloads

* [Windows](https://github.com/rwhogg/what-the-hex/releases/download/v0.1.0/What.The.Hex.msi)
* [macOS](https://github.com/rwhogg/what-the-hex/releases/download/v0.1.0/What.The.Hex_macos.zip)
* Linux: not currently provided. Yes, I know I'm a Linux user and so this is hypocritical. Bug me about it enough, and I'll fix it.

## Rules

* You begin with an 8 x 5 grid of hexagons
* Each hexagon has one of 4 colors on each edge
* You can rotate any given hexagon 60 degrees counterclockwise with a left click, 60 degrees clockwise with a right click
* You start with 100 seconds on the clock
* If a rhombus with 4 hexagons bordering it has all 4 edges the same color, you get a match
    * Most matches are worth 100 points, but each match you make with the current advantage color gives you 300 instead
* Every 10 seconds, random hexagons are regenerated
    * Hexagons are internally black by default but turn blue when they are selected for replacement
* The number of hexagons refreshed per cycle increases by 1 every 20 seconds
* You win if you make 20 matches

## Requirements

You'll need at least Python 3.7 (3.8 or 3.9 should work, but only 3.7 is supported at this time).
You'll also need Node.js so you can run pyright.
To build an executable on Windows, you'll need .NET Framework 4 (this is a dependency of Toga)

Get Python via whatever method you prefer.

If you have more than one Python version installed, I'd recommend setting up a virtualenv with the one you want.

Once Python is set up, the other dependencies can be installed with this script:

```bash
% ./script/bootstrap
```

To run the game on its own, just run:

```bash
% briefcase dev
```

If you want to build an installable standalone executable, run:

```bash
% briefcase package
% briefcase run
```

Before committing, please run this script:

```bash
% script/precommit.sh
```

## Created By

![Wombat OSG](wombat-osg.png)
A Division Of
![Boarish Entertainment](boarish-entertainment.png)

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

You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to the
Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

Licenses of (possible) runtime dependencies are included in the [game/resources/licenses/](game/resources/licenses)
directory.

## Acknowledgements

![Python Powered](https://www.python.org/static/community_logos/python-powered-w-140x56.png)
![Powered by pygame](pygame_powered.gif)

In addition to the above development requirements, special thanks to www.kenney.nl and to the
[Superpowers app](http://superpowers-html5.com/) team for providing a number of the assets.
