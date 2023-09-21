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
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class ResourcePool : MonoBehaviour
{
    [Header("Values")]
    public float moveRate;
    public float moveSpeed;
    public float baseScaleMultiplier; // how large the icon starts
    public float scaleMultiplierIncreasePerUnit; // how much the icon increases in size
    public int unitsPerIcon; // how many units are represented by one icon
    public float iconSpawnRate;
    public ResourceType type;

    [Header("Prefabs")]
    public GameObject resourceIconPrefab;
    public GameObject materialPoolEffectObjectPrefab;
    private VisualEffect materialPoolEffect;

    [Space]
    public string spawnedIconTag = "SpawnedIcon";

    public bool IsComplete
    {
        get
        {
            return active == false;
        }
    }

    private RectTransform resourceCounterIconTransform;
    private int startAmount;
    private int amount; // how much was spent
    private bool active;
    private bool shouldSpawnIcons;
    private GameObject poolIcon;
    private Transform poolTarget;
    private ResourceCounter resourceCounter;
    private Vector3 baseLocalScale;
    private float currentScaleFactor;
    private List<GameObject> spawnedIcons;

    // Coroutines
    private IEnumerator moveIcons;
    private IEnumerator spawnIcons;

    public void OnTriggerEnter(Collider other)
    {
        if (active && other.tag == spawnedIconTag && poolIcon != null)
        {
            if (!poolIcon.activeInHierarchy)
            {
                currentScaleFactor = baseScaleMultiplier;
                poolIcon.transform.localScale = new Vector3(baseLocalScale.x * currentScaleFactor, baseLocalScale.y * currentScaleFactor, baseLocalScale.z * currentScaleFactor);
                poolIcon.SetActive(true);
            }
            else
            {
                currentScaleFactor += scaleMultiplierIncreasePerUnit;
            }
            spawnedIcons.Remove(other.gameObject);
            Destroy(other.gameObject);
            poolIcon.transform.localScale = (baseLocalScale * currentScaleFactor);
        }
    }

    private IEnumerator MoveIcons()
    {
        while (Application.isPlaying)
        {
            if (active && spawnedIcons.Count > 0)
            {
                foreach (GameObject icon in spawnedIcons)
                {

                    if (icon != null)
                    {
                        Vector3 heading = poolTarget.position - resourceCounterIconTransform.position;
                        float distance = heading.magnitude;
                        Vector3 direction = heading / distance;
                        icon.transform.Translate(direction * moveSpeed);
                    }
                }
            }
            if (amount == 0 && spawnedIcons.Count == 0)
            {
                ShowVisualEffect();
            }
            yield return new WaitForSeconds(moveRate);
        }
    }

    private void ShowVisualEffect()
    {
        shouldSpawnIcons = false;
        StopAllCoroutines();

        poolIcon.SetActive(false);

        DeactivatePool();

        Transform towerUpgradePosition = UpgradeCastle.GetSelectedTower().transform.Find("ParticlePoint");

        GameObject materialPoolEffectObject = Instantiate(materialPoolEffectObjectPrefab);

        materialPoolEffect = materialPoolEffectObject.GetComponent<VisualEffect>();

        materialPoolEffect.SetVector3("StartPosition", poolIcon.transform.position);

        materialPoolEffect.SetVector3("TowerPosition", towerUpgradePosition.position);

        materialPoolEffect.SendEvent("OnPlay");

        Invoke("DeactivatePool", 3.0f);
    }

    private IEnumerator SpawnIcons()
    {
        while (Application.isPlaying && shouldSpawnIcons)
        {
            yield return new WaitForSeconds(iconSpawnRate);
            if (amount > 0)
            {
                int amountToSubtract = unitsPerIcon;
                if (resourceCounter.value - amountToSubtract < 0)
                {
                    amountToSubtract = resourceCounter.value;
                }
                amount -= amountToSubtract;
                resourceCounter.SubtractAmount(amountToSubtract);

                GameObject resourceIcon = Instantiate(resourceIconPrefab, resourceCounterIconTransform, true);
                resourceIcon.transform.position = resourceCounterIconTransform.transform.position;
                resourceIcon.GetComponent<SphereCollider>().enabled = true;
                resourceIcon.transform.localScale *= 0.005F;
                resourceIcon.transform.Find(type.ToString()).gameObject.SetActive(true);
                Debug.Log(spawnedIconTag);
                resourceIcon.tag = spawnedIconTag;

                spawnedIcons.Add(resourceIcon);
            }
        }
    }

    #region Activate/Deactivate Methods
    public void ActivatePool(int value)
    {
        moveIcons = MoveIcons();
        spawnIcons = SpawnIcons();
        poolTarget = transform.Find("WorldPositionTarget");
        if (spawnedIcons == null)
        {
            spawnedIcons = new List<GameObject>();
        }
        else
        {
            spawnedIcons.Clear();
        }
        amount = value;
        startAmount = value;
        active = true;
        shouldSpawnIcons = true;
        StartCoroutine(spawnIcons);
        StartCoroutine(moveIcons);
    }

    private void DeactivatePool()
    {
        active = false;
        StopAllCoroutines();
    }
    #endregion

    #region Set Methods
    public void SetType(ResourceType newType)
    {
        type = newType;
        poolIcon = transform.Find("ResourceIcon").Find(type.ToString()).gameObject;
        baseLocalScale = poolIcon.transform.localScale;
    }
    public void SetResourceCounterIconTransform(RectTransform iconTransform)
    {
        resourceCounterIconTransform = iconTransform;
    }

    public void SetResourceCounterScript(ResourceCounter counter)
    {
        resourceCounter = counter;
    }
    #endregion
}
