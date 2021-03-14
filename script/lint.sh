#!/usr/bin/env bash

# stop the build if there are Python syntax errors or undefined names
echo "Checking for syntax errors and undefined names"
flake8 game --count --select=E9,F63,F7,F82 --show-source --statistics

echo "Checking for other flake8 errors"
flake8 game --count --exit-zero --statistics
