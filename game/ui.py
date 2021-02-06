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

import importlib

import toga

try:
    from . import colors
    from . import constants
    from . import game_loop
except ImportError:
    import colors
    import constants
    import game_loop

levels = ["one", "two"]


def init_ui(launcher):
    box = toga.Box()
    top_box = toga.Box()
    label = toga.Label(constants.PRETTY_GAME_NAME,
                       style=toga.style.pack.Pack(font_size=20,
                                                  color=colors.to_travertino(colors.RED),
                                                  text_align=toga.style.pack.CENTER,
                                                  padding=(10, 300)))
    top_box.add(label)
    box.add(top_box)
    level_select_box = toga.Box()
    level_label = toga.Label("Level", style=toga.style.pack.Pack(font_size=14,
                                                                 color=colors.to_travertino(colors.BLACK),
                                                                 text_align=toga.style.pack.CENTER,
                                                                 padding=(10, 300)))
    level_input = toga.NumberInput(min_value=1, max_value=len(levels))
    level_input.value = 1
    level_select_box.add(level_label)
    level_select_box.add(level_input)
    launcher.level_select = level_input
    box.add(level_select_box)
    button_box = toga.Box()
    button = toga.Button("Start!",
                         on_press=(lambda _: start_game(launcher)),
                         style=toga.style.pack.Pack(padding=(10, 350)))
    button.text_color = colors.to_travertino(colors.RED)
    button_box.add(button)
    box.add(button_box)
    box.style.update(direction=toga.style.pack.COLUMN)
    return box


def init_launcher() -> toga.App:
    return toga.App(constants.PRETTY_GAME_NAME, constants.PACKAGE, startup=init_ui)


def start_game(launcher):
    level_name = levels[int(launcher.level_select.value - 1)]
    try:
        level_one = importlib.import_module("levels." + level_name)
    except ModuleNotFoundError:
        level_one = importlib.import_module("game.levels." + level_name)
    game_loop.run_loop(launcher, level_one)
