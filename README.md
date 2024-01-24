# Grimmchildren

# This is to help you guys set up a symlink between the game and mod so you aren't constantly copying folders over every time you Build. I'm making the assumption you use a windows computer. I have no clue how well this will work on other systems

  

# First off, on the steam workshop (rainworlds specifically), find and subscribe to Slugbase. I have it set up as a mod dependency so it's needed to run.

# Find the path to your steam install by right clicking on the game in your library, then clicking "Manage" -> "Browse Local Files". Once there in the file explorer open "Rainworld_Data" -> "StreamingAssets" -> "mods". Keep track of this file path.

# Open PowerShell (or another command line) on your computer in administrative mode

# First, run this command using the file path pointing to the location of the "mods" folder. (be sure to replace the example path with your own one)

cd 'C:\Example\Path\Here\Rain World\RainWorld_Data\StreamingAssets\mods'

  

# Create the symlink; paste following command. Name will likely change as we decide the mods name. The final part of this command targets the mod folder in your github. (be sure to replace the example path with your own one) If you don't use github desktop, this will likely be buried much deeper in your system and you'll need to find it on your own.

New-Item -ItemType SymbolicLink -Path . -Name 'GrimmChildren' -Value 'C:\Example\Path\Here\Documents\GitHub\Grimmchildren\mod'

  

# Open the mod sln (in whatever editor you use) and click build. This will build the mod dll so that it can be used.

  

# Now open the game. Go to the remix menu and you should be able to see the mod. Enable it and click apply. This will likely require the game to restart. Everything should be working after that.
