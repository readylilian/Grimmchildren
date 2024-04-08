using UnityEngine;
using System.Collections;
using System;

namespace SlugTemplate.Hooks
{
    internal class BossHooks : MonoBehaviour
    {
        static int setupBoss = 0;
        static float counter = 0f;
        static bool count = false;
        //Stop is the length of time it takes for dialog
        static float stop = 18;
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
            if (rainWorldGame.cameras[0].room.roomSettings.name == "CD_CENTRALROOM"
                && count == false
                && bossBehavior == Behavior.Speaking)
            {
                for (int i = 0; i < rainWorldGame.cameras[0].room.physicalObjects.Length; i++)
                {
                    for (int j = 0; j < rainWorldGame.cameras[0].room.physicalObjects[i].Count; j++)
                    {
                        if (rainWorldGame.cameras[0].room.physicalObjects[i][j].ToString().ToLower().Contains("lizard"))
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
                            rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("Pea brained rat following false hope."), 60);
                            rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("These machines would sooner trap you with them then ever tell you the true path to freedom."), 60);
                            rainWorldGame.cameras[0].hud.dialogBox.NewMessage(self.inGameTranslator.Translate("You have doomed us all!"), 60);
                            count = true;
                        }
                    }
                }
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
                        //Damage is set to 2 rn, so I'm going to say you need three shots because this thing is hard to kill with our time delay
                        self.LizardState.health = 6f;
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
