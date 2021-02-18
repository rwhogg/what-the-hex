# Copyright 2021 Bob "Wombat" Hogg
#
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 2.1 of the License, or (at your option) any later version.

# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.

# You should have received a copy of the GNU Lesser General Public
# License along with this library; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

import pathlib

import typing


# FIXME: don't use this
# I suspect it's part of the problem on Android, but also I've had permissions
# issues with Windows that I think result from this.
def get_homedir():
    return str(pathlib.Path.home())


PRETTY_GAME_NAME: str = "What The Hex?"
PACKAGE: str = "site.rwhogg.whatthehex"
AUTHOR: str = "Boarish Entertainment"

CONFIG_DATA_FILE: str = ".what-the-hex.config"

# FIXME: several of these shouldn't be constants
# They should be calculated by resolution
EDGE_THICKNESS: int = 6
EXTRA_SECONDS: int = 5
FONT_SIZE: int = 24
HEXAGON_ROWS: int = 5
HEXAGON_COLUMNS: int = 8
HEXAGON_SIDE_LENGTH: int = 50
INCREASE_REFRESH_RATE_TIME_MILLIS: int = 20 * 1000
LOOP_FOREVER: int = -1
REFRESH_BACKGROUND_HEXAGONS_TIME_MILLIS: int = 10 * 1000
SCREEN_WIDTH: int = 1024
SCREEN_HEIGHT: int = 768
SCREEN_SIZE: typing.Tuple[int, int] = (SCREEN_WIDTH, SCREEN_HEIGHT)
