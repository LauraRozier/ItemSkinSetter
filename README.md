*Item Skin Setter* Sets the default skin ID for newly crafted items.

This plugin was written after a suggestion by [TFNBlackMarket](https://umod.org/user/TFNBlackMarket)


## Permissions

- **itemskinsetter.admin** -- Players and groups with this permission are able to use this plugin's commands.


## Chat Commands

- **/iss_set <Item ID or ShortName> <Skin ID>** -- Set the custom default skin ID for an item. ***(requires `itemskinsetter.admin` permission)***
- **/iss_get <Item ID or ShortName>** -- Retrieve the current custom default skin ID for an item. ***(requires `itemskinsetter.admin` permission)***
- **/iss_remove <Item ID or ShortName>** -- Remove a custom default skin ID definition. ***(requires `itemskinsetter.admin` permission)***


## Console Commands

- **iss_set <Item ID or ShortName> <Skin ID>** -- Set the custom default skin ID for an item.
- **iss_get <Item ID or ShortName>** -- Retrieve the current custom default skin ID for an item.
- **iss_remove <Item ID or ShortName>** -- Remove a custom default skin ID definition.
- **iss_getskins <Item ID or ShortName>** -- Retrieve the available skins for an item.


## Localization

```json
{
  "Err Invalid Args": "Invalid argument (count), please try again.",
  "Err Invalid Permission": "You do not have permission to use this command.",
  "Err Item Does Not Exist": "Item \"{0}\" does not exist.",
  "Err Skin Does Not Exist": "Skin with ID \"{0}\" does not exist.",
  "Msg Successful Skin Set": "The skin of item \"{0}\" ({1}) is successfully set to \"{2}\" ( Name = \"{3}\" ).",
  "Msg Item Skin Default": "The skin of item \"{0}\" ({1}) is default.",
  "Msg Item Skin": "The skin of item \"{0}\" ({1}) is \"{2}\" ( Name = \"{3}\" ).",
  "Msg Successful Remove": "Successfully removed modified default skin ID for \"{0}\"."
}
```
