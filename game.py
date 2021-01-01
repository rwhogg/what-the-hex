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

import sys

import pygame

pygame.init()
size = width, height = 640, 480
HEXAGON_SIDE_LENGTH = 70
screen = pygame.display.set_mode(size)
clock = pygame.time.Clock()

# FIXME: I'll probably want some nicer colors later. For now, primaries are fine
black = 0, 0, 0
blue = 0, 0, 255
green = 0, 255, 0
yellow = 255, 255, 0

def draw_hexagon(base_color, center, len):
    x_0 = center[0]
    y_0 = center[1]
    p1 = (0.5 * len + x_0, y_0 - 0.866025 * len)
    p2 = (len + x_0, y_0)
    p3 = (0.5 * len + x_0, 0.866025 * len + y_0)
    p4 = (x_0 - 0.5 * len, 0.866025 * len + y_0)
    p5 = (x_0 - len, y_0)
    p6 = (x_0 - 0.5 * len, y_0 - 0.866025 * len)
    points = [p1, p2, p3, p4, p5, p6]
    pygame.draw.polygon(screen, base_color, points)

while 1:
    clock.tick(1)
    for event in pygame.event.get():
        if event.type == pygame.QUIT: sys.exit()

    print("Tick")
    draw_hexagon(green, [300, 300], HEXAGON_SIDE_LENGTH)
    pygame.display.flip()
