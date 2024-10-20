# 2D 4X

## Using Godot 4.4.dev3.mono C#

This is the repository containing the open source code for the game creation streams that Lord Haywire does on his KilledByDev [Twitch](https://www.twitch.tv/killedbydev) 
and [Youtube](https://www.youtube.com/@killedbydev) channels.

The old Unity version is here: [2D-4X Unity](https://github.com/lordhaywire/2D-4X-Unity)

### Adding an in game county resource

These are notes for myself.

1. Create a county resource type enum.

2. Create a county resource in the County Resource folder.

3. Assign enums on the county resource, and fix all other county resource enums.

4. Add the county resource to the AllCountyResources array, and alphabetize them.

### How to Export from Godot

In Godot Project>Export make sure Embed PCK is checked and the Export path goes somewhere that is not in the project
folder.

Copy over the map folder from the project to the folder you are Exporting the game to.

Click Export Project.

### Thanks to the following people:

[Wind of Flatus](https://flatus.itch.io/) also has a [Grand Strategy Map System](https://github.com/HooniusDev/gs-map-system).

[Glitched Code](https://www.youtube.com/@GlitchedCode)

Stevens R Miller

thatguykeedo

fooblaz

#### Other People's Videos

[Good Solution Interactive](https://www.youtube.com/watch?v=UtbU2fa4fMM)