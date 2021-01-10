#!/usr/bin/env bash

pip install briefcase
pip install flake8 mypy nose pylint yapf

# This should install from pyproject.toml,
# but until https://github.com/pypa/pip/issues/8049 is resolved, not happening...
pip install pygame
