# About
**The Escort.**
Y'all are free to adapt the code for your own usage as long as the original authors (who developed the Slugbase and Rain World and the DLC) are fine with it.

# Version History (Newest to Oldest):
## Version 0: Escort's Beginning
Development starts here. Goal? To have a complete enough slugcat that can be used in Arena and other campaigns.

* ### 0.2 - The Escort
  * **(0.2.11)** HUD update
    * **dev01** Sprites adjusted and created, added overtake in tracker to make other sprites take priority over the hyped sprite (to be implemented properly soon).
    * **dev02** Speedster speed trail now a standalone object and an IDrawable object, trackers update on rwg.update instead, ended up using an ISingleCameraDrawable anyways.
    * **dev03** Exception logger updated
    * **dev04** Made sprite override on tracker side
    * **dev05** Sprite override on HUD side
    * **dev06** Escort sprite full override and swim tracker sprites
    * **dev07** Swim tracker finished
    * **dev08** Deflector crash potential fix outlined
    * **dev09** Huds finished
    * **dev10** Offset to move hud around
    * **dev11** More than 4 patch, and hud location option added
    * **dev12** Many more fixes to get it to working order. Fixes expecially for Brawler's hud. Also implented vertical stacking.
    * **dev13** Deflector fixes
    * **dev14** Speedster will no longer get stunned when sliding into creatures and walls. Wallbonk enhanced to beautiful degrees. Nerfed Speedster's stun duration (Doubled the stun duration). Gilded karma 10 powers are actually karma 10
    * **dev15** Brawler can now throw weapons by sliding and supershank is a little more reliable.
    * **dev16** Deflector's damage multiplier also applies to rocks and lilypucks. Made Deflector a better thrower. Deflector power is only spent when the spear actually hits.
    * **dev17** Arena mode has HUD
    * **dev18** Deflector perma damage fix for arena mode

  * **(0.2.10.1)** Applied the secret code in such a way that you can use the karma 10 stuff in arena. Removed legacy configs. Slight sprite fix for parry. Void sea stops Gilded charge.

  * **(0.2.10)** Gilded Update! Gilded will be able to change a rock into a firebomb and spears into firespears. Slide is half distance. Correctly applied speed reduction when slow movement stun is applied. Fixed damage output of different types of spears. Fixed graphical inconsistencies regarding framerate. Added translation layer and russian translation. Escort can now survive touching acid. Removed an unused option. Fixed secret code going away whenever you restart the game. Speedster can now save their charge when hibernating. Replaced Brawler's hud sprite when silly SFX is on. Updated Deflector's hud sprite. Normal escort can swim better. Deflector's damage permanently increases the more kills they get. Speedster gets stunned a lot less. Increased Speedster's speed benefits. Vertical poletech for Escort.

  * **(0.2.9.3)** Fixed Brawler damage

  * **(0.2.9.2)** Implemented the parry condition sprite, adjusted Brawler alt shank to be faster and have less damage

  * **(0.2.9.1)** Deflector now does 1 million damage with a spear on full empower power. Fixed food meter not properly deducting the correct amount of food.

  * **(0.2.9)** Each Escort has a different food requirement, and has different spawns. Patched out an obsolete code that causes crashes if there's more than 4 players (full support for 8 players will come out when the mod does). Clarified Long Wall Jump and told players to keep it off. Disabled experimental backpack due to crashes. Added legacy controls for tongue thing. Added a new check that will check for any Escorts without needing to change each function whenever there's a new Escort ID. Reenabled the transplanter. Potentially fixed issue where sometimes Escort can just not die in death situations. Gave default Escort poletech by default. Brawler shank adjusted such that you can shank faster if it doesn't land 1s -> 0.5s, Brawler punch buffed by being able to punch faster 1s -> 0.75s. Deflector buffed by increasing empowered duration for higher power stages (up to 8 seconds). Railgunner's base bite lethality rate reduced from 0.85 -> 0.75. Gave option to disable the sick flip sfx separately from the rest of silly sfx.

  * **(0.2.8.12)** Changed the behaviour of stunslide (activated by spears) such that it no longer sends Escort flying into oblivion uncontrollably and increased height in general. Added log options for those who want to see the logs. Fixed Gilded multi-stomp if they can't touch the ground. Railgunner has a 100% chance to die when bitten while malnourished(vulnerable) and will spasm upon getting stunned in any way.

  * **(0.2.8.11)** Buffed Railgunner's base double-up damage, buffed Deflector (removed speed limiter), reduced lag caused by this mod (console log only appears upon setting logging at certain values). Gilded gets crippled.
  
  * **(0.2.8.10)** Headbutt batflies, and have batflies give half a pip instead of just a quarter. Also fixed Gilded spamming stomp

  * **(0.2.8.9)** Gilded fixes (hyped should work correctly now), can't throw spears, stomp buff, and requirement to longpress to activate.

  * **(0.2.8.8)** Fixed Deflector traveling across poles so fast, made alternative VFX for empowered a bit easier to notice. Fixed the changelog fix since it reset configurations for Escort (and potentially would've for every new version.). More Gilded development.

  * **(0.2.8.7)** Fixed version appearing every time you want to edit Escort settings. Deflector mechanics addition, normal Escort lung capacity buff, rest of Escorts lung capacity nerf.

  * **(0.2.8.6)** Fixed options. Fixed the gilded mechanics.

  * **(0.2.8.5)** Fixed options? Fricked up the gilded mechanics

  * **(0.2.8.4)** Fixed Gilded being able to be selected even though it changes almost nothing. Is working on gilded and you can now try it out by a secret code.

  * **(0.2.8.3)** Brawler will throw explosive spears instead of trying to shank, shank now has a cooldown, and shank sound occurs only when successfully shanked. Very slightly reduced range for punch and alt shank. Increased Brawler Escort's rollpounce distance (and throwboosts while doing a dropkick/flip is increased to compensate for the terrible throwboosts). Simplified some nullchecks in a class.

  * **(0.2.8.2)** Nerfed railgunner CD (so that you can actually use the boosted damage and actually have risk of blowing up). Nerfed brawler punch (gave more cooldown), buffed shank, and added alternative shank mode. Buffed Deflector's speed to hunter speed instead of survivor speed. Added version notes (and -what's new- page). Minor code tweaks.

  * **(0.2.8.1)** Fixed the easier mode toggle showing up always (forgot to enable the code that disables them lol).

  * **(0.2.8)** Slightly fancier remix menu, 1.5k reward revealed (and will be implemented), and Jolly UI has build selection! (Will add Arena later)

  * **(0.2.7.3)** Got the jolly pup icon working, added an additional spawn area for Socks to actually make them exist. Buffed Launcher backpack and made them stop causing an aneurism

  * **(0.2.7.2)** Socks movement nerfed, added grapple movement buffs, added new grapple (not gonna reveal yet due to how janky it is), changed speartech so Escort doesn't yeet themselves off the cliff when throwing a spear while standing still, changed slidestun behaviour when caused by longslide (launch on second hit). Added additional effect when rolling onto poles with poletech and added new grapple variety. Buffed Deflector's longslide speed. Also made hype option actually disables the slider

  * **(0.2.7.1)** Increased spear spawn rate in DM Leg, fixed softcrash in arena when new feature is on, fixed lizard punt not working on other builds (other than brawler), decreased the number of kills required to trigger Vengeful Lizards event, disallowed grapple worms from grabbing onto Socks, and code reorganization

  * **(0.2.7)** Custom conversations! And Sock's speed and skills are highly dependent on how tired they are now (more tired = slower). Sock's spear damage also changes depending on tiredness. New feature: Vengeful lizards... After a certain amount of time, they start tracking you. Fixed a bug with Socks' grapple worm despawning other grapples (outside shelter).

  * **(0.2.6.3)** Fixed the grapple worm not respawning and reduced respawn timer

  * **(0.2.6.2)** Fixed one of the insults so it doesn't go off the button, added grapple respawn if detached. Killed the check tracker for now

  * **(0.2.6.1)** Fixed Socks' sleep screen

  * **(0.2.6)** Redid the CWT classes, rewrote my graphics functions to allow custom spriting flexibility (so other builds can have more sprites), secret Socks mode, nerfed the heck out of brawler punch, and fixed punching conditions

  * **(0.2.5.10)** Reduced Brawler's stunslide throw distance, holding a rock and no weapons will do a punch (otherwise it'll throw the rock or weapon), nerfed the shank a little. Escapist escapes player grasps in arena/friendlyfire mode. Dropkicks no longer do a sick flip (will reintroduce with tweaked mechanics).

  * **(0.2.5.9)** Moved things around in code for better organization, fixed chance of crash upon using old DMS, added a little logger (will complete later), Speedster climbs poles faster and goes across poles slower, gets stunned when colliding with walls, increased speedster durations, and increased tolerance of bodyslams. Railgunner's base cooldown and max cooldown increased. Buffed railgunner doubleup rocks.

  * **(0.2.5.8)** Fixed the trail. Now it should work fine even with DMS and shouldn't throw any exceptions

  * **(0.2.5.6)** Deactivated slugpup campaign, slight build adjustments, revamped Speedster, fixed their special effect, moved files around.

  * **(0.2.5.4)** Sleep screen!

  * **(0.2.5.3)** Brawler Escort no longer does a standing dropkick when in shanking mode. Escort's marking on the chest is not displayed on top of everything anymore!... becoming rotund will stretch the sprite ;>. Becoming fat with rotund world will also cause a silly SFX to trigger. Escort can now maybe catch spears midair. Potential fix of Brawler Escort exploding? Idk, I couldn't replicate it.

  * **(0.2.5.2)** Someone requested Escort to have rivulet pole tech. You're welcome.

  * **(0.2.5.1)** Escort campaign's rain timer is no longer super long... and overseer will no longer spawn. Nerfed Escort's polewalk and hang a wee bit. Fixed Escort's spears doing incorrect amount of damage (for the longest time, I haven't noticed... whoops). Escort will no longer thrust forward when throwing a spear while crouched (no more throwing Escort off the edge by complete accident!). Brawler shank overhaul, and the rest of Escort can now pick up dead creatures lighter than them alongside a spear or rock or lizard or any onehandable. Speedster Escort's windup will no longer go down when climbing or falling at a fast enough speed. More sfx for sick flips! Buffed Escapist's spear velocity but nerfed base damage.

  * **(0.2.5)** Speedster. Added a supplimentary sfx for parry to assist with the main parry sound. Adjusted the crawl speed to less jank. Weaker dropkicks also make a different sound to differenciate between a powerkick and a regular kick. Also repatched DMS, Spearthrow up and down in corridors! Brawler can spear walls!

  * **(0.2.4.2)** Escort's pole walk and hang speed adjusted, Escort can swim a bit better in water, Railgunner can pull spears from walls, code reorganization proper, and a bit more developer comments. Railgun does more spear damage when overcharged (but hyped does not affect spear damage). When railgun bombboosts, they're sent more up instead of sideways!

  * **(0.2.4.1)** Lillypuck flies better when doubleupped, bombs work better when doubleupped, Deflector's slide nerfed but at the same time new gimmick option

  * **(0.2.4)** New campaign and thumbnail image! Railgunner can now doubleup bombs and lillypucks, the former which will allow you to throw much further and also go right up into other's faces and blow up... the latter not working correctly but I'll adjust that later. Rolls get faster the longer you're rollin'! Batflies are eaten in one bite and Escort just eats things twice as fast! And also also fixed the boop sound playing when silly sfx are off. Also also tweaked so that using easy dropkick does not cause player to grab creatures. Gave Deflector a more lenient and faster slide. Added standard sfx for headbutting. Added accessibility tab for enabling the option to change some visual effects. And finally, eating a mushroom sets the minimum hype to the requirement level!

  * **(0.2.3.6)** Railgunner: Increased the doubleup capacity by 1, reduced cooldown increase by half, reverted damage buff

  * **(0.2.3.5)** Reduced shanking damage while I work on how to balance it, fixed lizard dropkick, fixed death explosion

  * **(0.2.3.4)** Buffed Escapist so they have more time to escape from grasps. Added a lot more try/catches to make sure errors occur less often. Undid revivify thing whatever it was. Added Brawler's shank! Escort can no longer onehand living creatures. Fixed recursion crash when using brawler build. Buffed Railgunner's doubleup

  * **(0.2.3.3)** Adjusted how malnourished works. Also adjusted how the CD is added for railgunner. Also also railgunner regular spear velocity buff. Also adjusted easykick so it's more lenient... and also adjusted the explosion for more hilarity

  * **(0.2.3.2)** Added vfx for railgunner and escapist's gimmicks, and an additional sfx (boop!) for the headbutt, and added cooldown for the dual throws (as well as a vfx to indicate it's in cooldown)

  * **(0.2.3.1)** Nerfed breathing underwater, increased visibility of markings at lowest hype, fixed railgunner hyped behaviour (throwing the spears the wrong way), tweaked railgunner's speartoss so it correctly launches railgunner up or down when doing up or down spear tosses.

  * **(0.2.3)** Added a new thing to Brawler to make it a bit fresher and added two new builds: Escapist & Railgunner! Also tweaked super wall flip so it's easier to do (now you only have to hold any diagonal directions!) And also also EASY MODE!

  * **(0.2.2.9)** Fixed Escort's speed in water (went too fast), nerfed Deflector and (temporarily) buffed Brawler

  * **(0.2.2.8)** Changed the bruiser build (part where it launches the player on spear throws) to a selectable option.

  * **(0.2.2.7)** DMS Patch! (Will have to repatch later)

  * **(0.2.2.6)** Added a check that prevents player from dying while dropkicking right into a lizard's maw and fixed parry sound triggering twice rapidly

  * **(0.2.2.5)** Fixed infinite flight that was possible by certain movements

  * **(0.2.2.4)** In Escort's story, spearmaster's spears may spawn! Also increased the spear spawn rates for a lot of regions (And calmed down on the exception log when checking for spears to prevent nullables.)

  * **(0.2.2.3)** Added SFX for grabbing lizards (and altered pitch of roll), and fixed check for dropkicking lizards (so when you aren't dunking them you make them fly appropriately)... and you can now hold lizards for a flat 8 seconds (and you can't spam grab)!

  * **(0.2.2.2)** Fixed Escort instance separation.

  * **(0.2.2.1)** Implemented a glow that applies when at full hype! Replaces the original spike graphics. (also secret RGB Marking mode)

  * **(0.2.2)** Cleaned up the hooks (visually) and implemented a new build: Deflector!

  * **(0.2.1.2)** Dress My Slugcat patch... patching is not ready yet but it's ready for the update

  * **(0.2.1.1)** Altered how parry is applied. You can now kill other players by sliding and throwing a grenade in their face!

  * **(0.2.1)** The builds apply for each player separately now!

  * **(0.2)** Here marks the spot where I begin working on the appearance and art of Escort. For now there is no compatibility with DMS so your results may vary when using such mod.

