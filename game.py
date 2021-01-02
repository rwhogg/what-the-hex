# Copyright 2021 Bob "Wombat" Hogg
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

import math
import random
import sys

import pygame

pretty_game_name = "Hexasexy"
size = width, height = 1024, 768
hexagon_side_length = 50
hexagon_rows = 5
hexagon_columns = 8
debug = False

pygame.init()
screen = pygame.display.set_mode(size)
pygame.display.set_caption(pretty_game_name)
clock = pygame.time.Clock()

font_name = "kenney-pixel-square.ttf"
font = pygame.font.Font(font_name, 16)

music_name = "drumming-sticks.ogg"
pygame.mixer.music.load(music_name)

rotate_sound_name = "rotate.ogg"
rotate_sound = pygame.mixer.Sound(rotate_sound_name)
match_sound_name = "match.wav"
match_sound = pygame.mixer.Sound(match_sound_name)

# FIXME: I'll probably want some nicer colors later. For now, primaries are fine
black = pygame.Color(0, 0, 0)
white = pygame.Color(255, 255, 255)
blue = pygame.Color(0, 0, 255)
green = pygame.Color(0, 255, 0)
yellow = pygame.Color(255, 255, 0)
red = pygame.Color(255, 0, 0)
purple = pygame.Color(255, 0, 255)
orange = pygame.Color(255, 0xa5, 0)
pink = pygame.Color(0xff, 0x14, 0x93)

edge_colors = [orange, purple, blue]


class HexagonStruct:
    def __init__(self, center, base_color, edge_colors) -> None:
        if len(edge_colors) != 6:
            raise Exception
        self.center = center
        self.base_color = base_color
        self.edge_colors = edge_colors

    def get_points(self):
        center_x = self.center[0]
        center_y = self.center[1]
        p0 = (0.5 * hexagon_side_length + center_x, center_y - 0.866025 * hexagon_side_length)
        p1 = (hexagon_side_length + center_x, center_y)
        p2 = (0.5 * hexagon_side_length + center_x, 0.866025 * hexagon_side_length + center_y)
        p3 = (center_x - 0.5 * hexagon_side_length, 0.866025 * hexagon_side_length + center_y)
        p4 = (center_x - hexagon_side_length, center_y)
        p5 = (center_x - 0.5 * hexagon_side_length, center_y - 0.866025 * hexagon_side_length)
        return [p0, p1, p2, p3, p4, p5]

    def get_big_radius(self):
        return hexagon_side_length * 2

    def get_small_radius(self):
        return hexagon_side_length * math.cos(30)

    def get_edges(self):
        points = self.get_points()
        return [[points[a], points[(a + 1) % 6]] for a in range(6)]

    def point_is_inside(self, point):
        # this isn't strictly correct, but it's accurate enough for my purposes
        # (the 30 is just a little extra tolerance)
        return math.hypot(point[0] - self.center[0], point[1] - self.center[1]) <= self.get_small_radius() + 30

    def rotate(self, dir):
        if dir == "right":
            last_color = self.edge_colors.pop(5)
            self.edge_colors.insert(0, last_color)
        elif dir == "left":
            first_color = self.edge_colors.pop(0)
            self.edge_colors.append(first_color)


def draw_hexagon(hexagon):
    points = hexagon.get_points()
    edges = hexagon.get_edges()
    colors = hexagon.edge_colors
    pygame.draw.polygon(screen, hexagon.base_color, points)
    for e in range(6):
        pygame.draw.line(screen, colors[e], edges[e][0], edges[e][1], 5)


def random_hexagon(center, base_color):
    colors = random.choices(edge_colors, k=6)
    return HexagonStruct(center, base_color, colors)


def random_hexagon_array(start):
    hexagons = [[] for i in range(hexagon_rows)]
    for i in range(hexagon_rows):
        for j in range(hexagon_columns):
            center_x = start[0] + j * hexagon_side_length * math.cos(30) * 13
            center_y = start[1] + i * hexagon_side_length * math.cos(30) * 12 + 20
            center = [center_x, center_y]
            hexagons[i].append(random_hexagon(center, green))
    return hexagons


hexagon_array = random_hexagon_array([width / 6, height / 6])


# FIXME: ideally, this would be determined via screen position calculation rather than iteration, but we'll see if this is fast enough...
def rotate_hexagon(dir, position):
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            hexagon = hexagon_array[row][column]
            if hexagon.point_is_inside(position):
                hexagon.rotate(dir)
                return hexagon, row, column
    return None, None, None

