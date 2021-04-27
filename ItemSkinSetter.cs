/* --- Contributor information ---
 * Please follow the following set of guidelines when working on this plugin,
 * this to help others understand this file more easily.
 *
 * NOTE: On Authors, new entries go BELOW the existing entries. As with any other software header comment.
 *
 * -- Authors --
 * Thimo (ThibmoRozier) <thibmorozier@live.nl> 2021-04-24 +
 *
 * -- Naming --
 * Avoid using non-alphabetic characters, eg: _
 * Avoid using numbers in method and class names (Upgrade methods are allowed to have these, for readability)
 * Private constants -------------------- SHOULD start with a uppercase "C" (PascalCase)
 * Private readonly fields -------------- SHOULD start with a uppercase "C" (PascalCase)
 * Private fields ----------------------- SHOULD start with a uppercase "F" (PascalCase)
 * Arguments/Parameters ----------------- SHOULD start with a lowercase "a" (camelCase)
 * Classes ------------------------------ SHOULD start with a uppercase character (PascalCase)
 * Methods ------------------------------ SHOULD start with a uppercase character (PascalCase)
 * Public properties (constants/fields) - SHOULD start with a uppercase character (PascalCase)
 * Variables ---------------------------- SHOULD start with a lowercase character (camelCase)
 *
 * -- Style --
 * Max-line-width ------- 160
 * Single-line comments - // Single-line comment
 * Multi-line comments -- Just like this comment block!
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Item Skin Setter", "ThibmoRozier", "1.0.0")]
    [Description("Sets the default skin ID for newly crafted items.")]
    public class ItemSkinSetter : RustPlugin
    {
        #region Constants
        private const string CPermAdmin = "itemskinsetter.admin";
        private const string CDataFileName = "ItemSkinSetter_data";
        #endregion Constants

        #region Variables
        private Dictionary<int, int> FItemSkinBindings;
        #endregion Variables

        #region Script Methods
        private string _(string aKey) => lang.GetMessage(aKey, this);

        /// <summary>
        /// Determine whether or not a given string is a number
        /// </summary>
        /// <param name="aStr"></param>
        /// <returns></returns>
        private bool IsNumber(string aStr)
        {
            if (String.IsNullOrEmpty(aStr))
                return false;

            char cur;

            for (int i = 0; i < aStr.Length; i++) {
                cur = aStr[i];

                if (cur.Equals('-') || cur.Equals('+'))
                    continue;

                if (Char.IsDigit(cur) == false)
                    return false;
            }

            return true;
        }

        private string SetSkinForItem(string aItemArg, ulong aSkinId)
        {
            ItemDefinition itemDef = null;
            IPlayerItemDefinition skinDef;

            if (!IsNumber(aItemArg)) {
                itemDef = ItemManager.FindItemDefinition(aItemArg);
            } else {
                int itemId;

                if (int.TryParse(aItemArg, out itemId))
                    itemDef = ItemManager.FindItemDefinition(itemId);
            }

            if (itemDef == null)
                return String.Format(_("Err Item Does Not Exist"), aItemArg);


            try {
                skinDef = itemDef.skins2.First(x => x.WorkshopId == aSkinId);
            } catch (InvalidOperationException) {
                return String.Format(_("Err Skin Does Not Exist"), aSkinId);
            }

            FItemSkinBindings[itemDef.itemid] = skinDef.DefinitionId;
            Interface.Oxide.DataFileSystem.WriteObject(CDataFileName, FItemSkinBindings);
            return String.Format(_("Msg Successful Skin Set"), itemDef.shortname, itemDef.itemid, aSkinId, skinDef.Name);
        }

        private string GetSkinForItem(string aItemArg)
        {
            ItemDefinition itemDef = null;

            if (!IsNumber(aItemArg)) {
                itemDef = ItemManager.FindItemDefinition(aItemArg);
            } else {
                int itemId;

                if (int.TryParse(aItemArg, out itemId))
                    itemDef = ItemManager.FindItemDefinition(itemId);
            }

            if (itemDef == null)
                return String.Format(_("Err Item Does Not Exist"), aItemArg);

            if (FItemSkinBindings.ContainsKey(itemDef.itemid)) {
                int skinId = FItemSkinBindings[itemDef.itemid];
                IPlayerItemDefinition skinDef = itemDef.skins2.First(x => x.DefinitionId == skinId);
                return String.Format(_("Msg Item Skin"), itemDef.shortname, itemDef.itemid, skinDef.WorkshopId, skinDef.Name);
            } else {
                return String.Format(_("Msg Item Skin Default"), itemDef.shortname, itemDef.itemid);
            }
        }

        private void ResetSkinForItem(string aItemArg)
        {
            if (!IsNumber(aItemArg)) {
                ItemDefinition itemDef = ItemManager.FindItemDefinition(aItemArg);

                if (itemDef != null)
                    FItemSkinBindings.Remove(int.Parse(aItemArg));
            } else {
                FItemSkinBindings.Remove(int.Parse(aItemArg));
            }

            Interface.Oxide.DataFileSystem.WriteObject(CDataFileName, FItemSkinBindings);
        }
        #endregion Script Methods

        #region Hooks
        protected override void LoadDefaultMessages() {
            lang.RegisterMessages(
                new Dictionary<string, string> {
                    { "Err Invalid Args", "Invalid argument (count), please try again." },
                    { "Err Invalid Permission", "You do not have permission to use this command." },
                    { "Err Item Does Not Exist", "Item \"{0}\" does not exist." },
                    { "Err Skin Does Not Exist", "Skin with ID \"{0}\" does not exist." },

                    { "Msg Successful Skin Set", "The skin of item \"{0}\" ({1}) is successfully set to \"{2}\" ( Name = \"{3}\" )." },
                    { "Msg Item Skin Default", "The skin of item \"{0}\" ({1}) is default." },
                    { "Msg Item Skin", "The skin of item \"{0}\" ({1}) is \"{2}\" ( Name = \"{3}\" )." },
                    { "Msg Successful Remove", "Successfully removed modified default skin ID for \"{0}\"." }
                }, this, "en"
            );
        }

        void OnServerInitialized()
        {
            permission.RegisterPermission(CPermAdmin, this);

            if (Interface.Oxide.DataFileSystem.ExistsDatafile(CDataFileName)) {
                FItemSkinBindings = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<int, int>>(CDataFileName);
            } else {
                FItemSkinBindings = new Dictionary<int, int>();
            }
        }

        object OnItemCraft(ItemCraftTask aTask, BasePlayer aPlayer, Item aItem)
        {
            // We only change when the skin ID is default
            if (aTask.skinID == 0 && FItemSkinBindings.ContainsKey(aTask.blueprint.targetItem.itemid))
                aTask.skinID = FItemSkinBindings[aTask.blueprint.targetItem.itemid];

            return null;
        }
        #endregion Hooks

        #region Commands
        [ChatCommand("iss_set")]
        private void ItemSkinSetterSetCmd(BasePlayer aPlayer, string aCommand, string[] aArgs) {
            IPlayer player = aPlayer.IPlayer;

            if (player.IsServer || aArgs.Length < 2 || !IsNumber(aArgs[1])) {
                player.Reply(_("Err Invalid Args"));
                return;
            }

            if (!permission.UserHasPermission(aPlayer.UserIDString, CPermAdmin)) {
                player.Reply(_("Err Invalid Permission"));
                return;
            }

            player.Reply(SetSkinForItem(aArgs[0], ulong.Parse(aArgs[1])));
        }

        [ConsoleCommand("iss_set")]
        private void ItemSkinSetterSetCmd(ConsoleSystem.Arg aArg) {
            if (aArg.Args.Length < 2 || !IsNumber(aArg.Args[1])) {
                Puts(_("Err Invalid Args"));
                return;
            }

            Puts(SetSkinForItem(aArg.Args[0], ulong.Parse(aArg.Args[1])));
        }

        [ChatCommand("iss_get")]
        private void ItemSkinSetterGetCmd(BasePlayer aPlayer, string aCommand, string[] aArgs) {
            IPlayer player = aPlayer.IPlayer;

            if (player.IsServer || aArgs.Length < 1) {
                player.Reply(_("Err Invalid Args"));
                return;
            }

            if (!permission.UserHasPermission(aPlayer.UserIDString, CPermAdmin)) {
                player.Reply(_("Err Invalid Permission"));
                return;
            }

            player.Reply(GetSkinForItem(aArgs[0]));
        }

        [ConsoleCommand("iss_get")]
        private void ItemSkinSetterGetCmd(ConsoleSystem.Arg aArg) {
            if (aArg.Args.Length < 1) {
                Puts(_("Err Invalid Args"));
                return;
            }

            Puts(GetSkinForItem(aArg.Args[0]));
        }

        [ChatCommand("iss_remove")]
        private void ItemSkinSetterRemoveCmd(BasePlayer aPlayer, string aCommand, string[] aArgs) {
            IPlayer player = aPlayer.IPlayer;

            if (player.IsServer || aArgs.Length < 1) {
                player.Reply(_("Err Invalid Args"));
                return;
            }

            if (!permission.UserHasPermission(aPlayer.UserIDString, CPermAdmin)) {
                player.Reply(_("Err Invalid Permission"));
                return;
            }

            ResetSkinForItem(aArgs[0]);
            player.Reply(_("Msg Successful Remove"), null, aArgs[0]);
        }

        [ConsoleCommand("iss_remove")]
        private void ItemSkinSetterRemoveCmd(ConsoleSystem.Arg aArg) {
            if (aArg.Args.Length < 1) {
                Puts(_("Err Invalid Args"));
                return;
            }

            ResetSkinForItem(aArg.Args[0]);
            Puts(_("Msg Successful Remove"), aArg.Args[0]);
        }

        [ConsoleCommand("iss_getskins")]
        private void ItemSkinSetterGetSkinsCmd(ConsoleSystem.Arg aArg) {
            if (aArg.Args.Length < 1) {
                Puts(_("Err Invalid Args"));
                return;
            }

    	    string itemArg = aArg.Args[0];
            ItemDefinition itemDef = null;

            if (!IsNumber(itemArg)) {
                itemDef = ItemManager.FindItemDefinition(itemArg);
            } else {
                int itemId;

                if (int.TryParse(itemArg, out itemId))
                    itemDef = ItemManager.FindItemDefinition(itemId);
            }

            if (itemDef == null)
                Puts(_("Err Item Does Not Exist"), itemArg);

            IPlayerItemDefinition[] skinDefs = itemDef.skins2;
            StringBuilder sb = new StringBuilder($"Skins for item \"{itemDef.shortname}\" ({itemDef.itemid})\n");

            for (int i = 0; i < skinDefs.Length; i++)
                sb.Append($"  - {skinDefs[i].Name} ({skinDefs[i].WorkshopId})\n");

            Puts(sb.ToString());
        }
        #endregion Commands
    }
}
