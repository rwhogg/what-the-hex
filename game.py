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
import os
import random
import sys

import pygame

os.environ["SDL_VIDEO_CENTERED"] = "1"

pretty_game_name = "What the Hex?"
size = width, height = 1024, 768
hexagon_side_length = 50
hexagon_rows = 5
hexagon_columns = 8
extra_seconds = 5
rotate_sound_name = "rotate.ogg"
font_name = "kenney-pixel-square.ttf"
music_name = "bg_music.ogg"
match_sound_name = "match.wav"
debug = False

# Events
refresh_matched_hexagons_event = pygame.USEREVENT + 1

# Colors
# FIXME: create separate loadable color schemes
# with symbolic color names rather than explicit ones
black = pygame.Color(0, 0, 0)
white = pygame.Color(255, 255, 255)

# using for other stuff
offwhite = pygame.Color(255, 0xfa, 0xf0)
faint_blue = pygame.Color(0x87, 0xce, 0xfa)
sky_blue = pygame.Color(0, 0xbf, 255)
dark_gray = pygame.Color(0x2f, 0x4f, 0x4f) # also too green...

# reserved for the colors of the various hexagon states
green = (0, 0, 0)#dark_gray#pygame.Color(0xfa, 0xf0, 0xe6)#offwhite # pygame.Color(0x77, 0x88, 0x99)##pygame.Color(0, 255, 0xf7)#(0, 0x64, 0)#(0, 0x80, 0x80)#(0xd3, 0xd3, 0xd3)#(0x90, 0xee, 0x90)
red = pygame.Color(0xdc, 0x14, 0x3c)
pink = pygame.Color(0xad, 255, 0x2f)

# edge colors
# FIXME: 3 colors are too few. Too easy to make matches unintentionally
# 6 sounds like too many, so probably 4 or 5???
# trying to get 5 mutually non-clashing colors, plus avoiding the hexagon state ones, is too hard...
# stick with 4, just make the game refresh more frequently
blue = pygame.Color(0, 0, 0x90)# too hard to distinguish from the purple
purple = pygame.Color(0x4b, 0, 82)
orange = pygame.Color(255, 0x63, 0x47)
maroon = pygame.Color(0x80, 0, 0) # too subtle
yellow = pygame.Color(255, 0xd7, 0) # hurts my eyes...
true_pink = pygame.Color(255, 0x14, 0x93)
teal = pygame.Color(0, 0x80, 0x80) # too hard to see against the green

dark_gray_no_green = pygame.Color(0x2f, 0, 0x4f)
very_red = pygame.Color(255, 0, 0) # clashes with the orange

edge_colors = [orange, purple, true_pink, yellow]

# Setup

pygame.init()
screen = pygame.display.set_mode(size)
pygame.display.set_caption(pretty_game_name)
clock = pygame.time.Clock()
font = pygame.font.Font(font_name, 24)
pygame.mixer.music.load(music_name)
rotate_sound = pygame.mixer.Sound(rotate_sound_name)
match_sound = pygame.mixer.Sound(match_sound_name)

class HexagonStruct:
    def __init__(self, center, base_color, edge_colors) -> None:
        if len(edge_colors) != 6:
            raise Exception
        self.center = center
        self.base_color = base_color
        self.edge_colors = edge_colors
        self.was_matched = False

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


def refresh_matched_hexagons():
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            if hexagon_array[row][column].was_matched:
                hexagon_array[row][column] = random_hexagon(hexagon_array[row][column].center, green)


# FIXME: ideally, this would be determined via screen position calculation rather than iteration
# But so far it seems acceptably fast
def rotate_hexagon(dir, position):
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            hexagon = hexagon_array[row][column]
            if hexagon.point_is_inside(position):
                hexagon.rotate(dir)
                return hexagon, row, column
    return None, None, None


def check_all_adjacent_diamonds(hexagon, row, column):
    hexagons_involved = set()
    count_diamonds = 0

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
    # top-right
    if row != 0 and column != len(hexagon_array[0]) - 1:
        hex_ne = hexagon_array[row - 1][column + 1]
        hex_n = hexagon_array[row - 1][column]
        hex_e = hexagon_array[row][column + 1]
        ne_eq = hexagon.edge_colors[tre] == hex_ne.edge_colors[ble]
        n_eq = hexagon.edge_colors[tre] == hex_n.edge_colors[bre]
        e_eq = hexagon.edge_colors[tre] == hex_e.edge_colors[tle]
        if  ne_eq and n_eq and e_eq:
            count_diamonds += 1
            hexagons_involved.add(hexagon)
            hexagons_involved.add(hex_ne)
            hexagons_involved.add(hex_n)
            hexagons_involved.add(hex_e)
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
        elif event.type == refresh_matched_hexagons_event:
            refresh_matched_hexagons()
        elif event.type == pygame.QUIT:
            sys.exit()

    extra_time = 0
    if hexagon_rotated is not None:
        rotate_sound.play()
        hexagons_in_match, diamonds_matched = check_all_adjacent_diamonds(hexagon_rotated, row, column)
        if diamonds_matched > 0:
            score += math.pow(diamonds_matched, 2) * 100
            match_sound.play()
            extra_time += extra_seconds * diamonds_matched * 1000
            for hexagon in hexagons_in_match:
                hexagon.base_color = pink
                hexagon.was_matched = True
            pygame.time.set_timer(refresh_matched_hexagons_event, 1000, True)

    if debug:
        print("Tick")

    screen.fill(dark_gray)
    #pygame.draw.rect(screen, sky_blue, pygame.Rect(100, 80, 850, 500))
    pygame.draw.rect(screen, (10, 10, 10), pygame.Rect(150, 150, 700, 370))
    for row in hexagon_array:
        for hexagon in row:
            draw_hexagon(hexagon)

    time_text_surface = font.render(f"Time {int(time_left / 1000)}        Score {int(score)}", True, red)
    time_text_rect = time_text_surface.get_rect()
    time_text_rect.center = (200, 45)
    screen.blit(time_text_surface, time_text_rect)

    if time_left <= 0:
        # FIXME this is a bad way of doing this
        return True, True

    pygame.display.flip()

    return (time_left - clock.get_time() + extra_time, score)


time_left = 100.0 * 1000.0
score = 0
game_loop(time_left, score) # first iteration so the screen comes up before the music starts
pygame.mixer.music.play(-1)
# TODO!!
#pygame.time.set_timer(refresh_background_hexagons_event
while True:
    time_left, score = game_loop(time_left, score)
    if time_left is True:
        break

if time_left is True:
    game_over()