def check_all_adjacent_diamonds(hexagon, row, column):
    print("Checking diamonds")
    print(hexagon.edge_colors)
    print(hexagon.get_edges())
    hexagons_involved = set()
    count_diamonds = 0

    tre = 0
    bre = 1
    ble = 3
    tle = 4

    # top-left
    if row != 0 and column != 0:
        print("tl check")
        hex_nw = hexagon_array[row - 1][column - 1]
        hex_n = hexagon_array[row - 1][column]
        hex_w = hexagon_array[row][column - 1]
        nw_eq = hexagon.edge_colors[tle] == hex_nw.edge_colors[bre]
        n_eq = hexagon.edge_colors[tle] == hex_n.edge_colors[ble]
        w_eq = hexagon.edge_colors[tle] == hex_w.edge_colors[tre]
        #print(hexagon.edge_colors[5], hex_nw.edge_colors[2], hex_n.edge_colors[4], hex_w.edge_colors[1])
        if nw_eq and n_eq and w_eq:
            print("tl match")
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_nw)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_w)
    # top-right
    if row != 0 and column != len(hexagon_array[0]) - 1:
        print("tr check")
        hex_ne = hexagon_array[row - 1][column + 1]
        hex_n = hexagon_array[row - 1][column]
        hex_e = hexagon_array[row][column + 1]
        ne_eq = hexagon.edge_colors[tre] == hex_ne.edge_colors[ble]
        n_eq = hexagon.edge_colors[tre] == hex_n.edge_colors[bre]
        e_eq = hexagon.edge_colors[tre] == hex_e.edge_colors[tle]
        #print(hexagon.edge_colors[tre], hex_ne.edge_colors[4], hex_n.edge_colors[2], hex_e.edge_colors[5])
        if  ne_eq and n_eq and e_eq:
            print("tr match")
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_ne)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_e)
    # bottom-right
    if row != len(hexagon_array) - 1 and column != len(hexagon_array[0]) - 1:
        print("br check")
        hex_se = hexagon_array[row + 1][column + 1]
        hex_s = hexagon_array[row + 1][column]
        hex_e = hexagon_array[row][column + 1]
        se_eq = hexagon.edge_colors[bre] == hex_se.edge_colors[tle]
        s_eq = hexagon.edge_colors[bre] == hex_s.edge_colors[tre]
        e_eq = hexagon.edge_colors[bre] == hex_e.edge_colors[ble]
        #print(hexagon.edge_colors[bre], hex_se.edge_colors[5], hex_s.edge_colors[1], hex_e.edge_colors[4])
        if se_eq and s_eq and e_eq:
            print("br match")
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_se)
            hexagons_involved.add(hex_s)
            hexagons_involved.add(hex_e)
    # bottom-left
    if row != len(hexagon_array) - 1 and column != 0:
        print("bl check")
        hex_sw = hexagon_array[row + 1][column - 1]
        hex_s = hexagon_array[row + 1][column]
        hex_w = hexagon_array[row][column - 1]
        sw_eq = hexagon.edge_colors[ble] == hex_sw.edge_colors[tre]
        s_eq = hexagon.edge_colors[ble] == hex_s.edge_colors[tle]
        w_eq = hexagon.edge_colors[ble] == hex_w.edge_colors[bre]
        #print(hexagon.edge_colors[4], hex_sw.edge_colors[1], hex_s.edge_colors[5], hex_w.edge_colors[2])
        if sw_eq and s_eq and w_eq:
            print("bl match")
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_sw)
            hexagons_involved.add(hex_s)
            hexagons_involved.add(hex_w)
    return (hexagons_involved, count_diamonds)


def game_over():
    pygame.mixer.music.stop()
    game_over_sound = pygame.mixer.Sound("game_over-sound.wav")
    game_over_voice = pygame.mixer.Sound("game_over-voice.ogg")
    game_over_sound.play()
    pygame.time.wait(int(game_over_sound.get_length() * 1000))
    game_over_voice.play()
    pygame.time.wait(int(game_over_voice.get_length() * 1000 + 1500))
    sys.exit()


def game_loop(time_left, score):
    clock.tick()

    hexagon_rotated, row, column = None, None, None
    for event in pygame.event.get():
        if event.type == pygame.MOUSEBUTTONDOWN:
            dir = "left" if event.button == 1 else "right"
            hexagon_rotated, row, column = rotate_hexagon(dir, event.pos)
            rotate_sound.play()
        elif event.type == pygame.QUIT:
            sys.exit()

    if hexagon_rotated is not None:
        hexagons_in_match, diamonds_matched = check_all_adjacent_diamonds(hexagon_rotated, row, column)
        if diamonds_matched > 0:
            score += math.pow(diamonds_matched, 2) * 100
            match_sound.play()
            for hexagon in hexagons_in_match:
                hexagon.base_color = pink

    if debug:
        print("Tick")

    screen.fill(white)
    for row in hexagon_array:
        for hexagon in row:
            draw_hexagon(hexagon)

    time_text_surface = font.render(f"Time {int(time_left / 1000)}        Score {int(score)}", True, black)
    screen.blit(time_text_surface, time_text_surface.get_rect())

    if time_left <= 0:
        # FIXME this is a bad way of doing this
        return True, True

    pygame.display.flip()

    return (time_left - clock.get_time(), score)


time_left = 300.0 * 1000.0
score = 0
game_loop(time_left, score) # first iteration so the screen comes up before the music starts
pygame.mixer.music.play(-1)
while True:
    time_left, score = game_loop(time_left, score)
    if time_left is True:
        break

if time_left is True:
    game_over()
