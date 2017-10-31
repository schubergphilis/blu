using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using System.Text;
using System.Threading;

namespace BluRunspace
{
    public class BluPsHost : PSHost
    {
        public BluPsHost()
        {
            var ui =new BluPsHostUserInterface();
            ui.DataReady += UiOnDataReady;
            UI = ui;
        }

        public event EventHandler<string> DataReady;

        private void UiOnDataReady(object sender, string message)
        {
            if (message == null)
            {
                return;
            }
            DataReady?.Invoke(sender, message);
        }

        public override void SetShouldExit(int exitCode)
        {
        }

        public override void EnterNestedPrompt()
        {
        }

        public override void ExitNestedPrompt()
        {
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override string Name { get; } = "BluPSHost";
        public override Version Version { get; } = new Version(4, 0, 0);
        public override Guid InstanceId { get; } = Guid.NewGuid();
        public override PSHostUserInterface UI { get; }
        public override CultureInfo CurrentCulture { get; } = Thread.CurrentThread.CurrentCulture;
        public override CultureInfo CurrentUICulture { get; } = Thread.CurrentThread.CurrentUICulture;
    }

    public class BluPsHostUserInterface : PSHostUserInterface
    {
        public event EventHandler<string> DataReady;
        
        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override SecureString ReadLineAsSecureString()
        {
            var pass = new SecureString();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass.RemoveAt(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);
            return pass;
        }

        public override void Write(string value)
        {
            DataReady?.Invoke(this, value);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            DataReady?.Invoke(this, value);
        }

        public override void WriteLine(string value)
        {
            DataReady?.Invoke(this, value + Environment.NewLine);
        }

        public override void WriteErrorLine(string value)
        {
            DataReady?.Invoke(this, "ERROR: " + value + Environment.NewLine);
        }

        public override void WriteDebugLine(string message)
        {
            DataReady?.Invoke(this, "DEBUG: " + message + Environment.NewLine);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            DataReady?.Invoke(this, "PROGRESS: " + record.PercentComplete + Environment.NewLine);
        }

        public override void WriteVerboseLine(string message)
        {
            DataReady?.Invoke(this, "VERBOSE: " + message + Environment.NewLine);
        }

        public override void WriteWarningLine(string message)
        {
            DataReady?.Invoke(this, "WARNING: " + message + Environment.NewLine);
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName,
            PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }


        public override PSHostRawUserInterface RawUI { get; } = new BluPsRawUiInterface();

    }

    internal class BluPsRawUiInterface : PSHostRawUserInterface
    {
        private ConsoleColor _foreground = ConsoleColor.White;
        public override ConsoleColor ForegroundColor
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
            }
        }

        private ConsoleColor _background = ConsoleColor.Black;
        public override ConsoleColor BackgroundColor
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
            }
        }

        public override Coordinates CursorPosition { get => new Coordinates(0,0); set { } }
        public override Coordinates WindowPosition { get => new Coordinates(0,0); set { } }
        public override int CursorSize { get => 1; set { } }
        public override Size BufferSize { get => new Size(80, 1000); set { } }
        public override Size WindowSize { get => new Size(80, 100); set { } }

        public override Size MaxWindowSize => new Size(80, 100);

        public override Size MaxPhysicalWindowSize => new Size(80, 100);

        public override bool KeyAvailable => false;

        public override string WindowTitle { get => "virtual PS host"; set { } }

        public override void FlushInputBuffer()
        {
            return;
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            return null;
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            return new KeyInfo();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            return;
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            return;
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            return;
        }
    }
}
