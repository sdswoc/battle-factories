using System.IO;
using UnityEngine;

public class ValueLoader : MonoBehaviour
{
    public static int tankRange;
    public static int tankMobility;
    public static int tankDamage;
    public static int tankHealth;
    public static int tankFuelUse;
    public static int tankCostCoin;
    public static int tankCostFuel;

    public static int truckRange;
    public static int truckMobility;
    public static int truckDamage;
    public static int truckHealth;
    public static int truckFuelUse;
    public static int truckCostCoin;
    public static int truckCostFuel;

    public static int medicRange;
    public static int medicMobility;
    public static int medicDamage;
    public static int medicHealth;
    public static int medicFuelUse;
    public static int medicCostCoin;
    public static int medicCostFuel;

    public static int factoryRange;
    public static int factoryHealth;

    public static int startCoin;
    public static int startFuel;
    public static int startCoinRate;
    public static int startFuelLimit;
    public static int upgradeCoinRate;
    public static int upgradeFuelLimit;

    public static int fuelUpgradeCostCoin;
    public static int moneyUpgradeCostCoin;

    public static int factorySetupTime;
    public static int turnTime;

    public static int flagRange;
    public static int flagCaptureTurnLimit;
    public static int flagBoundaryRange;

    private void Awake()
    {
        if (Directory.Exists(Application.persistentDataPath) && File.Exists(Application.persistentDataPath + "/values.txt"))
        {
            var ini = new IniFile();
            ini.Load(Application.persistentDataPath + "/values.txt", false);

            Debug.Log(tankRange = ini["tank"]["range"].ToInt(0));
            Debug.Log(tankMobility = ini["tank"]["mobility"].ToInt(0));
            Debug.Log(tankDamage = ini["tank"]["damage"].ToInt(0));
            Debug.Log(tankHealth = ini["tank"]["health"].ToInt(0));
            Debug.Log(tankFuelUse = ini["tank"]["fueluse"].ToInt(0));
            Debug.Log(tankCostCoin = ini["tank"]["costcoin"].ToInt(0));
            Debug.Log(tankCostFuel = ini["tank"]["costfuel"].ToInt(0));

            Debug.Log(truckRange = ini["truck"]["range"].ToInt(0));
            Debug.Log(truckMobility = ini["truck"]["mobility"].ToInt(0));
            Debug.Log(truckDamage = ini["truck"]["damage"].ToInt(0));
            Debug.Log(truckHealth = ini["truck"]["health"].ToInt(0));
            Debug.Log(truckFuelUse = ini["truck"]["fueluse"].ToInt(0));
            Debug.Log(truckCostCoin = ini["truck"]["costcoin"].ToInt(0));
            Debug.Log(truckCostFuel = ini["truck"]["costfuel"].ToInt(0));

            Debug.Log(medicRange = ini["medic"]["range"].ToInt(0));
            Debug.Log(medicMobility = ini["medic"]["mobility"].ToInt(0));
            Debug.Log(medicDamage = ini["medic"]["damage"].ToInt(0));
            Debug.Log(medicHealth = ini["medic"]["health"].ToInt(0));
            Debug.Log(medicFuelUse = ini["medic"]["fueluse"].ToInt(0));
            Debug.Log(medicCostCoin = ini["medic"]["costcoin"].ToInt(0));
            Debug.Log(medicCostFuel = ini["medic"]["costfuel"].ToInt(0));

            Debug.Log(factoryRange = ini["factory"]["range"].ToInt(0));
            Debug.Log(factoryHealth = ini["factory"]["health"].ToInt(0));

            Debug.Log(fuelUpgradeCostCoin = ini["upgrade"]["fuelupgradecostcoin"].ToInt(0));
            Debug.Log(moneyUpgradeCostCoin = ini["upgrade"]["coinupgradecostcoin"].ToInt(0));

            startCoin = ini["resource"]["startcoin"].ToInt(0);
            startFuel = ini["resource"]["startfuel"].ToInt(0);
            startCoinRate = ini["resource"]["startcoinrate"].ToInt(0);
            startFuelLimit = ini["resource"]["startfuellimit"].ToInt(0);
            upgradeCoinRate = ini["resource"]["upgradecoinrate"].ToInt(0);
            upgradeFuelLimit = ini["resource"]["upgradefuellimit"].ToInt(0);

            factorySetupTime = ini["time"]["factorysetuptime"].ToInt(0);
            turnTime = ini["time"]["turnTime"].ToInt(0);

            flagRange = ini["flag"]["range"].ToInt(0);
            flagCaptureTurnLimit = ini["flag"]["captureLimit"].ToInt(0);
            flagBoundaryRange = ini["flag"]["boundary"].ToInt(0);

            Debug.Log("Loaded");
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

            ini.Save(Application.persistentDataPath + "/values.txt");
            Debug.Log("Saved");
        }

    }
}
