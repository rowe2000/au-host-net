using System.Collections.Generic;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public abstract class Command : Storable<Command, Command>
    {
        private bool isDone;
        private readonly Stack<Command> stack = new Stack<Command>();
        public abstract bool SaveInScene { get; }

        public virtual bool Execute()
        {
            if (isDone)
                return false;

            isDone = true;

            return true;
        }

        public virtual bool Undo()
        {
            if (!isDone)
                return false;

            PopAll();
            isDone = false;
            
            return true;
        }

        protected virtual void PopAll()
        {
            while (stack.Count > 0)
            {
                var top = stack.Peek();
                if (top == null) 
                    continue;
                
                top.Undo();
                stack.Pop();
            }
        }
    
        protected virtual bool Push(Command command)
        {
            if (!command.Execute()) 
                return false;
            
            stack.Push(command);
            return true;
        }
    }
}