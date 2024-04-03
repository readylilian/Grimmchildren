using System;
using System.Collections.Generic;
using System.Linq;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace SlugTemplate.Hooks;

public static class RoomScripts
{
    public static void Init()
    {
        On.RoomSpecificScript.AddRoomSpecificScript += RoomSpecificScript_AddRoomSpecificScript;
    }

    // Adding scripts to rooms
    private static void RoomSpecificScript_AddRoomSpecificScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);

        if (room.abstractRoom.name == "CD_PUZZLEROOM1")
        {
	        // Creation of nested class, it starts like 20 lines below this
	        room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(14, 13), new Vector2(998.6288f, 1299.568f)));
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
		private int noCellPresenceTime;
		private IntVector2 goalPosition;
		private Vector2 startPosition;
		
		public PuzzleRoomEnergyCell(Room room, IntVector2 goal, Vector2 start)
		{
			this.room = room;
            primed = false;
            goalPosition = goal;
            startPosition = start;
            
            // Block for if the orb is already at the goal of the level
            if (this.room.game.session is StoryGameSession /*&& TODO: Save Data check*/)
            {
	            // Spawn Cell at goal
	           /* AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(this.room.world,
		            MoreSlugcatsEnums.AbstractObjectType.EnergyCell, null, this.room.GetWorldCoordinate(pos),
		            this.room.game.GetNewID())
	            {
		            destroyOnAbstraction = true
	            };
	            this.room.abstractRoom.AddEntity(abstractPhysicalObject);
            	abstractPhysicalObject.RealizeInRoom();
	            
	            // Turn Cell on
            	foundCell = (abstractPhysicalObject.realizedObject as EnergyCell);
            	foundCell!.firstChunk.pos = this.room.MiddleOfTile(pos);
            	foundCell.customAnimation = true;
            	foundCell.SetLocalGravity(0f);
            	foundCell.canBeHitByWeapons = false;
            	foundCell.FXCounter = 10000f;
            	
	            // Make the room zero-grav and lethal
	            RoomSettings.RoomEffect item = new RoomSettings.RoomEffect(RoomSettings.RoomEffect.Type.ZeroG, 1f, false);
            	this.room.roomSettings.effects.Add(item);
            	lethalMode = true;
            	return;*/
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

			// This rooms cell hasn't been taken yet
			if (room.game.session is StoryGameSession session &&
			    /*TODO: Save data check  &&*/
			    myEnergyCell == null)
			{
				AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(room.world,
					MoreSlugcatsEnums.AbstractObjectType.EnergyCell, null, room.GetWorldCoordinate(startPosition),
					room.game.GetNewID())
				{
					destroyOnAbstraction = true
				};
				room.abstractRoom.AddEntity(abstractPhysicalObject);
				abstractPhysicalObject.RealizeInRoom();
				myEnergyCell = (abstractPhysicalObject.realizedObject as EnergyCell);
				myEnergyCell!.firstChunk.pos = startPosition;
			}
			
			
			// Cell has already been placed at goal
			if (lethalMode)
			{
				startPosition = room.MiddleOfTile(goalPosition);
				if (foundCell != null)
				{
					foundCell.customAnimation = true;
					foundCell.moveToTarget = 0.9f;
					foundCell.scale = 20f;
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
					if (foundCell != null)
					{
						while (foundCell.grabbedBy.Count > 0)
						{
							Creature grabber = foundCell.grabbedBy[0].grabber;
							foundCell.grabbedBy[0].Release();
							grabber.Stun(10);
							grabber.firstChunk.vel += new Vector2(0f, -5f);
						}
					}
					else
					{
						if (!flag)
						{
							return;
						}

						// Get teh active cell variable
						using List<UpdatableAndDeletable>.Enumerator enumerator2 = room.updateList.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							UpdatableAndDeletable updatableAndDeletable = enumerator2.Current;
							if (updatableAndDeletable is not EnergyCell cell) continue;
							
							foundCell = cell;
							break;
						}
						if (foundCell == null)
						{
							noCellPresenceTime++;
							return;
						}
						noCellPresenceTime = 0;
						return;
					}
					foundCell.KeepOff();
					primed = true;
					return;
				}
				
				// Cell is in room
				case true when foundCell.room == room:
				{
					// Cell was just activated but animations still need to play
					if (!(foundCell.usingTime > 0f)) return;
					
					foundCell.KeepOff();
					foundCell.FireUp(room.MiddleOfTile(goalPosition));
					foundCell.AllGraspsLetGoOfThisObject(true);
					
					// Save data variables will track this now instead of room save data
					(room.game.session as StoryGameSession)?.RemovePersistentTracker(foundCell.abstractPhysicalObject);
					foundCell.canBeHitByWeapons = false;
					// TODO: Implement save data
					//room.game.GetStorySession.saveState.miscWorldSaveData.moonHeartRestored = true;
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
	}
}