using System.Collections.Generic;
using System.Linq;
using MoreSlugcats;
using RWCustom;
using SlugBase.SaveData;
using UnityEngine;

namespace SlugTemplate.Hooks;

public static class RoomScripts
{
    public static void Init()
    {
        On.RoomSpecificScript.AddRoomSpecificScript += RoomSpecificScript_AddRoomSpecificScript;
        //On.MoreSlugcats.EnergyCell.Update += EnergyCellOnUpdate;
    }

    private static void EnergyCellOnUpdate(On.MoreSlugcats.EnergyCell.orig_Update orig, EnergyCell self, bool eu)
    {
	    self.WeatherInertia();
	    for (int i = 0; i < self.bodyChunks.Length; i++)
	    {
		    self.bodyChunks[i].Update();
	    }
	    self.abstractPhysicalObject.pos.Tile = self.room.GetTilePosition(self.firstChunk.pos);
	    for (int j = 0; j < self.bodyChunkConnections.Length; j++)
	    {
		    self.bodyChunkConnections[j].Update();
	    }
	    if (self.grabbedBy.Count > 0)
	    {
		    for (int k = self.grabbedBy.Count - 1; k >= 0; k--)
		    {
			    if (self.grabbedBy[k].discontinued || self.grabbedBy[k].grabber.grasps[self.grabbedBy[k].graspUsed] != self.grabbedBy[k])
			    {
				    self.grabbedBy.RemoveAt(k);
			    }
		    }
	    }
	    if (self.room.abstractRoom.index != self.abstractPhysicalObject.pos.room)
	    {
		    self.abstractPhysicalObject.pos.room = self.room.abstractRoom.index;
	    }
	    if (!self.sticksRespawned)
	    {
		    self.RecreateSticksFromAbstract();
		    self.sticksRespawned = true;
	    }


	    self.evenUpdate = eu;
	    
	    
	    if (self.appendages != null)
	    {
		    for (int l = 0; l < self.appendages.Count; l++)
		    {
			    self.appendages[l].Update();
		    }
	    }
	    
	    
			BodyChunk bodyChunk = self.bodyChunks[0];
			float num = (float)bodyChunk.onSlope;
			IntVector2 contactPoint = bodyChunk.ContactPoint;
			bool flag = false;
			//if (self.recharging > 0f)
			//{
			//	self.recharging -= 1f;
			//	if (self.recharging <= 0f)
			//	{
			//		self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Core_Ready, self.firstChunk.pos, 1f, 1f);
			//		for (int i = 0; i < 5; i++)
			//		{
			//			self.room.AddObject(new Spark(self.firstChunk.pos + Custom.RNV() * UnityEngine.Random.value * 5f, Custom.RNV() * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), Color.white, null, 8, 32));
			//		}
			//	}
			//}
			//if (self.usingTime > 0f)
			//{
			//	self.usingTime -= 1f;
			//	self.customAnimation = true;
			//	self.moveToTarget = 0.1f;
			//	if (self.usingTime <= 0f)
			//	{
			//		self.customAnimation = false;
			//		self.moveToTarget = 0f;
			//		if (self.usingTime <= 0f)
			//		{
			//			self.recharging = 400f;
			//		}
			//		else
			//		{
			//			self.recharging = 100f;
			//		}
			//		self.usingTime = 0f;
			//		self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Core_Off, self.firstChunk.pos, 1f, 1f);
			//	}
			//}
			//if (self.chargeTime > 0f)
			//{
			//	self.chargeTime -= 1f;
			//	self.customAnimation = true;
			//	self.moveToTarget = Mathf.Lerp(0.05f, 0.1f, self.chargeTime / self.chargeDuration);
			//	if (self.chargeTime >= self.chargeDuration)
			//	{
			//		self.Use(false);
			//	}
			//	if (self.chargeTime >= self.chargeDuration || self.chargeTime <= 0f)
			//	{
			//		self.customAnimation = false;
			//		self.moveToTarget = 0f;
			//		self.chargeTime = 0f;
			//	}
			//}
			if (self.usingTime <= 0f && self.recharging <= 0f && self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player && (self.grabbedBy[0].grabber as Player).input[0].pckp)
			{
				self.chargeTime += 2f;
			}
			if (self.allowStabilization)
			{
				bool flag2 = self.room.RayTraceTilesForTerrain((int)bodyChunk.pos.x / 20, (int)bodyChunk.pos.y / 20, (int)bodyChunk.pos.x / 20, (int)bodyChunk.pos.y / 20 - 15);
				if (self.grabbedBy.Count == 0 && self.touchedGround && contactPoint != new IntVector2(0, -1) && flag2)
				{
					if (!self.isStabilized)
					{
						self.chargeTime = 10f;
						self.isStabilized = true;
						self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Core_Off, self.firstChunk.pos, 0.5f, 0.5f);
						self.gravity = -self.gravity * 2f;
					}
					bodyChunk.vel = new Vector2(bodyChunk.vel.x * 0.7f, bodyChunk.vel.y * 0.7f);
					self.gravity *= 0.7f;
					self.roll = Mathf.Lerp(self.roll, Mathf.Sign(self.roll) * 0.1f, 0.1f);
					return;
				}
			}
			self.isStabilized = false;
			if (self.grabbedBy.Count > 0)
			{
				self.touchedGround = false;
			}
			if (self.usingTime > 0f && self.grabbedBy.Count > 0 && self.Submersion == 0f)
			{
				self.gravity = 0f;
			}
			else
			{
				self.gravity = 0.9f;
			}
			if (self.grabbedBy.Count == 0 && self.stage < 0)
			{
				flag = true;
			}
			if (num == 0f && flag)
			{
				self.velocity = bodyChunk.vel;
				self.gravity = 0f;
			}
			if (num != 0f && flag)
			{
				self.gravity += 0.1f;
				self.velocity = new Vector2(self.velocity.y * self.bounce * -num + self.velocity.x, Mathf.Abs(self.velocity.x) * self.bounce) * (0.7f + 0.3f * Mathf.Abs(Mathf.Clamp(self.velocity.y, -1f, 0f)));
				self.velocity -= new Vector2(0f, self.gravity);
			}
			if (flag)
			{
				bodyChunk.vel = self.velocity;
			}
			//if (self.stage == 0)
			//{
			//	self.velocity = bodyChunk.pos;
			//	self.stage = 1;
			//	self.FXCounter = 0f;
			//	if (self.lightningMachine == null)
			//	{
			//		self.room.AddObject(self.lightningMachine = new LightningMachine(bodyChunk.pos, new Vector2(-1f, 1000f), new Vector2(1f, 1000f), 0f, false, true, 0.5f, 0.5f, 0.3f));
			//		self.lightningMachine.light = true;
			//		self.lightningMachine.random = true;
			//		self.lightningMachine.lightningParam = 1f;
			//		self.lightningMachine.lightningType = self.Hsl.x;
			//		self.lightningMachine.volume = 0.5f;
			//		self.lightningMachine.impactType = 1;
			//	}
			//}
			//if (self.stage == 1)
			//{
			//	self.lightningMachine.pos = bodyChunk.pos;
			//	self.lightningMachine.chance = 0.05f;
			//	if (self.FXCounter < 10f)
			//	{
			//		self.lightningMachine.chance = 0.3f * (1f - self.FXCounter / 10f);
			//		self.lightningMachine.startPoint = new Vector2(-100f, -500f);
			//		self.lightningMachine.endPoint = new Vector2(100f, -500f);
			//		self.FXCounter += 1.5000001f;
			//	}
			//	float num2 = Mathf.SmoothStep(0f, 1f, self.moveToTarget);
			//	bodyChunk.setPos = new Vector2?(new Vector2(Mathf.SmoothStep(self.velocity.x, self.target.x, num2), Mathf.Lerp(self.velocity.y, self.target.y, num2)));
			//	self.roll += Mathf.Lerp(2f * num2, 0f, num2);
			//	self.moveToTarget += 0.010833333f;
			//	if (self.moveToTarget > 1f)
			//	{
			//		self.stage = 2;
			//		self.moveToTarget = 0f;
			//		self.FXCounter = 0f;
			//		self.room.AddObject(self.halo = new LightSource(bodyChunk.pos, false, self.color, self));
			//		self.halo.affectedByPaletteDarkness = 0f;
			//		self.halo.rad = 0f;
			//		self.halo.flat = true;
			//		self.room.AddObject(new LightningMachine.Impact(bodyChunk.pos, 10f, self.color));
			//	}
			//}
			//if (self.stage == 3)
			//{
			//	self.lightningMachine.chance = Custom.LerpQuadEaseIn(0.1f, 0.9f, self.moveToTarget);
			//	if (self.FXCounter < 1f)
			//	{
			//		self.halo.rad += 1f;
			//		self.lightningMachine.chance = 1f - self.FXCounter / 1f;
			//		self.FXCounter += 6.666667f;
			//	}
			//	bodyChunk.setPos = new Vector2?(self.target);
			//	bodyChunk.vel *= 0f;
			//	self.moveToTarget = Mathf.Clamp(self.moveToTarget + 0.0009666667f, 0f, 0.9999f);
			//	self.roll += self.moveToTarget * 0.8f;
			//	self.halo.rad = self.halo.rad + (1f - self.moveToTarget) * 0.05f + 0.1f * self.moveToTarget;
			//	self.halo.rad = Mathf.Min(self.halo.rad, 300f);
			//	self.halo.alpha = Mathf.Lerp(0.7f, 0.5f, self.halo.Rad / 200f);
			//}
			if (contactPoint == new IntVector2(0, -1))
			{
				self.touchedGround = true;
			}
			if (contactPoint != new IntVector2(0, 0) && flag)
			{
				self.roll = self.velocity.x * 3.1415927f * 0.2f * -Mathf.Sign((float)contactPoint.y);
				return;
			}
			self.roll = Mathf.Lerp(self.roll, 0f, 0.1f);
    }

    // Adding scripts to rooms
    private static void RoomSpecificScript_AddRoomSpecificScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);

        if (room.abstractRoom.name == "CD_PUZZLEROOM1")
        {
	        // Creation of nested class, it starts like 20 lines below this (first coord is goal, second is orb)
	        room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(14, 13), new Vector2(998.6288f, 1299.568f),
		        "Puzzle1"));
        }

        else if (room.abstractRoom.name == "CD_PUZZLEROOM2")
        {
	        room.AddObject(new PuzzleRoomEnergyCell(room, new IntVector2(14, 13), new Vector2(707.2426f, 554.5851f),
		        "Puzzle2"));
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
		private string saveTerm;
		
		public PuzzleRoomEnergyCell(Room room, IntVector2 goal, Vector2 start, string saveSpecifier)
		{
			this.room = room;
            primed = false;
            goalPosition = goal;
            startPosition = start;
            saveTerm = saveSpecifier;
            
            bool orbSpawned = false;
            room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
	            .TryGet<bool>(saveTerm + "OrbSpawned", out orbSpawned);
            
            // Block for if the orb is already been saved
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

			bool orbSpawned = false;
			room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
				.TryGet<bool>(saveTerm + "OrbSpawned", out orbSpawned);

			// This rooms cell hasn't been spawned yet
			if (room.game.session is StoryGameSession session && ! orbSpawned && myEnergyCell == null)
			{
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
					room.game.GetStorySession.saveState.miscWorldSaveData.GetSlugBaseData()
						.Set(saveTerm + "OrbSpawned", true);
				}
				myEnergyCell = (abstractPhysicalObject.realizedObject as EnergyCell);
				myEnergyCell!.allowStabilization = true;
				myEnergyCell!.firstChunk.pos = startPosition;
				myEnergyCell.bodyChunks[0].vel = new Vector2(0, 0);
				//ReloadRooms();
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