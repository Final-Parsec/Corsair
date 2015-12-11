using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(RectTransform))]
public class TurretSelectionMenu : MonoBehaviour
{
    public GameObject turretSelectionPrefab;
    public Vector2 layoutdimensions;

    // Use this for initialization
    void Start()
    {
        RectTransform prefabRectTransform = turretSelectionPrefab.GetComponent<RectTransform>();
        RectTransform rectTransform = this.GetComponent<RectTransform>();

        Vector2 spacer = new Vector2((rectTransform.GetWidth() - prefabRectTransform.GetWidth() * layoutdimensions.x) / (layoutdimensions.x + 1),
                                     (rectTransform.GetHeight() - prefabRectTransform.GetHeight() * layoutdimensions.y) / (layoutdimensions.y + 1));

        // Move up so the menu is just above the WaveDisplay

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
            rectTransform.anchoredPosition.y + ObjectManager.GetInstance().WaveDisplay.topYcordinate);

        int turretCount = Enum.GetNames(typeof(TurretType)).Length;
        var buttons = new List<Button>();
        for (int y = 1; y <= layoutdimensions.y; y++)
        {
            for (int x = 1; x <= layoutdimensions.x; x++)
            {
                if (x * y > turretCount)
                {
                    break;
                }

                var button = Instantiate(turretSelectionPrefab, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero)) as GameObject;
                var buttonRT = button.GetComponent<RectTransform>();
                buttons.Add(button.GetComponent<Button>());

                buttonRT.pivot = new Vector2(0.5f, 0.5f);
                buttonRT.SetParent(this.transform);
                buttonRT.SetAnchorTopLeft();

                float xpos = spacer.x * x + buttonRT.GetWidth() * (x - 1) + buttonRT.GetWidth() / 2f;
                float ypos = spacer.y * y + buttonRT.GetHeight() * (y - 1) + buttonRT.GetHeight() / 2f;

                buttonRT.anchoredPosition = new Vector2(xpos, -ypos);

                //sprites[x].SetTexture(waveTextures[node.Value.waveId]);
            }
        }

        ObjectManager objectManager = ObjectManager.GetInstance();

        foreach (TurretType type in Enum.GetValues(typeof(TurretType)))
        {
            UnityEngine.Events.UnityAction action1;

            // The struggle
            var intCopy = (int)type;

            switch (type)
            {
                case TurretType.Pistolman:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[1].onClick.AddListener(action1);
                    break;
                case TurretType.Rifleman:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[0].onClick.AddListener(action1);
                    break;
                case TurretType.Cannon:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[2].onClick.AddListener(action1);
                    break;
                case TurretType.Netter:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[3].onClick.AddListener(action1);
                    break;
                case TurretType.Buckshot:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[4].onClick.AddListener(action1);
                    break;
                case TurretType.Molotov:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[5].onClick.AddListener(action1);
                    break;
                case TurretType.Sirens:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[6].onClick.AddListener(action1);
                    break;
                case TurretType.PiroThePirateHero:
                    action1 = () => { objectManager.GuiButtonMethods.TurretButtonPressed(intCopy); };
                    buttons[7].onClick.AddListener(action1);
                    break;
            }
        }
    }
}
