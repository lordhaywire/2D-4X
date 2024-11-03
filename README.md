# 2D 4X

## Using Godot 4.4.dev3.mono C#

This is the repository containing the open source code for the game creation streams that Lord Haywire does on his KilledByDev [Twitch](https://www.twitch.tv/killedbydev) 
and [Youtube](https://www.youtube.com/@killedbydev) channels.

The old Unity version is here: [2D-4X Unity](https://github.com/lordhaywire/2D-4X-Unity)

### Adding a county good

These are notes for myself.

1. Create a county resource type enum.

2. Create a county resource in the County Resource folder.

3. Assign enums on the county resource, and fix all other county resource enums.

4. Add the county resource to the AllCountyResources array, and alphabetize them.

### Output Good to County Improvement

1. Drag over county resource to County Output Goods dictionary in inspector.

2. Drag over GenericProduction resource over to County Output Goods dictionary in inspector.

3. Click add KeyValuePair for dictionary.

4. Right click GenericProduction and select Make Unique.

5. Adjust unique generic production to the correct amounts.  The main thing being work cost so that the

average daily amount generated is correct.

This needs to be setup correctly in the inspector so the average daily amount generated is generated.

MaxWorkers * Global Amount of Daily Work with bonus / work cost = average daily amount generated.

### How to Export from Godot

In Godot Project>Export make sure Embed PCK is checked and the Export path goes somewhere that is not in the project
folder.

Copy over the map folder from the project to the folder you are Exporting the game to.

Click Export Project.

### Thanks to the following people:

Zkonj

[Wind of Flatus](https://flatus.itch.io/) also has a [Grand Strategy Map System](https://github.com/HooniusDev/gs-map-system).

[Glitched Code](https://www.youtube.com/@GlitchedCode)

Stevens R Miller

thatguykeedo

fooblaz

#### Other People's Videos

[Good Solution Interactive](https://www.youtube.com/watch?v=UtbU2fa4fMM)