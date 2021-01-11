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
import toga

# Note, I think Toga is actually running as the main package in Briefcase,
# so this import style is necessary (but only in this file, strangely)
from . import colors
from . import constants
from . import events
from . import exceptions
from . import game_loop
from . import hexagon_utils
from . import setup
from . import utils


def run_loop(launcher):
    screen, clock, font, previous_hiscore, ui_images, sounds = setup.setup()
    hexagon_array = hexagon_utils.random_hexagon_array([constants.SCREEN_WIDTH / 8, constants.SCREEN_HEIGHT / 6],
                                                       constants.HEXAGON_ROWS, constants.HEXAGON_COLUMNS)
    time_left = constants.INITIAL_TIME_MILLIS
    num_to_match = constants.NUM_TO_MATCH
    num_to_refresh = 0
    score = 0

    game_loop.game_loop(time_left, score, num_to_refresh, clock, hexagon_array, screen, font, previous_hiscore,
                        ui_images, sounds, num_to_match,
                        launcher)  # first iteration so the screen comes up before the music starts
    num_to_refresh = 1
    pygame.mixer.music.play(constants.LOOP_FOREVER)
    pygame.time.set_timer(events.REFRESH_BACKGROUND_HEXAGONS_EVENT, constants.REFRESH_BACKGROUND_HEXAGONS_TIME_MILLIS)
    pygame.time.set_timer(events.INCREASE_REFRESH_RATE_EVENT, constants.INCREASE_REFRESH_RATE_TIME_MILLIS)
    while True:
        try:
            time_left, score, num_to_refresh, num_to_match = game_loop.game_loop(time_left, score, num_to_refresh,
                                                                                 clock, hexagon_array, screen, font,
                                                                                 previous_hiscore, ui_images, sounds,
                                                                                 num_to_match, launcher)
        except exceptions.GameOver as e:
            utils.game_over(launcher, max(e.score, previous_hiscore), sounds)
            return
        except exceptions.Won as e:
            utils.won(launcher, max(e.score, previous_hiscore), sounds)
            return
        except exceptions.Quit:
            utils.return_to_launcher(launcher)
            return


def start_game(launcher):
    #launcher.hide()
    run_loop(launcher)


def init_ui(launcher):
    box = toga.Box()
    label = toga.Label(constants.PRETTY_GAME_NAME,
                       style=toga.style.pack.Pack(font_size=20, color=colors.to_travertino(colors.RED)))
    box.add(label)
    button = toga.Button("Start!", on_press=lambda _: start_game(launcher))
    button.text_color = colors.to_travertino(colors.RED)
    box.add(button)
    return box


def init_launcher():
    return toga.App(constants.PRETTY_GAME_NAME, constants.PACKAGE, startup=init_ui)


if __name__ == "__main__":
    app = init_launcher()
    app.main_loop()
