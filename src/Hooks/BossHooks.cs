using RWCustom;
using System.Linq;
using UnityEngine;

namespace SlugTemplate.Hooks
{
    internal class BossHooks : MonoBehaviour
    {
        static int setupBoss = 0;
        static float counter = 0f;
        static bool count = false;
        //Stop is the length of time it takes for dialog
        static float stop = 18;

        static bool bossHere = false;
        static bool bossOver = false;
        static AbstractCreature boss;
        enum Behavior
        {
            Frozen,
            Speaking,
            Normal
        }

        private static Behavior bossBehavior;
        public static void Init()
        {
            On.Lizard.Update += BossUpdate;
            On.RainWorld.Update += RainWorldOnUpdate;
            bossBehavior = Behavior.Speaking;
        }

        private static void RainWorldOnUpdate(On.RainWorld.orig_Update orig, RainWorld self)
        {
            orig.Invoke(self);

            if (!(self.processManager.currentMainLoop is RainWorldGame rainWorldGame))
            {
                return;
            }
            if (bossOver)
            {
                var shortcut = rainWorldGame.cameras[0].room.shortcutsIndex[0];
                rainWorldGame.cameras[0].room.lockedShortcuts.Remove(shortcut);
            }
            //If no boss spawn boss
            if (bossHere == false && rainWorldGame.cameras[0].room.roomSettings.name == "CD_CENTRALROOM")
            {
                SpawnBoss(new Vector2(384.9567f, 1170.204f), self, rainWorldGame);
                bossHere = true;
            }
            if (rainWorldGame.cameras[0].room.roomSettings.name == "CD_CENTRALROOM")
            {
                bool foundBoss = false;
                for (int i = 0; i < rainWorldGame.cameras[0].room.physicalObjects.Length; i++)
                {
                    for (int j = 0; j < rainWorldGame.cameras[0].room.physicalObjects[i].Count; j++)
                    {
                        //is the boss here?
                        if (rainWorldGame.cameras[0].room.physicalObjects[i][j].ToString().ToLower().Contains("boss"))
                        {
                            foundBoss = true;
                            //If the boss dies then it's over, don't spawn or talk
                            bossOver = (rainWorldGame.cameras[0].room.abstractRoom.creatures.Find(x => x.Equals(boss))).state.dead;
                            if (bossBehavior == Behavior.Speaking && !bossOver && count == false)
                            {
                                if (rainWorldGame.cameras[0].hud.dialogBox == null)
                                {
                                    rainWorldGame.cameras[0].hud.InitDialogBox();
                                }
                                //Here is where you put the messages
                                rainWorldGame.cameras[0].hud.dialogBox.Interrupt(self.inGameTranslator.Translate("Stupid foolish child!"), 30);
                                float tempShake = rainWorldGame.cameras[0].microShake;
                                rainWorldGame.cameras[0].microShake = 1f;
                                rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("What have you done!"), 30);
                                rainWorldGame.cameras[0].microShake = tempShake;
                                rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("You have melted my salvation!"), 30);
                                rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("Pea brained rat following false hope."), 60);
                                rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("These machines would sooner trap you with them then ever tell you the true path to freedom."), 60);
                                rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("You have doomed us all!"), 60);
                                count = true;

                                foreach (var shortcut in rainWorldGame.cameras[0].room.shortcutsIndex)
                                {
                                    if (!rainWorldGame.cameras[0].room.lockedShortcuts.Contains(shortcut))
                                    {
                                        rainWorldGame.cameras[0].room.lockedShortcuts.Add(shortcut);
                                    }
                                }
                            }
                        }
                    }
                }
                System.Collections.Generic.List<UpdatableAndDeletable> list = rainWorldGame.cameras[0].room.updateList;
                var matching = list.Where(i => i.ToString().ToLower().Contains("ice"));
                if (matching.Any() && bossBehavior == Behavior.Normal)
                {
                    foreach (var item in matching)
                    {
                        item.Destroy();
                    }
                }
                //if the boss ever despawns, respawn it in the same area
                if (foundBoss == false && bossOver == false)
                {
                    Debug.Log("respawn boss");
                    bossHere = false;
                }

                Debug.Log("Found boss? " + foundBoss);
                Debug.Log("Boss over? " + bossOver);
                Debug.Log("Boss here?" + bossHere);
            }
            if (count)
            {
                //I know we don't like timers but nothing else is working due to differences
                //in being static or not
                counter += Time.deltaTime;
                //Once the timer is up start the boss fight
                if (counter >= stop)
                {
                    bossBehavior = Behavior.Normal;
                }
            }

        }
        private static void SpawnBoss(Vector2 bossLocation, RainWorld self, RainWorldGame game)
        {
            CreatureTemplate.Type bossType = (CreatureTemplate.Type)ExtEnumBase.Parse(typeof(CreatureTemplate.Type), ExtEnum<CreatureTemplate.Type>.values.entries.Find(x => x.Equals("Boss")), ignoreCase: false);
            IntVector2 tilePosition = game.cameras[0].room.GetTilePosition(bossLocation);
            WorldCoordinate worldCoordinate = game.cameras[0].room.GetWorldCoordinate(tilePosition);
            EntityID newID = game.GetNewID();
            boss = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(bossType), null, worldCoordinate, newID);
            game.cameras[0].room.abstractRoom.AddEntity(boss);
            boss.RealizeInRoom();
            Debug.Log("Spawning boss");

            //WHEN RESPAWNING IF WE'RE FIGHTING LOCK THE DOORS
            if (bossBehavior == Behavior.Speaking || bossBehavior == Behavior.Normal)
            {
                foreach (var shortcut in game.cameras[0].room.shortcutsIndex)
                {
                    if (!game.cameras[0].room.lockedShortcuts.Contains(shortcut))
                    {
                        game.cameras[0].room.lockedShortcuts.Add(shortcut);
                    }
                }
            }
        }
        private static void BossUpdate(On.Lizard.orig_Update orig, Lizard self, bool eu)
        {
            if (self.ToString().Contains("Pink Lizard") || self.ToString().Contains("Boss"))
            {
                //Only behave normally if we're in a fight
                if (setupBoss == 0)
                {
                    Debug.Log("Setup boss");
                    //Can't be friends with it
                    self.lizardParams.tamingDifficulty = 10000;
                    //Boss can't be killed before the fight
                    self.LizardState.health = 1000000000000000000f;
                    self.slatedForDeletetion = false;
                    self.abstractCreature.personality.aggression = 100;
                    self.AI.LizardPlayerRelationChange(-100, self.room.PlayersInRoom[0].abstractCreature);
                    setupBoss += 1;
                }
                if (bossBehavior == Behavior.Normal)
                {
                    //Set the health to something maybe killable
                    if (setupBoss == 1)
                    {
                        setupBoss += 1;
                        //Damage is set to 2 rn, so I'm going to say you need two shots because this thing is hard to kill with our time delay
                        self.LizardState.health = 4f;
                    }
                    orig(self, eu);
                }
            }
            else
            {
                orig(self, eu);
            }
        }
    }
}
