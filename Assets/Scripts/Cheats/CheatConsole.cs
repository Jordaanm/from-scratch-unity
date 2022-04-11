using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FromScratch.Interaction;
using FromScratch.Player;

namespace Cheats
{
    public class CheatConsole
    {
        private Dictionary<string, MethodInfo> commandDB;
        public CheatConsole()
        {
            BuildCommandDatabase();
        }

        private void BuildCommandDatabase()
        {
            commandDB = new Dictionary<string, MethodInfo>();
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .SelectMany(a => a.GetMethods())
                .Where(t => t.IsDefined(typeof(ConsoleCommandAttribute), false))
                .ToList()
                .ForEach(info =>
                {
                    var attr = info
                        .GetCustomAttributes(typeof(ConsoleCommandAttribute), false)
                        .Cast<ConsoleCommandAttribute>()
                        .First();

                    if (attr != null)
                    {
                        commandDB.Add(attr.name, info);
                    }
                });
        }

        public void Execute(string text, FromScratchPlayer player)
        {
            var pieces = text.Split(' ');
            MethodInfo command = FindCommand(pieces[0]);
            
            var args = pieces.ToList();
            args.RemoveAt(0);

            if (command != null)
            {
                command.Invoke(null, new object[]
                {
                    args,
                    player
                });
            }
        }

        MethodInfo FindCommand(string name)
        {
            return commandDB.ContainsKey(name) ? commandDB[name] : null;
        }
    }
}