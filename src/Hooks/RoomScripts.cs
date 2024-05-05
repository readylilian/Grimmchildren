using System;
using System.Collections.Generic;
using System.Linq;
using MoreSlugcats;
using RWCustom;
using SlugBase.SaveData;
using UnityEngine;

namespace SlugTemplate.Hooks;

public static class RoomScripts
{
	// Only necessary because a first build runs room scripts twice for some reason
	private static bool room1Ran = false;
	private static bool room2Ran = false;
    private static bool room3Ran = false;
    private static bool room4Ran = false;

    public static void Init()
    {
	    On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
        On.RoomSpecificScript.AddRoomSpecificScript += RoomSpecificScript_AddRoomSpecificScript;
    }

    private static void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig,
	    ProcessManager self, ProcessManager.ProcessID ID)
    {
	    // Main goal here is to prevent the room scripts from running twice in the same cycle
	    // This only happens on the first run after making a build, though I don't know how that would carry over on steam
	    // The goal here is to reset checking variables after every rest cycle
	    if ((ID == ProcessManager.ProcessID.Game && self.oldProcess.ID != ProcessManager.ProcessID.MultiplayerMenu) ||
	        ID == ProcessManager.ProcessID.MultiplayerMenu)
	    {
		    room1Ran = false;
		    room2Ran = false;
            room3Ran = false;
			room4Ran = false;
        }

	    orig(self, ID);
    }


    // Meant to output if all puzzles are solved
    public static bool AllOrbsCollected(Player player)
    {
	    bool puzzle1Complete;
	    bool puzzle2Complete;
	    bool puzzle3Complete;
	    bool puzzle4Complete;
	    
	    player.room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
		    .TryGet("Puzzle1" + "Completed", out puzzle1Complete);
	    
	    player.room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
		    .TryGet("Puzzle2" + "Completed", out puzzle2Complete);
	    
	    player.room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
		    .TryGet("Puzzle3" + "Completed", out puzzle3Complete);
	    
	    player.room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
		    .TryGet("Puzzle4" + "Completed", out puzzle4Complete);

	    return puzzle1Complete && puzzle2Complete && puzzle3Complete && puzzle4Complete;
    }

    // Adding scripts to rooms
    private static void RoomSpecificScript_AddRoomSpecificScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);
		


        if (room.abstractRoom.name == "CD_PUZZLEROOM1" && !room1Ran)
        {
	        // Creation of nested class, it starts like 20 lines below this (first coord is goal, second is orb)
	        room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(12, 13), new Vector2(998.6288f, 1299.568f),
		        "Puzzle1"));
	        room1Ran = true;
        }

        else if (room.abstractRoom.name == "CD_PUZZLEROOM2" && !room2Ran)
        {
	        room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(43, 13), new Vector2(217.3387f, 1360.429f),
		        "Puzzle2"));
	        room2Ran = true;
        }

        else if (room.abstractRoom.name == "CD_PUZZLEROOM3" && !room3Ran)
        {
            room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(55, 110), new Vector2(1267.535f, 262.053f),
                "Puzzle3"));
            room3Ran = true;
        }

        else if (room.abstractRoom.name == "CD_PUZZLEROOM4" && !room4Ran)
        {
            room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(26, 11), new Vector2(2150.032f, 2920.698f),
                "Puzzle4"));
            room4Ran = true;
        }
    }


    // This is a single nested class, you would make a copy of this for other rooms if they need new function
    public sealed class PuzzleRoomEnergyCell : UpdatableAndDeletable
	{
		private EnergyCell myEnergyCell;
		public Player player;
		private bool primed;
		private EnergyCell foundCell;
		private bool finalPhase;
		private bool lethalMode;
		private IntVector2 goalPosition;
		private Vector2 startPosition;
		private string saveTerm;
		private float lethalTimer = 0;
		
		public PuzzleRoomEnergyCell(Room room, IntVector2 goal, Vector2 start, string saveSpecifier)
		{
			this.room = room;
            primed = false;
            goalPosition = goal;
            startPosition = start;
            saveTerm = saveSpecifier;
            
            bool puzzleCompleted = false;
            room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
	            .TryGet(saveTerm + "Completed", out puzzleCompleted);
            
            // Block for if the orb is already been saved
            if (this.room.game.session is StoryGameSession && puzzleCompleted)
            {
	            lethalTimer = 20;
	            
	            // Spawn Cell at goal
	            AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(this.room.world,
		            MoreSlugcatsEnums.AbstractObjectType.EnergyCell, null, this.room.GetWorldCoordinate(goal),
		            this.room.game.GetNewID())
	            {
		            destroyOnAbstraction = true
	            };
	            this.room.abstractRoom.AddEntity(abstractPhysicalObject);
            	abstractPhysicalObject.RealizeInRoom();
	            
	            // Turn Cell on
            	foundCell = (abstractPhysicalObject.realizedObject as EnergyCell);
            	foundCell!.firstChunk.pos = this.room.MiddleOfTile(goal);
            	foundCell.customAnimation = true;
            	foundCell.SetLocalGravity(0f);
            	foundCell.canBeHitByWeapons = false;
            	foundCell.FXCounter = 10000f;
            	
	            // Make the cell area lethal
            	lethalMode = true;
            	return;
            }

            // Make sounds play for active orb
            foreach (var ambientSound in this.room.roomSettings.ambientSounds.Where(ambientSound =>
	                     ambientSound.sample is "MSC-FlangePad.ogg" or "SO_SFX-AlphaWaves.ogg"))
            {
	            ambientSound.volume = 0f;
            }
		}
		

		public override void Update(bool eu)
		{
			base.Update(eu);
			
			// Get Player 1
			AbstractCreature firstAlivePlayer = room.game.FirstAlivePlayer;
			if (room.game.Players.Count > 0 && firstAlivePlayer is {realizedCreature: not null})
			{
				player = (firstAlivePlayer.realizedCreature as Player);
			}
			else
			{
				player = null;
			}

			bool orbSpawned = false;
			room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
				.TryGet<bool>(saveTerm + "OrbSpawned", out orbSpawned);
			
			// This rooms cell hasn't been spawned yet
			if (room.game.session is StoryGameSession session && !orbSpawned && myEnergyCell == null)
			{
				AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(room.world,
					MoreSlugcatsEnums.AbstractObjectType.EnergyCell, null, room.GetWorldCoordinate(startPosition),
					room.game.GetNewID());
				abstractPhysicalObject.destroyOnAbstraction = true;
				room.abstractRoom.AddEntity(abstractPhysicalObject);
				abstractPhysicalObject.RealizeInRoom();
				myEnergyCell = (abstractPhysicalObject.realizedObject as EnergyCell);
				myEnergyCell.firstChunk.pos = startPosition;
			}

			if (myEnergyCell != null && player != null && player.room == myEnergyCell.room && !orbSpawned)
			{
				//myEnergyCell.firstChunk.pos = startPosition;
				//myEnergyCell.firstChunk.vel = Vector2.zero;

				if (myEnergyCell.grabbedBy.Count > 0)
				{
					room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
						.Set(saveTerm + "OrbSpawned", true);
					
					room.DestroyObject(myEnergyCell.abstractPhysicalObject.ID);
					
					// Create new energy cell
					AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(room.world,
						MoreSlugcatsEnums.AbstractObjectType.EnergyCell, null, room.GetWorldCoordinate(Vector2.zero),
						room.game.GetNewID()); 
					
					room.abstractRoom.AddEntity(abstractPhysicalObject);
					abstractPhysicalObject.RealizeInRoom();
				
					// Put it in save data
					if (AbstractPhysicalObject.UsesAPersistantTracker(abstractPhysicalObject))
					{
						room.game.GetStorySession.AddNewPersistentTracker(abstractPhysicalObject);
					}
					myEnergyCell = (abstractPhysicalObject.realizedObject as EnergyCell);
					myEnergyCell!.firstChunk.pos = startPosition;
					myEnergyCell.bodyChunks[0].vel = Vector2.zero;
					ReloadRooms();

					// left hand grab
					player.SlugcatGrab(myEnergyCell, 0);

					// Use right hand instead, if you've made it this far then one of your hands should be free
					if (myEnergyCell.grabbedBy.Count == 0)
					{
						player.SlugcatGrab(myEnergyCell, 1);
					} 
				}
			}
			

			// orb proceeds to end of it's turn on animation
			if (foundCell?.stage == 3 && !lethalMode)
			{
				lethalTimer += Time.deltaTime;

				if (lethalTimer >= 16)
				{
					lethalMode = true; // vortex can now kill you
				}
			}
			

			// Cell has already been placed at goal
			if (lethalMode)
			{
				startPosition = room.MiddleOfTile(goalPosition);
				if (foundCell != null)
				{
					foundCell.customAnimation = true;
					foundCell.moveToTarget = 0.9f;
					foundCell.scale = 10f;
					foundCell.firstChunk.pos = startPosition;
					foundCell.firstChunk.vel = Vector2.zero;
				}

				// Kill anything that gets too close
				foreach (var abstractCreature in room.abstractRoom.creatures.Where(abstractCreature =>
					         abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Overseer &&
					         abstractCreature.realizedCreature != null && !abstractCreature.state.dead &&
					         Vector2.Distance(abstractCreature.realizedCreature.DangerPos, startPosition) < 100f))
				{
					room.AddObject(new ShockWave(abstractCreature.realizedCreature.DangerPos, 150f, 0.4f, 30));
					for (int i = 0; i < 30; i++)
					{
						room.AddObject(new Spark(
							abstractCreature.realizedCreature.DangerPos +
							Custom.RNV() * UnityEngine.Random.value * 40f,
							Custom.RNV() * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), Color.white, null, 4,
							18));
					}

					room.AddObject(new ElectricDeath.SparkFlash(abstractCreature.realizedCreature.DangerPos,
						10.5f));
					room.PlaySound(SoundID.Zapper_Zap, 0f, 0.8f, 0.5f);
					abstractCreature.realizedCreature.Die();
					abstractCreature.realizedCreature.Destroy();
				}
				return;
			}
			
			// Cell activation is currently happening
			if (finalPhase)
			{
				if (room.game.cameras[0].room != room)
				{
					Destroy();
				}
				return;
			}
			
			// Cell has not been activated yet
			switch (primed)
			{
				case false:
				{
					bool flag = false;
					
					// Check if a player is in the room
					foreach (var t in room.game.Players.Where(t => t.Room == room.abstractRoom))
					{
						flag = true;
					}
					
					// cell literally hasn't been found
					if (foundCell == null)
					{
						if (!flag)
						{
							return;
						}

						// Get the active cell variable
						using List<UpdatableAndDeletable>.Enumerator enumerator2 = room.updateList.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							UpdatableAndDeletable updatableAndDeletable = enumerator2.Current;
							if (updatableAndDeletable is not EnergyCell cell) continue;
							
							foundCell = cell;
							break;
						}
						
						return;
					}
					
					
					if (Vector2.Distance(new Vector2(goalPosition.x * 18, goalPosition.y * 18),
						    myEnergyCell.bodyChunks[0].pos) < 120)
					{
						foundCell.KeepOff();
						primed = true;
					}

					return;
				}
				
				// Cell is in room
				case true when foundCell.room == room:
				{
					
					
					// Cell was just activated but animations still need to play
					if (!(foundCell.usingTime > 0f)) return;

					// Don't go unless within distance
					if (Vector2.Distance(new Vector2(goalPosition.x * 18, goalPosition.y * 18),
						    myEnergyCell.bodyChunks[0].pos) >= 120)
					{
						return;
					}

					foundCell.KeepOff();
					foundCell.FireUp(room.MiddleOfTile(goalPosition));
					foundCell.AllGraspsLetGoOfThisObject(true);
					
					// Save data variables will track this now instead of room save data
					(room.game.session as StoryGameSession)?.RemovePersistentTracker(foundCell.abstractPhysicalObject);
					foundCell.canBeHitByWeapons = false;
					room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
						.Set(saveTerm + "Completed", true);
					finalPhase = true;
					return;
				}
				
				// Cell isn't even in the room
				case true when foundCell.room != room:
					primed = false;
					foundCell = null;
					break;
			}
		}

		private void ReloadRooms()
		{
			for (int i = room.world.activeRooms.Count - 1; i >= 0; i--)
			{
				if (room.world.activeRooms[i] != room.game.cameras[0].room)
				{
					if (room.game.roomRealizer != null)
					{
						room.game.roomRealizer.KillRoom(room.world.activeRooms[i].abstractRoom);
					}
					else
					{
						room.world.activeRooms[i].abstractRoom.Abstractize();
					}
				}
			}
		}
	}
}