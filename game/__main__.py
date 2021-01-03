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
import sys

import pygame

import colors
import constants
import events
import game_resources
import hexagon_utils
import utils

# Setup
def setup():
    icon = None
    if pygame.image.get_extended():
        icon = pygame.image.load(game_resources.ICON_NAME)
    if icon is not None:
        pygame.display.set_icon(icon)
    pygame.init()
    screen = pygame.display.set_mode(constants.SCREEN_SIZE)
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

    return screen, clock, font, bg_image, refresh_sound, rotate_sound, match_sound, mouse_left_image, mouse_right_image, rotate_clockwise_image, rotate_counterclockwise_image, icon


screen, clock, font, bg_image, refresh_sound, rotate_sound, match_sound, mouse_left_image, mouse_right_image, rotate_clockwise_image, rotate_counterclockwise_image, icon = setup()
previous_high_score = utils.get_old_hiscore()
hexagon_array = hexagon_utils.random_hexagon_array([constants.SCREEN_WIDTH / 8, constants.SCREEN_HEIGHT / 6])


def game_loop(time_remaining, current_score, hexagons_to_refresh):
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
                hexagon_utils.pick_background_hexagons_to_refresh(hexagon_array, hexagons_to_refresh)
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
            current_score += math.pow(diamonds_matched, 2) * 100
            match_sound.play()
            extra_time += constants.EXTRA_SECONDS * diamonds_matched * 1000
            for hexagon in hexagons_in_match:
                hexagon.base_color = color_to_flash
                hexagon.was_matched = True
            pygame.time.set_timer(events.REFRESH_MATCHED_HEXAGONS_EVENT, 1000, True)

    # UI drawing

    if bg_image is None:
        screen.fill(colors.DARK_GRAY)
    else:
        screen.blit(bg_image, pygame.Rect(0, 0, constants.SCREEN_WIDTH, constants.SCREEN_HEIGHT))

    pygame.draw.rect(screen, colors.RHOMBUS_COLOR,
                     pygame.Rect(150, constants.SCREEN_HEIGHT / 6, 750, 385))
    for row in hexagon_array:
        for hexagon in row:
            hexagon_utils.draw_hexagon(screen, hexagon)

    current_high_score = int(max(score, previous_high_score))
    time_and_score = f"Time {int(time_remaining / 1000)}        Score {int(current_score)}        HiScore {current_high_score}"
    time_text_surface = font.render(time_and_score, True, colors.RED)
    time_text_rect = time_text_surface.get_rect()
    time_text_rect.center = (300, 45)
    screen.blit(time_text_surface, time_text_rect)

    if mouse_left_image is not None:
        pygame.draw.rect(screen, colors.GRAY,
                         pygame.Rect(0, 600, constants.SCREEN_WIDTH, 300))
        screen.blit(mouse_left_image, pygame.Rect(200, 650, 100, 100))
        screen.blit(rotate_counterclockwise_image,
                    pygame.Rect(250, 650, 100, 100))
        screen.blit(rotate_clockwise_image,
                    pygame.Rect(constants.SCREEN_WIDTH - 350, 650, 100, 100))
        screen.blit(mouse_right_image, pygame.Rect(constants.SCREEN_WIDTH - 300, 650, 100, 100))
        screen.blit(icon, pygame.Rect(25, 650, 100, 100))
        screen.blit(icon, pygame.Rect(constants.SCREEN_WIDTH - 125, 650, 100, 100))

    if time_remaining <= 0:
        # FIXME this is a bad way of doing this
        return True, True, True

    pygame.display.flip()

    return time_remaining - clock.get_time() + extra_time, current_score, hexagons_to_refresh


time_left = constants.INITIAL_TIME_MILLIS
num_to_refresh = 0
score = 0
game_loop(time_left, score, num_to_refresh
          )  # first iteration so the screen comes up before the music starts
num_to_refresh = 1
pygame.mixer.music.play(constants.LOOP_FOREVER)
pygame.time.set_timer(events.REFRESH_BACKGROUND_HEXAGONS_EVENT, constants.REFRESH_BACKGROUND_HEXAGONS_TIME_MILLIS)
pygame.time.set_timer(events.INCREASE_REFRESH_RATE_EVENT, constants.INCREASE_REFRESH_RATE_TIME_MILLIS)
while True:
    time_left, score, num_to_refresh = game_loop(time_left, score, num_to_refresh)
    if time_left is True:
        utils.game_over(max(score, previous_high_score))
