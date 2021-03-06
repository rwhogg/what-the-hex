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
import random
import typing

import pygame

try:
    from . import colors
    from . import constants
    from . import hexagon_struct
    from . import utils
except ImportError:
    import colors
    import constants
    import hexagon_struct
    import utils


def check_all_adjacent_diamonds(hexagon_array: hexagon_struct.HexagonArray, hexagon: hexagon_struct.HexagonStruct,
                                row: int, column: int) \
        -> typing.Tuple[set, typing.Dict[colors.ColorLike, int], typing.Optional[colors.ColorLike]]:
    # NOTE: this method is rather precarious.
    # Make sure you know what you're doing if you ever change it. It is very easy to get things wrong here.
    # (Hopefully, I never have to edit this method ever again...)

    hexagons_involved = set()
    match_colors = {}

    tre = 0
    bre = 1
    ble = 3
    tle = 4

    # top-left
    if row != 0 and column != 0:
        hex_nw = hexagon_array[row - 1][column - 1]
        hex_n = hexagon_array[row - 1][column]
        hex_w = hexagon_array[row][column - 1]
        nw_eq = hexagon.edge_colors[tle] == hex_nw.edge_colors[bre]
        n_eq = hexagon.edge_colors[tle] == hex_n.edge_colors[ble]
        w_eq = hexagon.edge_colors[tle] == hex_w.edge_colors[tre]
        if nw_eq and n_eq and w_eq:
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_nw)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_w)
            # No need to += 1 here because this is definitely the first instance
            # Also note that pygame.Color is not hashable, so must convert to tuple
            match_colors[colors.to_tuple(hexagon.edge_colors[tle])] = 1
    # top-right
    if row != 0 and column != len(hexagon_array[0]) - 1:
        hex_ne = hexagon_array[row - 1][column + 1]
        hex_n = hexagon_array[row - 1][column]
        hex_e = hexagon_array[row][column + 1]
        ne_eq = hexagon.edge_colors[tre] == hex_ne.edge_colors[ble]
        n_eq = hexagon.edge_colors[tre] == hex_n.edge_colors[bre]
        e_eq = hexagon.edge_colors[tre] == hex_e.edge_colors[tle]
        if ne_eq and n_eq and e_eq:
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_ne)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_e)
            try:
                match_colors[colors.to_tuple(hexagon.edge_colors[tre])] += 1
            except KeyError:
                match_colors[colors.to_tuple(hexagon.edge_colors[tre])] = 1
    # bottom-right
    if row != len(hexagon_array) - 1 and column != len(hexagon_array[0]) - 1:
        hex_se = hexagon_array[row + 1][column + 1]
        hex_s = hexagon_array[row + 1][column]
        hex_e = hexagon_array[row][column + 1]
        se_eq = hexagon.edge_colors[bre] == hex_se.edge_colors[tle]
        s_eq = hexagon.edge_colors[bre] == hex_s.edge_colors[tre]
        e_eq = hexagon.edge_colors[bre] == hex_e.edge_colors[ble]
        if se_eq and s_eq and e_eq:
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_se)
            hexagons_involved.add(hex_s)
            hexagons_involved.add(hex_e)
            try:
                match_colors[colors.to_tuple(hexagon.edge_colors[bre])] += 1
            except KeyError:
                match_colors[colors.to_tuple(hexagon.edge_colors[bre])] = 1
    # bottom-left
    if row != len(hexagon_array) - 1 and column != 0:
        hex_sw = hexagon_array[row + 1][column - 1]
        hex_s = hexagon_array[row + 1][column]
        hex_w = hexagon_array[row][column - 1]
        sw_eq = hexagon.edge_colors[ble] == hex_sw.edge_colors[tre]
        s_eq = hexagon.edge_colors[ble] == hex_s.edge_colors[tle]
        w_eq = hexagon.edge_colors[ble] == hex_w.edge_colors[bre]
        if sw_eq and s_eq and w_eq:
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_sw)
            hexagons_involved.add(hex_s)
            hexagons_involved.add(hex_w)
            try:
                match_colors[colors.to_tuple(hexagon.edge_colors[ble])] += 1
            except KeyError:
                match_colors[colors.to_tuple(hexagon.edge_colors[ble])] = 1

    match_color_list = list(match_colors.keys())
    color_to_flash = None
    if len(match_color_list) != 0:
        color_to_flash = pygame.Color(utils.pick_from(match_color_list))
    return hexagons_involved, match_colors, color_to_flash


