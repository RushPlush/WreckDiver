using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MappingHandler : MonoBehaviour
{
    
    [SerializeField] private List<SpriteMapping> mappingsList;
    [SerializeField] private List<ImageMapping> imageMappings;
    [SerializeField] private List<ButtonHintMapping> buttonHintMappings;

    private void Awake()
    {
        DeviceListener.OnDeviceChange += DeviceListenerOnDeviceChange;
        foreach(var buttonHintMapping in buttonHintMappings)
            buttonHintMapping.buttonHint.SetActive(false);
    }

    private void OnDestroy()
    {
        DeviceListener.OnDeviceChange -= DeviceListenerOnDeviceChange;
    }

    private void DeviceListenerOnDeviceChange(DeviceType device)
    {
        foreach (var (mapping, image) in imageMappings)
        {
            var (_, sprites) = mappingsList.Find(x => x.type == mapping);
            if (!image || sprites == null) continue;
            image.sprite = sprites[device];
        }
    }

    public void ToggleInteractionHint() => Toggle(MappingType.Interact);

    public bool IsInteractionHintActive() => IsActive(MappingType.Interact);

    public void ToggleInteractionGuide() => ToggleAll(MappingType.InteractUp, MappingType.InteractDown, MappingType.InteractLeft, MappingType.InteractRight, MappingType.InteractExit);

    public bool IsInteractionGuideActive() => IsAllActive(MappingType.InteractUp, MappingType.InteractDown, MappingType.InteractLeft, MappingType.InteractRight, MappingType.InteractExit);

    public void ToggleMoveGuide() => ToggleAll(MappingType.MoveUp, MappingType.MoveDown, MappingType.MoveLeft, MappingType.MoveRight, MappingType.LookAround);

    public bool IsMoveGuideActive() => IsAllActive(MappingType.MoveUp, MappingType.MoveDown, MappingType.MoveLeft, MappingType.MoveRight, MappingType.LookAround);

    public void ToggleInventoryGuide() => ToggleAll(MappingType.InventoryNext, MappingType.InventoryPrevious, MappingType.UseItem);

    public bool IsInventoryGuideActive() => IsAllActive(MappingType.InventoryNext, MappingType.InventoryPrevious, MappingType.UseItem);

    // ReSharper disable MemberCanBePrivate.Global
    public void Toggle(MappingType type) => buttonHintMappings.Find(x => x.type == type).buttonHint.SetActive(!buttonHintMappings.Find(x => x.type == type).buttonHint.activeSelf);

    public void ToggleAll(params MappingType[] types) => buttonHintMappings.FindAll(x => types.Contains(x.type)).ForEach(x => x.buttonHint.SetActive(!x.buttonHint.activeSelf));

    public bool IsActive(MappingType type) => buttonHintMappings.Find(x => x.type == type).buttonHint.activeSelf;

    public bool IsAllActive(params MappingType[] types) => buttonHintMappings.FindAll(x => types.Contains(x.type)).All(x => x.buttonHint.activeSelf);
    // ReSharper restore MemberCanBePrivate.Global
}

[CustomEditor(typeof(MappingHandler))]
public class MappingHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Toggle Interaction Hint")) ((MappingHandler)target).ToggleInteractionHint();
        if (GUILayout.Button("Toggle Interaction Guide")) ((MappingHandler)target).ToggleInteractionGuide();
    }
}

#region Serializable Classes

[Serializable]
public class DeviceSpriteMapping
{
    [FormerlySerializedAs("Device")]
    public DeviceType device;
    [FormerlySerializedAs("Sprite")]
    public Sprite sprite;

    public static implicit operator KeyValuePair<DeviceType, Sprite>(DeviceSpriteMapping deviceSpriteMapping) => new(deviceSpriteMapping.device, deviceSpriteMapping.sprite);
    public static implicit operator DeviceSpriteMapping(KeyValuePair<DeviceType, Sprite> sprite) => new() { device = sprite.Key, sprite = sprite.Value };

    public void Deconstruct(out DeviceType device, out Sprite sprite) => (device, sprite) = (this.device, this.sprite);
}

[Serializable]
public class SpriteMapping
{
    [FormerlySerializedAs("Type")]
    public MappingType type;
    [FormerlySerializedAs("Mappings")]
    public List<DeviceSpriteMapping> mappings;

    public static implicit operator KeyValuePair<MappingType, Dictionary<DeviceType, Sprite>>(SpriteMapping spriteMapping) => new(spriteMapping.type, spriteMapping.mappings.ToDictionary(x => x.device, x => x.sprite));
    public static implicit operator SpriteMapping(KeyValuePair<MappingType, Dictionary<DeviceType, Sprite>> mapping) => new() { type = mapping.Key, mappings = mapping.Value.Select(x => (DeviceSpriteMapping)x).ToList() };

    public void Deconstruct(out MappingType type, out Dictionary<DeviceType, Sprite> mappings) => (type, mappings) = (this.type, this.mappings.ToDictionary(x => x.device, x => x.sprite));
}

[Serializable]
public class ImageMapping
{
    [FormerlySerializedAs("Type")]
    public MappingType type;
    [FormerlySerializedAs("Image")]
    public Image image;

    public static implicit operator KeyValuePair<MappingType, Image>(ImageMapping imageMapping) => new(imageMapping.type, imageMapping.image);
    public static implicit operator ImageMapping(KeyValuePair<MappingType, Image> mapping) => new() { type = mapping.Key, image = mapping.Value };

    public void Deconstruct(out MappingType type, out Image image) => (type, image) = (this.type, this.image);
}

[Serializable]
public class ButtonHintMapping
{
    public MappingType type;
    public GameObject buttonHint;

    public void Deconstruct(out MappingType type, out GameObject buttonHint) => (type, buttonHint) = (this.type, this.buttonHint);
}

#endregion

public enum MappingType
{
    Interact,
    UseItem,
    InventoryNext,
    InventoryPrevious,
    InteractUp,
    InteractDown,
    InteractLeft,
    InteractRight,
    InteractExit,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    LookAround
}