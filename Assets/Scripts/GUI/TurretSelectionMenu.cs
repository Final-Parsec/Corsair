﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class TurretSelectionMenu : MonoBehaviour
{
    /// <summary>
    /// The <see cref="Button"/> prefab to use in the panel.
    /// </summary>
    /// <remarks>Must have a <see cref="RectTransform"/> attatched.</remarks>
    public GameObject turretSelectionPrefab;

    /// <summary>
    /// The layout of the <see cref="Button"/>s. ex: 2x4, 1x8
    /// </summary>
    public Vector2 layoutdimensions;

    /// <summary>
    /// The space between <see cref="Button"/>s.
    /// The <see cref="Button"/> size is calculated using this number.
    /// </summary>
    /// <remarks>
    /// Put a zero (0) value to ignore this axis and use a calculated padding value.
    /// Must have a value.
    /// </remarks>
    public Vector2 buttonPadding = new Vector2(10, 0);
    
    public IDictionary<string, Sprite> turretSprites = new Dictionary<string, Sprite>();

    private ObjectManager objectManager = ObjectManager.GetInstance();

    /// <summary>
    /// Runs when the TurretSelectionMenu is created.
    /// Populates the panel with turret buttons.
    /// </summary>
    void Start()
    {
        turretSprites = ObjectManager.LoadResources<Sprite>("GUI/Turret Images/", Enum.GetNames(typeof(TurretType)));

        RectTransform rectTransform = this.GetComponent<RectTransform>();
        
        float buttonSize = buttonPadding.x > 0 ? (rectTransform.GetWidth() - (buttonPadding.x * (layoutdimensions.x + 1))) / layoutdimensions.x :
                                                 (rectTransform.GetHeight() - (buttonPadding.y * (layoutdimensions.y + 1))) / layoutdimensions.y;

        Vector2 spacer = new Vector2((rectTransform.GetWidth() - buttonSize * layoutdimensions.x) / (layoutdimensions.x + 1),
                                     (rectTransform.GetHeight() - buttonSize * layoutdimensions.y) / (layoutdimensions.y + 1));

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

                buttonRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
                buttonRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);
                buttonRT.pivot = new Vector2(0.5f, 0.5f);
                buttonRT.SetParent(this.transform, false);
                buttonRT.SetAnchorTopLeft();

                float xpos = spacer.x * x + buttonRT.GetWidth() * (x - 1) + buttonRT.GetWidth();
                float ypos = spacer.y * y + buttonRT.GetHeight() * (y - 1) + buttonRT.GetHeight() / 2f;

                buttonRT.anchoredPosition = new Vector2(xpos, -ypos);
            }
        }

        for(int x = 0; x < Enum.GetNames(typeof(TurretType)).Length; x++)
        {
            int struggle = x;
            UnityAction action = () => { objectManager.GuiButtonMethods.TurretButtonPressed(struggle); };
            buttons[x].onClick.AddListener(action);
            buttons[x].GetComponent<Image>().sprite = turretSprites[((TurretType) x).ToString()];

            var buttonRT = buttons[x].GetComponent<RectTransform>();
            var priceObject = buttonRT.Find("Price");
            priceObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-buttonRT.GetWidth(), 0);
            priceObject.GetComponent<Text>().text = "$" + objectManager.TurretFactory.turretCosts[x] + "M";
        }

        objectManager.GuiButtonMethods.TurretButtonPressed((int)TurretType.Basic);
    }
}
