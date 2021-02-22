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
    from . import colors
    from . import constants
    from . import hexagon_struct
    from . import hexagon_utils
except ImportError:
    import colors
    import constants
    import hexagon_struct
    import hexagon_utils


def draw_bg(screen: pygame.Surface, images: dict):
    screen.blit(images["bg_image"], pygame.Rect(0, 0, constants.SCREEN_WIDTH, constants.SCREEN_HEIGHT))


def draw_bottom(screen: pygame.Surface, images: dict, advantage_color: pygame.Color, font: pygame.font.Font):
    panel_top = 600
    images_top = 650
    image_size = (100, 100)
    pygame.draw.rect(screen, colors.GRAY, pygame.Rect((0, panel_top), (constants.SCREEN_WIDTH, 300)))

    screen.blit(images["mouse_left_image"], pygame.Rect((200, images_top), image_size))
    screen.blit(images["rotate_counterclockwise_image"], pygame.Rect((250, images_top), image_size))

    # FIXME: I need an abstraction layer for text rendering. It currently sucks
    advantage_color_text = "Advantage Color"
    advantage_color_surface: pygame.Surface = font.render(advantage_color_text, True, colors.RED)
    advantage_color_rect = advantage_color_surface.get_rect()
    advantage_color_rect.center = (int(constants.SCREEN_WIDTH / 2), panel_top + 20)
    screen.blit(advantage_color_surface, advantage_color_rect)

    advantage_hexagon = hexagon_struct.HexagonStruct([constants.SCREEN_WIDTH / 2, images_top + 50], advantage_color,
                                                     [advantage_color for i in range(6)])
    hexagon_utils.draw_hexagon(screen, advantage_hexagon)

    screen.blit(images["rotate_clockwise_image"], pygame.Rect((constants.SCREEN_WIDTH - 350, images_top), image_size))
    screen.blit(images["mouse_right_image"], pygame.Rect((constants.SCREEN_WIDTH - 300, images_top), image_size))
    screen.blit(images["icon"], pygame.Rect((25, images_top), image_size))
    screen.blit(images["icon"], pygame.Rect((constants.SCREEN_WIDTH - 125, images_top), image_size))


def draw_rhombuses(screen: pygame.Surface, rhombus_color: pygame.Color):
    # FIXME: get rid of these hardcoded numbers
    pygame.draw.rect(screen, rhombus_color, pygame.Rect(150, constants.SCREEN_HEIGHT / 6, 750, 385))


def draw_stats(screen: pygame.Surface, font: pygame.font.Font, stats: dict):
    current_high_score = int(max(stats["current_score"], stats["previous_hiscore"]))
    time_remaining_text = "Time " + str(int(stats["time_remaining"] / 1000))
    score_text = "Score " + str(int(stats["current_score"]))
    hiscore_text = "HiScore " + str(int(current_high_score))
    matches_left_text = "Matches " + str(int(stats["matches_left"]))
    time_and_score = f"{time_remaining_text}        {score_text}        {hiscore_text}        {matches_left_text}"
    time_text_surface = font.render(time_and_score, True, colors.RED)
    time_text_rect = time_text_surface.get_rect()
    time_text_rect.center = (5 * constants.SCREEN_WIDTH / 11, 45)
    screen.blit(time_text_surface, time_text_rect)


def draw_ui(screen: pygame.Surface, ui_images: dict, hexagon_array, font: pygame.font.Font, stats: dict, colors: dict):
    draw_bg(screen, ui_images)
    draw_rhombuses(screen, colors["rhombus_color"])
    for row in hexagon_array:
        for hexagon in row:
            hexagon_utils.draw_hexagon(screen, hexagon)
    draw_stats(screen, font, stats)
    draw_bottom(screen, ui_images, stats["advantage_color"], font)
    pygame.display.flip()


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
