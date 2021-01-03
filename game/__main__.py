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
import os
import os.path
import random
import sys

from pathlib import Path

import pygame

import constants
import game_resources

os.environ["SDL_VIDEO_CENTERED"] = "1"


icon = None
if pygame.image.get_extended():
    icon = pygame.image.load(game_resources.ICON_NAME)
size = width, height = 1024, 768
hexagon_side_length = 50
hexagon_rows = 5
hexagon_columns = 8
extra_seconds = 5
edge_thickness = 6

home_dir = str(Path.home())
hiscore_file_path = os.path.join(home_dir, ".what-the-hex.hiscore")
debug = False

# Events
refresh_matched_hexagons_event = pygame.USEREVENT + 1
refresh_background_hexagons_event = refresh_matched_hexagons_event + 1
increase_refresh_rate_event = refresh_background_hexagons_event + 1

# Colors
# FIXME: create separate loadable color schemes
# with symbolic color names rather than explicit ones
black = pygame.Color(0, 0, 0)
white = pygame.Color(255, 255, 255)

# using for other stuff
offwhite = pygame.Color(255, 0xfa, 0xf0)
faint_blue = pygame.Color(0x87, 0xce, 0xfa)
sky_blue = pygame.Color(0, 0xbf, 255)
dark_gray = pygame.Color(0x2f, 0x4f, 0x4f)  # also too green...

# reserved for the colors of the various hexagon states
green = black  # FIXME yes, I know, green isn't black. Lay off me, I was experimenting with the colors _a lot_
red = pygame.Color(0xdc, 0x14, 0x3c)
crimson = pygame.Color(0x40, 0xe0, 0xd0)  # FIXME not actually crimson
refresh_color = faint_blue
pink = pygame.Color(0xad, 255, 0x2f)

# edge colors
# FIXME: 3 colors are too few. Too easy to make matches unintentionally
# 6 sounds like too many, so probably 4 or 5???
# trying to get 5 mutually non-clashing colors, plus avoiding the hexagon state ones, is too hard...
# stick with 4, just make the game refresh more frequently
blue = pygame.Color(0, 0, 0x90)  # too hard to distinguish from the purple
purple = pygame.Color(0x4b, 0, 82)
orange = pygame.Color(255, 0x63, 0x47)
maroon = pygame.Color(0x80, 0, 0)  # too subtle
yellow = pygame.Color(255, 0xd7, 0)  # hurts my eyes...
true_pink = pygame.Color(255, 0x14, 0x93)
teal = pygame.Color(0, 0x80, 0x80)  # too hard to see against the green
true_green = pygame.Color(0x7c, 0xfc, 0)

dark_gray_no_green = pygame.Color(0x2f, 0, 0x4f)
very_red = pygame.Color(255, 0, 0)  # clashes with the orange

edge_colors = [true_green, purple, true_pink, yellow]

# Setup

high_score = 0
if os.path.isfile(hiscore_file_path) and os.access(hiscore_file_path, os.R_OK):
    with open(hiscore_file_path, "r") as high_score_file:
        high_score = int(high_score_file.read())
previous_high_score = high_score

if icon is not None:
    pygame.display.set_icon(icon)
pygame.init()
screen = pygame.display.set_mode(size)
pygame.display.set_caption(constants.PRETTY_GAME_NAME)
clock = pygame.time.Clock()
font = pygame.font.Font(game_resources.FONT_NAME, 24)
pygame.mixer.music.load(game_resources.MUSIC_NAME)
refresh_sound = pygame.mixer.Sound(game_resources.REFRESH_SOUND_NAME)
rotate_sound = pygame.mixer.Sound(game_resources.ROTATE_SOUND_NAME)
match_sound = pygame.mixer.Sound(game_resources.MATCH_SOUND_NAME)

