// Copyright 2021 Bob "Wombat" Hogg
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


/**
 * Enum that specifies a "direction" - either left or right.
 * Can correspond to mouse button clicks, controller buttons,
 * touch screen button presses, and rotations.
 * LEFT should map to CounterClockwise, RIGHT to Clockwise.
 */
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1027:Mark enums with FlagsAttribute", Justification = "Not combinable flags")]
public enum Direction
{
    LEFT = 0,
    COUNTERCLOCKWISE = 0,
    RIGHT = 1,
    CLOCKWISE = 1
}
