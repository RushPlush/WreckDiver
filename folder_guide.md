# Folder Guide
## Scripts
Scripts shall be placed under `Assets/Scripts` and if their specialized for a specific component under a subfolder for the specific component in the main scripts folder.
## Models
### New
Models shall be placed when first imported under `Assets/Models/Not Extracted` to signify that this model is new and requires unpacking of materials and texture assigning.
### Unpacked
Unpacked models shall be placed under `Assets/Models/Extracted` and set in a subfolder that is indicative of what the model main use case is for.
## Materials
Materials when created shall be placed under `Assets/Materials` and in the correct subfolder that matches the name of where the model that this material is used for.
## Textures
Textures when created shall be placed under `Assets/Materials` and in teh correct subfolder that matches the name of where the model that this texture is used for.

Textures shall also follow the following name guide:
- Albedo/Diffuse: `${name}_AlbedoTransparency.png`
- Normal: `${name}_Normal.png`
- Metallic: `${name}_MetallicSmoothness.png`
- Other: `${name}_${type}.png`

For a view of how this looks check the folder for [Ceiling Lamp Textures](https://github.com/RushPlush/WreckDiver/tree/main/Assets/Textures/Ceiling%20Lamp)
## External scripts and resources
Most of unity external scripts and resources will have a default import path that is set to `Assets` this shall not be changed as auto update of these packages will also point to these locations where the first import of them were located.