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

import pygame

try:
    from . import constants
    from . import events
    from . import exceptions
    from . import hexagon_utils
    from . import setup
    from . import utils
except ImportError:
    import constants
    import events
    import exceptions
    import hexagon_utils
    import setup
    import utils


def draw_ui(screen, ui_images: dict, hexagon_array, font, stats, colors):
    utils.draw_bg(screen, ui_images)
    utils.draw_rhombuses(screen, colors["rhombus_color"])
    for row in hexagon_array:
        for hexagon in row:
            hexagon_utils.draw_hexagon(screen, hexagon)
    utils.draw_stats(screen, font, stats)
    utils.draw_bottom(screen, ui_images)


def game_loop(time_remaining: int, current_score: int, hexagons_to_refresh: int, clock: pygame.time.Clock,
              hexagon_array, screen, font, previous_hiscore, ui_images: dict, sounds: dict, colors: dict,
              num_to_match: int, launcher) -> tuple:
    clock.tick()

    if time_remaining <= 0:
        raise exceptions.GameOver(current_score)
    elif num_to_match <= 0:
        raise exceptions.Won(current_score)

    hexagon_rotated = None
    row_num = column_num = 0
    for event in pygame.event.get():
        if event.type == pygame.MOUSEBUTTONDOWN:
            direction = "left" if event.button == 1 else "right"
            hexagon_rotated, row_num, column_num = hexagon_utils.rotate_hexagon(hexagon_array, direction, event.pos)
        elif event.type == events.REFRESH_MATCHED_HEXAGONS_EVENT:
            hexagon_utils.refresh_matched_hexagons(hexagon_array, colors["initial_hexagon_color"],
                                                   colors["edge_color_options"])
        elif event.type == events.REFRESH_BACKGROUND_HEXAGONS_EVENT:
            if hexagons_to_refresh > 0:
                hexagon_utils.refresh_background_hexagons(hexagon_array, colors["initial_hexagon_color"],
                                                          colors["edge_color_options"])
                sounds["refresh_sound"].play()
                hexagon_utils.pick_background_hexagons_to_refresh(hexagon_array, hexagons_to_refresh,
                                                                  colors["refresh_color"])
        elif event.type == events.INCREASE_REFRESH_RATE_EVENT:
            hexagons_to_refresh += 1
        elif event.type == pygame.QUIT:
            utils.return_to_launcher(launcher)
            raise exceptions.Quit

    extra_time = 0
    if hexagon_rotated is not None:
        sounds["rotate_sound"].play()
        hexagons_in_match, diamonds_matched, color_to_flash = hexagon_utils.check_all_adjacent_diamonds(
            hexagon_array, hexagon_rotated, row_num, column_num)
        if diamonds_matched > 0:
            current_score += int(math.pow(diamonds_matched, 2)) * 100
            sounds["match_sound"].play()
            extra_time += constants.EXTRA_SECONDS * diamonds_matched * 1000
            num_to_match = max(num_to_match - diamonds_matched, 0)
            for hexagon in hexagons_in_match:
                hexagon.base_color = color_to_flash
                hexagon.was_matched = True
            pygame.time.set_timer(events.REFRESH_MATCHED_HEXAGONS_EVENT, 1000, True)

    # UI drawing
    stats = {
        "current_score": current_score,
        "previous_hiscore": previous_hiscore,
        "time_remaining": time_remaining,
        "matches_left": num_to_match
    }
    draw_ui(screen, ui_images, hexagon_array, font, stats, colors)

    pygame.display.flip()

    new_time_remaining = time_remaining - clock.get_time() + extra_time
    return new_time_remaining, current_score, hexagons_to_refresh, num_to_match


def run_loop(launcher, level_data):
    screen, clock, font, previous_hiscore, ui_images, sounds, colors = setup.setup(level_data)
    hexagon_array = hexagon_utils.random_hexagon_array([constants.SCREEN_WIDTH / 8, constants.SCREEN_HEIGHT / 6],
                                                       constants.HEXAGON_ROWS, constants.HEXAGON_COLUMNS,
                                                       colors["initial_hexagon_color"], colors["edge_color_options"])
    time_left = constants.INITIAL_TIME_MILLIS
    num_to_match = constants.NUM_TO_MATCH
    num_to_refresh = 0
    score = 0

    game_loop(time_left, score, num_to_refresh, clock, hexagon_array, screen, font, previous_hiscore, ui_images, sounds,
              colors, num_to_match, launcher)  # first iteration so the screen comes up before the music starts
    num_to_refresh = 1
    pygame.mixer.music.play(constants.LOOP_FOREVER)
    pygame.time.set_timer(events.REFRESH_BACKGROUND_HEXAGONS_EVENT, constants.REFRESH_BACKGROUND_HEXAGONS_TIME_MILLIS)
    pygame.time.set_timer(events.INCREASE_REFRESH_RATE_EVENT, constants.INCREASE_REFRESH_RATE_TIME_MILLIS)
    while True:
        try:
            time_left, score, num_to_refresh, num_to_match = game_loop(time_left, score, num_to_refresh, clock,
                                                                       hexagon_array, screen, font, previous_hiscore,
                                                                       ui_images, sounds, colors, num_to_match,
                                                                       launcher)
        except exceptions.GameOver as e:
            utils.game_over(launcher, max(e.score, previous_hiscore), sounds)
            return
        except exceptions.Won as e:
            utils.won(launcher, max(e.score, previous_hiscore), sounds)
            return
        except exceptions.Quit:
            utils.return_to_launcher(launcher)
            return
