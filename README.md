# About
**The Escort.**
Y'all are free to adapt the code for your own usage as long as the original authors (who developed the Slugbase and Rain World and the DLC) are fine with it.

# Version History (Newest to Oldest):
## Version 0: Escort's Beginning
Development starts here. Goal? To have a complete enough slugcat that can be used in Arena and other campaigns.

### 0.2 - The Escort
**(0.2)** Here marks the spot where I begin working on the appearance and art of Escort. For now there is no compatibility with DMS so your results may vary when using such mod.

### 0.1 - Steam Workshopped
**(0.1.10.1)** Sprites?! 

**(0.1.10)** New Escort portraits!

**(0.1.9.18)** Fixed spear throw (had the malnourish stat inverted)

**(0.1.9.17)** Sorta cleaned up Escort_Violence()... will return to this later.

**(0.1.9.16)** Sorta cleaned up Escort_Collision()... will return to this again later.

**(0.1.9.15)** Sorta cleaned up Escort_ThrownSpear()... will return to this again later.

**(0.1.9.14)** Cleaned up Escort_Die()

**(0.1.9.13)** Cleaned up Escort_Grabability()

**(0.1.9.12)** Cleaned up Escort_TossObject()

**(0.1.9.11)** Cleaned up Escort_StickySpear()

**(0.1.9.10)** Cleaned up Escort_UpdateAnimation()

**(0.1.9.9)** Cleaned up Escort_UpdateBodyMode()

**(0.1.9.8)** Cleaned up Escort_HeavyCarry()

**(0.1.9.7)** Cleaned up Escort_CheckInput()

**(0.1.9.6)** Cleaned up Escort_MovementUpdate() and fixed odd visual glitch in Escort_Update()

**(0.1.9.5)** Cleaned up Escort_WallJump()

**(0.1.9.4)** Cleaned up Escort_Jump()

**(0.1.9.3)** Undid some changes in Escort_AerobicIncrease() that messed up the aerobic gain

**(0.1.9.2)** Cleaned up Escort_Update()

**(0.1.9.1)** Cleaned up Escort_AerobicIncrease()

**(0.1.9)** Revivify patches!... sorta kinda. Also fixed potential crashes... Also split the tabs and added a new feature called: Builds! And also also, made parryslides a bit more consistent (so stunslides don't make parryslides not work and it works much better on lizards)... and also you can parry jellyfishes properly now.

**(0.1.8.0)** aPrIl FoOlS!

**(0.1.8)** Escort headbutt. This will be the last feature added before preparation of the code cleanup. Oh one last thing, switched trampoline to requiring you to flip instead of just jumping on stuff to prevent accidental activations (and more physics accurate since it'll seem like they're pushing themselves off a creature.) Another change is that you can trampoline on dead lizards now! HUZZAH! (Elevator behaviour is unchanged.)

**(0.1.7.1)** King tusks finally fall out when parried! HUZZAH HUZZAH

**(0.1.7)** Settings?! SETTINGS!!! Also fixed wall jump behaviour, and reimplemented Escort's Elevator the way it was when it was first featured... and also added friendly fire back to arena mode (it'll behave like gourmand, who can slam into others regardless of whether spears hit or not.)... oh and one more thing: Fixed the category name *(how did it even get deleted in the first place?)*

**(0.1.6.5)** Spears go faster when you do whiplashes at the same time! It's a bit precise, but you'll get it. Hyped spear damage has been reduced from 1.75 to 1.5. Also tweaked the parry slide to be more consistent.

**(0.1.6.4)** Fixed some parry behaviours and also added a few more trycatches to prevent potential problems

