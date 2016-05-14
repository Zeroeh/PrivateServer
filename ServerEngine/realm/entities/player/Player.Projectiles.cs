using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.svrPackets;
using ServerEngine.cliPackets;

namespace ServerEngine.realm.entities
{
    public partial class Player
    {
        public void PlayerShoot(RealmTime time, PlayerShootPacket pkt)
        {
            System.Diagnostics.Debug.WriteLine(pkt.Position);
            Item item = XmlDatas.ItemDescs[pkt.ContainerType];
            if (item.ObjectType == Inventory[0].ObjectType || item.ObjectType == Inventory[1].ObjectType)
            {
                var prjDesc = item.Projectiles[0];
                projectileId = pkt.BulletId;
                Projectile prj = CreateProjectile(prjDesc, item.ObjectType,
                    0,
                    pkt.Time + tickMapping, pkt.Position, pkt.Angle);
                Owner.EnterWorld(prj);
                Owner.BroadcastPacket(new AllyShootPacket()
                {
                    OwnerId = Id,
                    Angle = pkt.Angle,
                    ContainerType = pkt.ContainerType,
                    BulletId = pkt.BulletId
                }, this);
                fames.Shoot(prj);
            }
        }
		//Buggy enemy hit shit going on
        public void EnemyHit(RealmTime time, EnemyHitPacket pkt) //Related to the problem in Projectile.cs
        {
            try
            {
                var entity = Owner.GetEntity(pkt.TargetId);
                Projectile prj = (this as IProjectileOwner).Projectiles[pkt.BulletId];
                prj.Damage = (int)statsMgr.GetAttackDamage(prj.Descriptor.MinDamage, prj.Descriptor.MaxDamage);
				prj.ForceHit(entity, time); //was commented
                if (pkt.Killed && !(entity is Wall))
                {
                    psr.SendPacket(new UpdatePacket()
                    {
                        Tiles = new UpdatePacket.TileData[0],
                        NewObjects = new ObjectDef[] { entity.ToDefinition() },
                        RemovedObjectIds = new int[] { pkt.TargetId }
                    });
                    clientEntities.Remove(entity);
                }
            }
            catch
            {

            }
        }

        public void OtherHit(RealmTime time, OtherHitPacket pkt)
        {

        }

        public void SquareHit(RealmTime time, SquareHitPacket pkt)
        {
            
        }

        public void PlayerHit(RealmTime time, PlayerHitPacket pkt)
        {
            
        }

        public void ShootAck(RealmTime time, ShootAckPacket pkt)
        {
            
        }
    }
}
