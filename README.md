# 2D 4X

## Using Godot 4.4.1.stable.mono C#

This is the repository containing the open source code for the game creation streams that Lord Haywire does on his KilledByDev [Twitch](https://www.twitch.tv/killedbydev) 
and [YouTube](https://www.youtube.com/@killedbydev) channels.

The old Unity version is here: [2D-4X Unity](https://github.com/lordhaywire/2D-4X-Unity)

### These are notes for myself.

#### Adding an Activity

1. Create an enum in the Activities enums.

2. Create an activity resource in the Activity resource folder.

#### Adding a county good

1. Create a CountyGoodType enum.

2. Create a good in the Resources > Goods folder.

3. This will throw off the enums on already created Goods.  Update all enums in all goods.

#### Adding a County Improvement

1. Write this!

#### Construction Good for County Improvement

Goods Construction Cost is a one time fee charged when the county improvement is initially built.

1. Drag over the Good resource.

2. Set the int that is the cost.

3. Click Add Key/Value Pair

#### Output Good for County Improvement

1. Click the Nil dictionary part of OutputGoods dictionary to create a new KeyValuePair.

2. Drag over good to Output Goods dictionary in inspector as the new key.

3. Create a new ProductionData resource in Output Goods dictionary in inspector as the value.

4. Click add KeyValuePair for dictionary.

4. Right click new ProductionData and select Make Unique. (I think this actually is already Unique if you create a new ProductionData like in step 3.)

5. Adjust new ProductionData to the correct amounts. Work cost must be at least 1. This way the average daily amount generated.

This needs to be set up correctly in the inspector so the average daily amount generated is generated.

MaxWorkers * Global Amount of Daily Work / work cost = average daily amount generated.

#### Input Good to County Improvement

Each input good is per worker per day.

1. Drag over the Good resource.

2. Set the int that is the cost.

3. Click Add Key/Value Pair

#### Story Event Categories

1 - Found County Good

2 - Found Faction Good

3 - Found Scavengeable Good (either canned food, or remnants)

4 - Found County Improvement

5 - Found People


#### How to Export from Godot

In Godot Project>Export makes sure Embed PCK is checked, and the Export path goes somewhere that is not in the project
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