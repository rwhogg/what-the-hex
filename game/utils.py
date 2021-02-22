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
    from . import constants
except ImportError:
    import constants


def seconds_to_millis_plus_spare(seconds: int) -> int:
    return seconds * 1000 + 1000


def write_hiscore(hiscore: float):
    user_data_dir = appdirs.user_data_dir(constants.PACKAGE, constants.AUTHOR)
    config_data_file = os.path.join(user_data_dir, constants.CONFIG_DATA_FILE)
    config = configparser.RawConfigParser()
    config.read(config_data_file)
    config.set(configparser.DEFAULTSECT, "hiscore", str(int(hiscore)))
    os.makedirs(user_data_dir, exist_ok=True)
    try:
        with open(config_data_file, "w") as hiscore_file:
            config.write(hiscore_file)
    except PermissionError:
        logging.error("Unable to open high score file")


def clean_out_sound():
    pygame.mixer.music.stop()
    pygame.time.wait(1000)


def return_to_launcher(launcher, hiscore=None):
    if hiscore is not None:
        write_hiscore(hiscore)

    pygame.quit()


def game_over(launcher, hiscore, sounds):
    clean_out_sound()
    sounds["game_over_sound"].play()
    pygame.time.wait(int(sounds["game_over_sound"].get_length() * 1000))
    sounds["game_over_voice"].play()
    pygame.time.wait(int(sounds["game_over_voice"].get_length() * 1000 + 1500))
    return_to_launcher(launcher, hiscore)


def won(launcher, hiscore: int, sounds: dict):
    clean_out_sound()
    sounds["win_sound"].play()
    pygame.time.wait(int(sounds["win_sound"].get_length() * 1000))
    sounds["win_voice"].play()
    pygame.time.wait(int(sounds["win_voice"].get_length() * 1000 + 1500))
    return_to_launcher(launcher, hiscore)


def load_config():
    user_data_dir = appdirs.user_data_dir(constants.PACKAGE, constants.AUTHOR)
    config_data_file = os.path.join(user_data_dir, constants.CONFIG_DATA_FILE)
    config = configparser.RawConfigParser()
    config.read(config_data_file)
    old_hiscore = config.getint(configparser.DEFAULTSECT, "hiscore", fallback=0)
    return old_hiscore
