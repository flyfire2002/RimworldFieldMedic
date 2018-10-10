# RimworldFieldMedic
This mod of Rimworld adds the functionality to temporarily patch up bleeding pawns in the field.
The patch expires after a while, so you still need to give the pawn more formal heals, but this buys you time.

## Credits
 Most code are ported from [NoImageAvailable/CombatExtended](https://github.com/NoImageAvailable/CombatExtended), so great thanks to NoImageAvailable for releasing his code under CC-BY-NC-SA-4.0.
 
## How it works?
* New apparel: Medic Bag.
  * Costs 30 cloth, 5 med, and 2 components to make ...
    * ... on tailor table
      * ... after you've researched Medicine Production.
  * The colonist needs to have 5 crafting and 4 medical skills.
  * Raiders will not spawn with medic bags. Traders may have medic bags in stock.
* A pawn needs to wear it before using it.
  * It occupies waist layer, covering the torso and shoulders, thus it shouldn't conflict with most apparels.
* The pawn needs to be downed, not in a bed, and is bleeding to be stabilized by a Medic-Bag-wearing pawn.
  * The helper's medical tend quality affects how effective the stabilization is.
* Stabilizing each bleeding part costs 5% of the bag. The bag can cause "Tattered apparel" just like any other apparels.
 
## License
As mentioned above, CC-BY-NC-SA-4.0. Mention both NoImageAvailable and me :)
