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



# Unsorted
## Pitch Black
### Complete
* Give Speedster slightly increased passive movement speed as the charges build up
* Rework Dropkick and lizard dunk
* Rework Railgunner's recoil

### Incomplete
* Allow Gilded to craft while levitating [maybe, debating on this one]
* Increase Gilded's crafting speed as the karma cap increases
* Add tutorial/info buttons next to each build in the remix settings for noobsbeginners to have a decent idea how to do Escort-ing
* The Yippee button
    * Hear the dev saying Yip- or Yippee whenever you press a button, unlocked by a secret code revealed only on April 1st EST
* Rework shank sound so it has less reverb
* Hybrid builds! Theoretically expands the playable builds from 7 to 42 (and eventually 10 to 90)! Take two builds, and mash them together, with unique combo results!
* New Build: Barbarian Escort
    * Use creatures as shields or battering rams!
        * Hold a live creature, keep it passified as you use them as a shield!
        * Do a slide with a creature in hand to do a battering ram!
        * Press down while falling to do a massive stomp!
    * Fat. Very, VERY heavy! Rotund world here thy comes!
    * Bonus health!
* New Build: Power Escort
    * Each arm has a power bar, which allows Power Escort to use telekinesis, shockwave, and lasers!
        * Power recharges slowly, but gains bonus power on consumption of food
        * Can pull the closest creature in front of Power Escort towards them (if not stopped will collide, resulting in a weak Speedster's crash-like effect)
        * Can then blast the creature with a powerful cone-area blast
        * When empty-handed, pressing throw will charge up a short range laser blast!
    * Can use powers past the limit, but has an increasing percentage to backfire and break the power meter, which means on next cycle the power starts depleted
* New Secret Unlockable Build: Unstable Escort
    * Pray to RNGesus for the optimal outcome, as Unstable Escort speeds through the unforgiving world!
    * Faster base speed compared to Speedster (though speedster's speed ability will always outpace Unstable
    * Has a 0.5%~5% (based on hype) chance every second to trip (and a further 20%~50% of letting go of items)
    * Throwing a weapon has a 20%~40% chance of it being melee (like Brawler), 75%~40% chance of regular throw, and 5%~20% chance of tossing the object
    * Jumping is replaced with a blink, which is similar to Escapist's dash, except half the distance and has modified velocity
        * first two blinks are always possible
        * every blink after that has a 0.5^(blinks done so far - 1) chance of being available.
        * there is a 20 frame window between each blink that the player is able to press jump to do a blink
        * Pressing just jump (or the first jump) will always make Unstable blink upwards unless in zero G
        * after the first jump (or in zero g), Unstable will blink towards the direction the player is holding
        * each blink will propel the slugcat towards that direction, with each subsequent blink in the same direction amplifying the velocity (until reaching the speed that will cause the slugcat to get stunned upon coming into contact with a wall). Changing direction resets the velocity
        * Each blink adds 2 (3 if malnourished) seconds to the cooldown, and upon ending the blink sequence, Unstable will go into cooldown where they will be exhausted
    * While exhausted, Unstable cannot blink and has a further 50% chance on a successful throw of just tossing the object instead.
    * Unlocking this build will require the completion of Escort's campaign
* A secret code that says "I love you!" randomly thoughout a playthrough
* Allow Escort (default only) to meet Spearmaster

### Dead
* Unique sprites for each build
    * Brawler: Painted Violet. May or may not get a monocle
    * Deflector: Painted purple, gets glow-in-the-dark leg bracelets that cause a light trace (like Tron or something)
    * Escapist: Painted sea green. Has different eyes (markings merged)
    * Railgunner: Painted cyan. Does away with the chest X, gains a (battery) meter on either side of their neck
    * Speedster: Painted turquoise. Add an idle animation where they consume cheese
    * Gilded: Painted dark blue. Add a flickering red karma 10 sprite behind the scug when they reach karma cap 9 (lore stuff)
* Rework the pup so they actually behave correctly
* Rework the pup (code) so it's more stable
