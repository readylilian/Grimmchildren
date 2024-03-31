using System;
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

    private static void RoomSpecificScript_AddRoomSpecificScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);

        if (room.abstractRoom.name == "CD_PUZZLEROOM1")
        {
            room.AddObject(new PuzzleRoomEnergyCell(room));
        }
    }


    public sealed class PuzzleRoomEnergyCell : UpdatableAndDeletable
	{
		public PuzzleRoomEnergyCell(Room room)
		{
			this.room = room;
            primed = false;
            IntVector2 pos = new IntVector2(225, 255); // Goal
            
            // Block for if the orb is already at the goal of the level
            if (this.room.game.session is StoryGameSession /*&& TODO: Save Data check*/)
            {
	            // Spawn Cell at goal
	            AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(this.room.world,
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
			Vector2 vector = new Vector2(998.6288f, 1299.568f);
			
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
					MoreSlugcatsEnums.AbstractObjectType.EnergyCell, null, room.GetWorldCoordinate(vector),
					room.game.GetNewID())
				{
					destroyOnAbstraction = true
				};
				room.abstractRoom.AddEntity(abstractPhysicalObject);
				abstractPhysicalObject.RealizeInRoom();
				myEnergyCell = (abstractPhysicalObject.realizedObject as EnergyCell);
				myEnergyCell!.firstChunk.pos = vector;
			}
		}

		private EnergyCell myEnergyCell;

		public Player player;
		
		// Token: 0x04004083 RID: 16515
		private bool primed;

		// Token: 0x04004084 RID: 16516
		private EnergyCell foundCell;

		// Token: 0x04004085 RID: 16517
		private bool finalPhase;

		// Token: 0x04004086 RID: 16518
		private bool lethalMode;

		// Token: 0x04004087 RID: 16519
		private int noCellPresenceTime;
	}
}