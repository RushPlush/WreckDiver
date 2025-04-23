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

    public void ToggleInteractionHint() => buttonHintMappings.Find(x => x.type == MappingType.Interact).buttonHint.SetActive(!buttonHintMappings.Find(x => x.type == MappingType.Interact).buttonHint.activeSelf);

    public bool IsInteractionHintActive() => buttonHintMappings.Find(x => x.type == MappingType.Interact).buttonHint.activeSelf;

    public void ToggleInteractionGuide()
    {
        foreach (var (mapping, gameObject) in buttonHintMappings)
        {
            switch (mapping)
            {
                case MappingType.InteractUp:
                case MappingType.InteractDown:
                case MappingType.InteractLeft:
                case MappingType.InteractRight:
                case MappingType.InteractExit:
                    gameObject.SetActive(!gameObject.activeSelf);
                    break;
                default:
                    break;
            }
        }
    }

    public bool IsInteractionGuideActive() => buttonHintMappings.FindAll(x => x.type is MappingType.InteractUp or MappingType.InteractDown or MappingType.InteractLeft or MappingType.InteractRight or MappingType.InteractExit).All(x => x.buttonHint.activeSelf);
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
    InteractExit
}