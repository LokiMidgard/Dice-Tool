using System;
using System.Text;

namespace Dice.Parser
{
    internal class IndentionWriter
    {
        private int indentation = 0;
        private StringBuilder content = new StringBuilder();
        public IDisposable Indent => new IndentWatcher(this);

        public int IndentionDepth { get; set; }

        public void WriteLine(string input)
        {
            var indent = new string(' ', this.indentation * this.IndentionDepth);
            using var reader = new System.IO.StringReader(input);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                this.content.Append(indent);
                this.content.AppendLine(line);
            }
        }

        public override string ToString() => this.content.ToString();

        private sealed class IndentWatcher : IDisposable
        {

            private IndentionWriter indentionWriter;
            private bool disposedValue;

            public IndentWatcher(IndentionWriter indentionWriter)
            {
                this.indentionWriter = indentionWriter;
                this.indentionWriter.indentation++;
            }


            public void Dispose()
            {
                if (!this.disposedValue)
                {
                    this.disposedValue = true;
                    this.indentionWriter.indentation--;
                }
            }
        }

        internal void WriteLine() => this.content.AppendLine();
    }

}