def draw_hexagon(screen: pygame.Surface, hexagon: hexagon_struct.HexagonStruct) -> None:
    points = hexagon.get_points()
    edges = hexagon.get_edges()
    pygame.draw.polygon(screen, hexagon.base_color, points)
    for e in range(6):
        pygame.draw.line(screen, hexagon.edge_colors[e], edges[e][0], edges[e][1], constants.EDGE_THICKNESS)


def pick_background_hexagons_to_refresh(hexagon_array: hexagon_struct.HexagonArray, num_hexagons_to_refresh: int,
                                        refresh_color: colors.ColorLike) -> None:
    # Note, this mutates the hexagon_array rather than returning anything
    num_refreshed = 0
    num_columns = len(hexagon_array[0])
    num_rows = len(hexagon_array)
    i = 0
    while num_refreshed < num_hexagons_to_refresh:
        column = random.randrange(0, num_columns - 1)
        row = random.randrange(0, num_rows - 1)
        if not hexagon_array[row][column].was_matched:
            hexagon_array[row][column].to_refresh = True
            hexagon_array[row][column].base_color = refresh_color
            num_refreshed += 1
        # iteration count, make sure we never get stuck here
        i += 1
        if i == 20:
            break


def random_hexagon(center: hexagon_struct.Point, base_color: colors.ColorLike,
                   color_options: typing.List[colors.ColorLike]) -> hexagon_struct.HexagonStruct:
    random_colors: typing.List[colors.ColorLike] = random.choices(color_options, k=6)
    return hexagon_struct.HexagonStruct(center, base_color, random_colors)


def random_hexagon_array(start: typing.Sequence[float], num_rows: int, num_columns: int,
                         initial_hexagon_color: colors.ColorLike, color_options: typing.List[colors.ColorLike])\
        -> hexagon_struct.HexagonArray:
    hexagons: hexagon_struct.HexagonArray = [[] for _ in range(num_rows)]
    for i in range(num_rows):
        for j in range(num_columns):
            center_x = start[0] + j * constants.HEXAGON_SIDE_LENGTH * math.cos(30) * 14
            center_y = start[1] + i * constants.HEXAGON_SIDE_LENGTH * math.cos(30) * 12.5
            center: hexagon_struct.Point = (center_x, center_y)
            hexagons[i].append(random_hexagon(center, initial_hexagon_color, color_options))
    return hexagons


def refresh_matched_hexagons(hexagon_array: hexagon_struct.HexagonArray, initial_hexagon_color: colors.ColorLike,
                             color_options: typing.List[colors.ColorLike]) -> None:
    refresh_hexagons(hexagon_array, lambda hexagon: hexagon.was_matched, initial_hexagon_color, color_options)


def refresh_background_hexagons(hexagon_array: hexagon_struct.HexagonArray, initial_hexagon_color: colors.ColorLike,
                                color_options: typing.List[colors.ColorLike]) -> None:
    refresh_hexagons(hexagon_array, lambda hexagon: hexagon.to_refresh, initial_hexagon_color, color_options)


def refresh_hexagons(hexagon_array: hexagon_struct.HexagonArray,
                     predicate: typing.Callable[[hexagon_struct.HexagonStruct], bool],
                     initial_hexagon_color: colors.ColorLike, color_options: typing.List[colors.ColorLike]) -> None:
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            if predicate(hexagon_array[row][column]):
                hexagon_array[row][column] = random_hexagon(hexagon_array[row][column].center, initial_hexagon_color,
                                                            color_options)


# FIXME: ideally, this would be determined via screen position calculation rather than iteration
# But so far it seems acceptably fast
def rotate_hexagon(hexagon_array: hexagon_struct.HexagonArray, direction: str, position) -> tuple:
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            hexagon = hexagon_array[row][column]
            if hexagon.point_is_inside(position) and not hexagon.was_matched:
                hexagon.rotate(direction)
                return hexagon, row, column
    return None, None, None
