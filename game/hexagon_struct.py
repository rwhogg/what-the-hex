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

import math
import typing

import constants


class HexagonStruct:
    def __init__(self, center, base_color, edge_colors) -> None:
        if len(edge_colors) != 6:
            raise Exception
        self.center = center
        self.base_color = base_color
        self.edge_colors = edge_colors
        self.was_matched = False
        self.to_refresh = False

    def get_points(self) -> typing.List[typing.Tuple[float, float]]:
        center_x = self.center[0]
        center_y = self.center[1]
        p0 = (0.5 * constants.HEXAGON_SIDE_LENGTH + center_x,
              center_y - 0.866025 * constants.HEXAGON_SIDE_LENGTH)
        p1 = (constants.HEXAGON_SIDE_LENGTH + center_x, center_y)
        p2 = (0.5 * constants.HEXAGON_SIDE_LENGTH + center_x,
              0.866025 * constants.HEXAGON_SIDE_LENGTH + center_y)
        p3 = (center_x - 0.5 * constants.HEXAGON_SIDE_LENGTH,
              0.866025 * constants.HEXAGON_SIDE_LENGTH + center_y)
        p4 = (center_x - constants.HEXAGON_SIDE_LENGTH, center_y)
        p5 = (center_x - 0.5 * constants.HEXAGON_SIDE_LENGTH,
              center_y - 0.866025 * constants.HEXAGON_SIDE_LENGTH)
        return [p0, p1, p2, p3, p4, p5]

    @staticmethod
    def get_big_radius() -> int:
        return constants.HEXAGON_SIDE_LENGTH * 2

    @staticmethod
    def get_small_radius() -> float:
        return constants.HEXAGON_SIDE_LENGTH * math.cos(30)

    def get_edges(self) -> list:
        points = self.get_points()
        return [[points[a], points[(a + 1) % 6]] for a in range(6)]

    def point_is_inside(self, point) -> bool:
        # this isn't strictly correct, but it's accurate enough for my purposes
        # (the 35 is just a little extra tolerance)
        return math.hypot(point[0] - self.center[0], point[1] -
                          self.center[1]) <= self.get_small_radius() + 35

    def rotate(self, direction) -> None:
        if direction == "right":
            last_color = self.edge_colors.pop(5)
            self.edge_colors.insert(0, last_color)
        elif direction == "left":
            first_color = self.edge_colors.pop(0)
            self.edge_colors.append(first_color)


HexagonArray = typing.List[typing.List[HexagonStruct]]
