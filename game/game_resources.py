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

import os.path


def get_res(file_name) -> str:
    return os.path.join(os.path.dirname(os.path.realpath(__file__)),
                        "resources", file_name)


# Images
BACKGROUND_IMAGE_NAME: str = get_res("bg.png")
ICON_NAME: str = get_res("icon.png")
MOUSE_LEFT_IMAGE_NAME: str = get_res("mouseLeft.png")
MOUSE_RIGHT_IMAGE_NAME: str = get_res("mouseRight.png")
ROTATE_CLOCKWISE_IMAGE_NAME: str = get_res("rotate_clockwise.png")
ROTATE_COUNTERCLOCKWISE_IMAGE_NAME: str = get_res(
    "rotate_counterclockwise.png")

# Sounds
GAME_OVER_SOUND_NAME: str = get_res("game_over-sound.wav")
GAME_OVER_VOICE_NAME: str = get_res("game_over-voice.ogg")
REFRESH_SOUND_NAME: str = get_res("refresh.ogg")
ROTATE_SOUND_NAME: str = get_res("rotate.ogg")
MATCH_SOUND_NAME: str = get_res("match.wav")

# Fonts
FONT_NAME: str = get_res("kenney-pixel-square.ttf")

# Music
MUSIC_NAME: str = get_res("bg_music.ogg")
