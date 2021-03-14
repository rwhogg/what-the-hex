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

import random
import typing

import pygame

try:
    from . import config_utils
except ImportError:
    import config_utils


def seconds_to_millis_plus_spare(seconds: int) -> int:
    return seconds * 1000 + 1000


def pick_from(lst: list):
    return random.choices(lst, k=1)[0]


def is_number(var: typing.Any):
    return type(var) in [int, float, complex]


def clean_out_sound() -> None:
    pygame.mixer.music.stop()
    pygame.time.wait(1000)


def return_to_launcher(hiscore=None) -> None:
    if is_number(hiscore):
        config_utils.write_hiscore(hiscore)

    pygame.quit()


def game_over(hiscore, sounds) -> None:
    clean_out_sound()
    sounds["game_over_sound"].play()
    pygame.time.wait(int(sounds["game_over_sound"].get_length() * 1000))
    sounds["game_over_voice"].play()
    pygame.time.wait(int(sounds["game_over_voice"].get_length() * 1000 + 1500))
    return_to_launcher(hiscore)


def won(hiscore: int, sounds: dict) -> None:
    clean_out_sound()
    sounds["win_sound"].play()
    pygame.time.wait(int(sounds["win_sound"].get_length() * 1000))
    sounds["win_voice"].play()
    pygame.time.wait(int(sounds["win_voice"].get_length() * 1000 + 1500))
    return_to_launcher(hiscore)