**(0.1.6.3)** Code cleanup a bit, made it such that Escort deals damage again, parry slide will ignore lizard bite rng now, and adjusted some mechanics that have yet to be fully implemented (you'll see ;))

**(0.1.6.2)** Grabbing lizards are a thing, and "throwing" them initiates a dropkick almost immediately. Also attempted to patch some issues I guess

**(0.1.6.1)** Long wall jumps will be an option and the option for flip wall jump will also be handled with an option (but handled by flip jumps). For now long wall jumps are disabled because they change the behaviour of regular wall jumping a little

**(0.1.6)** Walljump mechanics changed a wee (alot) bit and also has new stuff on top of it! Also finally fixed Garbage Waste map (not that that changes the sheer difficulty of navigating through it)

**(0.1.5.1)** May or may not be a patch for revivify (haven't found a proper solution yet), dropkicking while in hyped mode sends you up!

**(0.1.5)** Made scavs more friendly at the start, gutter water is no more ;) Also began development on all kinds of stuff, such as another funny thing, and a patch for revivify.

**(0.1.4.3)** Tweaked the behaviours of combat and how spears work (also you will no longer accidentally throw yourself off a pole simply by throwing something), turned off audio for now (but know the option of turning it on will soon be added, and chaos be there)

**(0.1.4.2)** Began work on options (not working yet)

**(0.1.4.1)** Adjusted the audio level

**(0.1.4)** Custom sounds! New death sound and new flip sound!

**(0.1.3.2)** Friendly fire is back... properly this time I promise. Friendly fire is only activated when you activate it.

**(0.1.3.1)** Released a patch that fixes Rotund mod! Also nerfed weight and sneak ability to balance out the dual spears.

**(0.1.3)** Now you can dual-wield spears!... and also lizards :P

**(0.1.2.3)** More code organization *(you should see little gameplay impact... if at all)*, made pole walk faster, and changed the behaviours of spear throws *(Hyped allows you to follow-up a spear throw with a stun-slide in certain conditions! Or if you miss the stun slide, a long slide!)*

**(0.1.2.2)** Code organization (a little), fixed softlocks caused by Parry Slide, and a bit more debug output.

**(0.1.2.1)** Took away the option to friendly fire to players straight up (while I fix the option anyways).

**(0.1.2)** Remember when I said I was going to do Headbutt or better wall jump? I lied. Have a specialized spear throwing mechanic. Also buffed Escort's polewalking speed

**(0.1.1.4)** Reduced the Hyped dropkick damage multiplier from 2x to 1.6x and reduced the base damage to 0.7 (hyped: 1.12). Also increased the opacity of the Battle-Hyped visual indicator

**(0.1.1.3)** An attempt was made at completing Parry Slide, but the current implementation doesn't work fully... so here's a small bump of an update that may or may not have improved the parry that's already existing. (Next update will be Headbutt or better wall jump)

**(0.1.1.2)** Raised the regular dropkick damage to 0.75 and the hyped damage to 1.6, and added Batflies and slugpups (if friendly fire is off) to *"can't Stun Slide/Drop Kick"*

**(0.1.1.1)** Disabled Garbage Worm auto aggression which may have caused some soft locks...*(it didn't work anyways)*

**(0.1.1)** Implemented Escort's Battle-Hyped system, when they're tired to a certain degree, they become powerful! Maintaining the tiredness requires Escort to constantly be engaging in battles as the tired meter builds up much slower on Escort compared to everyone else. Also made the Escort's campaign's rain timer longer to help you get out of the gu'uh whilst I've yet to implement better wall jump. Also organized a bit of code and began development of the latter half of Parry Slide *(and tweaked the parry behaviour to improve its effectiveness... more on this once the full Parry Slide is implemented.)*.

**(0.1.0.3)** Replaced the placeholder art *(by the same person thanks Walkfu!... though I did a wee bit of editing to it)*, added a dumb placeholder menu scene, and modified description for steam page and remix menu such that they're separate and properl.

**(0.1.0.2)** Tweaked lizard aggression stat behaviour, got started with the remix menu interface, added an asset of Escort's death sound *(all I have to do now is find a way to actually import and use it)*, modified the readme and description a bit, and added a non-functioning code that is supposed to make garbage worms angy at Escort *(The Escort probably said something mean to them)*

**(0.1.0.1)** Updated name to be a bit more suitable and a bit more code organizing.

**(0.1)** Uploaded to steam workshop to see how others react to the slugcat.

### 0.0 - Pre-release
**(0.0.13.1)** Added a changable value to trampoline to tune on the fly.

**(0.0.13)** Gave Escort faster movement on pole walking or hanging, added a stop condition for the infiniroll *(so it behaves more like how regular rolls work, as long as you're holding the right buttons it keeps rollin')*, and attempted to patch out Escort's Elevator

**(0.0.12.3)** Changed parry sound so it actually gives feedback properly

**(0.0.12.2)** Fixed friendly fire shenanigans, nerfed stun and damage of Stun Slide, buffed damage of drop kick, and acknowledge Escort's Elevator *(ramping off of creature may cause the Escort to launch upwards)*

**(0.0.12.1)** Changed the translation to a flip since a rocket jump from a charge pounce is a bit too overpowered

**(0.0.12)** Translated charge pounce to rocket jump

**(0.0.11.3)** Added some kind of roadmap *(it's not much)*

**(0.0.11.2)** Added text to README and finally updated the github repository

**(0.0.11.1)** Fixed Parry Slide to a certain degree. It will sometimes parry lizard bites, untested for noodlefly mother spearing, and does not work against thrown spears.

**(0.0.11)** Implemented Parry Slide... which also doesn't work

**(0.0.10.1)** Added the spear grab effect to Stun Slide

**(0.0.10)** Added Better Slide... which is meant to make slide pounces easier, however it does not work...

**(0.0.9.4)** Removed the animations to reduce the chance of triggering Drop Kick from a Stun Slide or repeating Drop Kick infinitely

**(0.0.9.3)** Removed the static bounceback after finding a more reliable trajectory setting

**(0.0.9.2)** Changed the animation that plays at the end of Stun Slide and Drop Kick

**(0.0.9.1)** Added static bounceback since the original solution's trajectory was inconsistent

**(0.0.9)** Adjusted trajectory of Drop Kick to make it more consistent

**(0.0.8)** Attempted to implement exhausion such that Escort doesn't get tired as fast... not working as of now

**(0.0.7)** Implemented Infiniroll

**(0.0.6.1)** Converted Super Stun Pounce to Drop Kick *(and removed knockback on Stun Slide)*

**(0.0.6)** Converted Slam Slide to Stun Slide by giving it stun, and reduced stun on Super Stun Pounce and gave it a lot more knockback

**(0.0.5)** Implemented CarryHeavy, allowing Escort to lug around things that are heavier than them

**(0.0.4.2)** Added spear grab to Super Stun Pounce *(alike the behaviour of whiplash)*

**(0.0.4.1)** Adjusted Slam Slide to launch Escort forward instead of bouncing back, giving them a unique movement tech that allows them to cover more horizontal distance

**(0.0.4)** Implemented BodySlam *(Slam Slide and Super Stun Pounce)*, allowing Escort to slide into creatures to cause knockback. 

**(0.0.3)** Implemented BetterCrawl, allowing Escort to crawl at different speeds.

**(0.0.2.2)** Set the default color of Escort to royal blue

**(0.0.2.1)** Adjusted the slug stats some more

**(0.0.2)** Made the mod work and linked the project so I can stop copy and pasting everything

**(0.0.1)** Imported the template, disabled some template features, and adjusted the slug stats

**(0.0)** Cloned template.


# Future Planned Features:
- Kinder scavs (taking spears don't reduce reputation)
- *[CUT CONTENT]* Exhausion/stamina mechanic where being at full exhausion reduces Escort's power
- Settings to adjust Escort's abilities (and have different presets that enable and disable bunch of hidden values for different "builds")
- *[COMPLETE!]* Reduce bite lethality
- *[SEMI-IMPLEMENTED]* Implement parrying spears with slide (make it less lenient)
- Grab thrown objects
- *[CUT CONTENT]* Actually prioritize spears when doing Stun Slide or Drop Kick
- Increase grab radius of spears
- Escort shoot out of pipes
- Escort turns faster in enclosed spaces
- Escort slide throw doesn't interrupt slide
- *[CUT CONTENT]* Escort throw spear while sliding causes ricochet effect (like regular tosses)
- Escort's spear speeds up after arc
- Escort can get out of centipede grasps (but has a steep cooldown)
- Gamr Mode (if I ever finish the damn mod): Double spawns, reduced spear spawns, pipejukes don't work now (lizards can grab you if they pass by you in the pipe), hunted by lizard event occurs always, pursued by a silver ghost lizard (that spawns every 2.5 minutes in the pipe closest to you, despawned by gates, warded off by shelters) that can manuver like a full-speed white (but doesn't have a tongue), but can also pass through walls, cannot be hit, and the sole intent of their movement is to bite you and kill you.
- Taming lizards is much more difficult
- Escort Combo System
- Escort lizard grabbing noise ("Lizard, Get! Lizard, Get!")
- Add more spears to garbage waste


# Roadmap (Subject to change)
## Version 0: Escort's Beginning
### 0.2: Complete art placeholders
### 0.3: Stabilize code?
### 0.4: Give Escort a unique body feature
## Version 1: Escort the Arena Slugcat (Goes on an ADVENTURE)
### 1.1: Modify behaviour in thicc water and oneway water currents
## Version 2: Escort armed and ready
### 2.1: Centipede armor?
### 2.2: Acquire such centipede armor?
## Version 3: Escort and their special Slugpup
## Version 4: Escort's Story
### 4.1: Starting region
### 4.2: Checkpoint 1
### 4.3: Checkpoint 2
### 4.4: Checkpoint 3
### 4.5: Ending 1 (Intended Story)
### 4.6: Ending 2 (Asc)
### 4.7: Ending 3 (Special super SECRET (somewhere in Outskirts))


Use this template on GitHub or [download the code](https://github.com/SlimeCubed/ExampleSlugBaseMod/archive/refs/heads/master.zip), whichever is easiest.

Links:
- [Template Walkthrough](https://slimecubed.github.io/slugbase/articles/template.html) for a guide to this template.
- [SlugBase Docs](https://slimecubed.github.io/slugbase/) for information regarding custom slugcats.
- [Modding Wiki](https://rainworldmodding.miraheze.org/wiki/Downpour_Reference/Mod_Directories) for `modinfo.json` documentation.