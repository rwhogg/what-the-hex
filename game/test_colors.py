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

import colors


def test_colors():
    assert colors.EDGE_COLOR_OPTIONS == [pygame.Color(0x7c, 0xfc, 0), pygame.Color(0x4b, 0, 0x82), pygame.Color(255, 0x14, 0x93), pygame.Color(255, 0xd7, 0)]


if __name__ == '__main__':
    unittest.main()
