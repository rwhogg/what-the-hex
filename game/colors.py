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

import typing

import pygame
import travertino.colors

# Note: explicitly including travertino color classes here, because pygame doesn't know how to handle them
# Also, pygame only likes integer colors, it rejects floats
ColorLike = typing.Union[typing.Tuple[int, int, int], typing.Tuple[int, int, int, int], pygame.Color]

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


def to_tuple(color: ColorLike) -> tuple:
    return (color.r, color.g, color.b) if isinstance(color, pygame.Color) else color


def to_travertino(color: ColorLike) -> travertino.colors.rgb:
    return travertino.colors.rgb(*to_tuple(color))
