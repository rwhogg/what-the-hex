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

import os
import sys

import pygame

import colors
import constants
import game_resources


def draw_bg(screen, bg_image):
    if bg_image is None:
        screen.fill(colors.BACKGROUND_BACKUP_COLOR)
    else:
        screen.blit(
            bg_image,
            pygame.Rect(0, 0, constants.SCREEN_WIDTH, constants.SCREEN_HEIGHT))


def draw_bottom(screen, icon, mouse_right_image, mouse_left_image,
                rotate_clockwise_image, rotate_counterclockwise_image):
    if mouse_left_image is not None:
        pygame.draw.rect(screen, colors.GRAY,
                         pygame.Rect(0, 600, constants.SCREEN_WIDTH, 300))
        screen.blit(mouse_left_image, pygame.Rect(200, 650, 100, 100))
        screen.blit(rotate_counterclockwise_image,
                    pygame.Rect(250, 650, 100, 100))
        screen.blit(rotate_clockwise_image,
                    pygame.Rect(constants.SCREEN_WIDTH - 350, 650, 100, 100))
        screen.blit(mouse_right_image,
                    pygame.Rect(constants.SCREEN_WIDTH - 300, 650, 100, 100))
        screen.blit(icon, pygame.Rect(25, 650, 100, 100))
        screen.blit(icon,
                    pygame.Rect(constants.SCREEN_WIDTH - 125, 650, 100, 100))


def draw_rhombuses(screen):
    pygame.draw.rect(screen, colors.RHOMBUS_COLOR,
                     pygame.Rect(150, constants.SCREEN_HEIGHT / 6, 750, 385))


def draw_stats(screen, font, stats):
    current_high_score = int(max(stats["current_score"], stats["previous_hiscore"]))
    time_remaining_text = str(int(stats["time_remaining"] / 1000))
    score_text = str(int(stats["current_score"]))
    hiscore_text = str(int(current_high_score))
    time_and_score = f"Time {time_remaining_text}        Score {score_text}        HiScore {hiscore_text}"
    time_text_surface = font.render(time_and_score, True, colors.RED)
    time_text_rect = time_text_surface.get_rect()
    time_text_rect.center = (300, 45)
    screen.blit(time_text_surface, time_text_rect)


def game_over(high_score_value):
    pygame.mixer.music.stop()
    game_over_sound = pygame.mixer.Sound(game_resources.GAME_OVER_SOUND_NAME)
    game_over_voice = pygame.mixer.Sound(game_resources.GAME_OVER_VOICE_NAME)
    game_over_sound.play()
    pygame.time.wait(int(game_over_sound.get_length() * 1000))
    game_over_voice.play()
    pygame.time.wait(int(game_over_voice.get_length() * 1000 + 1500))
    with open(constants.HISCORE_FILE_PATH, "w") as hiscore_file:
        hiscore_file.write(str(int(high_score_value)))
    sys.exit()


# FIXME: there is some bug with the high score handling
def get_old_hiscore():
    if os.path.isfile(constants.HISCORE_FILE_PATH) and os.access(
            constants.HISCORE_FILE_PATH, os.R_OK):
        with open(constants.HISCORE_FILE_PATH, "r") as high_score_file:
            return int(high_score_file.read())
    return 0