mouse_left_image, rotate_counterclockwise_image, mouse_right_image, rotate_clockwise_image = None, None, None, None
bg_image = None
if pygame.image.get_extended():
    mouse_left_image = pygame.image.load(game_resources.MOUSE_LEFT_IMAGE_NAME)
    rotate_counterclockwise_image = pygame.image.load(
        game_resources.ROTATE_COUNTERCLOCKWISE_IMAGE_NAME)
    mouse_right_image = pygame.image.load(game_resources.MOUSE_RIGHT_IMAGE_NAME)
    rotate_clockwise_image = pygame.image.load(game_resources.ROTATE_CLOCKWISE_IMAGE_NAME)
    bg_image = pygame.image.load(game_resources.BACKGROUND_IMAGE_NAME)


class HexagonStruct:
    def __init__(self, center, base_color, edge_colors) -> None:
        if len(edge_colors) != 6:
            raise Exception
        self.center = center
        self.base_color = base_color
        self.edge_colors = edge_colors
        self.was_matched = False
        self.to_refresh = False

    def get_points(self):
        center_x = self.center[0]
        center_y = self.center[1]
        p0 = (0.5 * hexagon_side_length + center_x,
              center_y - 0.866025 * hexagon_side_length)
        p1 = (hexagon_side_length + center_x, center_y)
        p2 = (0.5 * hexagon_side_length + center_x,
              0.866025 * hexagon_side_length + center_y)
        p3 = (center_x - 0.5 * hexagon_side_length,
              0.866025 * hexagon_side_length + center_y)
        p4 = (center_x - hexagon_side_length, center_y)
        p5 = (center_x - 0.5 * hexagon_side_length,
              center_y - 0.866025 * hexagon_side_length)
        return [p0, p1, p2, p3, p4, p5]

    @staticmethod
    def get_big_radius():
        return hexagon_side_length * 2

    @staticmethod
    def get_small_radius():
        return hexagon_side_length * math.cos(30)

    def get_edges(self):
        points = self.get_points()
        return [[points[a], points[(a + 1) % 6]] for a in range(6)]

    def point_is_inside(self, point):
        # this isn't strictly correct, but it's accurate enough for my purposes
        # (the 35 is just a little extra tolerance)
        return math.hypot(point[0] - self.center[0], point[1] -
                          self.center[1]) <= self.get_small_radius() + 35

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
        pygame.draw.line(screen, colors[e], edges[e][0], edges[e][1],
                         edge_thickness)


def random_hexagon(center, base_color):
    colors = random.choices(edge_colors, k=6)
    return HexagonStruct(center, base_color, colors)


def random_hexagon_array(start):
    hexagons = [[] for _ in range(hexagon_rows)]
    for i in range(hexagon_rows):
        for j in range(hexagon_columns):
            center_x = start[0] + j * hexagon_side_length * math.cos(30) * 14
            center_y = start[1] + i * hexagon_side_length * math.cos(30) * 12.5
            center = [center_x, center_y]
            hexagons[i].append(random_hexagon(center, green))
    return hexagons


hexagon_array = random_hexagon_array([width / 8, height / 6])


def refresh_matched_hexagons():
    refresh_hexagons(lambda hexagon: hexagon.was_matched)


def refresh_background_hexagons():
    refresh_hexagons(lambda hexagon: hexagon.to_refresh)


def refresh_hexagons(predicate):
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            if predicate(hexagon_array[row][column]):
                hexagon_array[row][column] = random_hexagon(
                    hexagon_array[row][column].center, green)


def pick_background_hexagons_to_refresh(num_to_refresh):
    num_refreshed = 0
    num_columns = len(hexagon_array[0])
    num_rows = len(hexagon_array)
    i = 0
    while num_refreshed < num_to_refresh:
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

    return (hexagons_involved, count_diamonds, color_to_flash)


def game_over(high_score):
    pygame.mixer.music.stop()
    game_over_sound = pygame.mixer.Sound(game_resources.GAME_OVER_SOUND_NAME)
    game_over_voice = pygame.mixer.Sound(game_resources.GAME_OVER_VOICE_NAME)
    game_over_sound.play()
    pygame.time.wait(int(game_over_sound.get_length() * 1000))
    game_over_voice.play()
    pygame.time.wait(int(game_over_voice.get_length() * 1000 + 1500))
    print(high_score)
    with open(hiscore_file_path, "w") as high_score_file:
        high_score_file.write(str(int(high_score)))
    sys.exit()


