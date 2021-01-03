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

import pygame

import colors
import constants
import hexagon_struct


def check_all_adjacent_diamonds(hexagon_array, hexagon, row, column):
    hexagons_involved = set()
    count_diamonds = 0
    color_to_flash = None

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
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_nw)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_w)
            color_to_flash = hexagon.edge_colors[tle]
    # top-right
    if row != 0 and column != len(hexagon_array[0]) - 1:
        hex_ne = hexagon_array[row - 1][column + 1]
        hex_n = hexagon_array[row - 1][column]
        hex_e = hexagon_array[row][column + 1]
        ne_eq = hexagon.edge_colors[tre] == hex_ne.edge_colors[ble]
        n_eq = hexagon.edge_colors[tre] == hex_n.edge_colors[bre]
        e_eq = hexagon.edge_colors[tre] == hex_e.edge_colors[tle]
        if ne_eq and n_eq and e_eq:
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_ne)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_e)
            color_to_flash = hexagon.edge_colors[tre]
    # bottom-right
    if row != len(hexagon_array) - 1 and column != len(hexagon_array[0]) - 1:
        hex_se = hexagon_array[row + 1][column + 1]
        hex_s = hexagon_array[row + 1][column]
        hex_e = hexagon_array[row][column + 1]
        se_eq = hexagon.edge_colors[bre] == hex_se.edge_colors[tle]
        s_eq = hexagon.edge_colors[bre] == hex_s.edge_colors[tre]
        e_eq = hexagon.edge_colors[bre] == hex_e.edge_colors[ble]
        if se_eq and s_eq and e_eq:
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_se)
            hexagons_involved.add(hex_s)
            hexagons_involved.add(hex_e)
            color_to_flash = hexagon.edge_colors[bre]
    # bottom-left
    if row != len(hexagon_array) - 1 and column != 0:
        hex_sw = hexagon_array[row + 1][column - 1]
        hex_s = hexagon_array[row + 1][column]
        hex_w = hexagon_array[row][column - 1]
        sw_eq = hexagon.edge_colors[ble] == hex_sw.edge_colors[tre]
        s_eq = hexagon.edge_colors[ble] == hex_s.edge_colors[tle]
        w_eq = hexagon.edge_colors[ble] == hex_w.edge_colors[bre]
        if sw_eq and s_eq and w_eq:
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_sw)
            hexagons_involved.add(hex_s)
            hexagons_involved.add(hex_w)
            color_to_flash = hexagon.edge_colors[ble]

    return hexagons_involved, count_diamonds, color_to_flash


def draw_hexagon(screen, hexagon):
    points = hexagon.get_points()
    edges = hexagon.get_edges()
    pygame.draw.polygon(screen, hexagon.base_color, points)
    for e in range(6):
        pygame.draw.line(screen, hexagon.edge_colors[e], edges[e][0],
                         edges[e][1], constants.EDGE_THICKNESS)


def pick_background_hexagons_to_refresh(hexagon_array,
                                        num_hexagons_to_refresh):
    num_refreshed = 0
    num_columns = len(hexagon_array[0])
    num_rows = len(hexagon_array)
    i = 0
    while num_refreshed < num_hexagons_to_refresh:
        column = random.randrange(0, num_columns - 1)
        row = random.randrange(0, num_rows - 1)
        if not hexagon_array[row][column].was_matched:
            hexagon_array[row][column].to_refresh = True
            hexagon_array[row][column].base_color = colors.REFRESH_COLOR
            num_refreshed += 1
        # iteration count, make sure we never get stuck here
        i += 1
        if i == 20:
            break


def random_hexagon(center, base_color):
    random_colors = random.choices(colors.EDGE_COLOR_OPTIONS, k=6)
    return hexagon_struct.HexagonStruct(center, base_color, random_colors)


def random_hexagon_array(start):
    hexagons = [[] for _ in range(constants.HEXAGON_ROWS)]
    for i in range(constants.HEXAGON_ROWS):
        for j in range(constants.HEXAGON_COLUMNS):
            center_x = start[0] + j * constants.HEXAGON_SIDE_LENGTH * math.cos(
                30) * 14
            center_y = start[1] + i * constants.HEXAGON_SIDE_LENGTH * math.cos(
                30) * 12.5
            center = [center_x, center_y]
            hexagons[i].append(random_hexagon(center, colors.BLACK))
    return hexagons


def refresh_matched_hexagons(hexagon_array):
    refresh_hexagons(hexagon_array, lambda hexagon: hexagon.was_matched)


def refresh_background_hexagons(hexagon_array):
    refresh_hexagons(hexagon_array, lambda hexagon: hexagon.to_refresh)


def refresh_hexagons(hexagon_array, predicate):
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            if predicate(hexagon_array[row][column]):
                hexagon_array[row][column] = random_hexagon(
                    hexagon_array[row][column].center, colors.BLACK)


# FIXME: ideally, this would be determined via screen position calculation rather than iteration
# But so far it seems acceptably fast
def rotate_hexagon(hexagon_array, direction, position):
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            hexagon = hexagon_array[row][column]
            if hexagon.point_is_inside(position):
                hexagon.rotate(direction)
                return hexagon, row, column
    return None, None, None
