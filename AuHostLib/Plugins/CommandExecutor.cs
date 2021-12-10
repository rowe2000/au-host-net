using System;
using System.Collections.Generic;
using AuHost.Commands;
using AuHost.Models;

namespace AuHost.Plugins
{
    public class CommandExecutor
    {
        private Document document;

        public void SetDocument(Document value)
        {
            Commands.Clear();
            NonSceneCommands.Clear();
            document = value;
        }

        public event Action Changed;

        public void OnChanged()
        {
            Changed?.Invoke();
        }

        public List<Command> NonSceneCommands { get; } = new List<Command>();

        public CommandStack Commands { get; } = new CommandStack();
        
        public void Execute(Command command)
        {
            if (command == null) 
                return;
            
            Commands.Execute(command);

            if (command.SaveInScene)
                document.CurrentScene.AddCommand(command);
            else
                NonSceneCommands.Add(command);

            OnChanged();
        }

        public void Undo()
        {
            var command = Commands.Undo();
            if (command == null) 
                return;
            
            if (command.SaveInScene)
                document.CurrentScene.RemoveCommand(command);
            else
                NonSceneCommands.Remove(command);

            OnChanged();
        }
    }
}