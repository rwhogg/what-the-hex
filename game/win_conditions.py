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

from abc import ABC, abstractmethod


class WinConditions(ABC):

    @abstractmethod
    def has_won(self, stats):
        pass


class MinMatches(WinConditions):

    def __init__(self, matches_needed):
        self.matches_needed = matches_needed

    def has_won(self, stats):
        matches = stats.get("matches")
        if matches is None:
            return False
        return sum(matches.values()) >= self.matches_needed


class MinMatchesOfAllColors(WinConditions):

    def __init__(self, matches_needed):
        self.matches_needed = matches_needed

    def has_won(self, stats):
        matches = stats.get("matches")
        if matches is None:
            return False
        return all(map(lambda match: match >= self.matches_needed, matches.values()))
