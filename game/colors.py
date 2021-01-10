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

import collections.abc

import pygame

BLACK: pygame.Color = pygame.Color(0, 0, 0)
DARK_GRAY: pygame.Color = pygame.Color(0x2f, 0x4f, 0x4f)
FAINT_BLUE: pygame.Color = pygame.Color(0x87, 0xce, 0xfa)
FAINT_GRAY: pygame.Color = pygame.Color(0x30, 0x30, 0x30)
GRAY: pygame.Color = pygame.Color(0x93, 0x70, 0xdb)
GREEN: pygame.Color = pygame.Color(0x7c, 0xfc, 0)
PURPLE: pygame.Color = pygame.Color(0x4b, 0, 0x82)
PINK: pygame.Color = pygame.Color(255, 0x14, 0x93)
RED: pygame.Color = pygame.Color(0xdc, 0x14, 0x3c)
WHITE: pygame.Color = pygame.Color(255, 255, 255)
YELLOW: pygame.Color = pygame.Color(255, 0xd7, 0)

# FIXME: create separate loadable color schemes
# with symbolic color names rather than explicit ones

# FIXME: not sure if this should go here, or if this should just be for generic colors
BACKGROUND_BACKUP_COLOR: pygame.Color = DARK_GRAY
EDGE_COLOR_OPTIONS: collections.abc.Sequence = [GREEN, PURPLE, PINK, YELLOW]
INITIAL_HEXAGON_COLOR: pygame.Color = BLACK
REFRESH_COLOR: pygame.Color = FAINT_BLUE
RHOMBUS_COLOR: pygame.Color = FAINT_GRAY
