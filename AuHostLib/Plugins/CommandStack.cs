using System.Collections.Generic;
using System.Linq;
using AuHost.Commands;

namespace AuHost.Plugins
{
    public class CommandStack
    {
        private readonly List<Command> doneCommands = new List<Command>();
        private readonly List<Command> notDoneStack = new List<Command>();
        
        public Command Peek()
        {
            return doneCommands.LastOrDefault();
        }

        public Command Undo()
        {
            if (!doneCommands.Any())
                return null;

            var command = doneCommands.Last();

            if (!command.Undo())
            {
                return null;
            }

            notDoneStack.Add(command);
            return command;        
        }

        public bool Execute(Command command)
        {
            if (!command.Execute())
            {
                return false;
            }

            doneCommands.Add(command);
            notDoneStack.Clear();

            return true;
        }

        public bool ExecuteAll()
        {
            while (notDoneStack.Any())
                if (!Execute())
                    return false;

            return true;
        }

        public bool Execute()
        {
            if (!notDoneStack.Any())
                return true;

            var command = notDoneStack.Last();
            notDoneStack.Remove(command);

            if (!command.Execute())
            {
                return false;
            }

            doneCommands.Add(command);

            return true;
        }

        public bool UndoAll()
        {
            while (doneCommands.Any())
                if (Undo() == null)
                    return false;

            return true;
        }
    }
}