# RimworldFieldMedic
This mod of Rimworld adds the functionality to temporarily patch up bleeding pawns in the field.
The patch expires after a while, so you still need to give the pawn more formal heals, but this buys you time.

## Credits
Most code are ported from [NoImageAvailable/CombatExtended](https://github.com/NoImageAvailable/CombatExtended), so great thanks to NoImageAvailable for releasing his code under CC-BY-NC-SA-4.0.

I used Chicken Pluckster's [Outfit Creation Kit](https://steamcommunity.com/sharedfiles/filedetails/?id=1114369188) to create the Medic Bag sprite. Thank you CP, the templates are super useful.
 
## How it works?
* New apparel: Medic Bag.
  * Costs 30 cloth, 5 med, and 2 components to make ...
    * ... on tailor table
      * ... after you've researched Medicine Production.
  * The colonist needs to have 5 crafting and 4 medical skills.
  * Raiders will not spawn with medic bags. Traders may have medic bags in stock.
* A pawn needs to wear it before using it.
  * It occupies waist layer, covering the torso, thus it shouldn't conflict with most apparels.
* The pawn needs to be downed, not in a bed, and is bleeding to be stabilized by a Medic-Bag-wearing pawn.
  * The helper's medical tend quality affects how effective the stabilization is.
* Stabilizing each bleeding part costs 5% of the bag. The bag can cause "Tattered apparel" just like any other apparels.
 
## License
As mentioned above, CC-BY-NC-SA-4.0. Mention both NoImageAvailable and me :)

# 野战医生模组
这个RimWorld的模组加入了为游戏内人物进行临时包扎的功能。因为临时包扎只有有限时的效果，所以玩家仍需尽快将受伤的人物送往病床进行正规的治疗。临时包扎的作用就是为正规的治疗争取时间。

## 鸣谢
本模组大多数的代码是移植自[NoImageAvailable的CombatExtended](https://github.com/NoImageAvailable/CombatExtended)。特此鸣谢NoImageAvailable将其代码以CC-BY-NC-SA-4.0授权公开。

Chicken Pluckster的[Outfit Creation Kit](https://steamcommunity.com/sharedfiles/filedetails/?id=1114369188)对我创作急救包的图像带来了莫大的帮助，特此鸣谢。

## 游戏内机制
* 新增了一件“衣物”: 急救包
  * 急救包可于裁缝台制作。
  * 制作急救包需要30块布，5份常规药品（医药），以及2个零部件。
  * 制作急救包需要先研究*医药生产*
  * 制作者需要有5级手工及4级医疗技能
  * 袭击者不会装备急救包。商队可能会出售急救包
* 殖民者需要穿着急救包才可以为其他人物进行临时包扎。
  * 急救包占用配件格，覆盖躯干，因此可以与所有原版衣物同时装备。
* 临时包扎的对象为倒地的、不在病床上的、流血中的人物
  * 临时包扎的有效程度取决于施救者的医疗水平
* 每包扎一处出血，急救包便会损失5%的HP。如同其他任何衣物，HP低于50%会导致“穿着磨损的衣服”的debuff。

## 授权
如上文所述，本模组以CC-BY-NC-SA-4.0授权公开。
