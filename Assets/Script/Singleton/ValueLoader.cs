using System.IO;
using UnityEngine;

namespace Singleton
{
    public class ValueLoader : MonoBehaviour
    {
        public static string fileName = "/values.fk";
        public static int tankRange = 16;
        public static int tankMobility = 14;
        public static int tankDamage = -7;
        public static int tankHealth = 30;
        public static int tankFuelUse = 6;
        public static int tankCostCoin = 35;
        public static int tankCostFuel = 8;

        public static int truckRange = 20;
        public static int truckMobility = 18;
        public static int truckDamage = -6;
        public static int truckHealth = 28;
        public static int truckFuelUse = 8;
        public static int truckCostCoin = 42;
        public static int truckCostFuel = 9;

        public static int medicRange = 10;
        public static int medicMobility = 10 ;
        public static int medicDamage = 2;
        public static int medicHealth = 20;
        public static int medicFuelUse = 7;
        public static int medicCostCoin = 44;
        public static int medicCostFuel = 10;

        public static int factoryRange = 18;
        public static int factoryHealth = 140;

        public static int startCoin = 40;
        public static int startFuel = 10;
        public static int startCoinRate = 10;
        public static int startFuelLimit = 10;
        public static int upgradeCoinRate = 2;
        public static int upgradeFuelLimit = 2;

        public static int fuelUpgradeCostCoin = 80;
        public static int moneyUpgradeCostCoin = 180;

        public static int factorySetupTime = 10;
        public static int turnTime = 25;

        public static int flagRange = 10;
        public static int flagCaptureTurnLimit = 10;
        public static int flagBoundaryRange = 45;

        private void Awake()
        {
            if (Directory.Exists(Application.persistentDataPath) && File.Exists(Application.persistentDataPath + fileName))
            {
                var ini = new IniFile();
                ini.Load(Application.persistentDataPath + fileName, false);

                tankRange = ini["tank"]["range"].ToInt(tankRange);
                tankMobility = ini["tank"]["mobility"].ToInt(tankMobility);
                tankDamage = ini["tank"]["damage"].ToInt(tankDamage);
                tankHealth = ini["tank"]["health"].ToInt(tankHealth);
                tankFuelUse = ini["tank"]["fueluse"].ToInt(tankFuelUse);
                tankCostCoin = ini["tank"]["costcoin"].ToInt(tankCostCoin);
                tankCostFuel = ini["tank"]["costfuel"].ToInt(tankCostFuel);

                truckRange = ini["truck"]["range"].ToInt(truckRange);
                truckMobility = ini["truck"]["mobility"].ToInt(truckMobility);
                truckDamage = ini["truck"]["damage"].ToInt(truckDamage);
                truckHealth = ini["truck"]["health"].ToInt(truckHealth);
                truckFuelUse = ini["truck"]["fueluse"].ToInt(truckFuelUse);
                truckCostCoin = ini["truck"]["costcoin"].ToInt(truckCostCoin);
                truckCostFuel = ini["truck"]["costfuel"].ToInt(truckCostFuel);

                medicRange = ini["medic"]["range"].ToInt(medicRange);
                medicMobility = ini["medic"]["mobility"].ToInt(medicMobility);
                medicDamage = ini["medic"]["damage"].ToInt(medicDamage);
                medicHealth = ini["medic"]["health"].ToInt(medicHealth);
                medicFuelUse = ini["medic"]["fueluse"].ToInt(medicFuelUse);
                medicCostCoin = ini["medic"]["costcoin"].ToInt(medicCostCoin);
                medicCostFuel = ini["medic"]["costfuel"].ToInt(medicCostFuel);

                factoryRange = ini["factory"]["range"].ToInt(factoryRange);
                factoryHealth = ini["factory"]["health"].ToInt(factoryHealth);

                fuelUpgradeCostCoin = ini["upgrade"]["fuelupgradecostcoin"].ToInt(fuelUpgradeCostCoin);
                moneyUpgradeCostCoin = ini["upgrade"]["coinupgradecostcoin"].ToInt(moneyUpgradeCostCoin);

                startCoin = ini["resource"]["startcoin"].ToInt(startCoin);
                startFuel = ini["resource"]["startfuel"].ToInt(startFuel);
                startCoinRate = ini["resource"]["startcoinrate"].ToInt(startCoinRate);
                startFuelLimit = ini["resource"]["startfuellimit"].ToInt(startFuelLimit);
                upgradeCoinRate = ini["resource"]["upgradecoinrate"].ToInt(upgradeCoinRate);
                upgradeFuelLimit = ini["resource"]["upgradefuellimit"].ToInt(upgradeFuelLimit);

                factorySetupTime = ini["time"]["factorysetuptime"].ToInt(factorySetupTime);
                turnTime = ini["time"]["turnTime"].ToInt(turnTime);

                flagRange = ini["flag"]["range"].ToInt(flagRange);
                flagCaptureTurnLimit = ini["flag"]["captureLimit"].ToInt(flagCaptureTurnLimit);
                flagBoundaryRange = ini["flag"]["boundary"].ToInt(flagBoundaryRange);
            }
            else
            {
                var ini = new IniFile();

                ini["tank"]["range"] = tankRange;
                ini["tank"]["mobility"] = tankMobility;
                ini["tank"]["damage"] = tankDamage;
                ini["tank"]["health"] = tankHealth;
                ini["tank"]["fueluse"] = tankFuelUse;
                ini["tank"]["costcoin"] = tankCostCoin;
                ini["tank"]["costfuel"] = tankCostFuel;

                ini["truck"]["range"] = truckRange;
                ini["truck"]["mobility"] = truckMobility;
                ini["truck"]["damage"] = truckDamage;
                ini["truck"]["health"] = truckHealth;
                ini["truck"]["fueluse"] = truckFuelUse;
                ini["truck"]["costcoin"] = truckCostCoin;
                ini["truck"]["costfuel"] = truckCostFuel;

                ini["medic"]["range"] = medicRange;
                ini["medic"]["mobility"] = medicMobility;
                ini["medic"]["damage"] = medicDamage;
                ini["medic"]["health"] = medicHealth;
                ini["medic"]["fueluse"] = medicFuelUse;
                ini["medic"]["costcoin"] = medicCostCoin;
                ini["medic"]["costfuel"] = medicCostFuel;

                ini["factory"]["range"] = factoryRange;
                ini["factory"]["health"] = factoryHealth;

                ini["upgrade"]["fuelupgradecostcoin"] = fuelUpgradeCostCoin;
                ini["upgrade"]["coinupgradecostcoin"] = moneyUpgradeCostCoin;

                ini["resource"]["startcoin"] = startCoin;
                ini["resource"]["startfuel"] = startFuel;
                ini["resource"]["startcoinrate"] = startCoinRate;
                ini["resource"]["startfuellimit"] = startFuelLimit;
                ini["resource"]["upgradecoinrate"] = upgradeCoinRate;
                ini["resource"]["upgradefuellimit"] = upgradeFuelLimit;

                ini["time"]["factorysetuptime"] = factorySetupTime;
                ini["time"]["turnTime"] = factorySetupTime;

                ini["flag"]["range"] = flagRange;
                ini["flag"]["captureLimit"] = flagCaptureTurnLimit;
                ini["flag"]["boundary"] = flagBoundaryRange;

                ini.Save(Application.persistentDataPath + fileName);
            }

        }
    }
}