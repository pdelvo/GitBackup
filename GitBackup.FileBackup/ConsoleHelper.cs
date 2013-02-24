using System;

namespace GitBackup.FileBackup
{
    public static class ConsoleHelper
    {
        public static IDisposable Color(ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            return new ActionDisposer(() =>
                                          {
                                              Console.ForegroundColor = currentColor;
                                          });
        }

        class ActionDisposer : IDisposable
        {
            private readonly Action _action;

            public ActionDisposer(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action ();
            }
        }

    }
}
