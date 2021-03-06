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

    advantage_hexagon = hexagon_struct.HexagonStruct((constants.SCREEN_WIDTH / 2, images_top + 50), advantage_color,
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
