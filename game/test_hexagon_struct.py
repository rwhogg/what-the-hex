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

import unittest

import pygame

import hexagon_struct


def test_get_big_radius():
    hexagon = hexagon_struct.HexagonStruct([300, 300],
                                           pygame.Color(255, 255, 255), [
                                               pygame.Color(0x10, 0x10, 0x10),
                                               pygame.Color(0x10, 0x10, 0x10),
                                               pygame.Color(0x10, 0x10, 0x10),
                                               pygame.Color(0x10, 0x10, 0x10),
                                               pygame.Color(0x10, 0x10, 0x10),
                                               pygame.Color(0x10, 0x10, 0x10)
                                           ])
    assert hexagon.get_big_radius() == 100


if __name__ == "main":
    unittest.main()
