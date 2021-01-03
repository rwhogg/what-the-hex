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

import constants
import game_resources


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


def get_old_hiscore():
    if os.path.isfile(constants.HISCORE_FILE_PATH) and os.access(constants.HISCORE_FILE_PATH, os.R_OK):
        with open(constants.HISCORE_FILE_PATH, "r") as high_score_file:
            return int(high_score_file.read())
    return 0
