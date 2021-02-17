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

# pyright: reportMissingImports=false

import collections.abc

import pygame

try:
    from game import colors
    from game import game_resources
    from game import utils
    from game import win_conditions
except ImportError:
    import colors
    import game_resources
    import utils
    import win_conditions

background: str = game_resources.get_background("starry.png")
music: str = game_resources.get_music("time_driving.ogg")

# Colors
bg_backup_color: pygame.Color = colors.DARK_GRAY
edge_color_options: collections.abc.Sequence = [colors.GREEN, colors.PURPLE, colors.PINK, colors.YELLOW]
initial_hexagon_color: pygame.Color = colors.BLACK
refresh_color: pygame.Color = colors.FAINT_BLUE
rhombus_color: pygame.Color = colors.FAINT_GRAY

# system
starting_time: int = utils.seconds_to_millis_plus_spare(100)
win_condition: win_conditions.WinConditions = win_conditions.MinMatches(20)
