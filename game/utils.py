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

import pygame

try:
    from . import colors
    from . import constants
except ImportError:
    import colors
    import constants


def draw_bg(screen, images):
    screen.blit(images["bg_image"], pygame.Rect(0, 0, constants.SCREEN_WIDTH, constants.SCREEN_HEIGHT))


def draw_bottom(screen, images):
    pygame.draw.rect(screen, colors.GRAY, pygame.Rect(0, 600, constants.SCREEN_WIDTH, 300))
    screen.blit(images["mouse_left_image"], pygame.Rect(200, 650, 100, 100))
    screen.blit(images["rotate_counterclockwise_image"], pygame.Rect(250, 650, 100, 100))
    screen.blit(images["rotate_clockwise_image"], pygame.Rect(constants.SCREEN_WIDTH - 350, 650, 100, 100))
    screen.blit(images["mouse_right_image"], pygame.Rect(constants.SCREEN_WIDTH - 300, 650, 100, 100))
    screen.blit(images["icon"], pygame.Rect(25, 650, 100, 100))
    screen.blit(images["icon"], pygame.Rect(constants.SCREEN_WIDTH - 125, 650, 100, 100))


def draw_rhombuses(screen, rhombus_color):
    # FIXME: get rid of these hardcoded numbers
    pygame.draw.rect(screen, rhombus_color, pygame.Rect(150, constants.SCREEN_HEIGHT / 6, 750, 385))


def draw_stats(screen, font, stats):
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


def seconds_to_millis_plus_spare(seconds: int) -> int:
    return seconds * 1000 + 1000


def write_hiscore(hiscore: float):
    with open(constants.HISCORE_FILE_PATH, "w") as hiscore_file:
        hiscore_file.write(str(int(hiscore)))


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


def get_old_hiscore():
    if os.path.isfile(constants.HISCORE_FILE_PATH) and os.access(constants.HISCORE_FILE_PATH, os.R_OK):
        with open(constants.HISCORE_FILE_PATH, "r") as high_score_file:
            return int(high_score_file.read())
    return 0
