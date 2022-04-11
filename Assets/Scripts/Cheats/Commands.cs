using System;
using System.Collections.Generic;
using FromScratch.Inventory;
using FromScratch.Player;
using UnityEngine;
using Util;

namespace Cheats
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string name { get; set; }
    }

    public class Commands
    {
        public static string Arg(List<string> args, int index, string defaultValue)
        {
            return args.Count >= index ? args[index] : defaultValue;
        }
        
        [ConsoleCommand(name = "additem")]
        public static void AddItem(List<string> args, FromScratchPlayer player)
        {
            string itemId = Arg(args, 0, "grass");
            string qtyText = Arg(args, 1, "1");

            var itemDB = GameDatabases.Instance.items.GetDatabase();
            if (itemDB.ContainsKey(itemId))
            {
                ItemData item = itemDB[itemId];
                int qty = 1;
                if(int.TryParse(qtyText, out qty))
                {
                    Debug.Log("Adding Item from console command");
                    player.character.characterInventory.Container.AddItem(item, qty);
                }
            }
        }
        
    }
}