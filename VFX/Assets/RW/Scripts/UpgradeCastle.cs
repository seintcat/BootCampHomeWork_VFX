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
using System.Collections.Generic;
using UnityEngine;


public class UpgradeCastle : MonoBehaviour
{
    [Header("UI Controls")]
    public List<Resource> resourceTypes = new List<Resource>();
    public List<Upgrade> availableUpgrades = new List<Upgrade>();

    [Header("Default Values")]
    public int startingValue;
    public int startingCapacity;

    [Header("UI Containers")]
    public RectTransform resourceCounterContainer;
    public RectTransform upgradeButtonContainer;
    public RectTransform resourcePoolContainer;

    [Header("Prefabs")]
    public GameObject resourceCounterPrefab;
    public GameObject upgradeButtonPrefab;
    public GameObject resourcePoolPrefab;

    private List<GameObject> upgradeButtons;

    private static List<ResourcePool> ResourcePools;
    private static List<ResourceCounter> ResourceCounters;

    private static GameObject SelectedTower;
    private static Upgrade CurrentSelectedUpgrade;
    private static GameObject StaticResourcePoolPrefeb;
    private static RectTransform StaticResourcePoolContainer;

    public static bool PurchasingUpgrade;

    #region Monobehaviour Methods
    void Start()
    {
        upgradeButtons = new List<GameObject>();
        ResourceCounters = new List<ResourceCounter>();
        ResourcePools = new List<ResourcePool>();
        if (resourcePoolPrefab != null)
        {
            StaticResourcePoolPrefeb = resourcePoolPrefab;
        }
        if (resourcePoolContainer != null)
        {
            StaticResourcePoolContainer = resourcePoolContainer;
        }
        foreach (Resource resource in resourceTypes)
        {
            // create counters
            GameObject counter = Instantiate(resourceCounterPrefab, resourceCounterContainer);
            ResourceCounter counterScript = counter.GetComponent<ResourceCounter>();
            counterScript.SetType(resource.type);
            counterScript.SetCapacity(startingCapacity);
            counterScript.SetValue(startingValue);
            counterScript.SetColor(resource.color);
            ResourceCounters.Add(counterScript);
        }

        foreach (Upgrade upgrade in availableUpgrades)
        {
            GameObject upgradeButton = Instantiate(upgradeButtonPrefab, upgradeButtonContainer);
            UpgradeButton upgradeButtonScript = upgradeButton.GetComponent<UpgradeButton>();
            upgradeButtonScript.SetUpgrade(upgrade);
            upgradeButtons.Add(upgradeButton);
        }
    }

    private void Update()
    {
        
        if (CurrentSelectedUpgrade != null && SelectedTower != null)
        {
            int activePoolsCompleted = 0;
            foreach (ResourcePool pool in ResourcePools)
            {
                if (pool.IsComplete)
                {
                    activePoolsCompleted += 1;
                }
            }
            
            if (activePoolsCompleted == CurrentSelectedUpgrade.price.Count)
            {
                ApplyUpgrade();
            }
        }
    }
    #endregion

    #region Selected Tower Methods
    public static void SetSelectedTower (GameObject tower)
    {
        if (!PurchasingUpgrade)
        {
            if (SelectedTower != null)
            {
                SelectedTower.GetComponent<UpgradableTower>().ResetMaterial();
            }

            SelectedTower = tower;
        }
    }

    public static void UnSetSelectedTower()
    {
        if (SelectedTower != null)
        {
            SelectedTower.GetComponent<UpgradableTower>().ResetMaterial();
            SelectedTower = null;
        }
    }

    public static GameObject GetSelectedTower()
    {
        return SelectedTower;
    }
    #endregion

    #region Tower Upgrade Methods
    public static void PurchaseUpgrade(Upgrade upgrade)
    {
        PurchasingUpgrade = true;
        if (SelectedTower != null)
        {
            bool validPurchase = true;
            foreach (Price price in upgrade.price)
            {
                foreach (ResourceCounter counter in ResourceCounters)
                {
                    if (counter.type == price.type)
                    {
                        if (counter.value - price.amount < 0)
                        {
                            validPurchase = false;
                        }
                    }
                }
            }

            if (validPurchase)
            {
                CurrentSelectedUpgrade = upgrade;
                // remove button(s?)
                ResourcePools.Clear();
                foreach (Price price in upgrade.price)
                {
                    ResourcePool thisPool = CreatePool(price.type);
                    thisPool.ActivatePool(price.amount);
                }
            }
            else
            {
                PurchasingUpgrade = false;
            }
        }
    }

    public static void ApplyUpgrade()
    {
        if (SelectedTower != null && CurrentSelectedUpgrade != null)
        {
            foreach (ResourcePool pool in ResourcePools)
            {
                Destroy(pool.gameObject);
            }
            SelectedTower.GetComponent<UpgradableTower>().SetUpgrade(CurrentSelectedUpgrade.type);
            CurrentSelectedUpgrade = null;
            UnSetSelectedTower();
            PurchasingUpgrade = false;
        }
    }
    #endregion

    #region Resource Pool Creation
    public static ResourcePool CreatePool(ResourceType type)
    {
        ResourceCounter poolCounter = ResourceCounters[0];
        foreach (ResourceCounter counter in ResourceCounters)
        {
            if (counter.type == type)
            {
                poolCounter = counter;
                break;
            }
        }
        // create pools
        GameObject pool = Instantiate(StaticResourcePoolPrefeb, StaticResourcePoolContainer);
        ResourcePool poolScript = pool.GetComponent<ResourcePool>();
        poolScript.SetType(type);
        RectTransform counterIconTransform = poolCounter.gameObject.transform.Find("ResourceIcon").GetComponent<RectTransform>();
        poolScript.SetResourceCounterIconTransform(counterIconTransform);
        poolScript.SetResourceCounterScript(poolCounter);
        ResourcePools.Add(poolScript);
        return poolScript;
    }
    #endregion

}

public enum UpgradeType
{
    Archer,
    Fire,
    Ice,
    Stone,
    None
}

public enum ResourceType
{
    Gold,
    Wood,
    Stone
}
