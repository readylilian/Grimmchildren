# Grimmchildren

# This is to help you guys set up a symlink between the game and mod so you aren't constantly copying folders over every time you Build. I'm making the assumption you use a windows computer. I have no clue how well this will work on other systems

# First off, on the steam workshop (rainworlds specifically), find and subscribe to Slugbase. I have it set up as a mod dependency so it's needed to run.

# Open PowerShell (or another command line) on your computer in administrative mode
# You'll see multiple capital A's in both commands; that means replace that with your system username. I hope you know what that means. You might not be in onedrive, so if not delete that. If you're steam/game is from your hardrive instead of computer, you'll likely have to figure out the filepath on your own

# First, run this command. It just takes you to your steam folder mod file. 
cd 'C:\Users\AAAAAAAA\OneDrive\Desktop\steamFiles\Rain World\RainWorld_Data\StreamingAssets\mods'

# Create the symlink; paste following command. Name will likely change as we decide the mods name. The final part of this command targets the mod folder in your github. If you don't use github desktop, this will likely be buried much deeper in your system and you'll need to find it on your own.
New-Item -ItemType SymbolicLink -Path . -Name 'GrimmChildren' -Value 'C:\Users\AAAAAAAA\OneDrive\Documents\GitHub\Grimmchildren\mod'

# Open the mod sln (in whatever editor you use) and click build. This will build the mod dll so that it can be used.

# Now open the game. Go to the remix menu and you should be able to see the mod. Enable it and click apply. This will likely require the game to restart. Everything should be working after that. 