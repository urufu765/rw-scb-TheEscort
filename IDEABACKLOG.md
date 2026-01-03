# Backlogs
### Done.
* Escapist can no longer throw spears? But unique backspear.
* Socks grapple works in ZeroG properly
* Socks has reduced movement bonuses when tired
* Each escort build changes color if custom colors are off, or default jolly colors are on.
* Add/remove sound effect to tell if Escort is dropkicking a live creature or a dead one
* Add Railgunner knockback for other double-ups (maybe by having the knockback get introduced in frame 2 or 3)
* Deflector's damage increases by 0.001f for every kill in the whole campaign.
* Let Escort jump poles quirkly
* Escort Ending 1
* Redo Escapist (so it ain't for some "Tories" (what does this even mean, Balagaga?!))
### Will do
* Escort can throw better in water.
* Lizards grabbing player will no longer cause flight (player lets go of lizard when grabbed by that lizard). 
* Give speedster more VFX for how charged they are, and fix the VFX for getting the charge when going up or down (to make it look less awkward)
* Remove reverb from shank noise
* Add spiking
* Redo dropkick and slidestun code as a method
* Escort stats screen where it shows the number of ascended creatures
* Separate the build selection stuff into a method with an extended list for a method
* Shatter Saint like a piece of glass when dropkicked in arena mode
* Make Escapist's description and color change when old is active
* Change New Escapist's color so it isn't as similar to Railgunner
* Default changes the color of the shortcut pipes TOO
* Separate food bar reduction to easier mode
* Prelineage creatures along Spearmaster default path (consult Vestige database). Frequently farmed paths have a no creature spawn with a 50% chance of lineage/respawn
* New way options are processed
* Add Chocosocks in honor of fanart by Shizarival
* Translate in the stuff (Speartech) and have placeholders for each different speartech moves
* Have food pips get affected by player number too
### Might do
* Default can spike and punt and also get rebound(?)
* Changed how settings are applied in game (you shouldn't notice any changes)
* Brawler also no longer stuns creatures when grabbing onto them.
* VFX for punches are different from shanks
* Silly buttons {(single press)TB808 cowbell, YIPPEE, Hey, ComeOn!, Yip-(while held)hug, All I Want For Christmas Is You cover} (5k special)
### Thinking about it
* Swim speed is properly affected by their movement speed.  Dropkick is now affected by player weight (Hey rotund world!)
* Kickflips are done with slide pounces but not on roll pounces
* Socks grapple gives a bit more air time when not attached to anything
* Socks can have different grapples for different effects (tonguing a shocky thing gives it electric tongue)
* Make Socks grapple have less... elasticness and not be grabbable by wormgrass
* Give Deflector throwskill of 1 to make ricochets possible... and also let the roll throws travel
* Might remove Brawler shank for zeroG (or at least lessen it)
* Change how sprites are loaded (have only one spritesheet), create a template, and create multiple versions just to get started. (And also make the head sprite resize dynamically such as shrinking and moving when going left vs right vs standing etc.)
* Add in all the tutorials
* Investigate line of sight type shadow, tint visual radius of creatures
* Switch logger from Debug.Log to Logger
### Trash
* Brawler can no longer dualwield any weapon.
* Railgunner gets double the amount of slowstun
* Escapist only tosses spears now, but much longer leniency on escaping
* Normal Escort cutscene: Swims up from the water, grabs two spears, and spears one in the weird jump.
* Reduce stunslide uncontrollability by changeing it to a roll and balance it. Also have a new way to trigger the combo
### Forgot (why I added)/(the purpose of) this
* Fix midair spear grab
* Properly implement Grapple Backpack template
* Parry Upgrade update
* Wallpounces don't work with dk animation
* When brawler picks up the inspector, they can use it to traverse zero g (heavy metal theme playes if silly sfx is on)
* added foodpips for default escort (later), 
* Add tutorial to new area
### Outdated
* Brawler starts at Outskirts [SU-A02], cutscene: Brawler is given a letter (which is a pipe bomb) and told to go visit Pebbles for a tea party (Hunter campaign)
* Deflector starts at Sky Islands [SI-C03], cutscene: Gamer voice is heard, then Deflector falls from the sky, then the point HUD appears, then Deflector is told to KILL THEM ALL in big letters (Enot campaign)
* Escapist starts at Subterranean (filtration system) [], cutscene: Broadcast is put where they are, told to go to moon for a briefing (Monk campaign)
* Railgunner starts at Garbage Wastes [GW-C02_PAST], cutscene: Waits, aims and shoots a lizard (Artificer campaign)
* Speedster starts at Farms Array [LF-E03], cutscene: A slideshow of different kinds of cheese, then runs into a scavenger, causing the cheese to flicker away for a moment, before getting chased down by scavs (rivulet campaign)
* Gilded starts at Flatlands and goes towards the Gutters/Drainage
### Irrelevant/Miscellanioususos
* For hydrox cat, cursor color is body color and cursor outline is eyes, if there's more than one of the same color give or take 5%, set a random color (recursion, have a separate array with whether the color is set or unset and recurse until all colors are set)
* *(0.2.12)* Ability update!
    * Regular Escort can Suplex
    * Bombs do explosive punches (Brawler)
    * Deflects weapons properly (sends them back to the sender) (Deflector)
    * NONE-REWORK SOON(Escapist)
    * Two rocks + one food pip = bomb, two spears + one food pip = fire spear (Railgunner)
    * While in speeding state, sliding under enemy causes them to launch upwards (Speedster)
    * NONE-TOO RECENT(Gilded)
* *(0.2.11)* Fleshed Out Story (Part 2) Update
* *(0.2.10)* Gilded. (another idea: Blast away the creatures for safety)
* *(0.2.8.13)* Added option to revert to previous behaviour for those who prefer the old ways


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
