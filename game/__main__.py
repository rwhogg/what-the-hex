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

import logging
import os
import pathlib

# FIXME: this should be using the appdirs
logging.basicConfig(filename=os.path.join(str(pathlib.Path.home()), ".what-the-hex.log"), level=logging.INFO)

# Note, I think Toga is actually running as the main package in Briefcase,
# so this import style is necessary.
try:
    from . import ui
except ImportError:
    import ui

if __name__ == "__main__":
    logging.info("Initializing")
    app = ui.init_launcher()
    app.main_loop()
