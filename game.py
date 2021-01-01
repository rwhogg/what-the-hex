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

# FIXME: I'll probably want some nicer colors later. For now, primaries are fine
black = 0, 0, 0
white = 255, 255, 255
blue = 0, 0, 255
green = 0, 255, 0
yellow = 255, 255, 0
red = 255, 0, 0
purple = 255, 0, 255
orange = 255, 0xa5, 0

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

    def get_small_radius(self):
        return hexagon_side_length * math.cos(30)

    def get_edges(self):
        points = self.get_points()
        return [[points[a], points[(a + 1) % 6]] for a in range(6)]


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


def game_over():
    pygame.mixer.music.stop()
    game_over_sound = pygame.mixer.Sound("game_over-sound.wav")
    game_over_voice = pygame.mixer.Sound("game_over-voice.ogg")
    game_over_sound.play()
    pygame.time.wait(int(game_over_sound.get_length() * 1000))
    game_over_voice.play()
    pygame.time.wait(int(game_over_voice.get_length() * 1000 + 1500))
    sys.exit()


def game_loop(time_left):
    clock.tick()

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            sys.exit()

    if debug:
        print("Tick")

    screen.fill(white)
    for row in hexagon_array:
        for hexagon in row:
            draw_hexagon(hexagon)

    time_text_surface = font.render(f"Time {int(time_left / 1000)}", True, black)
    screen.blit(time_text_surface, time_text_surface.get_rect())

    if time_left <= 0:
        return True

    pygame.display.flip()

    return time_left - clock.get_time()


time_left = 300.0 * 1000.0
game_loop(time_left) # first iteration so the screen comes up before the music starts
pygame.mixer.music.play(-1)
while True:
    time_left = game_loop(time_left)
    if time_left is True:
        break

if time_left is True:
    game_over()
