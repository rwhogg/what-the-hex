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

# FIXME: ideally, this would be determined via screen position calculation rather than iteration
# But so far it seems acceptably fast
def rotate_hexagon(hexagon_array, direction, position):
    for row in range(len(hexagon_array)):
        for column in range(len(hexagon_array[row])):
            hexagon = hexagon_array[row][column]
            if hexagon.point_is_inside(position):
                hexagon.rotate(direction)
                return hexagon, row, column
    return None, None, None
