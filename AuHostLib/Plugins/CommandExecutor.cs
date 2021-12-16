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

        public TCommand Execute<TCommand>(params object[] args) where TCommand : Command
        {
            TCommand instance;
            try
            {
                instance = (TCommand) Activator.CreateInstance(typeof(TCommand), args);
            }
            catch (Exception e)
            {
                throw new ArgumentException($@"Can't create instance of {typeof(TCommand).Name} because of the arguments: {string.Join(", ", args)}", e);
            }

            return Execute(instance);
        }

        public TCommand Execute<TCommand>(TCommand command) where TCommand : Command
        {
            if (command == null) 
                return default;
            
            Commands.Execute(command);

            if (command.SaveInScene)
                document.CurrentScene.AddCommand(command);
            else
                NonSceneCommands.Add(command);

            OnChanged();

            return command;
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