* *(0.2.X)* Idea backlog...
    * Changed how settings are applied in game (you shouldn't notice any changes)
    * Escort can throw better in water.
    * Swim speed is properly affected by their movement speed.  Dropkick is now affected by player weight (Hey rotund world!)
    * Brawler can no longer dualwield any weapon.
    * Escapist can no longer throw spears? But unique backspear.
    * Double flashbang causes shockwave on impact, double rock knocks back harder, double firecracker throws hard and activates instantly. Lizards grabbing player will no longer cause flight (player lets go of lizard when grabbed by that lizard). Brawler also no longer stuns creatures when grabbing onto them.
    * Railgunner gets double the amount of slowstun
    * Kickflips are done with slide pounces but not on roll pounces
    * VFX for punches are different from shanks
    * Fix midair spear grab
    * Escapist only tosses spears now, but much longer leniency on escaping
    * Socks grapple gives a bit more air time when not attached to anything
    * Socks grapple works in ZeroG properly
    * Socks can have different grapples for different effects (tonguing a shocky thing gives it electric tongue)
    * Make Socks grapple have less... elasticness and not be grabbable by wormgrass
    * Socks has reduced movement bonuses when tired
    * Give Deflector throwskill of 1 to make ricochets possible... and also let the roll throws travel
    * Give speedster more VFX for how charged they are, and fix the VFX for getting the charge when going up or down (to make it look less awkward)
    * Properly implement Grapple Backpack template
    * Parry Upgrade update
    * Remove reverb from shank noise
    * Might remove Brawler shank for zeroG (or at least lessen it)
    * Change how sprites are loaded (have only one spritesheet), create a template, and create multiple versions just to get started. (And also make the head sprite resize dynamically such as shrinking and moving when going left vs right vs standing etc.)
    * Each escort build changes color if custom colors are off, or default jolly colors are on.
    * Normal Escort cutscene: Swims up from the water, grabs two spears, and spears one in the weird jump.
    * Brawler starts at Outskirts [SU-A02], cutscene: Brawler is given a letter (which is a pipe bomb) and told to go visit Pebbles for a tea party (Hunter campaign)
    * Deflector starts at Sky Islands [SI-C03], cutscene: Gamer voice is heard, then Deflector falls from the sky, then the point HUD appears, then Deflector is told to KILL THEM ALL in big letters (Enot campaign)
    * Escapist starts at Subterranean (filtration system) [], cutscene: Broadcast is put where they are, told to go to moon for a briefing (Monk campaign)
    * Railgunner starts at Garbage Wastes [GW-C02_PAST], cutscene: Waits, aims and shoots a lizard (Artificer campaign)
    * Speedster starts at Farms Array [LF-E03], cutscene: A slideshow of different kinds of cheese, then runs into a scavenger, causing the cheese to flicker away for a moment, before getting chased down by scavs (rivulet campaign)
    * Gilded starts at Flatlands and goes towards the Gutters/Drainage
    * Add/remove sound effect to tell if Escort is dropkicking a live creature or a dead one
    * Add Railgunner knockback for other double-ups (maybe by having the knockback get introduced in frame 2 or 3)
    * Reduce stunslide uncontrollability by changeing it to a roll and balance it. Also have a new way to trigger the combo
    * Deflector's damage increases by 0.001f for every kill in the whole campaign.
    * Wallpounces don't work with dk animation
    * Add spiking
    * *dev23* Default can spike and punt and also get rebound
    * Redo dropkick and slidestun code as a method
    * Convert lizard dunking into a separate thing instead of relying on dropkick to do the thing
    * When brawler picks up the inspector, they can use it to traverse zero g (heavy metal theme playes if silly sfx is on)
    * For hydrox cat, cursor color is body color and cursor outline is eyes, if there's more than one of the same color give or take 5%, set a random color (recursion, have a separate array with whether the color is set or unset and recurse until all colors are set)
    * *dev08* added foodpips for default escort (later), 
    * *dev* Let Escort jump poles quirkly
    * *dev* Add tutorial to new area
    * *dev* Escort Ending 1
    * *dev* Escort stats screen where it shows the number of ascended creatures
    * *dev* Add in all the tutorials


* *(0.2.13)* Redo Escapist (so it ain't for some "Tories" (what does this even mean, Balagaga?!))

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