def game_loop(time_left, score, num_to_refresh, high_score):
    clock.tick()

    hexagon_rotated, row, column = None, None, None
    for event in pygame.event.get():
        if event.type == pygame.MOUSEBUTTONDOWN:
            dir = "left" if event.button == 1 else "right"
            hexagon_rotated, row, column = rotate_hexagon(dir, event.pos)
        elif event.type == refresh_matched_hexagons_event:
            refresh_matched_hexagons()
        elif event.type == refresh_background_hexagons_event:
            if num_to_refresh > 0:
                refresh_background_hexagons()
                refresh_sound.play()
                pick_background_hexagons_to_refresh(num_to_refresh)
        elif event.type == increase_refresh_rate_event:
            num_to_refresh += 1
        elif event.type == pygame.QUIT:
            sys.exit()

    extra_time = 0
    if hexagon_rotated is not None:
        rotate_sound.play()
        hexagons_in_match, diamonds_matched, color_to_flash = check_all_adjacent_diamonds(
            hexagon_rotated, row, column)
        if diamonds_matched > 0:
            score += math.pow(diamonds_matched, 2) * 100
            if score >= high_score:
                high_score = int(score)
            match_sound.play()
            extra_time += extra_seconds * diamonds_matched * 1000
            for hexagon in hexagons_in_match:
                hexagon.base_color = color_to_flash
                hexagon.was_matched = True
            pygame.time.set_timer(refresh_matched_hexagons_event, 1000, True)

    if debug:
        print("Tick")

    # UI drawing

    if bg_image is None:
        screen.fill(dark_gray)
    else:
        screen.blit(bg_image, pygame.Rect(0, 0, width, height))

    diamond_color = pygame.Color(0x30, 0x30, 0x30)
    pygame.draw.rect(screen, diamond_color,
                     pygame.Rect(150, height / 6, 750, 385))
    for row in hexagon_array:
        for hexagon in row:
            draw_hexagon(hexagon)

    time_and_score = f"Time {int(time_left / 1000)}        Score {int(score)}        HiScore {int(high_score)}"
    time_text_surface = font.render(time_and_score, True, red)
    time_text_rect = time_text_surface.get_rect()
    time_text_rect.center = (300, 45)
    screen.blit(time_text_surface, time_text_rect)

    if mouse_left_image is not None:
        pygame.draw.rect(screen, (0x93, 0x70, 0xdb),
                         pygame.Rect(0, 600, width, 300))
        screen.blit(mouse_left_image, pygame.Rect(200, 650, 100, 100))
        screen.blit(rotate_counterclockwise_image,
                    pygame.Rect(250, 650, 100, 100))
        screen.blit(rotate_clockwise_image,
                    pygame.Rect(width - 350, 650, 100, 100))
        screen.blit(mouse_right_image, pygame.Rect(width - 300, 650, 100, 100))
        screen.blit(icon, pygame.Rect(25, 650, 100, 100))
        screen.blit(icon, pygame.Rect(width - 125, 650, 100, 100))

    if time_left <= 0:
        # FIXME this is a bad way of doing this
        return True, True, True, high_score

    pygame.display.flip()

    return (time_left - clock.get_time() + extra_time, score, num_to_refresh,
            high_score)


time_left = 100.0 * 1000.0
num_to_refresh = 0
score = 0
game_loop(time_left, score, num_to_refresh, high_score
          )  # first iteration so the screen comes up before the music starts
num_to_refresh = 1
pygame.mixer.music.play(-1)
pygame.time.set_timer(refresh_background_hexagons_event, 10 * 1000)
pygame.time.set_timer(increase_refresh_rate_event, 20 * 1000)
while True:
    time_left, score, num_to_refresh, high_score = game_loop(
        time_left, score, num_to_refresh, high_score)
    if time_left is True:
        break

if time_left is True:
    print(high_score)
    print(previous_high_score)
    game_over(max(high_score, previous_high_score))
