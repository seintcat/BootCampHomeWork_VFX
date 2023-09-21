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
using UnityEngine.UI;


public class ResourceCounter : MonoBehaviour
{
    public int value;
    public int capacity;
    public ResourceType type;

    public Text amountStoredLabel;
    public RectTransform resourceIconContainer;
    public RectTransform meterFill;
    public Color color;

    private float meterFillMaxWidth;

    void Awake()
    {
        meterFillMaxWidth = meterFill.sizeDelta.x;
    }

    public void SetType (ResourceType newType)
    {
        type = newType;

        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resourceIconContainer.Find(resourceType.ToString()).gameObject.SetActive(false);
        }
        
        resourceIconContainer.Find(type.ToString()).gameObject.SetActive(true);
    }

    public void SubtractAmount(int amount)
    {
        SetValue(value - amount);
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        amountStoredLabel.text = value + " / " + capacity;

        UpdateMeterFillSize();
    }

    public void SetCapacity(int newCapacity)
    {
        capacity = newCapacity;
        amountStoredLabel.text = value + " / " + capacity;

        UpdateMeterFillSize();
    }

    public void SetColor(Color newColor)
    {
        Image meterFillImage = meterFill.GetComponent<Image>();
        meterFillImage.color = newColor;
        color = newColor;
    }
    private void UpdateMeterFillSize()
    {
        float newMeterWidth = meterFillMaxWidth * value / capacity;
        meterFill.sizeDelta = new Vector2(newMeterWidth, meterFill.sizeDelta.y);
    }
}
