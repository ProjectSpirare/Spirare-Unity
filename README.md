# Spirare

## Install

### Using Git
Edit `manifest.json` as follows.

```json
{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.atteneder.gltfast",
        "com.openupm"
      ]
    }
  ]
  "dependencies": {
    "com.atteneder.gltfast": "2.5.0",
    "com.tarukosu.spirare": "https://github.com/ProjectSpirare/Spirare-Unity.git?path=Assets/Spirare",
    "com.tarukosu.spirare.examples": "https://github.com/ProjectSpirare/Spirare-Unity.git?path=Assets/Spirare.Examples",

...

```

### Import sample scene

Open `Window > Package Manager` and select "Spirare Examples".  
Click "Import into Project" button.

![image](https://user-images.githubusercontent.com/4415085/104809917-4c810680-5834-11eb-8ee9-342abbba83a0.png)
