namespace FinalParsec.Corsair.Turrets
{
	using System.Collections.Generic;
    using System.Linq;
	public static class TurretUpgrades
	{
		private static Dictionary<string, Upgrade[]> upgrades;

		public static int infernoCost = 40;
        public static int armageddonCost = 20;
        public static int burnCost = 30;

        public static int chainLightningCost = 20;
        public static int frostCost = 30;
        public static int lightningStrikeCost = 30;

        public static int poisonCost = 30;
        public static int mindControlCost = 50;
        public static int hexCost = 20;

        public static float costScaling = .5f; // costs of upgrades increase by 50% with each upgrade

        public static Upgrade GetUpgrade(string name, int rank)
        {
            if(upgrades == null)
            {
                MakeUpgrades();
            }

            Upgrade[] upgradeList;
            if (upgrades.TryGetValue(name, out upgradeList) && rank < upgradeList.Length && rank >= 0)
            {
                return upgradeList[rank];
            }
            return null;
        }

        public static IEnumerable<string> GetUpgradeNames()
        {
            if (upgrades == null)
            {
                MakeUpgrades();
            }

            return upgrades.Keys;
        }

        private static void MakeUpgrades()
		{
            upgrades = new Dictionary<string, Upgrade[]>();

			//Range
			Stat[] stats = {
				new Stat(Attribute.Range, 1f)
			};

			Upgrade[] tempUpgrades = new Upgrade[] {
				new Upgrade ("Range", "A good range.", 30, stats),
				new Upgrade ("Raange", "A better range!", 45, stats),
				new Upgrade ("Raaange", "A tremendous range!!!1!", 60, stats)
			};

			upgrades.Add("Range", tempUpgrades);

            //Damage
            stats = new Stat[] {
                new Stat(Attribute.Damage, 1f)
            };

            tempUpgrades = new Upgrade[] {
                new Upgrade ("Damage", "A good damage.", 30, stats),
                new Upgrade ("Daamage", "A better damage!", 45, stats),
                new Upgrade ("Daaamage", "A tremendous damage!!!1!", 60, stats)
            };

            upgrades.Add("Damage", tempUpgrades);

            //Speed
            stats = new Stat[] {
                new Stat(Attribute.RateOfFire, 1f)
            };

            tempUpgrades = new Upgrade[] {
                new Upgrade ("Speed", "A good speed.", 30, stats),
                new Upgrade ("Speeed", "A better speed!", 45, stats),
                new Upgrade ("Speeeed", "A tremendous speed!!!1!", 60, stats)
            };

            upgrades.Add("Speed", tempUpgrades);
        }

		public static string GetUpgradeCost(int cost, int upgradeLevel)
		{
			if (upgradeLevel == 3)
				return "";

			return "("+ (cost+(int)(cost * upgradeLevel * TurretUpgrades.costScaling))+")";
		}
	}
}