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

import sys

import pygame

import constants
import game_resources
import utils


def check_prerequisites():
    if not pygame.image.get_extended():
        raise Exception("PyGame must be built with extended image support.")
    if sys.version_info[0] != 3 or sys.version_info[1] < 7:
        raise Exception("Please use Python 3.7 or later.")


def setup() -> tuple:
    check_prerequisites()
    ui_images = {
        "icon":
        pygame.image.load(game_resources.ICON_NAME),
        "mouse_left_image":
        pygame.image.load(game_resources.MOUSE_LEFT_IMAGE_NAME),
        "rotate_counterclockwise_image":
        pygame.image.load(game_resources.ROTATE_COUNTERCLOCKWISE_IMAGE_NAME),
        "mouse_right_image":
        pygame.image.load(game_resources.MOUSE_RIGHT_IMAGE_NAME),
        "rotate_clockwise_image":
        pygame.image.load(game_resources.ROTATE_CLOCKWISE_IMAGE_NAME),
        "bg_image":
        pygame.image.load(game_resources.BACKGROUND_IMAGE_NAME)
    }
    pygame.display.set_icon(ui_images["icon"])
    pygame.init()
    screen = pygame.display.set_mode(constants.SCREEN_SIZE)
    pygame.display.set_caption(constants.PRETTY_GAME_NAME)
    clock = pygame.time.Clock()
    font = pygame.font.Font(game_resources.FONT_NAME, constants.FONT_SIZE)
    pygame.mixer.music.load(game_resources.MUSIC_NAME)
    refresh_sound = pygame.mixer.Sound(game_resources.REFRESH_SOUND_NAME)
    rotate_sound = pygame.mixer.Sound(game_resources.ROTATE_SOUND_NAME)
    match_sound = pygame.mixer.Sound(game_resources.MATCH_SOUND_NAME)

    previous_hiscore = utils.get_old_hiscore()

    return screen, clock, font, refresh_sound, rotate_sound, match_sound, previous_hiscore, ui_images