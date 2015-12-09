using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FinalParsec.Corsair;
using FinalParsec.Corsair.Turrets;

public class UpgradeButton
{
	public RawImage image;
	public Text upgradeName;
	public Text description;
	public Text stats;
	public Button button;

	public UpgradeButton(Button button, Text upgradeName, Text description, Text stats, RawImage image)
	{
		this.button = button;
		this.upgradeName = upgradeName;
		this.description = description;
		this.stats = stats;
		this.image = image;
	}
}

public class TurretFocusMenu : MonoBehaviour
{
	private Image upgradeBackground;
	private Image selectedTurretBackground;
	private Image selectedTurretImage;
	private Text selectedTurretStats;

	private List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

	private Text sellPrice;
	public Dictionary<string,Texture> IconLookup = new Dictionary<string,Texture>();
	
	private ObjectManager objectManager;
	private Turret selectedTurret = null;
	
	private GameObject upgradeMenu;
	private Animator upgradeAnimator;
	
	private GameSpeed lastGameSpeed = GameSpeed.X1;

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
				isActive = false;
				objectManager.gameState.GameSpeed = lastGameSpeed;
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

				isActive = true;
				value.Select();
				upgradeAnimator.SetTrigger ("Swipe In");
				AttachToTurret();
				
				lastGameSpeed = objectManager.gameState.GameSpeed;
				objectManager.gameState.GameSpeed = GameSpeed.Paused;
			}
		}
	}
	
	
	// Called when the sell button is pressed.
	public void Sell()
	{
		if (selectedTurret == null)
			return;

		objectManager.GuiButtonMethods.PlaySellSound();
		
		objectManager.gameState.playerMoney += SelectedTurret.Msrp;
		objectManager.NodeManager.UnBlockNode(SelectedTurret.transform.position);
		Destroy(SelectedTurret.gameObject);
		SelectedTurret = null;
	}
	
	public void UpgradeSelectedTurret(int upgradeType)
	{
        if (this.SelectedTurret == null)
        {
            return;
        }

		objectManager.GuiButtonMethods.PlayDefaultSound();

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
		selectedTurretImage.sprite = selectedTurret.selectedSprite;
		selectedTurretImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( 30f, 30f + 15*selectedTurret.Level);
		
		sellPrice.text = "+(" + selectedTurret.Msrp + ")";

        selectedTurretStats.text = selectedTurret.GetStats ();

	    foreach (var upgradeButton in upgradeButtons)
	    {
            upgradeButton.upgradeName.text = "";
            upgradeButton.description.text = "";
            upgradeButton.stats.text = "";
        }

	    UpdateUpgradeButton(selectedTurret.turretModel.UpgradeNames);
	}

	private void UpdateUpgradeButton(IList<string> upgradeNames)
	{
	    for (var x = 0; x < upgradeNames.Count; x++)
	    {
            var upgrade = TurretUpgrades.GetUpgrade(upgradeNames[x], selectedTurret.turretModel.UpgradePaths[upgradeNames[x]]);
            if (upgrade == null)
            {
                // turret is fully upgraded on this path
                upgradeButtons[x].upgradeName.text = "MAX ";
                upgradeButtons[x].upgradeName.text += upgradeNames[x];
                //upgradeButtons[x].description.text = TurretUpgrades.upgrades[upgradeNames[x]].Description;
                //upgradeButtons[0].stats.text = TurretUpgrades.upgrades[upgradeNames[x]].GetPrettyStats();
                upgradeButtons[x].image.texture = IconLookup[upgradeNames[x]];
            }
            else
            {
                upgradeButtons[x].upgradeName.text = upgrade.Cost + " ";
                upgradeButtons[x].upgradeName.text += upgradeNames[x];
                //upgradeButtons[x].description.text = TurretUpgrades.upgrades[upgradeNames[x]].Description;
                //upgradeButtons[0].stats.text = TurretUpgrades.upgrades[upgradeNames[x]].GetPrettyStats();
                upgradeButtons[x].image.texture = IconLookup[upgradeNames[x]];
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
		TurretUpgrades.MakeUpgrades ();

		upgradeBackground = GameObject.FindGameObjectWithTag(Tags.UpgradePanel).GetComponent<Image>();
		selectedTurretBackground = GameObject.FindGameObjectWithTag(Tags.SelectedTurretPanel).GetComponent<Image> ();
		selectedTurretImage = GameObject.FindGameObjectWithTag(Tags.SelectedTurretPanel).transform.FindChild("SelectedImage").GetComponent<Image> ();
		selectedTurretStats = GameObject.FindGameObjectWithTag (Tags.SelectedTurretPanel).transform.Find ("TurretStats").GetComponent<Text> ();
		
		int upgradeType = 0;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(Tags.UpgradeButton))
		{
			// The struggle
			int upTypeReal = upgradeType;

			UpgradeButton upgradeButton = new UpgradeButton(obj.GetComponent<Button>(),obj.transform.Find("UpgradeName").GetComponent<Text>(),
			                                                obj.transform.Find("UpgradeDescription").GetComponent<Text>(),
			                                                obj.transform.Find("UpgradeStats").GetComponent<Text>(),
			                                                obj.transform.Find("UpgradeImage").GetComponent<RawImage> ());

			upgradeButtons.Add(upgradeButton);
		    var button = obj.GetComponent<Button>();
            UnityEngine.Events.UnityAction action1 = () => { UpgradeSelectedTurret(upTypeReal); };
            button.onClick.AddListener(action1);
			upgradeType++;
		}
		
		LoadIcons ();
		
		// Turret Upgrade Menu
		upgradeMenu = GameObject.Find ("UpgradeMenu");
		upgradeAnimator = upgradeMenu.GetComponent<Animator>();
		
		
		sellPrice = GameObject.Find ("CashBack").GetComponent<Text> ();
	}
	
	private void LoadIcons()
	{
		IconLookup.Add ("Range", Resources.Load("GUI/Upgrade Icons/Range") as Texture);
		IconLookup.Add ("Damage", Resources.Load("GUI/Upgrade Icons/Damage") as Texture);
		IconLookup.Add ("Speed", Resources.Load("GUI/Upgrade Icons/Speed") as Texture);
	}
	
	#endregion

}