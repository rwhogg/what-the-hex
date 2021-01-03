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

import os.path
import pathlib

PRETTY_GAME_NAME = "What The Hex?"

EDGE_THICKNESS = 6
EXTRA_SECONDS = 5
FONT_SIZE = 24
HEXAGON_ROWS = 5
HEXAGON_COLUMNS = 8
HEXAGON_SIDE_LENGTH = 50
HISCORE_FILE_PATH = os.path.join(str(pathlib.Path.home()), ".what-the-hex.hiscore")
INCREASE_REFRESH_RATE_TIME_MILLIS = 20 * 1000
INITIAL_TIME_MILLIS = 100.0 * 1000.0
LOOP_FOREVER = -1
REFRESH_BACKGROUND_HEXAGONS_TIME_MILLIS = 10 * 1000
SCREEN_WIDTH = 1024
SCREEN_HEIGHT = 768
SCREEN_SIZE = (SCREEN_WIDTH, SCREEN_HEIGHT)
