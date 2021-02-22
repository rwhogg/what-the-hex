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

import configparser
import logging
import os
import os.path

import appdirs
import pygame

try:
    from . import config_utils
    from . import constants
except ImportError:
    import config_utils
    import constants


def seconds_to_millis_plus_spare(seconds: int) -> int:
    return seconds * 1000 + 1000


def clean_out_sound() -> None:
    pygame.mixer.music.stop()
    pygame.time.wait(1000)


def return_to_launcher(launcher, hiscore=None) -> None:
    if hiscore is not None:
        config_utils.write_hiscore(hiscore)

    pygame.quit()


def game_over(launcher, hiscore, sounds) -> None:
    clean_out_sound()
    sounds["game_over_sound"].play()
    pygame.time.wait(int(sounds["game_over_sound"].get_length() * 1000))
    sounds["game_over_voice"].play()
    pygame.time.wait(int(sounds["game_over_voice"].get_length() * 1000 + 1500))
    return_to_launcher(launcher, hiscore)


def won(launcher, hiscore: int, sounds: dict) -> None:
    clean_out_sound()
    sounds["win_sound"].play()
    pygame.time.wait(int(sounds["win_sound"].get_length() * 1000))
    sounds["win_voice"].play()
    pygame.time.wait(int(sounds["win_voice"].get_length() * 1000 + 1500))
    return_to_launcher(launcher, hiscore)
