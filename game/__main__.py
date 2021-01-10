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

import pygame

import constants
import events
import game_loop
import hexagon_utils
import setup
import utils


def run_loop():
    screen, clock, font, previous_hiscore, ui_images, sounds = setup.setup()
    hexagon_array = hexagon_utils.random_hexagon_array(
        [constants.SCREEN_WIDTH / 8, constants.SCREEN_HEIGHT / 6])
    time_left = constants.INITIAL_TIME_MILLIS
    num_to_match = constants.NUM_TO_MATCH
    num_to_refresh = 0
    score = 0

    game_loop.game_loop(
        time_left, score, num_to_refresh, clock, hexagon_array, screen, font,
        previous_hiscore, ui_images, sounds, num_to_match
    )  # first iteration so the screen comes up before the music starts
    num_to_refresh = 1
    pygame.mixer.music.play(constants.LOOP_FOREVER)
    pygame.time.set_timer(events.REFRESH_BACKGROUND_HEXAGONS_EVENT,
                          constants.REFRESH_BACKGROUND_HEXAGONS_TIME_MILLIS)
    pygame.time.set_timer(events.INCREASE_REFRESH_RATE_EVENT,
                          constants.INCREASE_REFRESH_RATE_TIME_MILLIS)
    while True:
        time_left, score, num_to_refresh, num_to_match = game_loop.game_loop(
            time_left, score, num_to_refresh, clock, hexagon_array, screen,
            font, previous_hiscore, ui_images, sounds, num_to_match)
        if time_left is True:
            utils.game_over(max(score, previous_hiscore))


if __name__ == "__main__":
    run_loop()
