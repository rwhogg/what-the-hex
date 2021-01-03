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
import sys

import pygame

import colors
import constants
import events
import game_resources
import hexagon_utils
import utils

icon = None
if pygame.image.get_extended():
    icon = pygame.image.load(game_resources.ICON_NAME)
size = width, height = 1024, 768

extra_seconds = 5


# Colors
# FIXME: create separate loadable color schemes
# with symbolic color names rather than explicit ones

# reserved for the colors of the various hexagon states
green = colors.BLACK  # FIXME yes, I know, green isn't black. Lay off me, I was experimenting with the colors _a lot_
refresh_color = colors.FAINT_BLUE
diamond_color = pygame.Color(0x30, 0x30, 0x30)


# Setup

high_score = 0


previous_high_score = utils.get_old_hiscore()

if icon is not None:
    pygame.display.set_icon(icon)
pygame.init()
screen = pygame.display.set_mode(size)
pygame.display.set_caption(constants.PRETTY_GAME_NAME)
clock = pygame.time.Clock()
font = pygame.font.Font(game_resources.FONT_NAME, constants.FONT_SIZE)
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


hexagon_array = hexagon_utils.random_hexagon_array([width / 8, height / 6])


def pick_background_hexagons_to_refresh(num_hexagons_to_refresh):
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


def game_loop(time_remaining, score, hexagons_to_refresh, high_score):
    clock.tick()

    hexagon_rotated, row, column = None, None, None
    for event in pygame.event.get():
        if event.type == pygame.MOUSEBUTTONDOWN:
            direction = "left" if event.button == 1 else "right"
            hexagon_rotated, row, column = hexagon_utils.rotate_hexagon(hexagon_array, direction, event.pos)
        elif event.type == events.REFRESH_MATCHED_HEXAGONS_EVENT:
            hexagon_utils.refresh_matched_hexagons(hexagon_array)
        elif event.type == events.REFRESH_BACKGROUND_HEXAGONS_EVENT:
            if hexagons_to_refresh > 0:
                hexagon_utils.refresh_background_hexagons(hexagon_array)
                refresh_sound.play()
                pick_background_hexagons_to_refresh(hexagons_to_refresh)
        elif event.type == events.INCREASE_REFRESH_RATE_EVENT:
            hexagons_to_refresh += 1
        elif event.type == pygame.QUIT:
            sys.exit()

    extra_time = 0
    if hexagon_rotated is not None:
        rotate_sound.play()
        hexagons_in_match, diamonds_matched, color_to_flash = hexagon_utils.check_all_adjacent_diamonds(
            hexagon_array, hexagon_rotated, row, column)
        if diamonds_matched > 0:
            score += math.pow(diamonds_matched, 2) * 100
            if score >= high_score:
                high_score = int(score)
            match_sound.play()
            extra_time += extra_seconds * diamonds_matched * 1000
            for hexagon in hexagons_in_match:
                hexagon.base_color = color_to_flash
                hexagon.was_matched = True
            pygame.time.set_timer(events.REFRESH_MATCHED_HEXAGONS_EVENT, 1000, True)

    # UI drawing

    if bg_image is None:
        screen.fill(colors.DARK_GRAY)
    else:
        screen.blit(bg_image, pygame.Rect(0, 0, width, height))

    pygame.draw.rect(screen, diamond_color,
                     pygame.Rect(150, height / 6, 750, 385))
    for row in hexagon_array:
        for hexagon in row:
            hexagon_utils.draw_hexagon(screen, hexagon)

    time_and_score = f"Time {int(time_remaining / 1000)}        Score {int(score)}        HiScore {int(high_score)}"
    time_text_surface = font.render(time_and_score, True, colors.RED)
    time_text_rect = time_text_surface.get_rect()
    time_text_rect.center = (300, 45)
    screen.blit(time_text_surface, time_text_rect)

    if mouse_left_image is not None:
        pygame.draw.rect(screen, colors.GRAY,
                         pygame.Rect(0, 600, width, 300))
        screen.blit(mouse_left_image, pygame.Rect(200, 650, 100, 100))
        screen.blit(rotate_counterclockwise_image,
                    pygame.Rect(250, 650, 100, 100))
        screen.blit(rotate_clockwise_image,
                    pygame.Rect(width - 350, 650, 100, 100))
        screen.blit(mouse_right_image, pygame.Rect(width - 300, 650, 100, 100))
        screen.blit(icon, pygame.Rect(25, 650, 100, 100))
        screen.blit(icon, pygame.Rect(width - 125, 650, 100, 100))

    if time_remaining <= 0:
        # FIXME this is a bad way of doing this
        return True, True, True, high_score

    pygame.display.flip()

    return (time_remaining - clock.get_time() + extra_time, score, hexagons_to_refresh,
            high_score)


time_left = constants.INITIAL_TIME_MILLIS
num_to_refresh = 0
score = 0
game_loop(time_left, score, num_to_refresh, high_score
          )  # first iteration so the screen comes up before the music starts
num_to_refresh = 1
pygame.mixer.music.play(constants.LOOP_FOREVER)
pygame.time.set_timer(events.REFRESH_BACKGROUND_HEXAGONS_EVENT, 10 * 1000)
pygame.time.set_timer(events.INCREASE_REFRESH_RATE_EVENT, 20 * 1000)
while True:
    time_left, score, num_to_refresh, high_score = game_loop(
        time_left, score, num_to_refresh, high_score)
    if time_left is True:
        break

if time_left is True:
    utils.game_over(max(high_score, previous_high_score))
