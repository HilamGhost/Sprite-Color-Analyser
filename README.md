# Sprite Color Analyser

## 1. Introduction

In Cuphead, the player can `parry` every pink object and gain a charge of EX Move. Basically we can make this mechanic using a structure like this:
```csharp

public class CupheadBullet : MonoBehaviour
{
    public bool IsParryable;
}
```
And we will change the sprite to pink and done! But I think there is a more interesting solution for this: Color Checking! In this method we are try to catching every sprite includes specific color and do process we want. So we don't have to check the object is parryable. If we want to put a different mechanic, like if the enemy is red, water damage gives extra damage; if the enemy is blue, fire damage gives extra damage, we can easily adapt the mechanic just checking the sprite.

Unity’s native tools don’t offer fine-grained control or tolerance-based comparison for sprite color analysis. That’s where **ColorAnalyzer** comes in.

This utility helps analyze the visible portion of a sprite (ignoring transparent pixels) and detect whether a specific color is dominant, based on a user-defined **tolerance** and **dominance percentage** threshold. It’s especially useful for:

- Color-tagged gameplay logic (e.g., "red" enemies take extra fire damage)
- Texture or sprite consistency checks
- Visual diagnostics or debugging tools
- Procedural classification or grouping

It works by reading pixel data from the sprite's texture, grouping similar colors (based on configurable tolerance), and reporting their percentage contribution.

---

## 2. How to Use It

### ✅ Requirements
- Unity (tested with 2020.3 and above)

### 🧩 Installation
Simply copy the `ColorAnalyzer.cs` script into your Unity project's folder.

---
![image](https://github.com/user-attachments/assets/df51c21f-8b76-433e-ac53-0e011c2e55b6)

### 🎮 Example Usage

```csharp
using UnityEngine;
using HilamPrototypes;

public class ExampleUsage : MonoBehaviour
{
    public Sprite targetSprite;
    public Color colorToCheck = Color.red;
    [Range(0f, 100f)] public float dominanceThreshold = 50f;

    void Start()
    {
        bool isDominant = ColorAnalyzer.IsColorDominant(targetSprite, colorToCheck, dominanceThreshold);
        Debug.Log($"Is red dominant? {isDominant}");
    }
}
