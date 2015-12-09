
namespace Assets.Scripts.Turrets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TurretModel : ICloneable
    {
        // Configurable
        public int aoeDamage = 0;
        public int aoeRange = 0;
        public int damage = 10;
        public int damageOverTime = 0;
        public float range = 5;
        public int rateOfFire = 5;
        public AttackOptionsFlags attackOptions;
        public TurretType turretType;
        public float Slow { get; set; }
        public float SlowDuration { get; set; }
        public int MindControlDuration { get; set; }
        public IList<string> UpgradeNames { get; set; }
        public IDictionary<string, int> UpgradePaths { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new TurretModel
            {
                aoeDamage = this.aoeDamage,
                aoeRange = this.aoeRange,
                damage = this.damage,
                damageOverTime = this.damageOverTime,
                range = this.range,
                rateOfFire = this.rateOfFire,
                attackOptions = this.attackOptions,
                Slow = Slow,
                SlowDuration = SlowDuration,
                MindControlDuration = this.MindControlDuration,
                turretType = this.turretType,
                UpgradeNames = this.UpgradeNames.Clone(),
                UpgradePaths = this.UpgradePaths.ToDictionary(entry => entry.Key, entry => entry.Value)
            };
        }
    }
}