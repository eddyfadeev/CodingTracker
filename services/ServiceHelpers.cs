using System.Reflection;
using CodingTracker.enums;
using Spectre.Console;

namespace CodingTracker.services;

internal abstract class ServiceHelpers
{
    internal static void InvokeActionForMenuEntry(Enum entry, object actionInstance)
    {
        var entryFieldInfo = entry.GetType().GetField(entry.ToString());
        var methodAttribute = entryFieldInfo.GetCustomAttribute<EnumHelpers.MethodAttribute>();

        if (methodAttribute != null)
        {
            var method = actionInstance.GetType().GetMethod(
                methodAttribute.MethodName, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (method != null)
            {
                method.Invoke(actionInstance, null);
            }
            else
            {
                AnsiConsole.WriteLine($"Method '{methodAttribute.MethodName}' not found.");
            }
        }
        else
        {
            AnsiConsole.WriteLine($"No methods assigned for {entry}.");
        }
    }
}