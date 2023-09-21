/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public RectTransform priceLabelContainer;
    public RectTransform upgradeIconContainer;
    public Button buttonComponent;
    public GameObject priceLabelPrefab;

    public static bool IsUIOverride { get; private set; }

    private Upgrade upgrade;

    #region Monobehaviour Methods
    private void OnEnable()
    {
        buttonComponent.onClick.AddListener(() => UpgradeCastle.PurchaseUpgrade(upgrade));
    }

    private void OnDisable()
    {
        buttonComponent.onClick.RemoveAllListeners();
    }

    void Update()
    {
        // It will turn true if hovering any UI Elements
        IsUIOverride = EventSystem.current.IsPointerOverGameObject();
        GameObject selectedTower = UpgradeCastle.GetSelectedTower();

        if (UpgradeCastle.PurchasingUpgrade || selectedTower == null)
        {
            buttonComponent.interactable = false;
        }
        else if (!UpgradeCastle.PurchasingUpgrade && selectedTower != null)
        {
            buttonComponent.interactable = true;
        }
    }
    #endregion

    public void SetUpgrade(Upgrade newUpgrade)
    {
        upgrade = newUpgrade;

        // set icon
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
        {
            if (upgradeType != UpgradeType.None)
            {
                upgradeIconContainer.Find(upgradeType.ToString()).gameObject.SetActive(false);
            }
        }

        upgradeIconContainer.Find(upgrade.type.ToString()).gameObject.SetActive(true);

        // set price
        foreach (Price price in upgrade.price)
        {
            GameObject priceLabel = Instantiate(priceLabelPrefab, priceLabelContainer);
            PriceLabel priceLabelScript = priceLabel.GetComponent<PriceLabel>();
            priceLabelScript.SetLabel(price);
        }
    }
}
