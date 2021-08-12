#!/usr/bin/env bash

# Copyright (c) 2007-2021 Juan Linietsky, Ariel Manzur.
# Copyright (c) 2014-2021 Godot Engine contributors.

# Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

# This script ensures proper POSIX text file formatting and a few other things.

set -uo pipefail
IFS=$'\n\t'

# Loops through all text files tracked by Git.
git grep -zIl '' |
while IFS= read -rd '' f; do
    # Exclude some types of files.
    if [[ "$f" == *"csproj" ]]; then
        continue
    elif [[ "$f" == *"hdr" ]]; then
        continue
    elif [[ "$f" == *"1bd" ]]; then
        continue
    elif [[ "$f" == "tape_todo"* ]]; then
        continue
    fi
    # Ensure that files are UTF-8 formatted.
    recode UTF-8 "$f" 2> /dev/null
    # Ensure that files have LF line endings and do not contain a BOM.
    dos2unix "$f" 2> /dev/null
    # Remove trailing space characters and ensures that files end
    # with newline characters. -l option handles newlines conveniently.
    perl -i -ple 's/\s*$//g' "$f"
    # Remove the character sequence "== true" if it has a leading space.
    perl -i -pe 's/\x20== true//g' "$f"
    # We don't want to change lines around braces in godot/tscn files.
    if [[ "$f" == *"godot" ]]; then
        continue
    elif [[ "$f" == *"tscn" ]]; then
        continue
    fi
    # Disallow empty lines after the opening brace.
    sed -z -i 's/\x7B\x0A\x0A/\x7B\x0A/g' "$f"
    # Disallow some empty lines before the closing brace.
    sed -z -i 's/\x0A\x0A\x7D/\x0A\x7D/g' "$f"
done

git diff > patch.patch
FILESIZE="$(stat -c%s patch.patch)"
MAXSIZE=5

# If no patch has been generated all is OK, clean up, and exit.
if (( FILESIZE < MAXSIZE )); then
    printf "Files in this commit comply with the formatting rules.\n"
    rm -f patch.patch
    exit 0
fi

# A patch has been created, notify the user, clean up, and exit.
printf "\n*** The following differences were found between the code "
printf "and the formatting rules:\n\n"
cat patch.patch
printf "\n*** Aborting, please fix your commit(s) with 'git commit --amend' or 'git rebase -i <hash>'\n"
rm -f patch.patch
exit 1
