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

color1, color2, color3, color4, color5, color6 = pygame.Color(
    1, 1, 1), pygame.Color(2, 2, 2), pygame.Color(3, 3, 3), pygame.Color(
        4, 4, 4), pygame.Color(5, 5, 5), pygame.Color(6, 6, 6)
hexagon = hexagon_struct.HexagonStruct([300, 300], pygame.Color(
    255, 255, 255), [color1, color2, color3, color4, color5, color6])


def test_get_big_radius():
    assert hexagon.get_big_radius() == 100


def test_rotate():
    hexagon.rotate("left")
    assert hexagon.edge_colors[0] == color2
    assert hexagon.edge_colors[1] == color3
    assert hexagon.edge_colors[2] == color4
    assert hexagon.edge_colors[3] == color5
    assert hexagon.edge_colors[4] == color6
    assert hexagon.edge_colors[5] == color1
    hexagon.rotate("right")
    assert hexagon.edge_colors[0] == color1
    assert hexagon.edge_colors[1] == color2
    assert hexagon.edge_colors[2] == color3
    assert hexagon.edge_colors[3] == color4
    assert hexagon.edge_colors[4] == color5
    assert hexagon.edge_colors[5] == color6


if __name__ == "main":
    unittest.main()
