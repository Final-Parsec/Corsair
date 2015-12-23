using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FinalParsec.Corsair;
using FinalParsec.Corsair.Turrets;

public class TurretFocusMenu : MonoBehaviour
{
	private List<Image> upgradeIcons = new List<Image>();

	private Text sellPrice;
	public IDictionary<string, Sprite> IconLookup;
	
	private ObjectManager objectManager;
	private Turret selectedTurret = null;
	
	private GameObject upgradeMenu;
	private Animator upgradeAnimator;
	
    [HideInInspector]
	public bool isActive = false;
	
	public Turret SelectedTurret
	{
		get
		{
			return selectedTurret;
		}
		set
		{
			Turret oldSelectedTurret = selectedTurret;
			
			if(selectedTurret != null && value == null)
			{
                objectManager.TurretRange.gameObject.SetActive(false);
                objectManager.GuiButtonMethods.RemoveTurretUpgradeMenu();
            }
			
			selectedTurret = value;

			
			if (oldSelectedTurret != null)
			{
				oldSelectedTurret.Deselect();
			}            
			
			if (value != null)
			{
				objectManager.TurretRange.gameObject.SetActive(true);
				objectManager.TurretRange.transform.position = new Vector3(selectedTurret.transform.position.x,
				                                                           selectedTurret.transform.position.y - 5,
				                                                           selectedTurret.transform.position.z);
				objectManager.TurretRange.ChangeSprite((int)value.turretModel.range);
                
				value.Select();
				upgradeAnimator.SetTrigger ("Swipe In");
				AttachToTurret();
			}
		}
	}
	
	public void UpgradeSelectedTurret(int upgradeType)
	{
        if (this.SelectedTurret == null)
        {
            return;
        }

		objectManager.AudioManager.PlayButtonSound();

        Debug.Log(upgradeType + "   " + SelectedTurret.turretModel.UpgradeNames[upgradeType]);

        var upgradeName = selectedTurret.turretModel.UpgradeNames[upgradeType];
        var upgradePathRank = selectedTurret.turretModel.UpgradePaths[upgradeName];

        if (SelectedTurret.Upgrade(TurretUpgrades.GetUpgrade(upgradeName, upgradePathRank)))
        {
            // increment the upgrade rank
            selectedTurret.turretModel.UpgradePaths[upgradeName] = upgradePathRank + 1;
        }
        
		AttachToTurret();
		
	}
	
	#region Internal methods
	
	private void AttachToTurret()
	{
		sellPrice.text = "+(" + selectedTurret.Msrp + ")";

	    UpdateUpgradeButton(selectedTurret.turretModel.UpgradeNames);
	}

	private void UpdateUpgradeButton(IList<string> upgradeNames)
	{
	    for (var x = 0; x < upgradeNames.Count; x++)
	    {
            var upgrade = TurretUpgrades.GetUpgrade(upgradeNames[x], selectedTurret.turretModel.UpgradePaths[upgradeNames[x]]);
            if (upgrade == null)
            {
                upgradeIcons[x].sprite = IconLookup[upgradeNames[x]];
            }
            else
            {
                upgradeIcons[x].sprite= IconLookup[upgradeNames[x]];
            }            
        }
	}
	
	// Runs when entity is Instantiated
	void Awake()
	{
		objectManager = ObjectManager.GetInstance();
	}
	
	
	// Use this for initialization
	void Start () {
		int upgradeType = 0;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(Tags.UpgradeButton))
		{
			// The struggle
			int upTypeReal = upgradeType;

            Image upgradeIcon= obj.GetComponent<Image>();

            upgradeIcons.Add(upgradeIcon);
		    var button = obj.GetComponent<Button>();
            UnityEngine.Events.UnityAction action1 = () => { UpgradeSelectedTurret(upTypeReal); };
            button.onClick.AddListener(action1);
			upgradeType++;
		}
		
		LoadIcons ();
		
		// Turret Upgrade Menu
		upgradeMenu = GameObject.Find ("TurretUpgradePanel");
		upgradeAnimator = upgradeMenu.GetComponent<Animator>();
		
		
		sellPrice = GameObject.Find ("CashBack").GetComponent<Text> ();
	}
	
	private void LoadIcons()
	{
        IconLookup = ObjectManager.LoadResources<Sprite>("GUI/Upgrade Icons/", TurretUpgrades.GetUpgradeNames());
	}
	
	#endregion

}