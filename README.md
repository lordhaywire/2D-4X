# 2D 4X

## Using Godot 4.4.1.stable.mono C#

This is the repository containing the open source code for the game creation streams that Lord Haywire does on his KilledByDev [Twitch](https://www.twitch.tv/killedbydev) 
and [Youtube](https://www.youtube.com/@killedbydev) channels.

The old Unity version is here: [2D-4X Unity](https://github.com/lordhaywire/2D-4X-Unity)

### These are notes for myself.

#### Adding an Activity

1. Create an enum in the Activities enums.

2. Create an activity resource in the Activity resource folder.

#### Adding a county good

1. Create a good type enum.

2. Create a good in the County Resource folder.

3. Assign enums on the good, and fix all other good enums.

#### Construction Good for County Improvement

Goods Construction Cost is a one time fee charged when the county improvement is initially started being built.

1. Drag over the Good resource.

2. Set the int that is the cost.

3. Click Add Key/Value Pair

#### Output Good to County Improvement

1. Drag over good to Output Goods dictionary in inspector.

2. Drag over GenericProduction resource over to Output Goods dictionary in inspector.

3. Click add KeyValuePair for dictionary.

4. Right click GenericProduction and select Make Unique.

5. Adjust unique generic production to the correct amounts.  The main thing being work cost so that the

average daily amount generated is correct.

This needs to be setup correctly in the inspector so the average daily amount generated is generated.

MaxWorkers * Global Amount of Daily Work with bonus / work cost = average daily amount generated.

#### Input Good to County Improvement

Each input good is per worker per day.

1. Drag over the Good resource.

2. Set the int that is the cost.

3. Click Add Key/Value Pair

#### How to Export from Godot

In Godot Project>Export make sure Embed PCK is checked and the Export path goes somewhere that is not in the project
folder.

Copy over the map folder from the project to the folder you are Exporting the game to.

Click Export Project.

#### Thanks to the following people:

Zkonj

[Wind of Flatus](https://flatus.itch.io/) also has a [Grand Strategy Map System](https://github.com/HooniusDev/gs-map-system).

[Glitched Code](https://www.youtube.com/@GlitchedCode)

Stevens R Miller

thatguykeedo

fooblaz

##### Other People's Videos

[Good Solution Interactive](https://www.youtube.com/watch?v=UtbU2fa4fMM)