* ### 0.1 - Steam Workshopped
  * **(0.1.10.1)** Sprites?!

  * **(0.1.10)** New Escort portraits!

  * **(0.1.9.18)** Fixed spear throw (had the malnourish stat inverted)

  * **(0.1.9.17)** Sorta cleaned up Escort_Violence()... will return to this later.

  * **(0.1.9.16)** Sorta cleaned up Escort_Collision()... will return to this again later.

  * **(0.1.9.15)** Sorta cleaned up Escort_ThrownSpear()... will return to this again later.

  * **(0.1.9.14)** Cleaned up Escort_Die()

  * **(0.1.9.13)** Cleaned up Escort_Grabability()

  * **(0.1.9.12)** Cleaned up Escort_TossObject()

  * **(0.1.9.11)** Cleaned up Escort_StickySpear()

  * **(0.1.9.10)** Cleaned up Escort_UpdateAnimation()

  * **(0.1.9.9)** Cleaned up Escort_UpdateBodyMode()

  * **(0.1.9.8)** Cleaned up Escort_HeavyCarry()

  * **(0.1.9.7)** Cleaned up Escort_CheckInput()

  * **(0.1.9.6)** Cleaned up Escort_MovementUpdate() and fixed odd visual glitch in Escort_Update()

  * **(0.1.9.5)** Cleaned up Escort_WallJump()

  * **(0.1.9.4)** Cleaned up Escort_Jump()

  * **(0.1.9.3)** Undid some changes in Escort_AerobicIncrease() that messed up the aerobic gain

  * **(0.1.9.2)** Cleaned up Escort_Update()

  * **(0.1.9.1)** Cleaned up Escort_AerobicIncrease()

  * **(0.1.9)** Revivify patches!... sorta kinda. Also fixed potential crashes... Also split the tabs and added a new feature called: Builds! And also also, made parryslides a bit more consistent (so stunslides don't make parryslides not work and it works much better on lizards)... and also you can parry jellyfishes properly now.

  * **(0.1.8.0)** aPrIl FoOlS!

  * **(0.1.8)** Escort headbutt. This will be the last feature added before preparation of the code cleanup. Oh one last thing, switched trampoline to requiring you to flip instead of just jumping on stuff to prevent accidental activations (and more physics accurate since it'll seem like they're pushing themselves off a creature.) Another change is that you can trampoline on dead lizards now! HUZZAH! (Elevator behaviour is unchanged.)

  * **(0.1.7.1)** King tusks finally fall out when parried! HUZZAH HUZZAH

  * **(0.1.7)** Settings?! SETTINGS!!! Also fixed wall jump behaviour, and reimplemented Escort's Elevator the way it was when it was first featured... and also added friendly fire back to arena mode (it'll behave like gourmand, who can slam into others regardless of whether spears hit or not.)... oh and one more thing: Fixed the category name *(how did it even get deleted in the first place?)*

  * **(0.1.6.5)** Spears go faster when you do whiplashes at the same time! It's a bit precise, but you'll get it. Hyped spear damage has been reduced from 1.75 to 1.5. Also tweaked the parry slide to be more consistent.

  * **(0.1.6.4)** Fixed some parry behaviours and also added a few more trycatches to prevent potential problems

  * **(0.1.6.3)** Code cleanup a bit, made it such that Escort deals damage again, parry slide will ignore lizard bite rng now, and adjusted some mechanics that have yet to be fully implemented (you'll see ;))

  * **(0.1.6.2)** Grabbing lizards are a thing, and "throwing" them initiates a dropkick almost immediately. Also attempted to patch some issues I guess

  * **(0.1.6.1)** Long wall jumps will be an option and the option for flip wall jump will also be handled with an option (but handled by flip jumps). For now long wall jumps are disabled because they change the behaviour of regular wall jumping a little

  * **(0.1.6)** Walljump mechanics changed a wee (alot) bit and also has new stuff on top of it! Also finally fixed Garbage Waste map (not that that changes the sheer difficulty of navigating through it)

  * **(0.1.5.1)** May or may not be a patch for revivify (haven't found a proper solution yet), dropkicking while in hyped mode sends you up!

  * **(0.1.5)** Made scavs more friendly at the start, gutter water is no more ;) Also began development on all kinds of stuff, such as another funny thing, and a patch for revivify.

  * **(0.1.4.3)** Tweaked the behaviours of combat and how spears work (also you will no longer accidentally throw yourself off a pole simply by throwing something), turned off audio for now (but know the option of turning it on will soon be added, and chaos be there)

  * **(0.1.4.2)** Began work on options (not working yet)

  * **(0.1.4.1)** Adjusted the audio level

  * **(0.1.4)** Custom sounds! New death sound and new flip sound!

  * **(0.1.3.2)** Friendly fire is back... properly this time I promise. Friendly fire is only activated when you activate it.

  * **(0.1.3.1)** Released a patch that fixes Rotund mod! Also nerfed weight and sneak ability to balance out the dual spears.

  * **(0.1.3)** Now you can dual-wield spears!... and also lizards :P

  * **(0.1.2.3)** More code organization *(you should see little gameplay impact... if at all)*, made pole walk faster, and changed the behaviours of spear throws *(Hyped allows you to follow-up a spear throw with a stun-slide in certain conditions! Or if you miss the stun slide, a long slide!)*

  * **(0.1.2.2)** Code organization (a little), fixed softlocks caused by Parry Slide, and a bit more debug output.

  * **(0.1.2.1)** Took away the option to friendly fire to players straight up (while I fix the option anyways).

  * **(0.1.2)** Remember when I said I was going to do Headbutt or better wall jump? I lied. Have a specialized spear throwing mechanic. Also buffed Escort's polewalking speed

  * **(0.1.1.4)** Reduced the Hyped dropkick damage multiplier from 2x to 1.6x and reduced the base damage to 0.7 (hyped: 1.12). Also increased the opacity of the Battle-Hyped visual indicator

  * **(0.1.1.3)** An attempt was made at completing Parry Slide, but the current implementation doesn't work fully... so here's a small bump of an update that may or may not have improved the parry that's already existing. (Next update will be Headbutt or better wall jump)

  * **(0.1.1.2)** Raised the regular dropkick damage to 0.75 and the hyped damage to 1.6, and added Batflies and slugpups (if friendly fire is off) to *"can't Stun Slide/Drop Kick"*

  * **(0.1.1.1)** Disabled Garbage Worm auto aggression which may have caused some soft locks...*(it didn't work anyways)*

  * **(0.1.1)** Implemented Escort's Battle-Hyped system, when they're tired to a certain degree, they become powerful! Maintaining the tiredness requires Escort to constantly be engaging in battles as the tired meter builds up much slower on Escort compared to everyone else. Also made the Escort's campaign's rain timer longer to help you get out of the gu'uh whilst I've yet to implement better wall jump. Also organized a bit of code and began development of the latter half of Parry Slide *(and tweaked the parry behaviour to improve its effectiveness... more on this once the full Parry Slide is implemented.)*.

  * **(0.1.0.3)** Replaced the placeholder art *(by the same person thanks Walkfu!... though I did a wee bit of editing to it)*, added a dumb placeholder menu scene, and modified description for steam page and remix menu such that they're separate and properl.

  * **(0.1.0.2)** Tweaked lizard aggression stat behaviour, got started with the remix menu interface, added an asset of Escort's death sound *(all I have to do now is find a way to actually import and use it)*, modified the readme and description a bit, and added a non-functioning code that is supposed to make garbage worms angy at Escort *(The Escort probably said something mean to them)*

  * **(0.1.0.1)** Updated name to be a bit more suitable and a bit more code organizing.

  * **(0.1)** Uploaded to steam workshop to see how others react to the slugcat.

* ### 0.0 - Pre-release
  * **(0.0.13.1)** Added a changable value to trampoline to tune on the fly.

  * **(0.0.13)** Gave Escort faster movement on pole walking or hanging, added a stop condition for the infiniroll *(so it behaves more like how regular rolls work, as long as you're holding the right buttons it keeps rollin')*, and attempted to patch out Escort's Elevator

  * **(0.0.12.3)** Changed parry sound so it actually gives feedback properly

  * **(0.0.12.2)** Fixed friendly fire shenanigans, nerfed stun and damage of Stun Slide, buffed damage of drop kick, and acknowledge Escort's Elevator *(ramping off of creature may cause the Escort to launch upwards)*

  * **(0.0.12.1)** Changed the translation to a flip since a rocket jump from a charge pounce is a bit too overpowered

  * **(0.0.12)** Translated charge pounce to rocket jump

  * **(0.0.11.3)** Added some kind of roadmap *(it's not much)*

  * **(0.0.11.2)** Added text to README and finally updated the github repository

  * **(0.0.11.1)** Fixed Parry Slide to a certain degree. It will sometimes parry lizard bites, untested for noodlefly mother spearing, and does not work against thrown spears.

  * **(0.0.11)** Implemented Parry Slide... which also doesn't work

  * **(0.0.10.1)** Added the spear grab effect to Stun Slide

  * **(0.0.10)** Added Better Slide... which is meant to make slide pounces easier, however it does not work...

  * **(0.0.9.4)** Removed the animations to reduce the chance of triggering Drop Kick from a Stun Slide or repeating Drop Kick infinitely

  * **(0.0.9.3)** Removed the static bounceback after finding a more reliable trajectory setting

  * **(0.0.9.2)** Changed the animation that plays at the end of Stun Slide and Drop Kick

  * **(0.0.9.1)** Added static bounceback since the original solution's trajectory was inconsistent

  * **(0.0.9)** Adjusted trajectory of Drop Kick to make it more consistent

  * **(0.0.8)** Attempted to implement exhausion such that Escort doesn't get tired as fast... not working as of now

  * **(0.0.7)** Implemented Infiniroll

  * **(0.0.6.1)** Converted Super Stun Pounce to Drop Kick *(and removed knockback on Stun Slide)*

  * **(0.0.6)** Converted Slam Slide to Stun Slide by giving it stun, and reduced stun on Super Stun Pounce and gave it a lot more knockback

  * **(0.0.5)** Implemented CarryHeavy, allowing Escort to lug around things that are heavier than them

  * **(0.0.4.2)** Added spear grab to Super Stun Pounce *(alike the behaviour of whiplash)*

  * **(0.0.4.1)** Adjusted Slam Slide to launch Escort forward instead of bouncing back, giving them a unique movement tech that allows them to cover more horizontal distance

  * **(0.0.4)** Implemented BodySlam *(Slam Slide and Super Stun Pounce)*, allowing Escort to slide into creatures to cause knockback.

  * **(0.0.3)** Implemented BetterCrawl, allowing Escort to crawl at different speeds.

  * **(0.0.2.2)** Set the default color of Escort to royal blue

  * **(0.0.2.1)** Adjusted the slug stats some more

  * **(0.0.2)** Made the mod work and linked the project so I can stop copy and pasting everything

  * **(0.0.1)** Imported the template, disabled some template features, and adjusted the slug stats

  * **(0.0)** Cloned template.


# Ideas:
- #### Not Implemented
  - Kinder scavs (taking spears don't reduce reputation)
  - Increase grab radius of spears
  - Escort shoot out of pipes
  - Escort turns faster in enclosed spaces
  - Escort's spear speeds up after arc
  - Gamr Mode (if I ever finish the damn mod): Double spawns, reduced spear spawns, pipejukes don't work now (lizards can grab you if they pass by you in the pipe), hunted by lizard event occurs always, pursued by a silver ghost lizard (that spawns every 2.5 minutes in the pipe closest to you, despawned by gates, warded off by shelters) that can manuver like a full-speed white (but doesn't have a tongue), but can also pass through walls, cannot be hit, and the sole intent of their movement is to bite you and kill you.
  - Taming lizards is much more difficult
  - Escort Combo System
  - Lizard head helmet with different effects (created when sleeping with dead lizard in shelter, acts like Moon's cloak)
- #### Implemented
  - Settings to adjust Escort's abilities (and have different presets that enable and disable bunch of hidden values for different "builds")
  - Reduce bite lethality
  - **[Range may need tweaking]** Implement parrying spears with slide (make it less lenient)
  - Grab thrown objects
  - **[Added as optional feature]** Escort slide throw doesn't interrupt slide
  - **[Given to Escapist]** Escort can get out of centipede grasps (but has a steep cooldown)
  - Escort lizard grabbing noise ("Lizard, Get! Lizard, Get!")
  - Add more spears to garbage waste
- #### Cut-content
  - Exhausion/stamina mechanic where being at full exhausion reduces Escort's power
  - Actually prioritize spears when doing Stun Slide or Drop Kick
  - Escort throw spear while sliding causes ricochet effect (like regular tosses) at throw skill 0



# Roadmap (Subject to change)
* Version 0: Escort's Beginning
  * 0.2: Complete art placeholders
  * 0.3: Stabilize code?
  * 0.4: Give Escort a unique body feature
* Version 1: Escort the Arena Slugcat (Goes on an ADVENTURE)
  * 1.1: Modify behaviour in thicc water and oneway water currents
* Version 2: Escort armed and ready
  * 2.1: Centipede armor?
  * 2.2: Acquire such centipede armor?
* Version 3: Escort and their special Slugpup
* Version 4: Escort's Story
  * 4.1: Starting region
  * 4.2: Checkpoint 1
  * 4.3: Checkpoint 2
  * 4.4: Checkpoint 3
  * 4.5: Ending 1 (Intended Story)
  * 4.6: Ending 2 (Asc)
  * 4.7: Ending 3 (Special super SECRET (somewhere in Outskirts))