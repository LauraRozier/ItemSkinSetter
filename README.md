*Item Skin Setter* Sets the default skin ID for newly crafted items.

This plugin was written after a suggestion by [TFNBlackMarket](https://umod.org/user/TFNBlackMarket)


## Console Commands

- **iss_get <Item ID or ShortName>** -- Retrieve the current custom default skin ID for an item.
- **iss_getskins <Item ID or ShortName>** -- Retrieve the available skins for an item.


## Configuration

You can remove entries you don't use. If you prefer to keep them in the config, setting the *Skin Id* to *0* (zero) will apply the default skin.

```json
{
  "Bindings": [
    {
      "Item Shortname": "door.double.hinged.metal",
      "Skin Id": 1904509199
    },
    {
      "Item Shortname": "door.double.hinged.toptier",
      "Skin Id": 2318482252
    },
    {
      "Item Shortname": "door.double.hinged.wood",
      "Skin Id": 0
    },
    {
      "Item Shortname": "door.hinged.metal",
      "Skin Id": 0
    },
    {
      "Item Shortname": "door.hinged.toptier",
      "Skin Id": 0
    },
    // etc..
  ]
}
```


## Localization

```json
{
  "Err Invalid Args": "Invalid argument (count), please try again.",
  "Err Item Does Not Exist": "Item \"{0}\" does not exist.",
  "Err Skin Does Not Exist": "Skin with ID \"{0}\" does not exist."
  "Msg Item Skin Default": "The skin of item \"{0}\" ({1}) is default.",
  "Msg Item Skin": "The skin of item \"{0}\" ({1}) is \"{2}\" ( Name = \"{3}\" )."
}
```
