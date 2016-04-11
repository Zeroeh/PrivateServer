using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.realm;
using ServerEngine.logic.attack;
using ServerEngine.logic.movement;
using ServerEngine.logic.loot;
using ServerEngine.logic.taunt;
using ServerEngine.logic.cond;

namespace ServerEngine.logic
{
	partial class BehaviorDb
	{
		static _ Additions = Behav()
			.Init(0x2000, Behaves("Reaper of Doom",
				new RunBehaviors(
					new State("idle",
						SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)
					),
					new State("begin", new QueuedBehavior(
						new SimpleTaunt("Who has awoken me?!"),
						CooldownExact.Instance(3000, NullBehavior.Instance),
						new SimpleTaunt("Well, time to have some fun!"),
						Timed.Instance(2000, False.Instance(Flashing.Instance(250, 0xffffff00))),
						SetState.Instance("grow")
					)),
					new State("grow", SimpleWandering.Instance(5), new QueuedBehavior(
						CooldownExact.Instance(1000, SetSize.Instance(120)),
						CooldownExact.Instance(1000, SetSize.Instance(140)),
						CooldownExact.Instance(1000, SetSize.Instance(160)),
						CooldownExact.Instance(1000, SetSize.Instance(180)),
						CooldownExact.Instance(1000, SetSize.Instance(200)),
						new SimpleTaunt("Fear my strongest form!"),
						CooldownExact.Instance(1000, SetState.Instance("attack"))
					)),
					new State("attack",
						StateOnce.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
						Chasing.Instance(5, 12, 1, null),
						Cooldown.Instance(500, SimpleAttack.Instance(12, projectileIndex: 1)),
						CooldownExact.Instance(1000, MultiAttack.Instance(8, 10 * (float)Math.PI / 180, 6)),
						HpLesserPercent.Instance((float)0.1, new RunBehaviors(
							SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
							SetState.Instance("dying")))
					),
					new State("dying",
						StateOnce.Instance(new SimpleTaunt("I CANNOT DIE!")),
						new QueuedBehavior(
							CooldownExact.Instance(1500, UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
							CooldownExact.Instance(500, SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))
						),
						Flashing.Instance(250, 0xffff0000),
						CooldownExact.Instance(200, RingAttack.Instance(8))
					)
				),
				loot: new LootBehavior(LootDef.Empty,
						Tuple.Create(100, new LootDef(0, 5, 0, 8,
							Tuple.Create(0.001, (ILoot)new TierLoot(8, ItemType.Ability)),
							Tuple.Create(0.005, (ILoot)new TierLoot(7, ItemType.Ability)),
							Tuple.Create(0.05, (ILoot)new TierLoot(6, ItemType.Ability)),
							Tuple.Create(0.001, (ILoot)new TierLoot(15, ItemType.Armor)),
							Tuple.Create(0.005, (ILoot)new TierLoot(14, ItemType.Armor)),
							Tuple.Create(0.05, (ILoot)new TierLoot(13, ItemType.Armor)),
							Tuple.Create(0.001, (ILoot)new TierLoot(14, ItemType.Weapon)),
							Tuple.Create(0.005, (ILoot)new TierLoot(13, ItemType.Weapon)),
							Tuple.Create(0.05, (ILoot)new TierLoot(12, ItemType.Weapon)),
							Tuple.Create(0.05, (ILoot)new TierLoot(5, ItemType.Ring)),

							Tuple.Create(0.01, (ILoot)new ItemLoot("Potion of Oryx")),

							Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Atk)),
							Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Wis)),
							Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Vit)),
							Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Spd))
					))),
				condBehaviors: new ConditionalBehavior[] {
					new OnHit(new State("idle", SetState.Instance("begin")))
				}
			))
				.Init(0x00456, Behaves("Avatar of the Forgotten King",
					new RunBehaviors(
						new State("idle",
							SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)
						),
						new State("begin", new QueuedBehavior(
							new SimpleTaunt("BURN!!!"),
							CooldownExact.Instance(1000, NullBehavior.Instance),
							new SimpleTaunt("HAHAHAHA!!"),
							Timed.Instance(2000, False.Instance(Flashing.Instance(250, 0xffffff00))),
							SetState.Instance("grow")
						)),
						new State("grow", SimpleWandering.Instance(5), new QueuedBehavior(
							CooldownExact.Instance(400, SetSize.Instance(140)),
							CooldownExact.Instance(400, SetSize.Instance(160)),
							CooldownExact.Instance(400, SetSize.Instance(180)),
							CooldownExact.Instance(400, SetSize.Instance(200)),
							CooldownExact.Instance(400, SetSize.Instance(220)),
							new SimpleTaunt("LEAVE THIS PLACE!"),
							CooldownExact.Instance(1000, SetState.Instance("attack"))
						)),
						new State("attack",
							StateOnce.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
							Chasing.Instance(9, 8, 16, null),
							Cooldown.Instance(1500, RingAttack.Instance(6, 20, offset: 20, projectileIndex: 0)),
							CooldownExact.Instance(1000, MultiAttack.Instance(10, 18 * (float)Math.PI / 2700, 9)),

							Cooldown.Instance(500, RingAttack.Instance(12, projectileIndex: 1)),
							CooldownExact.Instance(1000, MultiAttack.Instance(8, 10 * (float)Math.PI / 180, 6)),
							HpLesserPercent.Instance((float)0.1, new RunBehaviors(
								SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
								SetState.Instance("dying")))
						),
						new State("dying",
							StateOnce.Instance(new SimpleTaunt("YOU KNOW NOT WHAT YOU HAVE DONE!")),
							new QueuedBehavior(
								CooldownExact.Instance(4000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
								CooldownExact.Instance(1500, SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))
							),
							Flashing.Instance(250, 0xffff0000),
							Chasing.Instance(7.5f, 6, 0, null), SimpleWandering.Instance(4),
							Cooldown.Instance(100, RingAttack.Instance(10, projectileIndex: 1))
						)
					),
					loot: new LootBehavior(LootDef.Empty,
							Tuple.Create(100, new LootDef(0, 10, 0, 8,
								Tuple.Create(0.05, (ILoot)new TierLoot(6, ItemType.Ability)),
								Tuple.Create(0.05, (ILoot)new TierLoot(13, ItemType.Armor)),
								Tuple.Create(0.05, (ILoot)new TierLoot(12, ItemType.Weapon)),
								Tuple.Create(0.05, (ILoot)new TierLoot(6, ItemType.Ring)),

								Tuple.Create(0.01, (ILoot)new ItemLoot("Tablet of the King's Avatar")),
								Tuple.Create(0.01, (ILoot)new ItemLoot("Bracer of the Guardian")),
								Tuple.Create(0.01, (ILoot)new ItemLoot("The Twilight Gemstone")),
								Tuple.Create(0.01, (ILoot)new ItemLoot("The Forgotten Crown")),

								Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Atk)),
								Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Def)),
								Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Life)),
								Tuple.Create(0.1, (ILoot)new StatPotionLoot(StatPotion.Mana))
						))),
					condBehaviors: new ConditionalBehavior[] {
					new OnHit(new State("idle", SetState.Instance("begin")))
				}
				))
				.Init(0x00480, Behaves("Rock Dragon",
					new RunBehaviors(
						new State("idle",
							SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)
						),
						new State("begin", new QueuedBehavior(
							new SimpleTaunt("Why have you awoken me?"),
							CooldownExact.Instance(1750, NullBehavior.Instance),
							new SimpleTaunt("*yawn*"),
							Timed.Instance(2000, False.Instance(Flashing.Instance(250, 0xffffff00))),
							SetState.Instance("fly")
							)),
							new State("fly", SimpleWandering.Instance(5), new QueuedBehavior(
							CooldownExact.Instance(400, SetSize.Instance(120)),
							CooldownExact.Instance(400, SetSize.Instance(140)),
							CooldownExact.Instance(400, SetSize.Instance(160)),
							CooldownExact.Instance(400, SetSize.Instance(185)),
							new SimpleTaunt("Look into my eye and perish!"),
							CooldownExact.Instance(1000, SetState.Instance("attack"))
						)),
						new State("attack",
							StateOnce.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
							Circling.Instance(20, 40, 20, null),
							Cooldown.Instance(2500, RingAttack.Instance(6, 20, offset: 10, projectileIndex: 1)),
							Cooldown.Instance(1500, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 1, 1, projectileIndex: 0)),

							Cooldown.Instance(800, RingAttack.Instance(12, projectileIndex: 2)),
							CooldownExact.Instance(1000, MultiAttack.Instance(8, 10 * (float)Math.PI / 180, 6)),
							HpLesserPercent.Instance((float)0.1, new RunBehaviors(
								SetConditionEffect.Instance(ConditionEffectIndex.Armored)
						   )))),
						loot: new LootBehavior(LootDef.Empty,
						Tuple.Create(100, new LootDef(0, 2, 0, 1,
							Tuple.Create(0.01, (ILoot)new ItemLoot("Doku No Ken")),
							Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Def)),
							Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Vit)),
							Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Wis)),
							Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Dex)),
							Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Atk)),
							Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Spd))
				))),
				condBehaviors: new ConditionalBehavior[] {
					new OnHit(new State("idle", SetState.Instance("begin"))),
					new DeathPortal(0x753e, 80)
				}
			));
	}
}