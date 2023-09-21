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


public class UpgradableTower : MonoBehaviour
{
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material selectedMaterial;
    public Transform upgradeContainer;
    public GameObject burstEffect;

    private bool hovered;
    private bool selected;
    private GameObject towerModel;
    public UpgradeType upgrade { get; private set; }

    #region Monobehaviour Methods
    // Start is called before the first frame update
    void Start()
    {
        upgrade = UpgradeType.None;
        Transform modelsContainer = transform.Find("Models");

        foreach (Transform towerTransform in modelsContainer)
        {
            if (towerTransform.gameObject.activeInHierarchy == true)
            {
                towerModel = towerTransform.gameObject;
            }
        }
    }

    public void OnMouseOver()
    {
        if (towerModel != null && !selected && !UpgradeButton.IsUIOverride && upgrade == UpgradeType.None)
        {
            towerModel.GetComponent<MeshRenderer>().material = highlightMaterial;
            hovered = true;
        }

        if (UpgradeButton.IsUIOverride)
        {
            towerModel.GetComponent<MeshRenderer>().material = defaultMaterial;
            hovered = false;
        }
    }

    public void OnMouseExit()
    {
        if (towerModel != null && !selected)
        {
            towerModel.GetComponent<MeshRenderer>().material = defaultMaterial;
            hovered = false;
        }
    }

    public void OnMouseDown()
    {
        if (towerModel != null
            && !UpgradeCastle.PurchasingUpgrade
            && !selected
            && hovered
            && upgrade == UpgradeType.None)
        {
            towerModel.GetComponent<MeshRenderer>().material = selectedMaterial;
            hovered = false;
            selected = true;

            // send to game manager
            UpgradeCastle.SetSelectedTower(gameObject);
        }
    }
    #endregion

    public void SetUpgrade(UpgradeType newUpgrade)
    {
        upgrade = newUpgrade;
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
        {
            if (upgradeType != UpgradeType.None)
            {
                upgradeContainer.Find(upgradeType.ToString()).gameObject.SetActive(false);
            }
        }

        upgradeContainer.Find(upgrade.ToString()).gameObject.SetActive(true);
        burstEffect.SetActive(true);
    }

    public void ResetMaterial()
    {
        towerModel.GetComponent<MeshRenderer>().material = defaultMaterial;
        hovered = false;
        selected = false;
    }

    
}
